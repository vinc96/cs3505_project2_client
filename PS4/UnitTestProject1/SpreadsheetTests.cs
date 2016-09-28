using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// The maximum size of the stress tests (The largest our spreadsheet gets in these tests)
        /// </summary>
        int MAXSIZE = 1000;
        //Constructor Tests *******************************************************************************

        //Calling the constructor should be uneventful (no exceptions thrown)
        [TestMethod]
        public void PublicConstructorDefault()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(); 
        }

        //Constructors should provide empty spreadsheets
        [TestMethod]
        public void PublicConstructorIsEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //We should be able to edit spreadsheets without throwing exceptions
        [TestMethod]
        public void PublicConstructorCanEdit()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 12034.1);
        }

        //GetNamesOfAllNonEmptyCells Tests *******************************************************************

        //New sheets should have zero non-empty cells
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsNewSheet()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Setting a cell back to "" should make it empty again
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsSetToEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "NonEmpty");
            a1.SetCellContents("A1", "");
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Setting a cell to " " should not be empty
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsSingleSpace()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", " ");
            Assert.AreEqual(1, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //GetNamesOfAllNonEmptyCells should respect case sensitivity
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "NonEmpty");
            a1.SetCellContents("a1", "NonEmptyAgain");
            Assert.AreEqual(2, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("a1"));
        }

        //Trivial case with mixed data types
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsMixedDataTypes()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "NonEmpty");
            a1.SetCellContents("A2", 12.1234);
            a1.SetCellContents("A3", new Formula("A1 + 1 + B2"));
            Assert.AreEqual(3, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("A2"));
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("A3"));
        }

        //GetNamesOfAllNonEmptyCells Stress Test: Add then remove a bunch of values, making sure the size 
        //of the returned Enumerable is correct. (String Version)
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsStressTestString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            for (int i = 0; i < MAXSIZE; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, "SomeString");
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE; i > 0; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
        }

        //GetNamesOfAllNonEmptyCells Stress Test: Add then remove a bunch of values, making sure the size 
        //of the returned Enumerable is correct. (Double Version)
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsStressTestDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            for (int i = 0; i < MAXSIZE; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, 12.34);
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE; i > 0; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
        }

        //GetNamesOfAllNonEmptyCells Stress Test: Add then remove a bunch of values, making sure the size 
        //of the returned Enumerable is correct. (Formula Version)
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsStressTestFormula()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            for (int i = 0; i < MAXSIZE; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, new Formula("1 + A1"));
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE; i > 0; i++)
            {
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
                a1.SetCellContents("_" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
        }

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

        //SetCellContents(string, double) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStringDoubleExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetCellContents("&", 0.0);
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

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
        public void PublicSetCellContentsStrDoubRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetCellContents(string, double): SetCellContents must respect case sensitivity
        [TestMethod]
        public void PublicSetCellContentsStrDoubStringDoubleRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            a1.SetCellContents("a1", -978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("a1"), 1e-9);
        }

        //SetCellContents(string, double): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetCellContentsStrDoubOverwriteDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 978.248);
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            a1.SetCellContents("A1", -978.248);
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetCellContents(string, double): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetCellContentsStrDoubDirectDependents()
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
        public void PublicSetCellContentsStrDoubIndirectDependents()
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
        public void PublicSetCellContentsStrDoubMixedDependents()
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
        public void PublicSetCellContentsStrDoubMixedDependentsCaseSensitive()
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

        //SetCellContents(string, double): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetCellContentsStrDoubSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", 1.0);
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", 1.0));
        }

        //SetCellContents(string, string) Exception Tests *******************************************************************

        //SetCellContents(string, string) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStringStringExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetCellContents("&", "String");
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetCellContents(string, string) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringStringInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("AA1&", "SomeTextValue");
        }

        //SetCellContents(string, string) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringStringInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("25", "SomeTextValue");
        }

        //SetCellContents(string, string) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringStringInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("1ABAA111", "SomeTextValue");
        }

        //SetCellContents(string, string) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringStringInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents(null, "SomeTextValue");
        }

        //SetCellContents(string, string) must throw an (ArgumentNull) exception if passed a null text value
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublicSetCellContentsStringStringInvalidTextNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            string str = null;
            a1.SetCellContents("A1", str);
        }

        //SetCellContents(string, string) if an ArgumentNull exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStringStringArgNullExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                string str = null;
                a1.SetCellContents("A1", str);
            }
            catch (ArgumentNullException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetCellContents(string, string) Normal Tests *******************************************************************

        //SetCellContents(string, string): Cells that we initialize to string must return that same string
        [TestMethod]
        public void PublicSetCellContentsStrStrRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString");
            Assert.AreEqual("SomeString", (string)a1.GetCellContents("A1"));
        }

        //SetCellContents(string, string): SetCellContents must respect case sensitivity
        [TestMethod]
        public void PublicSetCellContentsStrStrRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString 1");
            a1.SetCellContents("a1", "SomeString 2");
            Assert.AreEqual("SomeString 1", (string)a1.GetCellContents("A1"));
            Assert.AreEqual("SomeString 2", (string)a1.GetCellContents("a1"));
        }

        //SetCellContents(string, string): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetCellContentsStrStrOverwriteString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString 1");
            Assert.AreEqual("SomeString 1", (string)a1.GetCellContents("A1"));
            a1.SetCellContents("A1", "SomeString 2");
            Assert.AreEqual("SomeString 2", (string)a1.GetCellContents("A1"));
        }

        //SetCellContents(string, string): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetCellContentsStrStrDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A1 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", -978.248));
        }

        //SetCellContents(string, string): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetCellContentsStrStrIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("B1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", -978.248));
        }

        //SetCellContents(string, string): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetCellContentsStrStrMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString 1");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", "SomeString 2"));
        }

        //SetCellContents(string, string): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetCellContentsStrStrMixedDependentsCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString 1");
            a1.SetCellContents("a1", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("a1 + 15"));
            a1.SetCellContents("c3", new Formula("A1 + 20"));
            a1.SetCellContents("D4", new Formula("C3 + 25"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", "SomeString 2")); //We SHOULD NOT contain D4.
        }

        //SetCellContents(string, string): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetCellContentsStrStrSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "SomeString");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", "SomeString"));
        }

        //SetCellContents(string, formula) Exception Tests *******************************************************************

        //SetCellContents(string, formula) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStringFormulaExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetCellContents("&", new Formula("1 + 1"));
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetCellContents(string, Formula) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringFormulaInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("AA1&", new Formula("1 + 2"));
        }

        //SetCellContents(string, Formula) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringFormulaInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("25", new Formula("1 + 2"));
        }

        //SetCellContents(string, Formula) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringFormulaInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("1ABAA111", new Formula("1 + 2"));
        }

        //SetCellContents(string, Formula) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetCellContentsStringFormulaInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents(null, new Formula("1 + 2"));
        }

        //SetCellContents(string, Formula) must throw an (ArgumentNull) exception if passed a null formula value
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublicSetCellContentsStringFormulaInvalidTextNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula form = null;
            a1.SetCellContents("A1", form);
        }

        //SetCellContents(string, Formula) if an ArgumentNull exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStringFormulaArgNullExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                Formula form = null;
                a1.SetCellContents("A1", form);
            }
            catch (ArgumentNullException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetCellContents(string, string): The method should throw a CircularException if the SetCellContents method would cause
        //a circular dependency (Direct Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetCellContentsStrFormDirectDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "A2 + 0");
            a1.SetCellContents("A2", new Formula("A1 + 10")); //Throw here
        }

        //SetCellContents(string, string): The method should throw a CircularException if the SetCellContents method would cause
        //a circular dependency (Indirect Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetCellContentsStrFormIndirectDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "A4");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("A3", new Formula("A2 + 10"));
            a1.SetCellContents("A4", new Formula("A3 + 10")); //Throw here
        }

        //SetCellContents(string, string): The method should throw a CircularException if the SetCellContents method would cause
        //a circular dependency (Mixed Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetCellContentsStrFormMixedDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "A4");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("A3", new Formula("A2 + 10"));
            a1.SetCellContents("A4", new Formula("A1 + 10")); //Throw here
        }

        //If we fail with a CircularDependency, the sheet must not change.
        [TestMethod]
        public void PublicSetCellContentsStrFormCircularDependencyNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A2", new Formula("A1"));
            try
            {
                a1.SetCellContents("A1", new Formula("A2"));
            }
            catch (CircularException e)
            {
                //Do nothing
            }
            Assert.AreEqual(1, a1.GetNamesOfAllNonemptyCells().Count()); //No size change
        }

        //SetCellContents(string, formula) Normal Tests *******************************************************************

        //SetCellContents(string, formula): Cells that we initialize to string must return that same formula
        [TestMethod]
        public void PublicSetCellContentsStrFormRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", new Formula("1 + 1"));
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
        }

        //SetCellContents(string, formula): SetCellContents must respect case sensitivity
        [TestMethod]
        public void PublicSetCellContentsStrFormRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", new Formula("1 + 1"));
            a1.SetCellContents("a1", new Formula("1 + 2"));
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
            Assert.AreEqual(new Formula("1 + 2"), (Formula)a1.GetCellContents("a1"));
        }

        //SetCellContents(string, formula): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetCellContentsStrFormOverwriteString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", new Formula("1 + 1"));
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
            a1.SetCellContents("A1", new Formula("1 + 2"));
            Assert.AreEqual(new Formula("1 + 2"), (Formula)a1.GetCellContents("A1"));
        }

        //SetCellContents(string, formula): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetCellContentsStrFormDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", new Formula("1 + 1"));
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A1 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", new Formula("1 + 2")));
        }

        //SetCellContents(string, formula): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetCellContentsStrFormIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "1 + 1");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("B1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", new Formula("1 + 2")));
        }

        //SetCellContents(string, formula): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetCellContentsStrFormMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "1 + 1");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", new Formula("1 + 2")));
        }

        //SetCellContents(string, formula): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetCellContentsStrFormMixedDependentsCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "1 + 1");
            a1.SetCellContents("a1", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("a1 + 15"));
            a1.SetCellContents("c3", new Formula("A1 + 20"));
            a1.SetCellContents("D4", new Formula("C3 + 25"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", new Formula("1 + 2"))); //We SHOULD NOT contain D4.
        }

        //SetCellContents(string, formula): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetCellContentsStrFormSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A1", "1 + 1");
            a1.SetCellContents("A2", new Formula("A1 + 10"));
            a1.SetCellContents("B1", new Formula("A2 + 15"));
            a1.SetCellContents("C3", new Formula("A1 + 20"));
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.AreEqual(testSet, (ISet<string>)a1.SetCellContents("A1", new Formula("1 + 2")));
        }

        //SetCellContents(string, formula): We should be able to depend on empty cells
        [TestMethod]
        public void PublicSetCellContentsStrFormDependOnEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.IsTrue(a1.SetCellContents("A1", new Formula("A2")).Contains("A2"));
        }

        //SetCellContents(string, formula): We should be able to depend on cells with strings
        [TestMethod]
        public void PublicSetCellContentsStrFormDependOnString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A2", "STRING!");
            a1.SetCellContents("A1", new Formula("A2"));
            Assert.IsTrue(a1.SetCellContents("A1", new Formula("A2")).Contains("A2"));
        }

        //SetCellContents(string, formula): We should be able to depend on cells with doubles
        [TestMethod]
        public void PublicSetCellContentsStrFormDependOnDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetCellContents("A2", 1.346);
            a1.SetCellContents("A1", new Formula("A2"));
            Assert.IsTrue(a1.SetCellContents("A1", new Formula("A2")).Contains("A2"));
        }

    }
}
