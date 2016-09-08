using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;

namespace FormulaEvaluatorTest
{
    class Test
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin Tests");
            ExceptionTests();
            StandardTests();
            Console.WriteLine("End Tests");
            Console.Read();
        }
        static int EmptyDeligate(String token)
        {
            throw new ArgumentException();
        }

        static int Always0Deligate(String token)
        {
            return 0;
        }
        static void ExceptionTests()
        {
            //We have to have something to process
            try
            {
                Evaluator.Evaluate("", EmptyDeligate);
                Console.WriteLine("\nEmpty String: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Empty String: TEST PASSED.");
            }
            //We don't allow nonvalid tokens
            try
            {
                Evaluator.Evaluate("1 + 3 #", EmptyDeligate);
                Console.WriteLine("\nInvalid Tokens: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Invalid Tokens: TEST PASSED.");
            }
            //We don't allow dividing by 0
            try
            {
                Evaluator.Evaluate("1 / (1 - 1)", EmptyDeligate);
                Console.WriteLine("\nDivide By 0: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Divide By 0: TEST PASSED.");
            }
            //We don't allow closing parenthesis before we open them
            try
            {
                Evaluator.Evaluate("1 / )(1 + 1)", EmptyDeligate);
                Console.WriteLine("\nClose no open Parenthesis: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Close no open Parenthesis: TEST PASSED.");
            }
            //We don't allow two sequential operators (+)
            try
            {
                Evaluator.Evaluate("1 + + 1", EmptyDeligate);
                Console.WriteLine("\nSequential Operators (+): TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Sequential Operators (+): TEST PASSED.");
            }
            //We don't allow two sequential operators (-)
            try
            {
                Evaluator.Evaluate("1 -- 1", EmptyDeligate);
                Console.WriteLine("\nSequential Operators (-): TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Sequential Operators (-): TEST PASSED.");
            }
            //We don't allow two sequential operators (*)
            try
            {
                Evaluator.Evaluate("1 ** 1", EmptyDeligate);
                Console.WriteLine("\nSequential Operators (*): TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Sequential Operators (*): TEST PASSED.");
            }

            //We don't allow two sequential operators (/)
            try
            {
                Evaluator.Evaluate("1 // 1", EmptyDeligate);
                Console.WriteLine("\nSequential Operators (/): TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Sequential Operators (/): TEST PASSED.");
            }
            //We don't allow invalid variables, despite looking similar to what we use.
            try
            {
                Evaluator.Evaluate("A1A2", Always0Deligate);
                Console.WriteLine("\nAlmost Variables: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Almost Variables: TEST PASSED.");
            }
            //We don't allow operators with only 1 operand
            try
            {
                Evaluator.Evaluate("1 + ", EmptyDeligate);
                Console.WriteLine("\nSingle Operand: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Single Operand: TEST PASSED.");
            }
            //We don't allow using variables that don't exist
            try
            {
                Evaluator.Evaluate("1 + A1", EmptyDeligate);
                Console.WriteLine("\nSingle Operand: TEST FAILED.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n" + e.ToString());
                Console.WriteLine("Single Operand: TEST PASSED.");
            }
        }
        static void StandardTests()
        {
            //Trivial Addition
            Console.WriteLine("Trivial Addition: " + (Evaluator.Evaluate("15 + 35", EmptyDeligate) == 50).ToString());
            Console.WriteLine("Trivial Addition: " + (Evaluator.Evaluate("559 + (10 - 19)", EmptyDeligate) == 550).ToString());
            //Trivial Subtraction
            Console.WriteLine("Trivial Subtraction 1: " + (Evaluator.Evaluate("40 - 10", EmptyDeligate) == 30).ToString());
            Console.WriteLine("Trivial Subtraction 2: " + (Evaluator.Evaluate("10 - 30", EmptyDeligate) == -20).ToString());
            //Trivial Division
            Console.WriteLine("Trivial Division 1: " + (Evaluator.Evaluate("12 / 3", EmptyDeligate) == 4).ToString());
            Console.WriteLine("Trivial Division 2: " + (Evaluator.Evaluate("55 / (0 - 5)", EmptyDeligate) == -11).ToString());
            //Trivial Multiplication
            Console.WriteLine("Trivial Multiplication: " + (Evaluator.Evaluate("30 * 10", EmptyDeligate) == 300).ToString());
            Console.WriteLine("Trivial Multiplication: " + (Evaluator.Evaluate("3 * (3 - 5)", EmptyDeligate) == -6).ToString());
            //Order of operations: multiplication vs addition
            Console.WriteLine("Order of operations: multiplication vs addition 1: " + (Evaluator.Evaluate("10 * 10 + 5", EmptyDeligate) == 105).ToString());
            Console.WriteLine("Order of operations: multiplication vs addition 2: " + (Evaluator.Evaluate("2 * 3 + 10", EmptyDeligate) == 16).ToString());
            //Order of operations: multiplication vs subtraction
            Console.WriteLine("Order of operations: multiplication vs subtraction 1: " + (Evaluator.Evaluate("10 * 10 - 5", EmptyDeligate) == 95).ToString());
            Console.WriteLine("Order of operations: multiplication vs subtraction 2: " + (Evaluator.Evaluate("2 * 3 - 10", EmptyDeligate) == -4).ToString());
            //Order of operations: division vs addition
            Console.WriteLine("Order of operations: division vs addition 1: " + (Evaluator.Evaluate("10 / 10 + 5", EmptyDeligate) == 6).ToString());
            Console.WriteLine("Order of operations: division vs addition 2: " + (Evaluator.Evaluate("2 / 3 + 10", EmptyDeligate) == 10).ToString());
            //Order of operations: division vs subtraction
            Console.WriteLine("Order of operations: division vs subtraction 1: " + (Evaluator.Evaluate("10 / 10 - 5", EmptyDeligate) == -4).ToString());
            Console.WriteLine("Order of operations: division vs subtraction 2: " + (Evaluator.Evaluate("2 / 3 - 10", EmptyDeligate) == -10).ToString());
            //Order of operations: multiplication vs Paren addition
            Console.WriteLine("Order of operations: multiplication vs Paren addition 1: " + (Evaluator.Evaluate("10 * (10 + 5)", EmptyDeligate) == 150).ToString());
            Console.WriteLine("Order of operations: multiplication vs Paren addition 2: " + (Evaluator.Evaluate("2 * (3 + 10)", EmptyDeligate) == 26).ToString());
            //Order of operations: multiplication vs Paren subtraction
            Console.WriteLine("Order of operations: multiplication vs Paren subtraction 1: " + (Evaluator.Evaluate("10 * (10 - 5)", EmptyDeligate) == 50).ToString());
            Console.WriteLine("Order of operations: multiplication vs Paren subtraction 2: " + (Evaluator.Evaluate("2 * (3 - 10)", EmptyDeligate) == -14).ToString());
            //Order of operations: division vs Paren addition
            Console.WriteLine("Order of operations: division vs Paren addition 1: " + (Evaluator.Evaluate("20 / (10 + 5)", EmptyDeligate) == 1).ToString());
            Console.WriteLine("Order of operations: division vs Paren addition 2: " + (Evaluator.Evaluate("2 / (3 + 10)", EmptyDeligate) == 0).ToString());
            //Order of operations: division vs Paren subtraction
            Console.WriteLine("Order of operations: division vs Paren subtraction 1: " + (Evaluator.Evaluate("10 / (10 - 5)", EmptyDeligate) == 2).ToString());
            Console.WriteLine("Order of operations: division vs Paren subtraction 2: " + (Evaluator.Evaluate("14 / (3 - 10)", EmptyDeligate) == -2).ToString());
            //Repeated addition
            Console.WriteLine("Repeated addition 1: " + (Evaluator.Evaluate("1 + 2 + 3", EmptyDeligate) == 6).ToString());
            Console.WriteLine("Repeated addition 2: " + (Evaluator.Evaluate("3 + 4 + 5", EmptyDeligate) == 12).ToString());
            //Repeated subtraction
            Console.WriteLine("Repeated subtraction 1: " + (Evaluator.Evaluate("1 - 2 - 3", EmptyDeligate) == -4).ToString());
            Console.WriteLine("Repeated subtraction 2: " + (Evaluator.Evaluate("3 - 4 - 5", EmptyDeligate) == -6).ToString());
            //Repeated multiplication
            Console.WriteLine("Repeated multiplication 1: " + (Evaluator.Evaluate("1 * 2 * 3", EmptyDeligate) == 6).ToString());
            Console.WriteLine("Repeated multiplication 2: " + (Evaluator.Evaluate("3 * 4 * 5", EmptyDeligate) == 60).ToString());
            //Repeated division
            Console.WriteLine("Repeated division 1: " + (Evaluator.Evaluate("1 / 2 / 3", EmptyDeligate) == 0).ToString());
            Console.WriteLine("Repeated division 2: " + (Evaluator.Evaluate("5 / 4 / 1", EmptyDeligate) == 1).ToString());

        }

    }
}
