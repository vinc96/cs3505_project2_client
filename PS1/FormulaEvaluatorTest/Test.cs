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
            //Trivial Subtraction
            Console.WriteLine("Trivial Subtraction: " + (Evaluator.Evaluate("40 - 10", EmptyDeligate) == 30).ToString());
            //Trivial Division
            Console.WriteLine("Trivial Division: " + (Evaluator.Evaluate("12 / 3", EmptyDeligate) == 4).ToString());
            //Trivial Multiplication
            Console.WriteLine("Trivial Multiplication: " + (Evaluator.Evaluate("30 * 10", EmptyDeligate) == 300).ToString());
        }

    }
}
