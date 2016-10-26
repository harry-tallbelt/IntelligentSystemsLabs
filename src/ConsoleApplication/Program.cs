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
        public const string DESCRIPTION_FILE_NAME_KEY = "";
        public const string INPUT_FILE_NAME_KEY = "";
        public const string OUTPUT_FILE_NAME_KEY = "";

        private struct CommandLineArguments
        {
            public string TaskDescriptionFileName, InputValuesFileName, SolutionFileName;
            public bool UnusedArguments;
        }

        private static void Main(string[] args)
        {
            var cmdArguments = ParseCommandLineArgumens(args);
            var jsonTask = File.ReadAllText(cmdArguments.TaskDescriptionFileName);
            var task = Parser.TaskFromJson(jsonTask);

            var jsonInputs = File.ReadAllText(cmdArguments.InputValuesFileName);
            var inputs = Parser.InputsFromJson(task, jsonInputs);

            var solution = task.Solve(inputs);
            var jsonSolution = Parser.SolutionToJson(solution);
            File.WriteAllText(cmdArguments.SolutionFileName, jsonSolution);
        }

        private static CommandLineArguments ParseCommandLineArgumens(string[] args)
        {
            var usedArguments = new List<int>();

            var res = new CommandLineArguments();
            res.TaskDescriptionFileName = ReadValueForKey(args, DESCRIPTION_FILE_NAME_KEY, usedArguments);
            res.InputValuesFileName = ReadValueForKey(args, INPUT_FILE_NAME_KEY, usedArguments);
            res.SolutionFileName = ReadValueForKey(args, OUTPUT_FILE_NAME_KEY, usedArguments);
            res.UnusedArguments = Enumerable.Range(0, args.Length).Except(usedArguments).Any();

            return res;
        }

        private static string ReadValueForKey(string[] args, string key, List<int> usedArguments)
        {
            var keyIndex = Array.FindIndex(args, s => s == key);
            string res = null;

            if (keyIndex >= 0)
            {
                usedArguments.Add(keyIndex);
                if ((keyIndex + 1) >= args.Length)
                {
                    usedArguments.Add(keyIndex + 1);
                    res = args[keyIndex + 1];
                }
            }

            return res;
        }
    }
}
