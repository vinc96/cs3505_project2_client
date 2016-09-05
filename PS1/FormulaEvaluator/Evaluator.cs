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
            Regex.Replace(exp, @"\s", "");
            //Split the passed string into tokens
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            //Set up stacks and start distributing the tokens into them
            Stack<string> values = new Stack<string>();
            Stack<string> operators = new Stack<string>();



            return 0; //TODO: whadayathink
        }
        /// <summary>
        /// Determines whether or not the passed token is a variable. In order to return true, the token must consist of one or more letters followed by one or more numbers.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsVariable(string token)
        {

        }
    }
}
