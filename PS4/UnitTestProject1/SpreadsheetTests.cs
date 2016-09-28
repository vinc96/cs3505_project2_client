using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class SpreadsheetTests
    {
        //GetNamesOfAllNonEmptyCells Tests *******************************************************************

        //GetCellContents Exception Tests *******************************************************************
        
        //GetCellContents must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("AA1&");
        }

        //GetCellContents must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("25");
        }

        //GetCellContents must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("1ABAA111");
        }

        //GetCellContents must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents(null);
        }

        //GetCellContents Normal Tests *******************************************************************

        //GetCellContents variables can start with underscores
        [TestMethod]
        public void PublicGetCellContentsValidVarStartUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("_A1");

        }

        //GetCellContents variables can start with letters
        [TestMethod]
        public void PublicGetCellContentsValidVarStartLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A1");

        }

        //GetCellContents variables are valid so long as all remaining characters are numbers, letters, or underscores. (Digit Case)
        [TestMethod]
        public void PublicGetCellContentsDigits()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A12145689911111111141545145");
        }

        //GetCellContents variables are valid so long as all remaining characters are numbers, letters, or underscores. (Letter Case)
        [TestMethod]
        public void PublicGetCellContentsLetters()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("AASFDfadsfqadsfFeAFljkhpLKH");
        }

        //GetCellContents variables are valid so long as all remaining characters are numbers, letters, or underscores. (Underscore Case)
        [TestMethod]
        public void PublicGetCellContentsUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A_____________________");
        }

        //GetCellContents variables are valid so long as all remaining characters are numbers, letters, or underscores. (Combined Case)
        [TestMethod]
        public void PublicGetCellContentsCombined()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A_____ljkhpL__sfqa15451dsfFe_____AAS1111111FDf12145611445adAF899KH");
        }

        //GetCellContents variables are valid for solitary valid characters (letter case)
        [TestMethod]
        public void PublicGetCellContentsSingleLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A");
        }

        //GetCellContents variables are valid for solitary valid characters (underscore case)
        [TestMethod]
        public void PublicGetCellContentsSingleUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("_");
        }

        //GetCellContents: Cells that have not been changed must have empty values
        [TestMethod]
        public void PublicGetCellContentsNonInitializedCellsEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.AreEqual("", (string) a1.GetCellContents("A1"));
        }

        //GetCellContents: Cells that we initialize to strings must return that same string
        [TestMethod]
        public void PublicGetCellContentsRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "ThisISATestString1923%Ann\n");
            Assert.AreEqual("ThisISATestString1923%Ann\n", (string) a1.GetCellContents("A1"));
        }

        //GetCellContents: Cells that we initialize to doubles must return that same double
        [TestMethod]
        public void PublicGetCellContentsRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", (double) 978.248);
            Assert.AreEqual((double)978.248, (double) a1.GetCellContents("A1"), 1e-9);
        }

        //GetCellContents: Cells that we initialize to formulas must return that same formula
        [TestMethod]
        public void PublicGetCellContentsRetrieveStoredFormula()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula f1 = new Formula("A1 + a2 + 34/2");
            a1.SetCellContents("A1", f1);
            Assert.AreEqual(f1, (Formula) a1.GetCellContents("A1"));
        }

        //GetCellContents: Cell names are case sensitive
        [TestMethod]
        public void PublicGetCellContentsCellNamesCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula f1 = new Formula("A1 + a2 + 34/2");
            a1.SetCellContents("A1", f1);
            a1.SetCellContents("a1", "lowercase");
            Assert.AreEqual(f1, (Formula) a1.GetCellContents("A1"));
            Assert.AreEqual("lowercase", (string) a1.GetCellContents("a1"));
        }

        //SetCellContents(string, double) Exception Tests *******************************************************************

        //SetCellContents(string, double) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringDoubleInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("AA1&", 0.0);
        }

        //SetCellContents(string, double) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringDoubleInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("25", 0.0);
        }

        //SetCellContents(string, double) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringDoubleInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("1ABAA111", 0.0);
        }

        //SetCellContents(string, double) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringDoubleInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents(null, 0.0);
        }

        //SetCellContents(string, double) Normal Tests *******************************************************************

        //SetCellContents(string, double): Cells that we initialize to doubles must return that same double
        [TestMethod]
        public void PublicSetCellContentsRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetCellContents(string, double): SetCellContents must respect case sensitivity
        [TestMethod]
        public void PublicSetCellContentsStringDoubleRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("a1", -978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("a1"), 1e-9);
        }

        //SetCellContents(string, double): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetCellContentsOverwriteDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            a1.SetCellContents("A1", -978.248);
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetCellContents(string, double): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetCellContentsDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A1 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>) a1.SetCellContents("A1", -978.248));
        }

        //SetCellContents(string, double): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetCellContentsIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("B1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>) a1.SetCellContents("A1", -978.248));
        }

        //SetCellContents(string, double): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetCellContentsMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>) a1.SetCellContents("A1", -978.248));
        }

        //SetCellContents(string, double): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetCellContentsMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("a1", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("a1 + 15"));
            a1.SetCellContents("c3", new Formula("A1 + 20"));
            a1.SetCellContents("D4", new Formula("C3 + 25"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", -978.248)); //We SHOULD NOT contain D4.
        }

        //SetCellContents(string, string) Exception Tests *******************************************************************

        //SetCellContents(string, string) Normal Tests *******************************************************************

        //SetCellContents(string, formula) Exception Tests *******************************************************************

        //SetCellContents(string, formula) Normal Tests *******************************************************************

        //We should be able to depend on empty cells




    }
}
