using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //Exception Cases

        //Invalid variable names should throw exceptions
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrows()
        {
            Formula f1 = new Formula("1ABC + 2");
        }

        //Invalid variable names should throw exceptions (We still have a baseline standard)
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableThrowsLongerConstructor()
        {
            Formula f1 = new Formula("1ABC + 2", s=>s, s=>true);
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


        
    }
}
