using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class FormulaTests
    {
        //Exception Cases*********************************************************************************

        //Invalid variable names should throw exceptions
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrows()
        {
            Formula f1 = new Formula("2 + 1ABC + 2");
        }

        //Invalid variable names should throw exceptions (We still have a baseline standard)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrowsLongerConstructor()
        {
            Formula f1 = new Formula("2 + 1ABC + 2", s=>s, s=>true);
        }

        //We should adhere to the stricter of the two standards we established (This tests the Normalize behavior).
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrowsStricterRulesNormalize()
        {
            Formula f1 = new Formula("A1 + 2", s => "1ABC", s => true);
        }

        //We should adhere to the stricter of the two standards we established (This tests the isValid behavior).
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrowsStricterRulesIsValid()
        {
            Formula f1 = new Formula("A1 + 2", s => s, s => false);
        }

        //Invalid tokens should throw exceptions (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidTokensShortConstructor()
        {
            Formula f1 = new Formula("2 + $");
        }

        //Invalid tokens should throw exceptions (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidtokensLongConstructor()
        {
            Formula f1 = new Formula("3 + $", s => s, s => true);
        }

        //We need at least one token (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OneTokenShortConstructor()
        {
            Formula f1 = new Formula(" ");
        }

        //We need at least one token (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OneTokenLongConstructor()
        {
            Formula f1 = new Formula(" ", s => s, s => true);
        }

        //We have to open parenthesis before we close them (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseParenShortConstructor()
        {
            Formula f1 = new Formula("1 + 2) + 1");
        }

        //We have to open parenthesis before we close them (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseParenLongConstructor()
        {
            Formula f1 = new Formula("1 + 2) + 1", s => s, s => true);
        }

        //We have to close parenthesis that we open (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseOpenParenShortConstructor()
        {
            Formula f1 = new Formula("(1 + 2");
        }

        //We have to close parenthesis that we open (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseOpenParenLongConstructor()
        {
            Formula f1 = new Formula("(1 + 2", s => s, s => true);
        }

        //The first token must be a number, variable or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenShortConstructor()
        {
            Formula f1 = new Formula("+ 2 + 4");
        }

        //The first token must be a number, variable or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void StartingTokenLongConstructor()
        {
            Formula f1 = new Formula("+ 1 + 2", s => s, s => true);
        }

        //The last token must be a number, variable or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenShortConstructor()
        {
            Formula f1 = new Formula("1 + 2 + ");
        }

        //The last token must be a number, variable or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void EndingTokenLongConstructor()
        {
            Formula f1 = new Formula("1 + 2 + ", s => s, s => true);
        }

        //Any token following an openparen must be a number, var, or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OpenParenTailShortConstructor()
        {
            Formula f1 = new Formula("() + 2 + 1)");
        }

        //Any token following an openparen must be a number, var, or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OpenParenTailLongConstructor()
        {
            Formula f1 = new Formula("() + 2 + 1)", s => s, s => true);
        }

        //Any token following an openparen must be a number, var, or openparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorTailShortConstructor()
        {
            Formula f1 = new Formula("+ + 2 + 1)");
        }

        //Any token following an openparen must be a number, var, or openparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OperatorTailLongConstructor()
        {
            Formula f1 = new Formula("+ + 2 + 1)", s => s, s => true);
        }

        //Any token following a number must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberTailShortConstructor()
        {
            Formula f1 = new Formula("2 + 1 A1");
        }

        //Any token following a number must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NumberTailLongConstructor()
        {
            Formula f1 = new Formula("2 + 1 A1", s => s, s => true);
        }

        //Any token following a variable must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void VarTailShortConstructor()
        {
            Formula f1 = new Formula("2 + A1 1");
        }

        //Any token following a variable must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void VarTailLongConstructor()
        {
            Formula f1 = new Formula("2 + A1 1", s => s, s => true);
        }

        //Any token following a close paren must be an operator or closeparen (Short constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseParenTailShortConstructor()
        {
            Formula f1 = new Formula("(2 + 1) A1");
        }

        //Any token following a close paren must be an operator or closeparen (long constructor)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CloseParenTailLongConstructor()
        {
            Formula f1 = new Formula("(2 + 1) A1", s => s, s => true);
        }



        //Normal Constructor Behavior.********************************************************************************

        //A single variable is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void SingleVariableShortConstructor()
        {
            Formula f1 = new Formula("A1");
        }

        //A single variable is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void SingleVariableLongConstructor()
        {
            Formula f1 = new Formula("A1", s => s, s => true);
        }

        //A single number is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void SingleNumberShortConstructor()
        {
            Formula f1 = new Formula("1");
        }

        //A single number is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void SingleNumberLongConstructor()
        {
            Formula f1 = new Formula("1", s => s, s => true);
        }

        //Basic addition is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicAdditionShortConstructor()
        {
            Formula f1 = new Formula("1 + 1");
        }

        //Basic addition is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicAdditionLongConstructor()
        {
            Formula f1 = new Formula("1 + 1", s => s, s => true);
        }

        //Basic subtraction is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicSubtractionShortConstructor()
        {
            Formula f1 = new Formula("1 - 1");
        }

        //Basic subtraction is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicSubtractionLongConstructor()
        {
            Formula f1 = new Formula("1 - 1", s => s, s => true);
        }

        //Basic multiplication is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicMultiplicationShortConstructor()
        {
            Formula f1 = new Formula("1 * 1");
        }

        //Basic multiplication is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicMultiplicationLongConstructor()
        {
            Formula f1 = new Formula("1 * 1", s => s, s => true);
        }

        //Basic Division is valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicDivisionShortConstructor()
        {
            Formula f1 = new Formula("1 / 1");
        }

        //Basic Division is valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicDivisionLongConstructor()
        {
            Formula f1 = new Formula("1 / 1", s => s, s => true);
        }

        //Basic addition is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicAdditionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 + A1");
        }

        //Basic addition is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicAdditionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 + A1", s => s, s => true);
        }

        //Basic subtraction is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicSubtractionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 - A1");
        }

        //Basic subtraction is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicSubtractionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 - A1", s => s, s => true);
        }

        //Basic multiplication is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicMultiplicationVarsShortConstructor()
        {
            Formula f1 = new Formula("1 * A1");
        }

        //Basic multiplication is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicMultiplicationVarsLongConstructor()
        {
            Formula f1 = new Formula("1 * A1", s => s, s => true);
        }

        //Basic Division is valid with variables. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void BasicDivisionVarsShortConstructor()
        {
            Formula f1 = new Formula("1 / A1");
        }

        //Basic Division is valid with variables. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void BasicDivisionVarsLongConstructor()
        {
            Formula f1 = new Formula("1 / A1", s => s, s => true);
        }

        //Equations consisting of only variables are valid. Fails if it throws exception. (Short Constructor)
        [TestMethod]
        public void OnlyVarsShortConstructor()
        {
            Formula f1 = new Formula("A2 / A1");
        }

        //Equations consisting of only variables are valid. Fails if it throws exception. (Long Constructor)
        [TestMethod]
        public void OnlyVarsLongConstructor()
        {
            Formula f1 = new Formula("A2 / A1", s => s, s => true);
        }

        //Equations containing all sorts of crazy combinaitons of operators are allowed. (Short Constructor)
        [TestMethod]
        public void CrazyEqnShortConstructor()
        {
            Formula f1 = new Formula("1 + 145 - 345/(224-2) + 554/22 - (445) + 1456 + (1 - 99) / 2");
        }

        //Equations containing all sorts of crazy combinaitons of operators are allowed. (Long Constructor)
        [TestMethod]
        public void CrazyEqnLongConstructor()
        {
            Formula f1 = new Formula("1 + 145 - 345/(224-2) + 554/22 - (445) + 1456 + (1 - 99) / 2", s => s, s => true);
        }

        //Evaluate Failure Cases:************************************************************************************

        

        //GetVariables:

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
