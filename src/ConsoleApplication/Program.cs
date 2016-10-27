using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using JsonParser;
using CoreLogic;

namespace ConsoleApplication
{
    class Program
    {
        public const string DESCRIPTION_FILE_NAME_KEY = "-task";
        public const string INPUT_FILE_NAME_KEY = "-in";
        public const string OUTPUT_FILE_NAME_KEY = "-out";

        private struct CommandLineArguments
        {
            public string TaskDescriptionFileName, InputValuesFileName, OutputFileName;
            public List<int> UnusedArguments;
        }

        private static void Main(string[] args)
        {
            var cmdArguments = ParseCommandLineArgumens(args);
            bool printHelp = false, continueExecution = true;

            if (cmdArguments.UnusedArguments.Any())
            {
                foreach (var index in cmdArguments.UnusedArguments)
                {
                    Console.Error.WriteLine($"Unknown command line argument: {args[index]}");
                }
                printHelp = true;
            }

            if (cmdArguments.TaskDescriptionFileName == null)
            {
                Console.Error.WriteLine("Task description file name is not provided.");
                printHelp = true; continueExecution = false;
            }
            else if (!File.Exists(cmdArguments.TaskDescriptionFileName))
            {
                Console.Error.WriteLine($"File {cmdArguments.TaskDescriptionFileName} does not exist.");
                printHelp = true; continueExecution = false;
            }

            string jsonTask = null;
            if (continueExecution)
            {
                try
                {
                    jsonTask = File.ReadAllText(cmdArguments.TaskDescriptionFileName);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Cannot read {cmdArguments.TaskDescriptionFileName} with error: {e.Message}");
                    continueExecution = false;
                }
            }

            Task task = null;
            if (continueExecution)
            {
                try
                {
                    task = Parser.TaskFromJson(jsonTask);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Cannot parse {cmdArguments.TaskDescriptionFileName} with error: {e.Message}");
                    continueExecution = false;
                }
            }

            if (cmdArguments.InputValuesFileName == null)
            {
                Console.Error.WriteLine("Input file name is not provided.");
                printHelp = true; continueExecution = false;
            }
            else if (!File.Exists(cmdArguments.InputValuesFileName))
            {
                Console.Error.WriteLine($"File {cmdArguments.InputValuesFileName} does not exist.");
                printHelp = true; continueExecution = false;
            }

            string jsonInputs = null;
            if (continueExecution)
            {
                try
                {
                    jsonInputs = File.ReadAllText(cmdArguments.InputValuesFileName);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Cannot read {cmdArguments.InputValuesFileName} with error: {e.Message}");
                    continueExecution = false;
                }
            }

            IDictionary<Parameter, double> inputs = null;
            if (continueExecution)
            {
                try
                {
                    inputs = Parser.InputsFromJson(task, jsonInputs);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Cannot parse {cmdArguments.InputValuesFileName} with error: {e.Message}");
                    continueExecution = false;
                }
            }

            IEnumerable<Task.OutputParameterSolution> solution = null;
            if (continueExecution)
            {
                try
                {
                    solution = task.Solve(inputs);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Cannot solve the task with error: {e.Message}");
                    continueExecution = false;
                }
            }
            
            if (continueExecution)
            {
                var jsonSolution = Parser.SolutionToJson(solution);  // Have no idea why that may fail, thus do not handle it.

                if (cmdArguments.OutputFileName == null)
                {
                    Console.WriteLine(jsonSolution);
                }
                else
                {
                    if (File.Exists(cmdArguments.OutputFileName))
                    {
                        Console.Error.WriteLine($"File {cmdArguments.OutputFileName} does not exist.");
                        printHelp = true; continueExecution = false;
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllText(cmdArguments.OutputFileName, jsonSolution);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine($"Cannot write to file {cmdArguments.OutputFileName} with error: {e.Message}");
                        }
                    }
                }
            }

            if (printHelp)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("Application usage: solve -task <task-file-name> -in <input-file-name> [-out <output-file-name>]");
                Console.Error.WriteLine("If output file is not provided, the output data is printed to stdout.");
                Console.Error.WriteLine();
            }
        }

        private static CommandLineArguments ParseCommandLineArgumens(string[] args)
        {
            var usedArguments = new List<int>();

            var res = new CommandLineArguments();
            res.TaskDescriptionFileName = ReadValueForKey(args, DESCRIPTION_FILE_NAME_KEY, usedArguments);
            res.InputValuesFileName = ReadValueForKey(args, INPUT_FILE_NAME_KEY, usedArguments);
            res.OutputFileName = ReadValueForKey(args, OUTPUT_FILE_NAME_KEY, usedArguments);
            res.UnusedArguments = Enumerable.Range(0, args.Length).Except(usedArguments).ToList();

            return res;
        }

        private static string ReadValueForKey(string[] args, string key, List<int> usedArguments)
        {
            var keyIndex = Array.FindIndex(args, s => s == key);
            string res = null;

            if (keyIndex >= 0)
            {
                usedArguments.Add(keyIndex);
                if ((keyIndex + 1) < args.Length)
                {
                    usedArguments.Add(keyIndex + 1);
                    res = args[keyIndex + 1];
                }
            }

            return res;
        }
    }
}
