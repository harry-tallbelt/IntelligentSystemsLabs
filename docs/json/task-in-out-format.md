## Storing tasks's inputs and outputs in JSON format

The program's JsonParser module is capable of storing tasks' inputs
and outputs as JSON-objects.

In CoreLogic library to acquire a solution for a task, you should
provide it with an instance of `IDictionary<Parameter,double>`,
that maps every input variable of a task to its specific value.

JSON-representation of it is simply:
```
{
  <string>: <float>,
  ...
}
```
Where
- `<string>` is a simple string as it is described for JSON format;
- `<float>` is a floating point number as it is described for JSON format.

The set of dictionaty's keys should include all of the target task's
input variables' names. Should it not be the case, a parsing error
will be produced.

The task's solution is acquired in the form of `IEnumerable<Task.OutputParameterSolution>`.
`Task.OutputParameterSolution` stores a value distribution for an output variable
and allows for calcualting its gravity center.

JSON-representation of the solution does not preserve the variables' value distribution
function, storing only its gravity center as a conrete answer.

It is simply:
```
{
  <string>: <float>,
  ...
}
```
Where
- `<string>` is a simple string as it is described for JSON format;
- `<float>` is a floating point number as it is described for JSON format.

Each of the dictionary's keys is the target task's output variable's name,
and this key's value is the variable's value distribution function gravity center.