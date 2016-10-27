## Storing tasks in JSON format

The program's JsonParser module is capable of storing tasks
(i.e. objects of `CoreLogic.Task`) as JSON-objects.
The current document describes the format of these JSON-objects,
as it is believed to be (at list roughly) readable and editable by humans.

### CoreLogic.Task

Any task is desribed by:
- its name;
- a set of input variables;
- a set of output variables;
- a set of rules.

So the JSON-representation of a task looks like this:
```
{
  "name": <string>,
  "in_vars": [
    <varibale>, ...
  ],
  "out_vars": [
    <varibale>, ...
  ],
  "rules": [
    <rule>, ...
  ],
}
```
Where
- `<string>` is a simple string as it is described for JSON format;
- `<variable>` is a JSON-representation of a `CoreLogic.Parameter` object as it is described below;
- `<rule>` is a JSON-representation of a `CoreLogic.Rule` object as it is described below.

### CoreLogic.Parameter

Any task's variable -- input or output one -- is described by:
- its name;
- a range, on which it can take values;
- a set of classes (fuzzy-sets).

As simple, as in previous case, the JSON representation of a variable looks like this:
```
{
  "name": <string>,
  "from": <float>,
  "to": <float>,
  "classes": [
    <class>, ...
  ]
}
```

Where
- `<string>` is a simple string as it is described for JSON format;
- `<float>` is a floating point number as it is described for JSON format;
- `<class>` is a JSON-representation of any object derived from `CoreLogic.Classes.Class` as it is described below.

Note that, though CoreLogic library uses class `CoreLogic.Range` to represent variable's range,
JSON-representation uplifts its two fields to the variable level and does not describe
`CoreLogic.Range` at all.

Also note that input and output variables' names should be unique
(but an input variable may have the same name as an output variable).

### CoreLogic.Classes.Class

In CoreLogic library `CoreLogic.Classes.Class` is an abstract class, that has these concrete heirs:
- `CoreLogic.Classes.ClassWithTriangularMF`;
- `CoreLogic.Classes.ClassWithTrapezoidalMF`;
- `CoreLogic.Classes.ClassWithGaussianMF`;
- `CoreLogic.Classes.ClassWithGeneralisedBellMF`;
- `CoreLogic.Classes.ClassWithSigmoidDifferenceMF`.

On contrary, to describe any class in JSON format we only
use one type of object that looks like this:
```
{
  "name": <string>,
  "type": <string>,
  "params": {
    /* type-specific parameters */
  }
}
```
Here `"name"` is, as usually, just a string as it is described for JSON format.
Every class has a name (this is actually a `CoreLogic.Classes.Class`'s field),
so this field does not depend on type of the described class.
Note that a class's name should be unique within the classes of a variable
it is tied to (i.e. if a variable has a class named "low-values", it cannot have
another class with the same name, but any other variable can).

The `"type"` field is used to distinguish the type of the class
and can only be one of these strings:
- `"triangular"`;
- `"trapezoidal"`;
- `"gaussian"`;
- `"generalised_bell"`;
- `"sigmoid_diff"`.

Naturally, the parsing will fail, should this feild contain any other value.

As the description notes, `"params"` dictionary contains type-specific parameters.
Here are the forms this dictionary should take, depending on `"type"`'s value:
- for `"triangular"` type the dictionary should look like this:
```
{
  "a": <float>,
  "b": <float>,
  "c": <float>
}
```
- for `"trapezoidal"` type the dictionary should look like this:
```
{
  "a": <float>,
  "b": <float>,
  "c": <float>,
  "d": <float>
}
```
- for `"gaussian"` type the dictionary should look like this:
```
{
  "c": <float>,
  "sigma": <float>
}
```
- for `"generalised_bell"` type the dictionary should look like this:
```
{
  "a": <float>,
  "b": <float>,
  "c": <float>
}
```
- for `"sigmoid_diff"` type the dictionary should look like this:
```
{
  "a1": <float>,
  "a2": <float>,
  "c1": <float>,
  "c2": <float>
}
```

Where `<float>` is a floating point number as it is described for JSON format.

The parsing will fail if the program is not able to find the required keys in the
dictionary. It will not fail if the dictionary contains any other keys, though.

### CoreLogic.Rule

Rules describe connection between input and output variables. What a rule
generally says is that an output variable's value lies in one of its classes
if some subset of input variables lies in some of their classes.

Thus, a rule consists of:
- a reference to a target output variable
- a reference to a target output variable's class
- an expression, describing the fuzzy set of allowed input variables' values

In JSON format this simply looks like:
```
{
  "var_name": <string>,
  "class_name": <string>,
  "expr": <expression>
}
```
Where
- `<string>` is a simple string as it is described for JSON format;
- `<expression>` is a JSON-representation of any object derived from
  `CoreLogic.Expressions.Expression` as it is described below.

Note that `"var_name"` should be one of the input variables' names
and `"class_name"` should be one of this variable's classes' names.

### CoreLogic.Expressions.Expression

In CoreLogic library `CoreLogic.Expressions.Expression` is an abstract class
that has these concrete heirs:
- `CoreLogic.Expressions.Negation`;
- `CoreLogic.Expressions.Disjunction`;
- `CoreLogic.Expressions.Conjunction`;
- `CoreLogic.Expressions.MembershipStatement`.

To describe any expression in JSON format we only use one type that looks like this:
```
{
  "type": <string>,
  "var_name": <string>,
  "class_name": <string>,
  "arg": <expression>,
  "left": <expression>,
  "right": <expression>
}
```
Where
- `<string>` is a simple string as it is described for JSON format;
- `<expression>` is a JSON-representation of any object derived from
  `CoreLogic.Expressions.Expression` as it is described in this very section.

The `"type"` field can only contain one of the following strings:
- `"neg"` to represent `CoreLogic.Expressions.Negation`;
- `"or"` to represent `CoreLogic.Expressions.Disjunction`;
- `"and"` to represent `CoreLogic.Expressions.Conjunction`;
- `"state"` to represent `CoreLogic.Expressions.MembershipStatement`;

Any other value in this field will produce a parsing error.

Each type of expression will need some of the other described fields,
but no type will need all of them, so you can safely omit the ones you don't need.

Here are the fields essential for different kinds of expressions:
- _negation_ expressions will need a value in `"arg"` field;
- _conjunction_ and _disjunction_ expressions will need values in
  `"left"` and `"right"` fields;
- _statement_ expressions will need values in `"var_name"` and `"class_name"` fields.

Statement expressions are used to state that an input variable's value
lies in one of this variable's class, so `"var_name"` and `"class_name"` field
should be an input variable's name and a name of one of its classes respectively.
