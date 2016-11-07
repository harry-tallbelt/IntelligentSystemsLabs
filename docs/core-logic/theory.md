## Fuzzy-logic inference theory

The current document is intended to describe the theoretical basis behind CoreLogic library.

The library is solely dedicated to solving fuzzy-logic inference problems.
Fuzzy-logic inference is a process of calculating the values of some output variables
from the known values of some input variables, based on the set of fuzzy-logic rules,
that describe the output variables' dependency on the input variables.

### Problem description

Each fuzzy-logic inference problem is characterized by:
- a set of input variables, divided in fuzzy-classes;
- a set of output variables, divided in fuzzy-classes;
- a set of rules, stated in terms of the variables' classes.

#### Variables

Each variable - input or output - has its domain range [a,b]. The range can
theoretically be infinite (i.e. not bounded), but, due to the membership function types
(see below) used in CoreLogic library, it will not be useful. Each variable's
domain range is partitioned by a set of fuzzy-classes. This set is called the variable's
fuzzy-class partition.

#### Fuzzy-classes

A fuzzy-class of some variable is, essentially, described with its membership function.
A membership function of a class maps all the possible values of the variable
(i.e. values from [a,b] segment) to [0,1] segment, thus providing a measure for
"how much" each possible value of the variable belongs to the class.

The membership functions can take lots of different forms,
but the ones implemented in the library are:
- triangular;
- trapezoidal;
- Gaussian;
- generalised Bell;
- sigmoid difference.

You can read about those in more detail and find some exaples
of membership functions' graphs here:
http://www-rohan.sdsu.edu/doc/matlab/toolbox/fuzzy/fuzzytu3.html

#### Inference rules

The fuzzy-class partitions are useful, because now the inference system can be stated
simply as a set of rules, each rule being of form "an output variable's value lies in
class X if an input variable's value lies in class Y". This statement will simply mean
that the output variable's membership value for class X is equal to the input variabble's
membership value's for class Y.

So `(v1 in C1) if (v2 in C2)` - value v1 is in class C1 if value v2 is in class C2 -
simply means `C1(v1) = C2(v2)` - value v1 is in class C1 to the same degree as
value v2 is in class C2.

To make things a little more flexible, the condition of a rule (the part afer 'if')
can include statements about several input variables. These statemets can be
connected via _and_, _or_ and _not_ operations. These operations result in
finding _min_ or _max_ between two values, or finding a _1-complement_ (`1 - x`)
to one value respectively.

For example:
- `(v1 in C1) if (v2 not in C2)` means `C1(v1) = 1 - C2(v2)`
- `(v1 in C1) if (v2 in C2) and (v3 in C3)` means `C1(v1) = min(C2(v2), C3(v3))`
- `(v1 in C1) if (v2 in C2) or (v3 in C3)` means `C1(v1) = max(C2(v2), C3(v3))`

Of course, the operations can be nested:

```(v1 in C1) if ((v2 is not in C2) and ((v2 is in C3) or (v3 is not in C4)))```

means

``` C1(v1) = min(1 - C2(v2), max(C3(v2), 1 - C4(v3))) ```.

### Fuzzy-logic inference process

Fuzzy-logic inference process consists of these three parts:

1. Fuzzyfication (concrete values -> fuzzy sets)
2. Rules-based inference
3. Defuzzyfication (fuzzy sets -> concrete values)

#### Fuzzyfication

The rules, used for inference, are stated in terms of classes, but the input of the
problem is simply a set of concrete values - one value for each input variable.
To work with the rules on the next stage, we fuzzyfy the inputs: for each variable's
concrete value we compute a set of membership values - one value for each class
from the variable's partitioning.

Suppose varible `V` is partitioned in classes `C1`, `C2` and `C3`.
On fuzzyfication stage we, given a concrete `V`'s value `v`,
compute `C1(v)`, `C2(v)` and `C3(v)`, where `Ci(x)` is the value of `Ci`'s
membership function on `x`.

#### Rules-based inference

On this stage we acquire a membership value for each class of each output
variable. This is done using inferance rules: each output variable's class has at
least one inferance rule associated with it. A class having a few rules
`R1`, `R2`, ... , `Rn` associated with it is the same as this class having only one rule,
which states `R1 or R2 or ... or R3`. Thus, we can say that the output variables' classes
and the (slightly modified) rules have one-to-one correspondence.

In the end, to acquire a membership value for each class of each output variable,
we simply compute each rule's conditions (see the examples in "Inference rules" part above),
using the membership values of the input variables' classes (which were computed on the previous step).

#### Defuzzyfication

After the previous step, we have a membership value for each of the output variables' classes.
For every output variable we can now build a value distribution: a function [a,b] -> [0,1],
that shows "to which degree" each possible value of the variable
can be a solution of the problem.

Let the membership values (calculated on the previous step) for an output variable `V`'s
classes `C1`, `C2`, ... , `Cn` be `c1`, `c2`, ... , `cn`.
Then `V`'s value distribution function is:
```
f(x) = max(min(c1, C1(x)), min(c2, C2(x)), ... , min(cn, Cn(x)))
```

Acquiring a concrete value from this distribution function is called deffuzyfication
and is usually (and always for CoreLogic library) computed as the gravity center of `f`.
A gravity ceneter of a function f on [a,b] is computed by division of two define integrals:
```
integral([a,b]; f(x)*x) / integral([a,b]; f(x))
```

Applying this formula to every value distribution function will produce
a concrete value for each output variable, which is the problem's solution.
