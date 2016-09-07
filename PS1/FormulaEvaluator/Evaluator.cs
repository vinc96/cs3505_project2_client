using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);
        /// <summary>
        /// Evaluates integer arthmetic expressions, passed to the method via a string, and a delegate to return the value of any variables found in the string.
        /// </summary>
        /// <param name="exp"> An integer expression, represented in the form of a string consisting of parenthesis, the symbols +,-,*,/, 
        ///                    nonnegative integers, whitespace, and variables consisting of one or more letters followed by one or more digits.</param>
        /// <param name="variableEvaluator">A delegate that takes variables consisting of one or more letters followed by one or more digits, 
        ///                    and returns an associated integer value if the variable has one. Throws ArgumentException if the variable has no value. </param>
        /// <returns>An integer value from the evaluated string if it fits the aformentioned requirements.</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Strip all the whitespace out of the string.
            exp = Regex.Replace(exp, @"\s+", "");
            //Split the passed string into tokens
            string[] tokens = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //Set up stacks and start distributing the tokens into them
            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();

            //Loop over the tokens
            for (int i = 0; i < tokens.Length; i++)
            {
                //Ignore empty strings
                if (tokens[i].Equals(""))
                {
                    continue;
                }

                //If we hit an invalid token, escape.
                if (!IsValidToken(tokens[i]))
                {
                    throw new ArgumentException("Invalid Tokens");
                }

                //Integer/Variable handling:
                int integer;
                bool isInteger = false;
                if (IsVariable(tokens[i]))
                {
                    try
                    {
                        integer = variableEvaluator(tokens[i]);
                    }
                    catch (ArgumentException)
                    {
                        throw new ArgumentException("Invalid Variable " + tokens[i] + ", no value.)");
                    }
                    isInteger = true;
                }
                else
                {
                    isInteger = int.TryParse(tokens[i], out integer);
                }
                if (isInteger)
                {
                    if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                    {
                        try
                        {
                            values.Push(Arithmator(operators.Pop(), values.Pop(), integer));
                        }
                        catch (InvalidOperationException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Attempted to multiply or divide by a nonexistant value)");
                        }
                        catch (DivideByZeroException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Attemtped to divide by 0)");
                        }
                    }
                    else
                    {
                        values.Push(integer);
                    }
                }

                //+ or - Operator Handling
                if (tokens[i].Equals("+") || tokens[i].Equals("-"))
                {
                    if (operators.IsOnTop("+") || operators.IsOnTop("-"))
                    {
                        try
                        {
                            values.Push(Arithmator(operators.Pop(), values.Pop(), values.Pop()));
                        }
                        catch (InvalidOperationException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Too many plus or minus signs)");
                        }
                    }
                    operators.Push(tokens[i]);
                }

                //* or / operator handling
                if (tokens[i].Equals("*") || tokens[i].Equals("/"))
                {
                    operators.Push(tokens[i]);
                }

                //Parenthisis handling
                if (tokens[i].Equals("("))
                {
                    operators.Push(tokens[i]);
                }
                if (tokens[i].Equals(")"))
                {
                    if (operators.IsOnTop("+") || operators.IsOnTop("-"))
                    {
                        try
                        {
                            values.Push(Arithmator(operators.Pop(), values.Pop(), values.Pop()));
                        }
                        catch (InvalidOperationException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Too many plus or minus signs)");
                        }
                    }

                    if (!operators.Pop().Equals("("))
                    {
                        throw new ArgumentException("Error: Misplaced Parenthesis");
                    }

                    if (operators.IsOnTop("*") || operators.IsOnTop("/"))
                    {
                        try
                        {
                            values.Push(Arithmator(operators.Pop(), values.Pop(), values.Pop()));
                        }
                        catch (InvalidOperationException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Too many multiply or divide signs)");
                        }
                        catch (DivideByZeroException)
                        {
                            throw new ArgumentException("Error: Invalid Expression (Attemtped to divide by 0)");
                        }
                    }

                }
            }
            
            //End behavior
            if (operators.Count == 0)
            {
                if (values.Count == 1)
                {
                    return values.Pop();
                }
                else
                {
                    throw new ArgumentException("Error: No Empty Strings Allowed");
                }
            }
            else if (operators.Count == 1)
            {
                if (operators.IsOnTop("+") || operators.IsOnTop("-"))
                {
                    try
                    {
                        values.Push(Arithmator(operators.Pop(), values.Pop(), values.Pop()));
                    }
                    catch (InvalidOperationException)
                    {
                        throw new ArgumentException("Error: Invalid Expression (There exists an operator with only one operand)");
                    }
                    return values.Pop();
                }
                else
                {
                    throw new ArgumentException("Error: Invalid Expression (Left over * or /)");
                }
            }
            else
            {
                throw new ArgumentException("Leftover Operators");
            }
        }
        /// <summary>
        /// Simply takes an operator, a value for a and b, and performs the specified operation on them, in the order they were given. Has no inbuilt error handling, all exceptions will be passed up the stack. Throws ArgumentException if it's passed an invalid operator.
        /// </summary>
        /// <param name="op">The operator to perform arthmetic with. Must be a +,-,* or a /. </param>
        /// <param name="a">The first value to perform math on. Ex: a/b, a*b, a+b.</param>
        /// <param name="b">The second value to perform math on. Ex: a/b, a*b, a+b.</param>
        /// <returns>A value resulting in the specified integer arithmetic. </returns>
        private static int Arithmator(string op, int a, int b)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                default:
                    throw new ArgumentException("Invalid Operator.");
            }
        }
        /// <summary>
        /// Determines whether or not the passed token is a variable. In order to return true, the token must consist of one or more letters followed by one or more numbers.
        /// </summary>
        /// <param name="token">The token to check</param>
        /// <returns>True if the token is a valid variable name, false otherwise.</returns>
        private static bool IsVariable(string token)
        {

            //We have to start with a letter
            if (!Char.IsLetter(token[0]))
            {
                return false;
            }

            int index = 1;
            //Loop for iterating over the letter portion of the variable
            while (true)
            {
                if (Char.IsLetter(token[index]))
                {
                    //If we end the string without a number, it's not a valid variable.
                    if (index == token.Length - 1)
                    {
                        return false;
                    }
                    index++; //Keep looking at the next index, ensuring it's a letter, or moving on to looking at the numbers if it's a number 
                }
                else if (Char.IsDigit(token[index]))
                {
                    //Move on to the number loop
                    break;
                }
                else
                {
                    //If we encounter anything else, it's not a valid variable.
                    return false;
                }
            }
            //Loop for interating over the number portion of the variable.
            while (true)
            {
                if (Char.IsDigit(token[index]))
                {
                    if (index == token.Length - 1)
                    {
                        return true; //If we've gotten to this point without breaking the rules, the variable is valid.
                    }
                    index++; //Keep looking at numbers until we're done.
                }
                else
                {
                    return false; //If we ever encounter anything other than a number at this point, it's not a valid variable.
                }
            }
        }
        /// <summary>
        /// Checks to see if a specified element is on the top of this stack.
        /// </summary>
        /// <typeparam name="T">The type of the stack.</typeparam>
        /// <param name="stack">The stack to look at the top of.</param>
        /// <param name="element">The element to check.</param>
        /// <returns>true if the element is on top of this stack, false if the element isn't, or the stack is empty.</returns>
        private static bool IsOnTop<T>(this Stack<T> stack, T element)
        {
            try
            {
                return stack.Peek().Equals(element);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private static bool IsValidToken(string token)
        {
            if (IsVariable(token))
            {
                return true;
            }
            if (token.Equals("+"))
            {
                return true;
            }
            if (token.Equals("-"))
            {
                return true;
            }
            if (token.Equals("*"))
            {
                return true;
            }
            if (token.Equals("/"))
            {
                return true;
            }
            if (token.Equals("("))
            {
                return true;
            }
            if (token.Equals(")"))
            {
                return true;
            }
            //Numbers
            int doesntmatter;
            return int.TryParse(token, out doesntmatter);
        }
    }

}
