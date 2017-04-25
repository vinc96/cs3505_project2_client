// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

//Implemented By Joshua Christensen (u0978248)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        ///An array of strings containing the normalized formula for this object. Populated during construction.
        private List<string> normalizedFormula;

        /// vinc: formula format holder
        public InvalidFormat formaterror;
        /// vinc: indicate whether or not formula format valid
        public bool ValidFormat { get { return formaterror == null; } }

        ///A set containing valid arithmetic operators. Done to shorten conditionals. 
        private HashSet<string> validOps;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {

            normalizedFormula = new List<string>(); //Initialize our list of tokens.
            // vinc: init formaterror
            formaterror = null;

            validOps = new HashSet<string>(); //Initialize our valid operators.
            validOps.Add("+");
            validOps.Add("-");
            validOps.Add("*");
            validOps.Add("/");

            double currentDouble; //If we're working with a double when looking at a specific token, it's stored here.

            int openParenSoFar = 0; //The paren pairs we've seen so far.
            int closedParenSoFar = 0;

            //Remembers if the last token was a number, variable, or a close paren.
            //If this is false, the last token was an open paren, or an operator.
            bool lastTokenNumVarCloseParen;

            IEnumerator<string> tokens = GetTokens(formula).GetEnumerator();

            //Code for the first element
            if (!tokens.MoveNext())
            {
                formaterror = new InvalidFormat(formula, "You must have at least one token. Don't pass empty strings to the constructor.");
                return;
                //throw new FormulaFormatException("You must have at least one token. Don't pass empty strings to the constructor.");
            }
            else
            {
                if (!(Double.TryParse(tokens.Current, out currentDouble) || IsVariable(tokens.Current) || tokens.Current.Equals("(")))
                {
                    formaterror = new InvalidFormat(formula, "The formula must start with a variable, a number, or an open parenthesis. " +
                                                        "It starts with:" + tokens.Current + " , choose another thing to start with");
                    return;
                    //throw new FormulaFormatException("The formula must start with a variable, a number, or an open parenthesis. " +
                    //                                    "It starts with:" + tokens.Current +" , choose another thing to start with");
                }
                else
                {
                    //Case for variables.
                    if (IsVariable(tokens.Current))
                    {
                        lastTokenNumVarCloseParen = true;
                        string normalizedToken = normalize(tokens.Current);
                        if (isValid(normalizedToken) && IsVariable(normalizedToken))
                        {
                            normalizedFormula.Add(normalizedToken);
                        }
                        else
                        {
                            formaterror = new InvalidFormat(formula, "Normalized token is not valid:" + normalizedToken +
                                ". Check your normalizing and validating functions. Check the token at the" +
                                (normalizedFormula.Count + 1) + "place");
                            return;
                            //throw new FormulaFormatException("Normalized token is not valid:" + normalizedToken +
                            //    ". Check your normalizing and validating functions. Check the token at the" +
                            //    (normalizedFormula.Count + 1) + "place");
                        }
                    }
                    else if (tokens.Current.Equals("("))
                    { //case for open parenthesis
                        lastTokenNumVarCloseParen = false;
                        openParenSoFar++;
                        normalizedFormula.Add(tokens.Current);

                    }
                    else
                    {
                        //If we get to this point, it's a double.
                        lastTokenNumVarCloseParen = true;
                        normalizedFormula.Add(currentDouble.ToString());
                    }
                }
            }

            int tokenListLengthStart; //The length of the token list at the beginning of each loop. Declared here to avoid allocation overhead.
            while (tokens.MoveNext())
            {
                tokenListLengthStart = normalizedFormula.Count;

                if (tokens.Current.Equals(")"))
                {
                    //If the previous element was an op or open paren, throw.
                    if (!lastTokenNumVarCloseParen)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have a close paren following an operator or open paren. Check the token at the"
                                    + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have a close paren following an operator or open paren. Check the token at the"
                        //    + (normalizedFormula.Count + 1) + "place");
                    }

                    closedParenSoFar++;
                    if (closedParenSoFar > openParenSoFar)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have a close paren without an opening one. Called at the"
                            + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have a close paren without an opening one. Called at the"
                        //    + (normalizedFormula.Count + 1) + "place");
                    }
                    lastTokenNumVarCloseParen = true;
                    normalizedFormula.Add(tokens.Current);
                }

                if (tokens.Current.Equals("("))
                {
                    //If the previous element was a var, num, or close paren, throw.
                    if (lastTokenNumVarCloseParen)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have an open paren following a close paren, variable, or number." + tokens.Current +
                            "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have an open paren following a close paren, variable, or number." + tokens.Current +
                        //    "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                    }
                    openParenSoFar++;
                    lastTokenNumVarCloseParen = false;
                    normalizedFormula.Add(tokens.Current);
                }

                if (IsVariable(tokens.Current))
                {
                    if (lastTokenNumVarCloseParen)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have a variable following another number, variable or close paren." + tokens.Current +
                            "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have a variable following another number, variable or close paren." + tokens.Current +
                        //    "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                    }
                    string normalizedToken = normalize(tokens.Current);
                    if (isValid(normalizedToken) && IsVariable(normalizedToken))
                    {
                        lastTokenNumVarCloseParen = true;
                        normalizedFormula.Add(normalizedToken);
                    }
                    else
                    {
                        formaterror = new InvalidFormat(formula,
                            "Normalized token is not valid: " + normalizedToken +
                            "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("Normalized token is not valid: " + normalizedToken +
                        //    "Check the token at the" + (normalizedFormula.Count + 1) + "place");
                    }
                }

                if (Double.TryParse(tokens.Current, out currentDouble))
                {
                    if (lastTokenNumVarCloseParen)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have a number following another number, variable or close paren: Check the token at the"
                            + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have a number following another number, variable or close paren: Check the token at the"
                        //    + (normalizedFormula.Count + 1) + "place");
                    }
                    lastTokenNumVarCloseParen = true;
                    normalizedFormula.Add(currentDouble.ToString());
                }

                if (validOps.Contains(tokens.Current))
                {
                    if (!lastTokenNumVarCloseParen)
                    {
                        formaterror = new InvalidFormat(formula,
                            "You have an operator following another operator, or an open paren: Check the token at the"
                            + (normalizedFormula.Count + 1) + "place");
                        return;
                        //throw new FormulaFormatException("You have an operator following another operator, or an open paren: Check the token at the"
                        //    + (normalizedFormula.Count + 1) + "place");
                    }
                    lastTokenNumVarCloseParen = false;
                    normalizedFormula.Add(tokens.Current);
                }

                //If we haven't added a new token by this point, then we encountered an invalid token, and we need to throw
                if (tokenListLengthStart == normalizedFormula.Count)
                {
                    formaterror = new InvalidFormat(formula,
                        "Invalid Tokens:" + tokens.Current);
                    return;
                    //throw new FormulaFormatException("Invalid Tokens:" + tokens.Current);
                }
            }

            //The number of open parenthesis has to match the number of close ones:
            if (openParenSoFar != closedParenSoFar)
            {
                formaterror = new InvalidFormat(formula,
                    "Number of open parenthisis need to match the number of close parenthisis");
                return;
                //throw new FormulaFormatException("Number of open parenthisis need to match the number of close parenthisis");
            }

            //The last token can only be a number, variable or close parenthisis
            if (!lastTokenNumVarCloseParen)
            {
                formaterror = new InvalidFormat(formula,
                    "Invalid value for final token:" + tokens.Current);
                return;
                //throw new FormulaFormatException("Invalid value for final token:" + tokens.Current);
            }

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            // check if formula causes FormulaFormatException
            if (!ValidFormat)
            {
                return formaterror;
            }

            //Set up stacks and start distributing the tokens into them
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            //Loop over the tokens
            foreach (string token in normalizedFormula)
            {
                //Integer/Variable handling:
                double number;
                bool isDouble = false;
                if (IsVariable(token))
                {
                    try
                    {
                        number = lookup(token);
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Variable " + token + " Does Not Exist");
                    }
                    isDouble = true;
                }
                else
                {
                    isDouble = double.TryParse(token, out number);
                }

                //Double Handling
                if (isDouble)
                {
                    if (IsOnTop(operators, "*") || IsOnTop(operators, "/"))
                    {
                        try
                        {
                            values.Push(Arithmator(operators.Pop(), values.Pop(), number));
                        }
                        catch (DivideByZeroException)
                        {
                            return new FormulaError("Error: Divide by Zero");
                        }
                    }
                    else
                    {
                        values.Push(number);
                    }
                }

                //+ or - Operator Handling
                if (token.Equals("+") || token.Equals("-"))
                {
                    if (IsOnTop(operators, "+") || IsOnTop(operators, "-"))
                    {

                        double b = values.Pop();
                        double a = values.Pop();
                        values.Push(Arithmator(operators.Pop(), a, b));

                    }
                    operators.Push(token);
                }

                //* or / operator handling
                if (token.Equals("*") || token.Equals("/"))
                {
                    operators.Push(token);
                }

                //Parenthisis handling
                if (token.Equals("("))
                {
                    operators.Push(token);
                }
                if (token.Equals(")"))
                {
                    if (IsOnTop(operators, "+") || IsOnTop(operators, "-"))
                    {

                        double b = values.Pop();
                        double a = values.Pop();
                        values.Push(Arithmator(operators.Pop(), a, b));
                        operators.Pop(); //Pop the openparen from the operators stack

                    }

                    if (IsOnTop(operators, "*") || IsOnTop(operators, "/"))
                    {
                        try
                        {
                            double b = values.Pop();
                            double a = values.Pop();
                            values.Push(Arithmator(operators.Pop(), a, b));
                        }
                        catch (DivideByZeroException)
                        {
                            return new FormulaError("Error: Divide by Zero");
                        }
                    }

                }
            }

            //End behavior
            if (operators.Count == 0)
            {
                return values.Pop();
            }
            else
            {

                double b = values.Pop();
                double a = values.Pop();
                values.Push(Arithmator(operators.Pop(), a, b));
                return values.Pop();

            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> normalizedVars = new HashSet<string>();
            foreach (string str in normalizedFormula)
            {
                if (IsVariable(str))
                {
                    normalizedVars.Add(str);
                }
            }
            return normalizedVars;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string returnValue = "";

            foreach (string token in normalizedFormula)
            {
                returnValue = returnValue + token;
            }
            return returnValue;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //Check null references
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            //Check object type
            if (!this.GetType().Equals(obj.GetType()))
            {
                return false; //Type mismatch
            }

            Formula otherFormula = (Formula)obj;

            //Now, check the internal representations
            if (this.normalizedFormula.Count != otherFormula.normalizedFormula.Count)
            {
                return false; //Length Mismatch
            }

            //Check every single element
            for (int i = 0; i < normalizedFormula.Count; i++)
            {
                if (!this.normalizedFormula[i].Equals(otherFormula.normalizedFormula[i]))
                {
                    return false; //We encountered an element that's not the same.
                }
            }

            //If we haven't been proven wrong, the objects are equal.
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
            {
                return true;//Both are null, return true
            }
            else if (ReferenceEquals(f1, null)) //f1 is null, but f2 is not
            {
                return false;
            }
            else
            {
                return f1.Equals(f2); //f1 is for sure non-null, return if f2 is equal to it. 
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2); //This operation should be the inverse of (==). 
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode(); //String already has a pretty good hash function, use that.
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
        /// <summary>
        /// Returns true if the string passed to this function is a valid variable (IE: Is a letter or underscore 
        /// followed by zero or more letters, digits, or underscores.
        /// </summary>
        /// <param name="variablename"> The string to check.</param>
        /// <returns>True if the the string passed was a valid variable name, false if not.</returns>
        private static bool IsVariable(String varname)
        {
            if (!(varname[0].Equals('_') || Char.IsLetter(varname[0])))
            {
                return false; //The variable must start with an underscore or letter.
            }

            for (int i = 1; i < varname.Length; i++)
            {
                if (!(varname[i].Equals('_') || Char.IsLetterOrDigit(varname[i])))
                {
                    return false; //Return false if we encounter anything that's not a letter, digit, or underscore.
                }
            }

            return true; //If we haven't broken the rules, its valid.
        }
        /// <summary>
        /// Simply takes an operator, a value for a and b, and performs the specified operation on them, in the 
        /// order they were given. Has no inbuilt error handling, all exceptions will be passed up the stack. 
        /// Throws ArgumentException if it's passed an invalid operator, throws DivideByZeroException if 
        /// the operator is divide, and the second value is a zero.
        /// </summary>
        /// <param name="op">The operator to perform arthmetic with. Must be a +,-,* or a /. </param>
        /// <param name="a">The first value to perform math on. Ex: a/b, a*b, a+b.</param>
        /// <param name="b">The second value to perform math on. Ex: a/b, a*b, a+b.</param>
        /// <returns>A value resulting in the specified double arithmetic. </returns>
        private static double Arithmator(string op, double a, double b)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b == 0)
                    {
                        throw new DivideByZeroException();
                    }
                    else
                    {
                        return a / b;
                    }
                default:
                    throw new ArgumentException("Invalid Operator.");
            }
        }
        /// <summary>
        /// Checks to see if a specified stack has a specified element on the top of it.
        /// </summary>
        /// <typeparam name="T">The type of the stack</typeparam>
        /// <param name="Stack">The stack to look in</param>
        /// <param name="Value">The item to look for.</param>
        /// <returns></returns>
        private static bool IsOnTop<T>(Stack<T> Stack, T Element)
        {
            try
            {
                return Stack.Peek().Equals(Element);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class InvalidFormat
    {
        public string message;
        public string formula;
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public InvalidFormat(string formula, String message)
        {
            this.formula = formula;
            this.message = message;
        }
    }

    ///// <summary>
    ///// Used to report syntactic errors in the argument to the Formula constructor.
    ///// </summary>
    //public class FormulaFormatException : Exception
    //{
    //    /// <summary>
    //    /// Constructs a FormulaFormatException containing the explanatory message.
    //    /// </summary>
    //    public FormulaFormatException(String message)
    //        : base(message)
    //    {
    //    }
    //}

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
