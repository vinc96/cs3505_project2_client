//Written By Joshua Christensen (u0978248)
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        //Exception Cases*********************************************************************************

        //Invalid variable names should throw exceptions
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVariableThrows()
        {
            Formula f1 = new Formula("2 + 1ABC + 2");
        }

        //Invalid variable names should throw exceptions (We still have a baseline standard)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVariableThrowsLongerConstructor()
        {
            Formula f1 = new Formula("2 + 1ABC + 2", s=>s, s=>true);
        }

        //We should adhere to the stricter of the two standards we established (This tests the Normalize behavior).
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVariableThrowsStricterRulesNormalize()
        {
            Formula f1 = new Formula("A1 + 2", s => "1ABC", s => true);
        }

        //We should adhere to the stricter of the two standards we established (This tests the isValid behavior).
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVariableThrowsStricterRulesIsValid()
        {
            Formula f1 = new Formula("A1 + 2", s => s, s => false);
        }

        //Invalid tokens should throw exceptions (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidTokensShortConstructor()
        {
            Formula f1 = new Formula("2 + $");
        }

        //Invalid tokens should throw exceptions (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidtokensLongConstructor()
        {
            Formula f1 = new Formula("3 + $", s => s, s => true);
        }

        //Invalid variables should throw exceptions (Using a decmal, which is illegal in a var name) (Short Constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVarNameShortConstructor()
        {
            Formula f1 = new Formula("2 + A2.3");
        }

        //Invalid variables should throw exceptions (Using a decmal, which is illegal in a var name) (Long Constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVarNameLongConstructor()
        {
            Formula f1 = new Formula("2 + A2.3", s => s, s => true);
        }

        //Invalid variables should throw exceptions (Starts normal, but encounters an illegal character) (Short Constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVarNameIllegalCharShortConstructor()
        {
            Formula f1 = new Formula("2 + A23_$$");
        }

        //Invalid variables should throw exceptions (Starts normal, but encounters an illegal character) (Long Constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidVarNameIllegalCharLongConstructor()
        {
            Formula f1 = new Formula("2 + A23_$$", s => s, s => true);
        }

        //We need at least one token (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOneTokenShortConstructor()
        {
            Formula f1 = new Formula(" ");
        }

        //We need at least one token (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOneTokenLongConstructor()
        {
            Formula f1 = new Formula(" ", s => s, s => true);
        }

        //We have to open parenthesis before we close them (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseParenShortConstructor()
        {
            Formula f1 = new Formula("1 + 2) + 1");
        }

        //We have to open parenthesis before we close them (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseParenLongConstructor()
        {
            Formula f1 = new Formula("1 + 2) + 1", s => s, s => true);
        }

        //We have to close parenthesis that we open (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseOpenParenShortConstructor()
        {
            Formula f1 = new Formula("(1 + 2");
        }

        //We have to close parenthesis that we open (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseOpenParenLongConstructor()
        {
            Formula f1 = new Formula("(1 + 2", s => s, s => true);
        }

        //The first token must be a number, variable or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicStartingTokenShortConstructor()
        {
            Formula f1 = new Formula("+ 2 + 4");
        }

        //The first token must be a number, variable or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicStartingTokenLongConstructor()
        {
            Formula f1 = new Formula("+ 1 + 2", s => s, s => true);
        }

        //The last token must be a number, variable or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicEndingTokenShortConstructor()
        {
            Formula f1 = new Formula("1 + 2 + ");
        }

        //The last token must be a number, variable or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicEndingTokenLongConstructor()
        {
            Formula f1 = new Formula("1 + 2 + ", s => s, s => true);
        }

        //Any token following an openparen must be a number, var, or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOpenParenTailShortConstructor()
        {
            Formula f1 = new Formula("() + 2 + 1)");
        }

        //Any token following an openparen must be a number, var, or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOpenParenTailLongConstructor()
        {
            Formula f1 = new Formula("() + 2 + 1)", s => s, s => true);
        }

        //Any token following an openparen must be a number, var, or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOperatorTailShortConstructor()
        {
            Formula f1 = new Formula("+ + 2 + 1)");
        }

        //Any token following an openparen must be a number, var, or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOperatorTailLongConstructor()
        {
            Formula f1 = new Formula("+ + 2 + 1)", s => s, s => true);
        }

        //Any token following a number must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicNumberTailShortConstructor()
        {
            Formula f1 = new Formula("2 + 1 A1");
        }

        //Any token following a number must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicNumberTailLongConstructor()
        {
            Formula f1 = new Formula("2 + 1 A1", s => s, s => true);
        }

        //Any token following a variable must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicVarTailShortConstructor()
        {
            Formula f1 = new Formula("2 + A1 1");
        }

        //Any token following a variable must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicVarTailLongConstructor()
        {
            Formula f1 = new Formula("2 + A1 1", s => s, s => true);
        }

        //Any token following a close paren must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseParenTailShortConstructor()
        {
            Formula f1 = new Formula("(2 + 1) A1");
        }

        //Any token following a close paren must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicCloseParenTailLongConstructor()
        {
            Formula f1 = new Formula("(2 + 1) A1", s => s, s => true);
        }

        //We shouldn't be able to parse anything larger than Double's max value (1.7e308) (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicDoubleMaxValueParseFailureShortConstructor()
        {
            Formula f1 = new Formula("1 + 2e308");
        }

        //We shouldn't be able to parse anything larger than Double's max value (1.7e308) (Long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicDoubleMaxValueParseFailureLongConstructor()
        {
            Formula f1 = new Formula("1 + 2e308");
        }

        //We can't have parenthesis following some number, without an operator in between
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOpenParenCantFollowNumber()
        {
            Formula f1 = new Formula("1(1 + 3)");
        }

        //We can't have parenthesis following some close paren, without an operator in between
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicOpenParenCantFollowCloseParen()
        {
            Formula f1 = new Formula("(1 + 2)(1 + 3)");
        }

        //If our normalizer produces invalid tokens, we have to throw.
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicInvalidNormalizer()
        {
            Formula f1 = new Formula("1 + A2 + A3", s => "2Invalid4U$$$", s => true);
        }

        //Double operator sequences are not valid
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void PublicSequencesOfOperators()
        {
            Formula f1 = new Formula("a + + 2");
        }

        //Normal Constructor Behavior.********************************************************************************

        //A single variable is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicSingleVariableShortConstructor()
        {
            Formula f1 = new Formula("A1");
        }

        //A single variable is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicSingleVariableLongConstructor()
        {
            Formula f1 = new Formula("A1", s => s, s => true);
        }

        //Variables can start with underscores. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicUnderscoreVariableShortConstructor()
        {
            Formula f1 = new Formula("_A1");
        }

        //Variables can start with underscores. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicUnderscoreVariableLongConstructor()
        {
            Formula f1 = new Formula("_A1", s => s, s => true);
        }

        //Variables can contain underscores. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicContainsUnderscoreVariableShortConstructor()
        {
            Formula f1 = new Formula("A1_2");
        }

        //Variables can contain underscores. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicContainsUnderscoreVariableLongConstructor()
        {
            Formula f1 = new Formula("A1_2", s => s, s => true);
        }

        //A single number is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicSingleNumberShortConstructor()
        {
            Formula f1 = new Formula("1");
        }

        //A single number is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicSingleNumberLongConstructor()
        {
            Formula f1 = new Formula("1", s => s, s => true);
        }

        //A single number in scientific notation is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicSingleNumberSciNotationShortConstructor()
        {
            Formula f1 = new Formula("1e23");
        }

        //A single number in scientific notation is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicSingleNumberSciNotationLongConstructor()
        {
            Formula f1 = new Formula("1e23", s => s, s => true);
        }

        //Basic addition is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicAdditionShortConstructor()
        {
            Formula f1 = new Formula("1 + 1");
        }

        //Basic addition is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicAdditionLongConstructor()
        {
            Formula f1 = new Formula("1 + 1", s => s, s => true);
        }

        //Basic subtraction is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicSubtractionShortConstructor()
        {
            Formula f1 = new Formula("1 - 1");
        }

        //Basic subtraction is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicSubtractionLongConstructor()
        {
            Formula f1 = new Formula("1 - 1", s => s, s => true);
        }

        //Basic multiplication is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicMultiplicationShortConstructor()
        {
            Formula f1 = new Formula("1 * 1");
        }

        //Basic multiplication is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicMultiplicationLongConstructor()
        {
            Formula f1 = new Formula("1 * 1", s => s, s => true);
        }

        //Basic Division is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicDivisionShortConstructor()
        {
            Formula f1 = new Formula("1 / 1");
        }

        //Basic Division is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicDivisionLongConstructor()
        {
            Formula f1 = new Formula("1 / 1", s => s, s => true);
        }

        //Basic addition is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicAdditionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 + A1");
        }

        //Basic addition is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicAdditionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 + A1", s => s, s => true);
        }

        //Basic subtraction is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicSubtractionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 - A1");
        }

        //Basic subtraction is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicSubtractionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 - A1", s => s, s => true);
        }

        //Basic multiplication is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicMultiplicationVarsShortConstructor()
        {
            Formula f1 = new Formula("1 * A1");
        }

        //Basic multiplication is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicMultiplicationVarsLongConstructor()
        {
            Formula f1 = new Formula("1 * A1", s => s, s => true);
        }

        //Basic Division is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicBasicDivisionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 / A1");
        }

        //Basic Division is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicBasicDivisionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 / A1", s => s, s => true);
        }

        //Equations consisting of only variables are valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void PublicOnlyVarsShortConstructor()
        {
            Formula f1 = new Formula("A2 / A1");
        }

        //Equations consisting of only variables are valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void PublicOnlyVarsLongConstructor()
        {
            Formula f1 = new Formula("A2 / A1", s => s, s => true);
        }

        //Equations containing all sorts of crazy combinaitons of operators are allowed. (Short Constructor)
        [TestMethod]
        public void PublicCrazyEqnShortConstructor()
        {
            Formula f1 = new Formula("1 + 145 - 345/(224-2) + 554/22 - (445) + 1456 + (1 - 99) / 2");
        }

        //Equations containing all sorts of crazy combinaitons of operators are allowed. (Long Constructor)
        [TestMethod]
        public void PublicCrazyEqnLongConstructor()
        {
            Formula f1 = new Formula("1 + 145 - 345/(224-2) + 554/22 - (445) + 1456 + (1 - 99) / 2", s => s, s => true);
        }

        //Evaluate Failure Cases:************************************************************************************
        
        //Divide by zero (simple):
        [TestMethod]
        public void PublicEvaluateDivideByZero()
        {
            Formula f1 = new Formula("1 + 2 + 3 / 0");
            FormulaError result = (FormulaError)f1.Evaluate(s => 1);
            Assert.AreEqual("Error: Divide by Zero", result.Reason);

        }

        //Divide by zero (As a result of some operation):
        [TestMethod]
        public void PublicEvaluateDivideByZeroOperation()
        {
            Formula f1 = new Formula("10 / (1 + 2 + 3 - 6)");
            FormulaError result = (FormulaError) f1.Evaluate(s => 1);
            Assert.AreEqual("Error: Divide by Zero", result.Reason);

        }

        //Divide by zero (Because some variable evaluated to 0):
        [TestMethod]
        public void PublicEvaluateDivideByZeroVariable()
        {
            Formula f1 = new Formula("1 + 2 + 3 / A2");
            FormulaError result = (FormulaError)f1.Evaluate(s => 0);
            Assert.AreEqual("Error: Divide by Zero", result.Reason);
        }

        //Variables don't exist
        [TestMethod]
        public void PublicEvaluateVarDNE()
        {
            Formula f1 = new Formula("1 + 2 + 3 / A2");
            FormulaError result = (FormulaError)f1.Evaluate(s => { throw new ArgumentException(); });
            Assert.AreEqual("Variable A2 Does Not Exist", result.Reason);
        }

        //Evaluate Standard Cases ***********************************************************************************
        //Multiplication by 0
        [TestMethod]
        public void PublicEvaluateMultiplyByZero()
        {
            Formula f1 = new Formula("10 * (10 - 10)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(0, result, 1e-9);
        }
        //Trivial Addition 1 (Standard Addition)
        [TestMethod]
        public void PublicEvaluateTrivialAddition1()
        {
            Formula f1 = new Formula("15 + 35");
            double result = (double) f1.Evaluate(s => 0);
            Assert.AreEqual(50, result, 1e-9);
        }
        //Trivial Addition 2 (Decimal Addition)
        [TestMethod]
        public void PublicEvaluateTrivialAddition2()
        {
            Formula f1 = new Formula("153.02 + 45.003");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(198.02300, result, 1e-9);
        }
        //Trivial Addition 3 (Negative Addition)
        [TestMethod]
        public void PublicEvaluateTrivialAddition3()
        {
            Formula f1 = new Formula("150 + (0 - 45)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(105, result, 1e-9);
        }
        //Trivial Subtraction 1 (Standard subtraction)
        [TestMethod]
        public void PublicEvaluateTrivialSubtraction1()
        {
            Formula f1 = new Formula("40 - 10");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(30, result, 1e-9);
        }
        //Trivial Subtraction 2 (Decimal Point Subtraction)
        [TestMethod]
        public void PublicEvaluateTrivialSubtraction2()
        {
            Formula f1 = new Formula("1 - .12516");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(0.87484, result, 1e-9);
        }
        //Trivial Subtraction 3 (Double Negative subtraction)
        [TestMethod]
        public void PublicEvaluateTrivialSubtraction3()
        {
            Formula f1 = new Formula("0 - (0 - 14565)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(14565, result, 1e-9);
        }
        //Trivial Subtraction 4 (Negative Result)
        [TestMethod]
        public void PublicEvaluateTrivialSubtraction4()
        {
            Formula f1 = new Formula("(0 - 14565)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-14565, result, 1e-9);
        }
        //Trivial Division 1 (Standard Division)
        [TestMethod]
        public void PublicEvaluateTrivialDivision1()
        {
            Formula f1 = new Formula("25 / 5");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(5, result, 1e-9);
        }
        //Trivial Division 2 (Division using and resulting in negatives)
        [TestMethod]
        public void PublicEvaluateTrivialDivision2()
        {
            Formula f1 = new Formula("25 / (0-5)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-5, result, 1e-9);
        }
        //Trivial Division 3 (Division resulting in decimals)
        [TestMethod]
        public void PublicEvaluateTrivialDivision3()
        {
            Formula f1 = new Formula("10 / 4");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(2.5, result, 1e-9);
        }
        //Trivial Division 4 (Division inside parenthesis)
        [TestMethod]
        public void PublicEvaluateTrivialDivision4()
        {
            Formula f1 = new Formula("(0 / 148) - 32");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-32, result, 1e-9);
        }
        //Trivial Multiplication 1 (Standard Multiplication)
        [TestMethod]
        public void PublicEvaluateTrivialMultiplication1()
        {
            Formula f1 = new Formula("25 * 5");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(125, result, 1e-9);
        }
        //Trivial Multiplication 2 (Decimal Multiplication)
        [TestMethod]
        public void PublicEvaluateTrivialMultiplication2()
        {
            Formula f1 = new Formula("13 * 5.5");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(71.5, result, 1e-9);
        }
        //Trivial Multiplication 3 (Multiplication using and resulting in negatives)
        [TestMethod]
        public void PublicEvaluateTrivialMultiplication3()
        {
            Formula f1 = new Formula("148 * (0 - 32)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-4736, result, 1e-9);
        }
        //Trivial Multiplication 4 (Multiplication inside parenthesis)
        [TestMethod]
        public void PublicEvaluateTrivialMultiplication4()
        {
            Formula f1 = new Formula("(148 * 0) - 32");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-32, result, 1e-9);
        }
        //Order of Operations: Multiplication vs Addition
        [TestMethod]
        public void PublicEvaluateOrderOpsMultVsAdd()
        {
            Formula f1 = new Formula("10 * 10 + 5");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(105, result, 1e-9);
        }
        //Order of Operations: Multiplication vs Subtraction
        [TestMethod]
        public void PublicEvaluateOrderOpsMultVsSub()
        {
            Formula f1 = new Formula("2 * 3 - 10");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-4, result, 1e-9);
        }
        //Order of Operations: Division vs Addition
        [TestMethod]
        public void PublicEvaluateOrderOpsDivisVsAdd()
        {
            Formula f1 = new Formula("10 / 10 + 5");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(6, result, 1e-9);
        }
        //Order of Operations: Division vs Subtraction
        [TestMethod]
        public void PublicEvaluateOrderOpsDivisVsSub()
        {
            Formula f1 = new Formula("15 / 10 - 10");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-8.5, result, 1e-9);
        }
        //Order of Operations: Multiplication vs Paren Addition
        [TestMethod]
        public void PublicEvaluateOrderOpsMultVsParenAdd()
        {
            Formula f1 = new Formula("15 * (10 + 10)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(300, result, 1e-9);
        }
        //Order of Operations: Multiplication vs Paren Subtraction
        [TestMethod]
        public void PublicEvaluateOrderOpsMultVsParenSub()
        {
            Formula f1 = new Formula("15 * (10 - 5)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(75, result, 1e-9);
        }
        //Order of Operations: Division vs Paren Addition
        [TestMethod]
        public void PublicEvaluateOrderOpsDivisVsParenAdd()
        {
            Formula f1 = new Formula("15 / (10 + 5)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(1, result, 1e-9);
        }
        //Order of Operations: Division vs Paren Subtraction
        [TestMethod]
        public void PublicEvaluateOrderOpsDivisVsParenSub()
        {
            Formula f1 = new Formula("43 / (10 - 5)");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(8.6, result, 1e-9);
        }
        //Repeated Addition
        [TestMethod]
        public void PublicEvaluateRepeatedAddition()
        {
            Formula f1 = new Formula("4.25 + 8.34 + 55.2 + 10000000 + 23.2345");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(10000091.0245, result, 1e-9);
        }
        //Repeated Subtraction
        [TestMethod]
        public void PublicEvaluateRepeatedSubtraction()
        {
            Formula f1 = new Formula("4.25 - 8.34 - 55.2 - 10000000 - 23.2345");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(-10000082.5245, result, 1e-9);
        }
        //Repeated Multiplication
        [TestMethod]
        public void PublicEvaluateRepeatedMultiplication()
        {
            Formula f1 = new Formula("4.25 * 8.34 * 55.2 * 10000000 * 23.2345");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(454597862580, result, 1e-9);
        }
        //Repeated Division
        [TestMethod]
        public void PublicEvaluateRepeatedDivision()
        {
            Formula f1 = new Formula("4.25 / 8.34 / 5.2 / 12 / 2.2345");
            double result = (double)f1.Evaluate(s => 0);
            Assert.AreEqual(0.00365475215, result, 1e-9);
        }
        //Using Variables 1:
        [TestMethod]
        public void PublicEvaluateUsingVariables1()
        {
            Formula f1 = new Formula("a3 + 4 + B3 / 2");
            double result = (double)f1.Evaluate(s => 1);
            Assert.AreEqual(5.5, result, 1e-9);
        }
        //Using Variables 2:
        [TestMethod]
        public void PublicEvaluateUsingVariables2()
        {
            Formula f1 = new Formula("_XyC * 10 * cFaD - 22");
            double result = (double)f1.Evaluate(s => 13);
            Assert.AreEqual(1668, result, 1e-9);
        }
        //Using Variables 3:
        [TestMethod]
        public void PublicEvaluateUsingVariables3()
        {
            Formula f1 = new Formula("_XyC * cFaD * ThisISTechnicallyAValidVARIABLE");
            double result = (double)f1.Evaluate(s => 2.2345);
            Assert.AreEqual(11.1568367136, result, 1e-9);
        }

        //ToString ****************************************************************************************

        //Trivial case of ToString:
        [TestMethod]
        public void PublicTrivialToString()
        {
            Formula f1 = new Formula("A1 + A2 - B3 * A4 / B5");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(f1, f2);
        }

        //Trivial case of ToString with uppercase and lower case:
        [TestMethod]
        public void PublicTrivialUpperAndLowerToString()
        {
            Formula f1 = new Formula("A1 + a2 - A3 * b4 / B5");
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual(f1, f2);
        }

        //The returned string must contain no spaces:
        [TestMethod]
        public void PublicNoSpacesToString()
        {
            Formula f1 = new Formula("A1 + A2 - A3 * A4 / A5");
            string stringified = f1.ToString();

            foreach (char c in stringified)
            {
                Assert.AreNotEqual(' ', c); //there should be no spaces
            }
        }

        //All tokens in the string must be normalized
        [TestMethod]
        public void PublicAllNormalizedToString()
        {
            Formula f1 = new Formula("A1 + A2 - A3 * A4 / A5", s=>s.ToLower(), s=>true);
            Formula f2 = new Formula(f1.ToString());
            Assert.AreEqual("a1+a2-a3*a4/a5", f1.ToString());
            Assert.AreEqual(f1, f2);
        }

        //Equals***************************************************************************

        //If an object is null, it cant be equal to a formula object
        [TestMethod]
        public void PublicNullObjectEquals()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsFalse(f1.Equals(null));
        }

        //If an object is not a formula object, it cant be equal to a formula object
        //Tricky, because the string equals the normalized formula
        [TestMethod]
        public void PublicNotForumlaObjectEquals()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsFalse(f1.Equals("A1+5/2+3"));
        }

        //Tokens must be in the same order if they are to be equal
        [TestMethod]
        public void PublicOutOfOrderEquals()
        {
            Formula f1 = new Formula("2 + 3 + A2");
            Formula f2 = new Formula("3 + 2 + A2");
            Assert.IsFalse(f1.Equals(f2));
        }

        //Numbers are compared numerically
        [TestMethod]
        public void PublicNumericComparrisonEquals()
        {
            Formula f1 = new Formula("20.000000 + A2 / 3");
            Formula f2 = new Formula("20 + A2 / 3");
            Formula f3 = new Formula("2e1 + A2 / 3");
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f3));
        }

        //Variables are compared using their normalized versions (True case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonEqualsTrueCase()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1", s=>s.ToUpper(), s=>true);
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1", s => s.ToUpper(), s => true);
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f2.Equals(f3));
        }

        //Variables are compared using their normalized versions (False case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonEqualsFalseCase()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1");
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1");
            Assert.IsFalse(f1.Equals(f2));
            Assert.IsFalse(f1.Equals(f3));
            Assert.IsFalse(f2.Equals(f3));
        }

        //Equations have to have the same tokens to be equal (operand case)
        [TestMethod]
        public void PublicSameTokensEqualsOperandCase()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3");
            Assert.IsFalse(f1.Equals(f2));
        }

        //Equations have to have the same tokens to be equal (Operator case)
        [TestMethod]
        public void PublicSameTokensEqualsOperatorCase()
        {
            Formula f1 = new Formula("1 - 2 + 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A2");
            Assert.IsFalse(f1.Equals(f2));
        }

        //Equations have to have the same tokens to be equal (Extra operands case)
        [TestMethod]
        public void PublicSameTokensEqualsExtraOperandCase()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3 + Z3");
            Assert.IsFalse(f1.Equals(f2));
        }

        //Spaces ought to be ignored.
        [TestMethod]
        public void PublicSpacesIgnoredEquals()
        {
            Formula f1 = new Formula("_XaF        +       1    / 2 - A3f_x");
            Formula f2 = new Formula("_XaF+1/2-A3f_x");
            Assert.IsTrue(f1.Equals(f2));
        }

        //If our normalizer converts everything to the same variable, equals ought to still work.
        [TestMethod]
        public void PublicRepeatEqualizer()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1", s=>"SAME", s=>true);
            Formula f2 = new Formula("A1 + A2 - B1 * B2 / C1", s => "SAME", s => true);
            Assert.IsTrue(f1.Equals(f2));
        }

        //If both variables reference the same object, they must be equal.
        [TestMethod]
        public void PublicSameReferenceEquals()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1 + 1");
            Assert.IsTrue(f1.Equals(f1));
        }

        //Equals Operator Tests *************************************************************************************************************

        //If an object is null, it cant be equal to a formula object (null on right)
        [TestMethod]
        public void PublicNullRightObjectEqualsOperator()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsFalse(f1 == null);
        }

        //If an object is null, it cant be equal to a formula object (null on left)
        [TestMethod]
        public void PublicNullLeftObjectEqualsOperator()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsFalse(null == f1);
        }

        //Tokens must be in the same order if they are to be equal
        [TestMethod]
        public void PublicOutOfOrderEqualsOperator()
        {
            Formula f1 = new Formula("2 + 3 + A2");
            Formula f2 = new Formula("3 + 2 + A2");
            Assert.IsFalse(f1 == f2);
        }

        //Numbers are compared numerically
        [TestMethod]
        public void PublicNumericComparrisonEqualsOperator()
        {
            Formula f1 = new Formula("20.000000 + A2 / 3");
            Formula f2 = new Formula("20 + A2 / 3");
            Formula f3 = new Formula("2e1 + A2 / 3");
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f3);
        }

        //Variables are compared using their normalized versions (True case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonEqualsTrueCaseOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1", s => s.ToUpper(), s => true);
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1", s => s.ToUpper(), s => true);
            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f2 == f3);
        }

        //Variables are compared using their normalized versions (False case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonEqualsFalseCaseOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1");
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1");
            Assert.IsFalse(f1 == f2);
            Assert.IsFalse(f1 == f3);
            Assert.IsFalse(f2 == f3);
        }

        //Equations have to have the same tokens to be equal (operand case)
        [TestMethod]
        public void PublicSameTokensEqualsOperandCaseOperator()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3");
            Assert.IsFalse(f1 == f2);
        }

        //Equations have to have the same tokens to be equal (Operator case)
        [TestMethod]
        public void PublicSameTokensEqualsOperatorCaseOperator()
        {
            Formula f1 = new Formula("1 - 2 + 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A2");
            Assert.IsFalse(f1 == f2);
        }

        //Equations have to have the same tokens to be equal (Extra operands case)
        [TestMethod]
        public void PublicSameTokensEqualsOperatorExtraOperandCase()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3 + Z3");
            Assert.IsFalse(f1 == f2);
        }

        //Spaces ought to be ignored.
        [TestMethod]
        public void PublicSpacesIgnoredEqualsOperator()
        {
            Formula f1 = new Formula("_XaF        +       1    / 2 - A3f_x");
            Formula f2 = new Formula("_XaF+1/2-A3f_x");
            Assert.IsTrue(f1 == f2);
        }

        //If our normalizer converts everything to the same variable, equals ought to still work.
        [TestMethod]
        public void PublicRepeatingVarEqualsOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1", s => "SAME", s => true);
            Formula f2 = new Formula("A1 + A2 - B1 * B2 / C1", s => "SAME", s => true);
            Assert.IsTrue(f1 == f2);
        }

        //If both are null, the operator equals returns true
        [TestMethod]
        public void PublicBothNullEqualsOperator()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
        }

        //If both variables reference the same object, they must be equal.
        [TestMethod]
        public void PublicSameReferenceEqualsOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1 + 1");
            Assert.IsTrue(f1 == f1);
        }

        //Does Not Equal Operator *******************************************************************************************************************

        //If an object is null, it cant be equal to a formula object (null on right)
        [TestMethod]
        public void PublicNullRightObjectNotEqualsOperator()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsTrue(f1 != null);
        }

        //If an object is null, it cant be equal to a formula object (null on left)
        [TestMethod]
        public void PublicNullLeftObjectNotEqualsOperator()
        {
            Formula f1 = new Formula("A1 + 5 / 2 + 3");
            Assert.IsTrue(null != f1);
        }

        //Tokens must be in the same order if they are to be equal
        [TestMethod]
        public void PublicOutOfOrderNotEqualsOperator()
        {
            Formula f1 = new Formula("2 + 3 + A2");
            Formula f2 = new Formula("3 + 2 + A2");
            Assert.IsTrue(f1 != f2);
        }

        //Numbers are compared numerically
        [TestMethod]
        public void PublicNumericComparrisonNotEqualsOperator()
        {
            Formula f1 = new Formula("20.000000 + A2 / 3");
            Formula f2 = new Formula("20 + A2 / 3");
            Formula f3 = new Formula("2e1 + A2 / 3");
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f3);
        }

        //Variables are compared using their normalized versions (False case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonNotEqualsTrueCaseOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1", s => s.ToUpper(), s => true);
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1", s => s.ToUpper(), s => true);
            Assert.IsFalse(f1 != f2);
            Assert.IsFalse(f2 != f3);
        }

        //Variables are compared using their normalized versions (True case)
        [TestMethod]
        public void PublicNormalizedVarsComparrisonNotEqualsFalseCaseOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1");
            Formula f2 = new Formula("a1 + a2 - b1 * b2 / c1");
            Formula f3 = new Formula("a1 + A2 - b1 * B2 / c1");
            Assert.IsTrue(f1 != f2);
            Assert.IsTrue(f1 != f3);
            Assert.IsTrue(f2 != f3);
        }

        //Equations have to have the same tokens to be equal (operand case)
        [TestMethod]
        public void PublicSameTokensNotEqualsOperandCaseOperator()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3");
            Assert.IsTrue(f1 != f2);
        }

        //Equations have to have the same tokens to be equal (Extra operands case)
        [TestMethod]
        public void PublicSameTokensNotEqualsOperatorExtraOperandCase()
        {
            Formula f1 = new Formula("1 + 2 - 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A3 + Z3");
            Assert.IsTrue(f1 != f2);
        }

        //Equations have to have the same tokens to be equal (Operator case)
        [TestMethod]
        public void PublicSameTokensNotEqualsOperatorCaseOperator()
        {
            Formula f1 = new Formula("1 - 2 + 3 * A1 / A2");
            Formula f2 = new Formula("1 + 2 - 3 * A1 / A2");
            Assert.IsTrue(f1 != f2);
        }

        //Spaces ought to be ignored.
        [TestMethod]
        public void PublicSpacesIgnoredNotEqualsOperator()
        {
            Formula f1 = new Formula("_XaF        +       1    / 2 - A3f_x");
            Formula f2 = new Formula("_XaF+1/2-A3f_x");
            Assert.IsFalse(f1 != f2);
        }

        //If our normalizer converts everything to the same variable, not equal ought to still work.
        [TestMethod]
        public void PublicRepeatingVarNotEqualsOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1", s => "SAME", s => true);
            Formula f2 = new Formula("A1 + A2 - B1 * B2 / C1", s => "SAME", s => true);
            Assert.IsFalse(f1 != f2);
        }

        //If both are null, the operator equals returns false
        [TestMethod]
        public void PublicBothNullNotEqualsOperator()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsFalse(f1 != f2);
        }
        
        //If both variables reference the same object, they can't be not equal.
        [TestMethod]
        public void PublicSameReferenceNotEqualsOperator()
        {
            Formula f1 = new Formula("A1 + A2 - B1 * B2 / C1 + 1");
            Assert.IsFalse(f1 != f1);
        }


        //GetHashCode:********************************************************************************************************************************

        //The trivial case of hash code. Two identical equations (as identical strings) must have the same hash function
        [TestMethod]
        public void PublicHashCodeTrivial()
        {
            Formula f1 = new Formula("A1 + 2 - B1 * B2 / 1");
            Formula f2 = new Formula("A1 + 2 - B1 * B2 / 1");
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        //Our definition of hash code equality relies on our Equals function. Ergo, we ignore spaces
        [TestMethod]
        public void PublicHashCodeIgnoreSpaces()
        {
            Formula f1 = new Formula("A1 + 2 - B1 * B2 / 1");
            Formula f2 = new Formula("A1  +      2-B1          *   B2         / 1");
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        //HashCode must respect normalize functions (as it obeys the rules of Equals())
        [TestMethod]
        public void PublicHashCodeRespectNormalize()
        {
            Formula f1 = new Formula("A1 + 2 - B1 * B2 / 1", s => "SAME", s => true);
            Formula f2 = new Formula("D1 + 2 - E1 * E2 / 1", s => "SAME", s => true);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        //HashCode must also compare numbers numerically, as does Equals()
        [TestMethod]
        public void PublicHashCodeNumericComparrisons()
        {
            Formula f1 = new Formula("20.000000 + A2 / 3");
            Formula f2 = new Formula("20 + A2 / 3");
            Formula f3 = new Formula("2e1 + A2 / 3");
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            Assert.AreEqual(f2.GetHashCode(), f3.GetHashCode());
        }

        //GetVariables:********************************************************************************************************************************8

        //A trivial case of this method, just for safety.
        [TestMethod]
        public void PublicSimpleGetVariablesBehavior()
        {
            Formula f1 = new Formula("A1 + A2 - A3 / A4 * A5");
            int A1Count = 0, A2Count = 0, A3Count = 0, A4Count = 0, A5Count = 0;
            IEnumerable<string> Enumerator = f1.GetVariables();
            foreach (string str in Enumerator)
            {
                if (str.Equals("A1"))
                {
                    A1Count++;
                }
                if (str.Equals("A2"))
                {
                    A2Count++;
                }
                if (str.Equals("A3"))
                {
                    A3Count++;
                }
                if (str.Equals("A4"))
                {
                    A4Count++;
                }
                if (str.Equals("A5"))
                {
                    A5Count++;
                }
            }

            Assert.AreEqual(1, A1Count);
            Assert.AreEqual(1, A2Count);
            Assert.AreEqual(1, A3Count);
            Assert.AreEqual(1, A4Count);
            Assert.AreEqual(1, A5Count);
        }

        //If we're using the identity normalizer, we can have distinct lower and uppercase varaibles
        [TestMethod]
        public void PublicIdentityNormalizerGetVariablesBehavior()
        {
            Formula f1 = new Formula("A1 + a1 - A1 / a1 * A1", s => s, s => true);
            int A1Count = 0, A2Count = 0;
            IEnumerable<string> Enumerator = f1.GetVariables();
            foreach (string str in Enumerator)
            {
                if (str.Equals("A1"))
                {
                    A1Count++;
                }
                if (str.Equals("a1"))
                {
                    A2Count++;
                }
            }
            Assert.AreEqual(1, A1Count);
            Assert.AreEqual(1, A2Count);
        }

        //A formula all consisting of the same variable in different forms. We should return one token.
        [TestMethod]
        public void PublicDuplicateGetVariablesBehavior()
        {
            Formula f1 = new Formula("A1 + a1 - A1 / a1 * A1", s => s.ToUpper(), s=>true);
            int A1Count = 0;
            IEnumerable<string> Enumerator = f1.GetVariables();
            foreach (string str in Enumerator)
            {
                if (str.Equals("A1"))
                {
                    A1Count++;
                }
            }
            Assert.AreEqual(1, A1Count);
            
        }

        //A mix of duplicate variables and uniques
        [TestMethod]
        public void PublicMixedGetVariablesBehavior()
        {
            Formula f1 = new Formula("A1 + A2 - a2 / A3 * A3", s=>s.ToUpper(), s=>true);
            int A1Count = 0, A2Count = 0, A3Count = 0;
            IEnumerable<string> Enumerator = f1.GetVariables();
            foreach (string str in Enumerator)
            {
                if (str.Equals("A1"))
                {
                    A1Count++;
                }
                if (str.Equals("A2"))
                {
                    A2Count++;
                }
                if (str.Equals("A3"))
                {
                    A3Count++;
                }
            }

            Assert.AreEqual(1, A1Count);
            Assert.AreEqual(1, A2Count);
            Assert.AreEqual(1, A3Count);
        }

        //We should adhere to the rules of the normalizer
        [TestMethod]
        public void PublicCheckNormalizerGetVariablesBehavior()
        {
            Formula f1 = new Formula("A2 + A3 - A4 / A5 * A6", s => "A1", s => true);
            int A1Count = 0;
            IEnumerable<string> Enumerator = f1.GetVariables();
            foreach (string str in Enumerator)
            {
                if (str.Equals("A1"))
                {
                    A1Count++;
                }
            }
            Assert.AreEqual(1, A1Count);

        }
    }
}
