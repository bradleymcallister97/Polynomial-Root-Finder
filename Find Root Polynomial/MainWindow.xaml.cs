﻿using System.Windows;
using System.Text.RegularExpressions;
using System;

namespace Find_Root_Polynomial
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
       The user inputs a polynomial function and a range for the x axis. The program will output the roots of the function.

       Main Steps:
       1. User inputs a function and a range for the x axis
       2. The coefficients from the function will be placed into an array
       3. The coefficients will be used to continuously derive the function until the order is down to 2
       4. Finds the roots of each derivative function starting at the order 2 function and work upwards until it get to the original function 
       5. The roots of the function can be found by using the roots of the functions first derivative and the x axis ranges
       6. The roots are put in a string that is designed to be displayed to the user
       7. The string is displayed to the user
       */
        public class Polynomial
        {
// PRIVATE:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            private double[] coefficients = new double[30], roots = new double[20];
            private int order = 0, numRoots = 0;
            private string[] rangeAxisX, subStrings;
            private double[,] derivative = new double[20, 30];
            private string equation = "", rangeTxtBox = "";

//*******************************************************************************************************************************
            // Takes in an x value and the index value to know which derivative function to use
            // Calculates and returns the y value corresponding to the given x value of the specified derivative function 
//*******************************************************************************************************************************
            private double getY(double x, int functionIndex)
            {
                double y = 0;

                for (int i = 0; i <= order; i++)
                {
                    y += derivative[functionIndex, i] * Math.Pow(x, i * 1.0);
                }

                return y;
            }
//*******************************************************************************************************************************
            // Takes in 2 different x values and the index value to know which derivative function to use
            // Returns the x value in between the 2 given x values that corresponds to a y value of 0, on the derivative function that has been specified
//*******************************************************************************************************************************
            private double performBisection(double p1, double p2, int functionIndex)
            {
                double p3 = (p1 + p2) / 2;

                if (getY(p1, functionIndex) * getY(p2, functionIndex) > 0)
                {
                    return double.Parse(rangeAxisX[1]) + 100.0;
                }
                else
                {
                    double TOL = 1e-14, previouseP3 = 0;
                    while (getY(p3, functionIndex) > TOL || getY(p3, functionIndex) < TOL * -1)
                    {
                        if (getY(p3, functionIndex) * getY(p1, functionIndex) < 0)
                            p2 = p3;
                        else // if (p3 * p1 < 0)
                            p1 = p3;

                        previouseP3 = p3;
                        p3 = (p2 + p1) / 2;
                    }
                    return p3;
                }
            }
//*******************************************************************************************************************************
            // Uses the coefficients from the function (coefficients array)
            // Derives the function and puts the newly derived function into the derivative array
            // Repeats until the function has been derived down to a function of order 2
//*******************************************************************************************************************************
            private void findAllDerivatives()
            {
                int derivativeOrder = order;
                for (int k = 0; k <= order; k++)
                {
                    derivative[order - 2, k] = coefficients[k];
                }
                for (int j = order - 3; j >= 0; j--)
                {
                    for (int i = derivativeOrder; i > 0; i--)
                    {
                        derivative[j, i - 1] = derivative[j + 1, i] * i;
                    }
                    derivativeOrder--;
                }
            }

//*******************************************************************************************************************************
            // Uses the nth derivative of the function that has an order of 2
            // Computes the quadratic formula
//*******************************************************************************************************************************
            private void quadraticFormulaVoid()
            {
                if (derivative[0, 1] * derivative[0, 1] - 4 * derivative[0, 2] * derivative[0, 0] < 0)
                {
                    numRoots = 0;
                }
                else if (derivative[0, 2] == 0)
                {
                    if (derivative[0,1] != 0)
                    {
                        roots[0] = Convert.ToDouble(derivative[0, 0]) / Convert.ToDouble(derivative[0, 1]);
                        numRoots = 1; 
                    }
                    else
                    {
                        numRoots = 0;
                    }
                }
                else
                {
                    roots[0] = (-1 * derivative[0, 1] + Math.Sqrt(derivative[0, 1] * derivative[0, 1] - 4 * derivative[0, 2] * derivative[0, 0])) / (2 * derivative[0, 2]);
                    roots[1] = (-1 * derivative[0, 1] - Math.Sqrt(derivative[0, 1] * derivative[0, 1] - 4 * derivative[0, 2] * derivative[0, 0])) / (2 * derivative[0, 2]);
                    if (roots[0] > roots[1])
                    {
                        double temp = roots[1];
                        roots[1] = roots[0];
                        roots[0] = temp;
                    }
                    numRoots = 2;
                    if (roots[0] - roots[1] < 1e-8 && roots[0] - roots[1] > -1e-8)
                    {
                        numRoots = 1;
                        roots[1] = 0;
                    }
                }
            }

//*******************************************************************************************************************************
            // Uses the coefficients of the function given by the user (which has an order of 2) 
            // Returns false if the function does not have any roots
            // Returns true if there are roots and puts the roots into the roots array
//*******************************************************************************************************************************
            private bool quadraticFormulaBool()
            {
                if (coefficients[1] * coefficients[1] - 4 * coefficients[2] * coefficients[0] < 0)
                {
                    return false;
                }
                else
                {
                    roots[0] = (-1 * coefficients[1] + Math.Sqrt(coefficients[1] * coefficients[1] - 4 * coefficients[2] * coefficients[0])) / (2 * coefficients[2]);
                    roots[1] = (-1 * coefficients[1] - Math.Sqrt(coefficients[1] * coefficients[1] - 4 * coefficients[2] * coefficients[0])) / (2 * coefficients[2]);
                    if (roots[0] > roots[1])
                    {
                        double temp = roots[1];
                        roots[1] = roots[0];
                        roots[0] = temp;
                    }
                    numRoots = 2;
                    if (roots[0] - roots[1] < 1e-8 && roots[0] - roots[1] > -1e-8)
                    {
                        numRoots = 1;
                        roots[1] = 0;
                    }
                    return true;
                }
            }

//*******************************************************************************************************************************
            // Takes in the index for which substring to use and the coefficient index to know where to place the coefficient in the coefficient array
            // Determines the coefficient for that index of the substring array and places the value in the specified index for the coefficient array
//*******************************************************************************************************************************
            private void calculateCoefficient(int substringIndex, int coefficientIndex)
            {
                int n = 0, sign = 1, decimalIndex = 100, decimalValue = 1;
                while (n < subStrings[substringIndex].IndexOf("x"))
                {
                    if (subStrings[substringIndex][n] == '+')
                    { }
                    else if (subStrings[substringIndex][n] == '-')
                    {
                        sign = -1;
                    }
                    else if (subStrings[substringIndex][n] == '.')
                    {
                        decimalIndex = n;
                    }
                    else
                    {
                        if (n > decimalIndex)
                        {
                            decimalValue *= 10;
                        }
                        coefficients[coefficientIndex] = coefficients[coefficientIndex] * 10 + double.Parse((subStrings[substringIndex][n]).ToString());
                    }
                    n++;
                }
                coefficients[coefficientIndex] /= decimalValue;
                coefficients[coefficientIndex] *= sign;
            }

//*******************************************************************************************************************************
            // Takes in startIndex to indicate if the first char in the substring is the positive sign, the endIndex specifies the index of the string for where to stop searching for the second number
            // Takes in the substringIndex for which substring to use and the char to indicate the operation that needs to be performed
            // Takes in a divideByZero which will equal true if there is a division by 0
            // Finds a number on each side of the specified operation  
            // Computes a value using the 2 numbers found and the operation specified
            // Replaces the value found with the 2 numbers found and the operation specified into the substring
//*******************************************************************************************************************************
            private void performOperation(bool startIndex, int endIndex, int substringIndex, char operation, ref bool divideByZero)
            {
                int n1 = 0, n2 = int.Parse(subStrings[substringIndex].IndexOf(operation).ToString()) + 1, sign = 1;
                if (startIndex || substringIndex > 0)
                {
                    n1 = 1;
                }
                double num1 = 0, num2 = 0;

                while (n1 < int.Parse(subStrings[substringIndex].IndexOf(operation).ToString())) // get num on left side of the symbol
                {
                    num1 = num1 * 10 + int.Parse((subStrings[substringIndex][n1]).ToString());
                    n1++;
                }
                while (n2 < endIndex) // get num on right side of the symbol
                {
                    num2 = num2 * 10 + int.Parse((subStrings[substringIndex][n2]).ToString());
                    n2++;
                }
                if (subStrings[substringIndex][0] == '-')
                {
                    sign = -1;
                }
                if (operation == '*')
                {
                    subStrings[substringIndex] = num1 * num2 * sign + subStrings[substringIndex].Remove(0, n2);
                }
                else if (operation == '/')
                {
                    if (num2 == 0)
                    {
                        displayText = "Cannot divide by 0";
                        divideByZero = true;
                    }
                    else
                    {
                        subStrings[substringIndex] = num1 / num2 * sign + subStrings[substringIndex].Remove(0, n2);
                    }
                }
            }

//*******************************************************************************************************************************
            // Uses the substring array
            // Determines if there is a multiplication or division that needs to be calculated in each substring index
            // Calls performOperation to compute the operation
            // Returns false if there has been a division by 0
//*******************************************************************************************************************************
            private bool simplifyEquation()
            {
                bool divideByZero = false;

                Regex multiply = new Regex("[0-9]+[*]+[0-9]");
                Regex divide = new Regex("[0-9]+[/]+[0-9]");

                for (int i = 0; i < subStrings.Length; i++)
                {
                    if (multiply.IsMatch(subStrings[i]) && divide.IsMatch(subStrings[i]))
                    {
                        if (int.Parse(subStrings[i].IndexOf("/").ToString()) < int.Parse(subStrings[i].IndexOf("*").ToString()))
                        {
                            if (subStrings[i].IndexOf("x") != -1)
                            {
                                performOperation(subStrings[i][0] == '-', int.Parse(subStrings[i].IndexOf("*").ToString()), i, '/', ref divideByZero);
                                performOperation(subStrings[i][0] == '-', int.Parse(subStrings[i].IndexOf("x").ToString()), i, '*', ref divideByZero);
                            }
                            else
                            {
                                performOperation(subStrings[i][0] == '-', int.Parse(subStrings[i].IndexOf("*").ToString()), i, '/', ref divideByZero);
                                performOperation(subStrings[i][0] == '-', subStrings.Length - 1, i, '*', ref divideByZero);
                            }
                        }
                    }
                    else if (multiply.IsMatch(subStrings[i]))
                    {
                        if (subStrings[i].IndexOf("x") != -1)
                        {
                            performOperation(subStrings[i][0] == '-', int.Parse(subStrings[i].IndexOf("x").ToString()), i, '*', ref divideByZero);
                        }
                        else
                        {
                            performOperation(subStrings[i][0] == '-', subStrings.Length - 1, i, '*', ref divideByZero);
                        }
                    }
                    else if (divide.IsMatch(subStrings[i]))
                    {
                        if (subStrings[i].IndexOf("x") != -1)
                        {
                            performOperation(subStrings[i][0] == '-', int.Parse(subStrings[i].IndexOf("x").ToString()), i, '/', ref divideByZero);
                        }
                        else
                        {
                            performOperation(subStrings[i][0] == '-', subStrings.Length - 1, i, '/', ref divideByZero);
                        }
                    }
                }
                return !divideByZero;
            }

//*******************************************************************************************************************************
            // Uses the equation string and the substring array
            // Will place spaces in between coefficients
            // Splits the equation string by the spaces added and places the pieces into the substring array
//*******************************************************************************************************************************
            private void fixSpacing()
            {
                for (int i = 1; i < equation.Length; i++)
                {
                    if ((equation[i] == '-' || equation[i] == '+') && equation[i - 1] != ' ')
                    {
                        equation = equation.Insert(i, " ");
                        i++; // So the loop will skip over the - or + since a char has just been added to the string
                    }
                    if (equation[i] == ' ')
                    {
                        while (equation[i + 1] == ' ') // Will remove multiple succession of spaces
                        {
                            equation = equation.Remove(i + 1, 1);
                        }
                    }
                }

                // Splits equation string into individual sections that are placed in an array
                Regex regex = new Regex(" ");
                subStrings = regex.Split(equation);
            }

// PUBLIC:-----------------------------------------------------------------------------------------------------------------------------------------------------------
            public string displayText = "";

//*******************************************************************************************************************************
            // Takes in an function equation and range string
            // Puts the strings into corresponding private strings
            // Uses the fixSpacing function to split the equation into an array
//*******************************************************************************************************************************
            public Polynomial(string _equation, string _range)  
            {
                this.equation = _equation;
                this.rangeTxtBox = _range;
                fixSpacing();
            }

//*******************************************************************************************************************************
            // Uses the equation string
            // Returns true if any unwanted character is found in the string
//*******************************************************************************************************************************
            public bool ErrorFound()
            {
                if (simplifyEquation())
                {
                    Regex incorrectCharacter = new Regex("[^x0-9+-/.*^\\s]");
                    Regex tooManyDecimals = new Regex("[.]+[.]");
                    if (incorrectCharacter.IsMatch(equation))
                    {
                        displayText = "There is a use of an unknown character";
                        return true;
                    }
                    else if (tooManyDecimals.IsMatch(equation))
                    {
                        displayText = "There are too many decimals next to eachother";
                        return true;
                    }
                }
                return false;
            }

//*******************************************************************************************************************************
            // Uses the substring array 
            // Extracts the coefficients from the users inputed function and puts them in the coefficient array
//*******************************************************************************************************************************
            public void findCoefficients()
            {
                Regex rgx = new Regex("to");
                rangeAxisX = rgx.Split(rangeTxtBox);

                int currentOrder = 0;
                for (int k = 0; k < subStrings.Length; k++)
                {
                    currentOrder = 0;
                    if (subStrings[k].IndexOf("^") != -1)
                    {
                        int n = subStrings[k].IndexOf("^")+ 1;
                        while (n < subStrings[k].Length)
                        {
                            currentOrder = currentOrder * 10 + int.Parse((subStrings[k][n]).ToString());
                            n++;
                        }
                        if (currentOrder > order)
                        {
                            order = currentOrder;
                        }
                    }
                    else if (subStrings[k].IndexOf("x") != -1)
                    {
                        if (order == 0)
                            order = 1;
                    }
                }

                if (order == 1) 
                {
                    calculateCoefficient(0, 1);
                    if (subStrings.Length > 1)
                    {
                        coefficients[0] = double.Parse(subStrings[1]);
                    }
                }
                else 
                {
                    int i = 0;
                    while (i < subStrings.Length)
                    {
                        if (subStrings[i].IndexOf("x") == -1)
                        {
                            coefficients[0] = double.Parse(subStrings[i]);
                        }
                        else
                        {
                            if (subStrings[i].IndexOf("^") == -1)
                            {
                                calculateCoefficient(i, 1);
                            }
                            else
                            {
                                int n = subStrings[i].IndexOf("^") + 1, tempValue = 0; 
                                while (n < subStrings[i].Length)
                                {
                                    tempValue = tempValue * 10 + int.Parse((subStrings[i][n]).ToString());
                                    n++;
                                }
                                calculateCoefficient(i, tempValue);
                            }
                        }
                        i++;
                    }
                }
            }

//*******************************************************************************************************************************
            // Uses the roots array and displayText
            // Puts roots into displayText so they can be displayed to the user
//*******************************************************************************************************************************
            public void setDisplayText()
            {
                for (int i = 0; i < numRoots; i++)
                {
                    displayText = displayText + "x(" + (i + 1) + ") = " + Math.Round(roots[i],3);
                    if ((i+1) % 2 == 0) 
                    {
                        displayText = displayText + "\n";
                    }
                    else
                    {
                        displayText = displayText + "\t\t";
                    }
                }
                if (numRoots == 0)
                {
                    displayText = "There are no solutions";
                }
            }

//*******************************************************************************************************************************
            // Uses the coefficient array
            // Determines the roots of the function and puts them into the roots array
//*******************************************************************************************************************************
            public void determineRoots()
            {
                if (order > 2)
                {
                    findAllDerivatives();
                    quadraticFormulaVoid();
                    double[] criticalPoints = new double[10];
                    
                    for (int i = 1; i < order-1; i++)
                    {
                        Array.Clear(criticalPoints, 0, criticalPoints.Length);
                        criticalPoints[0] = double.Parse(rangeAxisX[0]);
                        int n = 1;
                        while (n < numRoots + 1)
                        {
                            criticalPoints[n] = roots[n - 1];
                            n++;
                        }
                        if (numRoots == 0)
                            n = 1;
                        numRoots = 0;
                        criticalPoints[n] = double.Parse(rangeAxisX[1]);
                        double temp = 0;
                        int count = 0;
                        for (int k = 0; k < n; k++)
                        {
                            temp = performBisection(criticalPoints[k], criticalPoints[k + 1], i);
                            if (temp > double.Parse(rangeAxisX[1])) // no real root found
                            { }
                            else
                            {
                                roots[count] = temp;
                                count++;
                                numRoots++;
                            }
                        }
                    }
                }
                else if (order == 2)
                {
                    if (!quadraticFormulaBool())
                    {
                        displayText = "There are no solutions";
                    }
                }
                else
                {
                    if (order == 1)
                    {
                        if (coefficients[0] == 0)
                        {
                            roots[0] = 0.0;
                        }
                        else
                        {
                            roots[0] = coefficients[0] * -1 / coefficients[1];
                        }
                        numRoots = 1;
                    }
                    else
                    {
                        if (subStrings[0] == "0")
                            displayText = "There are an infinate number of solutions";
                        else
                            displayText = "There are no solutions";
                    }
                }
            }
        }

//*******************************************************************************************************************************
        // Click the calculate button
//*******************************************************************************************************************************
        private void button_Click(object sender, RoutedEventArgs e)
        {

            Polynomial poly = new Polynomial(txtEqn.Text, txtRange.Text);

            if (!poly.ErrorFound()) // No error found in the equation string
            {
                poly.findCoefficients(); 

                poly.determineRoots();

                poly.setDisplayText();                
            }

            lblAnswer.Content = poly.displayText;
        }
    }
}
