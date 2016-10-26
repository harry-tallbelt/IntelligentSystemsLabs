using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JsonParser
{
    public static class Parser
    {
        public static CoreLogic.Task TaskFromJsonString(string json)
        {
            var taskModel = JsonConvert.DeserializeObject<Models.Task>(json);

            var inVars = taskModel.in_vars.ConvertAll(ConvertModelToParameter);
            var outVars = taskModel.out_vars.ConvertAll(ConvertModelToParameter);
            var rules = taskModel.rules.ConvertAll(rule => ConvertModelToRule(rule, inVars, outVars));

            return new CoreLogic.Task(taskModel.name, inVars, outVars, rules);
        }

        public static string JsonStringFromTask(CoreLogic.Task task) => JsonStringFromTask(task, true);

        public static string JsonStringFromTask(CoreLogic.Task task, bool indentSubobjects)
        {
            var taskModel = new Models.Task {
                name = task.Name,
                in_vars = task.InputParameters.Select(ConvertParameterToModel).ToList(),
                out_vars = task.OutputParameters.Select(ConvertParameterToModel).ToList(),
                rules = task.Rules.Select(ConvertRuleToModel).ToList()
            };

            return JsonConvert.SerializeObject(taskModel, indentSubobjects ? Formatting.Indented : Formatting.None);
        }

        private static CoreLogic.Parameter ConvertModelToParameter(Models.Parameter parameterModel)
        {
            var range = new CoreLogic.Range(parameterModel.from, parameterModel.to);
            var classes = parameterModel.classes.ConvertAll(ConvertModelToClass);

            return new CoreLogic.Parameter(parameterModel.name, range, classes);
        }

        private static CoreLogic.Classes.Class ConvertModelToClass(Models.Class classModel)
        {
            if (classModel.type == MF_TRIANGULAR)
            {
                return new CoreLogic.Classes.ClassWithTriangularMF(
                    classModel.name,
                    classModel.@params[MF_TRIANGULAR_A],
                    classModel.@params[MF_TRIANGULAR_B],
                    classModel.@params[MF_TRIANGULAR_C]
                );
            }
            else if (classModel.type == MF_TRAPEZOIDAL)
            {
                return new CoreLogic.Classes.ClassWithTrapezoidalMF(
                    classModel.name,
                    classModel.@params[MF_TRAPEZOIDAL_A],
                    classModel.@params[MF_TRAPEZOIDAL_B],
                    classModel.@params[MF_TRAPEZOIDAL_C],
                    classModel.@params[MF_TRAPEZOIDAL_D]
                );
            }
            else if (classModel.type == MF_GAUSSIAN)
            {
                return new CoreLogic.Classes.ClassWithGaussianMF(
                    classModel.name,
                    classModel.@params[MF_GAUSSIAN_C],
                    classModel.@params[MF_GAUSSIAN_SIGMA]
                );
            }
            else if (classModel.type == MF_GENERALISED_BELL)
            {
                return new CoreLogic.Classes.ClassWithGeneralisedBellMF(
                    classModel.name,
                    classModel.@params[MF_GENERALISED_BELL_A],
                    classModel.@params[MF_GENERALISED_BELL_B],
                    classModel.@params[MF_GENERALISED_BELL_C]
                );
            }
            else if (classModel.type == MF_SIGMOID_DIFF)
            {
                return new CoreLogic.Classes.ClassWithSigmoidDifferenceMF(
                    classModel.name,
                    classModel.@params[MF_SIGMOID_DIFF_A1],
                    classModel.@params[MF_SIGMOID_DIFF_A2],
                    classModel.@params[MF_SIGMOID_DIFF_C1],
                    classModel.@params[MF_SIGMOID_DIFF_C2]
                );
            }
            else
            {
                throw new ArgumentException($"Attemt to parse an unknown type of classes: {classModel.type}");
            }
        }

        private static CoreLogic.Rule ConvertModelToRule(
            Models.Rule ruleModel, 
            IEnumerable<CoreLogic.Parameter> inputVariables, 
            IEnumerable<CoreLogic.Parameter> outputVariables
        )
        {
            var parameter = outputVariables.FirstOrDefault(p => p.Name == ruleModel.var_name);
            if (parameter == null)
            {
                throw new ArgumentException($"A rule references a non-existing output variable {ruleModel.var_name}.");
            }

            var clazz = parameter.Classes.FirstOrDefault(c => c.Name == ruleModel.class_name);
            if (clazz == null)
            {
                throw new ArgumentException($"A rule references a non-existing output variable's class {ruleModel.class_name}.");
            }

            var expression = ConvertModelToExpression(ruleModel.expr, inputVariables);

            return new CoreLogic.Rule(parameter, clazz, expression);
        }

        private static CoreLogic.Expressions.Expression ConvertModelToExpression(
            Models.Expression expressionModel,
            IEnumerable<CoreLogic.Parameter> inputVariables
        )
        {
            if (expressionModel.type == EXPR_CONJUNCTION)
            {
                var left = ConvertModelToExpression(expressionModel.left, inputVariables);
                var right = ConvertModelToExpression(expressionModel.right, inputVariables);
                return new CoreLogic.Expressions.Conjunction(left, right);
            }
            else if (expressionModel.type == EXPR_DISJUNCTION)
            {
                var left = ConvertModelToExpression(expressionModel.left, inputVariables);
                var right = ConvertModelToExpression(expressionModel.right, inputVariables);
                return new CoreLogic.Expressions.Disjunction(left, right);
            }
            else if (expressionModel.type == EXPR_NEGATION)
            {
                var argument = ConvertModelToExpression(expressionModel.arg, inputVariables);
                return new CoreLogic.Expressions.Negation(argument);
            }
            else if (expressionModel.type == EXPR_STATEMENT)
            {
                var parameter = inputVariables.FirstOrDefault(p => p.Name == expressionModel.var_name);
                if (parameter == null)
                {
                    throw new ArgumentException($"A statement references a non-existing input variable {expressionModel.var_name}.");
                }

                var clazz = parameter.Classes.FirstOrDefault(c => c.Name == expressionModel.class_name);
                if (clazz == null)
                {
                    throw new ArgumentException($"A statement references a non-existing input variable's class {expressionModel.class_name}.");
                }

                return new CoreLogic.Expressions.MembershipStatement(parameter, clazz);
            }
            else
            {
                throw new ArgumentException($"Attempt to handle an unknown type: {expressionModel.GetType().FullName}.");
            }
        }
        
        private static Models.Parameter ConvertParameterToModel(CoreLogic.Parameter parameter)
        {
            return new Models.Parameter {
                name = parameter.Name,
                from = parameter.Range.LowerBoundary,
                to = parameter.Range.UpperBoundary,
                classes = parameter.Classes.Select(ConvertClassToModel).ToList()
            };
        }

        private static Models.Class ConvertClassToModel(CoreLogic.Classes.Class clazz)
        {
            var classModel = new Models.Class { name = clazz.Name, @params = new Dictionary<string, double>() };

            if (clazz is CoreLogic.Classes.ClassWithTriangularMF)
            {
                classModel.type = MF_TRIANGULAR;
                var classWithTriangularMF = clazz as CoreLogic.Classes.ClassWithTriangularMF;
                classModel.@params[MF_TRIANGULAR_A] = classWithTriangularMF.A;
                classModel.@params[MF_TRIANGULAR_B] = classWithTriangularMF.B;
                classModel.@params[MF_TRIANGULAR_C] = classWithTriangularMF.C;
            }
            else if (clazz is CoreLogic.Classes.ClassWithTrapezoidalMF)
            {
                classModel.type = MF_TRAPEZOIDAL;
                var classWithTrapezoidalMF = clazz as CoreLogic.Classes.ClassWithTrapezoidalMF;
                classModel.@params[MF_TRAPEZOIDAL_A] = classWithTrapezoidalMF.A;
                classModel.@params[MF_TRAPEZOIDAL_B] = classWithTrapezoidalMF.B;
                classModel.@params[MF_TRAPEZOIDAL_C] = classWithTrapezoidalMF.C;
                classModel.@params[MF_TRAPEZOIDAL_D] = classWithTrapezoidalMF.D;
            }
            else if (clazz is CoreLogic.Classes.ClassWithGaussianMF)
            {
                classModel.type = MF_GAUSSIAN;
                var classWithGaussianMF = clazz as CoreLogic.Classes.ClassWithGaussianMF;
                classModel.@params[MF_GAUSSIAN_C] = classWithGaussianMF.C;
                classModel.@params[MF_GAUSSIAN_SIGMA] = classWithGaussianMF.Sigma;
            }
            else if (clazz is CoreLogic.Classes.ClassWithGeneralisedBellMF)
            {
                classModel.type = MF_GENERALISED_BELL;
                var classWithGeneralisedBellMF = clazz as CoreLogic.Classes.ClassWithGeneralisedBellMF;
                classModel.@params[MF_GENERALISED_BELL_A] = classWithGeneralisedBellMF.A;
                classModel.@params[MF_GENERALISED_BELL_B] = classWithGeneralisedBellMF.B;
                classModel.@params[MF_GENERALISED_BELL_C] = classWithGeneralisedBellMF.C;
            }
            else if (clazz is CoreLogic.Classes.ClassWithSigmoidDifferenceMF)
            {
                classModel.type = MF_SIGMOID_DIFF;
                var classWithSigmoidDifferenceMF = clazz as CoreLogic.Classes.ClassWithSigmoidDifferenceMF;
                classModel.@params[MF_SIGMOID_DIFF_A1] = classWithSigmoidDifferenceMF.A1;
                classModel.@params[MF_SIGMOID_DIFF_A2] = classWithSigmoidDifferenceMF.A2;
                classModel.@params[MF_SIGMOID_DIFF_C1] = classWithSigmoidDifferenceMF.C1;
                classModel.@params[MF_SIGMOID_DIFF_C2] = classWithSigmoidDifferenceMF.C2;
            }
            else
            {
                throw new ArgumentException($"Attempt to handle an unknown type: {clazz.GetType().FullName}.");
            }

            return classModel;
        }

        private static Models.Rule ConvertRuleToModel(CoreLogic.Rule rule)
        {
            return new Models.Rule {
                var_name = rule.Parameter.Name,
                class_name = rule.Class.Name,
                expr = ConvertExpressionToModel(rule.Expression)
            };
        }

        private static Models.Expression ConvertExpressionToModel(CoreLogic.Expressions.Expression expression)
        {
            var expressionModel = new Models.Expression();

            if (expression is CoreLogic.Expressions.Conjunction)
            {
                expressionModel.type = EXPR_CONJUNCTION;
                var conjunction = expression as CoreLogic.Expressions.Conjunction;
                expressionModel.left = ConvertExpressionToModel(conjunction.LeftArgument);
                expressionModel.right = ConvertExpressionToModel(conjunction.RightArgument);
            }
            else if (expression is CoreLogic.Expressions.Disjunction)
            {
                expressionModel.type = EXPR_DISJUNCTION;
                var disjunction = expression as CoreLogic.Expressions.Disjunction;
                expressionModel.left = ConvertExpressionToModel(disjunction.LeftArgument);
                expressionModel.right = ConvertExpressionToModel(disjunction.RightArgument);
            }
            else if (expression is CoreLogic.Expressions.Negation)
            {
                expressionModel.type = EXPR_NEGATION;
                var negation = expression as CoreLogic.Expressions.Negation;
                expressionModel.arg = ConvertExpressionToModel(negation.Argument);
            }
            else if (expression is CoreLogic.Expressions.MembershipStatement)
            {
                expressionModel.type = EXPR_STATEMENT;
                var statement = expression as CoreLogic.Expressions.MembershipStatement;
                expressionModel.var_name = statement.Parameter.Name;
                expressionModel.class_name = statement.Class.Name;
            }
            else
            {
                throw new ArgumentException($"Attempt to handle an unknown type: {expression.GetType().FullName}.");
            }

            return expressionModel;
        }

        private const string MF_TRIANGULAR = "triangular";
        private const string MF_TRIANGULAR_A = "a";
        private const string MF_TRIANGULAR_B = "b";
        private const string MF_TRIANGULAR_C = "c";

        private const string MF_TRAPEZOIDAL = "trapezoidal";
        private const string MF_TRAPEZOIDAL_A = "a";
        private const string MF_TRAPEZOIDAL_B = "b";
        private const string MF_TRAPEZOIDAL_C = "c";
        private const string MF_TRAPEZOIDAL_D = "d";

        private const string MF_GAUSSIAN = "gaussian";
        private const string MF_GAUSSIAN_C = "c";
        private const string MF_GAUSSIAN_SIGMA = "sigma";

        private const string MF_GENERALISED_BELL = "generalised_bell";
        private const string MF_GENERALISED_BELL_A = "a";
        private const string MF_GENERALISED_BELL_B = "b";
        private const string MF_GENERALISED_BELL_C = "c";

        private const string MF_SIGMOID_DIFF = "sigmoid_diff";
        private const string MF_SIGMOID_DIFF_A1 = "a1";
        private const string MF_SIGMOID_DIFF_A2 = "a2";
        private const string MF_SIGMOID_DIFF_C1 = "c1";
        private const string MF_SIGMOID_DIFF_C2 = "c2";

        private const string EXPR_CONJUNCTION = "and";
        private const string EXPR_DISJUNCTION = "or";
        private const string EXPR_STATEMENT = "state";
        private const string EXPR_NEGATION = "neg";
    }
}
