## Storing tasks's inputs and outputs in JSON format

The program's JsonParser module is capable of storing tasks' inputs
and outputs as JSON-objects.

In CoreLogic library, to acquire a solution for a task, you should
provide it with an instance of `IDictionary<Parameter,double>`,
that ties every input variable of the task to its value.

JSON-representation of it is simply:
```
{
  <string>: <float>,
  ...
}
```
Where
- `<string>` is a simple string in JSON format;
- `<float>` is a floating point number in JSON format.

The set of the dictionary's keys should include the names of all of the
target task's input variables. Should it not be the case, a parsing error
will be produced.

The task's solution is acquired in the form of `IEnumerable<Task.OutputParameterSolution>`.
`Task.OutputParameterSolution` stores a value distribution for an output variable
and allows to calcualte its gravity center.

The JSON-representation of the solution does not preserve the variables' value
distribution functions, storing only their gravity centers.

It is simply:
```
{
  <string>: <float>,
  ...
}
```
Where
- `<string>` is a simple string in JSON format;
- `<float>` is a floating point number in JSON format.

Each of the dictionary's keys is a name of the target task's output variable,
and the key's value is this variable's value distribution function's gravity center.