## Console application guide

The console application is designed to be used
from command line and should be called this way
(the order of keys does not matted):
```
application -task <filename> -in <filename> [-out <filename>]
```
The file, which name is given after the `-task` key, is assumed
to contain JSON-representation of `CoreLogic.Task` object.

The file, which name is given after the `-in` key, is assumed
to contain JSON-representation of `CoreLogic.Task.Solve(..)` method's
input data.

If the `-out` key is provided, the program will write
the task's solution in JSON-format to the file, which name
is stated after the key. Otherwise, the same output will
be written to stdout.

To read more about JSON-representation of `CoreLogic.Task`,
tasks' inputs and outputs and to see examples of those,
check out `/docs/json/`.