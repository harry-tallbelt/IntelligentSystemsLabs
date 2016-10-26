using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonParser.Models;
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
            var rules = Enumerable.Empty<CoreLogic.Rule>();

            return new CoreLogic.Task(taskModel.name, inVars, outVars, rules);
        }

        private static CoreLogic.Parameter ConvertModelToParameter(Parameter input)
        {
            throw new NotImplementedException();
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

        private static Models.Parameter ConvertParameterToModel(CoreLogic.Parameter parameter)
        {
            return new Models.Parameter {
                name = parameter.Name,
                from = parameter.Range.LowerBoundary,
                to = parameter.Range.UpperBoundary,
                classes = parameter.Classes.Select(ConvertClassToModel).ToList()
            };
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
                var_name = null,                        // TODO: cannot fill while Class does not know about Parameter
                class_name = rule.Class.Name,
                expr = ConvertExpressionToModel(rule.Expression)
            };
        }

        private const string EXPR_CONJUNCTION = "and";
        private const string EXPR_DISJUNCTION = "or";
        private const string EXPR_STATEMENT = "state";
        private const string EXPR_NEGATION = "neg";

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
                expressionModel.var_name = null;      // TODO: cannot fill while Class does not know about Parameter
                expressionModel.class_name = null;    // TODO: cannot fill while Class does not know about Parameter
            }
            else
            {
                throw new ArgumentException($"Attempt to handle an unknown type: {expression.GetType().FullName}.");
            }

            return expressionModel;
        }
    }
}
