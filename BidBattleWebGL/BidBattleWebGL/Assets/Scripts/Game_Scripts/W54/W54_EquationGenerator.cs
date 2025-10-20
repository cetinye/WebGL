using System;
using UnityEngine;
using W54;
using Random = UnityEngine.Random;

public class W54_EquationGenerator
{
    public W54_Equation GenerateEquation(W54_equationType type, int answerMaxValue)
    {
        W54_Equation equation = new W54_Equation();
        equation.equationType = type;
        switch (type)
        {
            case W54_equationType.SINGLE_DIGIT:
                GenerateSingleDigitEquation(equation);
                break;
            case W54_equationType.TWO_DIGIT:
                GenerateTwoDigitEquation(equation, answerMaxValue);
                break;
            case W54_equationType.TWO_OPS:
                GenerateTwoOpsEquation(equation, answerMaxValue);
                break;
            case W54_equationType.FOUR_OPS:
                GenerateFourOpsEquation(equation, answerMaxValue);
                break;
            case W54_equationType.ONE_PARENTHESIS:
                GenerateOneParenthesisEquation(equation, answerMaxValue);
                break;
            case W54_equationType.TWO_PARENTHESIS:
                GenerateTwoParenthesisEquation(equation, answerMaxValue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (equation.equationAnswer < 0)
        {
            return GenerateEquation(type, answerMaxValue);
        }
        
        return equation;
    }

    private void GenerateSingleDigitEquation(W54_Equation equation)
    {
        var rand = Random.Range(0, 10);
        
        equation.equationType = W54_equationType.SINGLE_DIGIT;
        equation.equationString = rand.ToString();
        equation.equationAnswer = rand;
    }
    
    private void GenerateTwoDigitEquation(W54_Equation equation, int maxVal)
    {
        var rand = Random.Range(10, maxVal);
        
        equation.equationType = W54_equationType.TWO_DIGIT;
        equation.equationString = rand.ToString();
        equation.equationAnswer = rand;
    }
    
    private void GenerateTwoOpsEquation(W54_Equation equation, int maxVal)
    {
        var randOp = (W54_OP)Random.Range(0, 2);
        equation.equationType = W54_equationType.TWO_OPS;

        if (randOp == W54_OP.ADD)
        {
            var firstOperand = Random.Range(1, maxVal);
            var secondOperand = Random.Range(1, maxVal - firstOperand);

            equation.equationString = firstOperand + " + " + secondOperand;
            equation.equationAnswer = firstOperand + secondOperand;
        }
        else
        {
            var firstOperand = Random.Range(1, maxVal);
            var secondOperand = Random.Range(1, firstOperand);

            equation.equationString = firstOperand + " - " + secondOperand;
            equation.equationAnswer = firstOperand - secondOperand;
        }
    }

    private void GenerateFourOpsEquation(W54_Equation equation, int maxVal)
    {
        var randOp = (W54_OP)Random.Range(0, 4);
        equation.equationType = W54_equationType.FOUR_OPS;

        switch (randOp)
        {
            case W54_OP.ADD:
            {
                var firstOperand = Random.Range(1, maxVal);
                var secondOperand = Random.Range(1, maxVal - firstOperand);

                equation.equationString = firstOperand + " + " + secondOperand;
                equation.equationAnswer = firstOperand + secondOperand;
                break;
            }
            case W54_OP.SUBTRACT:
            {
                var firstOperand = Random.Range(1, maxVal);
                var secondOperand = Random.Range(1, firstOperand);

                equation.equationString = firstOperand + " - " + secondOperand;
                equation.equationAnswer = firstOperand - secondOperand;
                break;
            }
            case W54_OP.MULTIPLY:
            {
                var firstOperand = Random.Range(1, maxVal);
                var secondOperand = Random.Range(1, maxVal / firstOperand);

                equation.equationString = firstOperand + " * " + secondOperand;
                equation.equationAnswer = firstOperand * secondOperand;
                break;
            }
            case W54_OP.DIVIDE:
            {
                var firstOperand = Random.Range(1, maxVal);
                var secondOperand = Random.Range(1, firstOperand);

                while ((firstOperand / (float)secondOperand) % 1 != 0)
                {
                    firstOperand = Random.Range(1, maxVal);
                    secondOperand = Random.Range(1, firstOperand);  
                }

                equation.equationString = firstOperand + " / " + secondOperand;
                equation.equationAnswer = (int)(firstOperand / secondOperand);
                break;
            }
        }
    }
    
    private void GenerateOneParenthesisEquation(W54_Equation equation, int maxVal)
    {
        equation.equationType = W54_equationType.ONE_PARENTHESIS;

        var tempEquation = new W54_Equation();
        GenerateFourOpsEquation(tempEquation, maxVal);
        var randOp = (W54_OP)Random.Range(0, 4);
        
        switch (randOp)
        {
            case W54_OP.ADD:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) + {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer + thirdOperand;
                } while (equation.equationAnswer > maxVal + 2);
                break;
            }
            case W54_OP.SUBTRACT:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) - {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer - thirdOperand;
                } while (equation.equationAnswer > maxVal + 2);
                break;
            }
            case W54_OP.MULTIPLY:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) * {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer * thirdOperand;
                } while (equation.equationAnswer > maxVal + 2);
                break;
            }
            case W54_OP.DIVIDE:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);
                    while (tempEquation.equationAnswer / thirdOperand % 1 != 0)
                    {
                        thirdOperand = Random.Range(1, maxVal);
                    }
                    
                    equation.equationString = $"({tempEquation.equationString}) / {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer / thirdOperand;
                } while (equation.equationAnswer > maxVal + 2);
                break;
            }
        }
    }
    
    private void GenerateTwoParenthesisEquation(W54_Equation equation, int maxVal)
    {
        equation.equationType = W54_equationType.TWO_PARENTHESIS;

        var tempEquation = new W54_Equation();
        GenerateOneParenthesisEquation(tempEquation, maxVal);
        var randOp = (W54_OP)Random.Range(0, 4);
        
        switch (randOp)
        {
            case W54_OP.ADD:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) + {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer + thirdOperand;
                } while (equation.equationAnswer > maxVal + 3);
                break;
            }
            case W54_OP.SUBTRACT:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) - {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer - thirdOperand;
                } while (equation.equationAnswer > maxVal + 3);
                break;
            }
            case W54_OP.MULTIPLY:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);

                    equation.equationString = $"({tempEquation.equationString}) * {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer * thirdOperand;
                } while (equation.equationAnswer > maxVal + 3);
                break;
            }
            case W54_OP.DIVIDE:
            {
                do
                {
                    var thirdOperand = Random.Range(1, maxVal);
                    while (tempEquation.equationAnswer / thirdOperand % 1 != 0)
                    {
                        thirdOperand = Random.Range(1, maxVal);
                    }
                    
                    equation.equationString = $"({tempEquation.equationString}) / {thirdOperand}";
                    equation.equationAnswer = tempEquation.equationAnswer / thirdOperand;
                } while (equation.equationAnswer > maxVal + 3);
                break;
            }
        }
    }
}