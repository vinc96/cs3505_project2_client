﻿//Written by Josh Christensen u0978248
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
        int MAXSIZE = 10000;
        //Constructor Tests *******************************************************************************

        //Calling the constructor should be uneventful (no exceptions thrown) (parameterless constructor)
        [TestMethod]
        public void PublicConstructorDefault()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(); 
        }

        //Calling the constructor should be uneventful (no exceptions thrown) (Three Argument constructor)
        [TestMethod]
        public void PublicConstructorThreeArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => true, s => s, "trivial version");
        }

        //Calling the constructor should be uneventful (no exceptions thrown) (Four Argument constructor)
        [TestMethod]
        public void PublicConstructorFourArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialValidFile", s => true, s => s, "trivial version");
        }

        //Constructors should provide empty spreadsheets (Single Argument Constructor)
        [TestMethod]
        public void PublicConstructorIsEmptySingleArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Constructors should provide empty spreadsheets (Three Argument Constructor)
        [TestMethod]
        public void PublicConstructorIsEmptyThreeArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => true, s => s, "trivial version");
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Constructors should provide empty spreadsheets (Four Argument Constructor)
        [TestMethod]
        public void PublicConstructorIsEmptyFourArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialValidFile", s => true, s => s, "trivial version");
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //We should be able to edit spreadsheets without throwing exceptions (Single Argument Constructor)
        [TestMethod]
        public void PublicConstructorCanEditSingleArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "12034.1");
        }

        //We should be able to edit spreadsheets without throwing exceptions (Triple Argument Constructor)
        [TestMethod]
        public void PublicConstructorCanEditThreeArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => true, s => s, "trivial version");
            a1.SetContentsOfCell("A1", "12034.1");
        }

        //We should be able to edit spreadsheets without throwing exceptions (Four Argument Constructor)
        [TestMethod]
        public void PublicConstructorCanEditFourArgument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialValidFile", s => true, s => s, "trivial version");
            a1.SetContentsOfCell("A1", "12034.1");
        }

        //Our default constructor should have an identiy normalize function
        [TestMethod]
        public void PublicConstructorIdentityNormalize()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("a1", "NonEmptyAgain");
            Assert.AreEqual(2, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetCellContents("A1").Equals("NonEmpty"));
            Assert.IsTrue(a1.GetCellContents("a1").Equals("NonEmptyAgain"));
        }

        //We should respect the normalize function passed to our constructor (3 argument version)
        [TestMethod]
        public void PublicConstructorRespectNormalize3Argument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => true, s => s.ToUpper(), "default" );
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("a1", "NonEmptyAgain");
            Assert.AreEqual(2, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetCellContents("A1").Equals("NonEmptyAgain"));
            Assert.IsTrue(a1.GetCellContents("a1").Equals("NonEmptyAgain"));
        }

        //We should respect the normalize function passed to our constructor (4 argument version)
        [TestMethod]
        public void PublicConstructorRespectNormalize4Argument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialEmptyFile", s => true, s => s.ToUpper(), "default");
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("a1", "NonEmptyAgain");
            Assert.AreEqual(2, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetCellContents("A1").Equals("NonEmptyAgain"));
            Assert.IsTrue(a1.GetCellContents("a1").Equals("NonEmptyAgain"));
        }

        // TODO: Write a test to ensure that the normalize function works when loading from a file with items that are changed whiltst normalizing

        //We should respect the variable validity of the validator passed to our constructor (3 argument version)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicConstructorRespectValidator3Argument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => false, s => s, "default");
            a1.SetContentsOfCell("A1", "NonEmpty");
        }

        //We should respect the variable validity of the validator passed to our constructor (4 argument version)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicConstructorRespectValidator4Argument()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialEmptyFile", s => false, s => s, "default");
            a1.SetContentsOfCell("A1", "NonEmpty");
        }

        //Our no argument constructor should have version "default"
        [TestMethod]
        public void PublicConstructorVersionNoArguments()
        {
            Spreadsheet a1 = new Spreadsheet();
            Assert.AreEqual("default", a1.Version);
        }

        //Our three argument constructor should have version equalling what we set it
        [TestMethod]
        public void PublicConstructorVersionThreeArguments()
        {
            Spreadsheet a1 = new Spreadsheet(s => true, s => s, "defaultVersion");
            Assert.AreEqual("defaultVersion", a1.Version);
        }

        //Our four argument constructor should have version equalling what we set it
        [TestMethod]
        public void PublicConstructorVersionFourArguments()
        {
            AbstractSpreadsheet a1 = new Spreadsheet("TrivialEmptyFile", s => true, s => s, "defaultVersion");
            Assert.AreEqual("defaultVersion", a1.Version);
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
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("A1", "");
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Setting a cell to " " should not be empty
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsSingleSpace()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", " ");
            Assert.AreEqual(1, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //GetNamesOfAllNonEmptyCells should respect case sensitivity with an identity normalizer.
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("a1", "NonEmptyAgain");
            Assert.AreEqual(2, a1.GetNamesOfAllNonemptyCells().Count());
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("a1"));
        }

        //GetNamesOfAllNonEmptyCells should respect the passed identity normalizer.
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsRespectIdentityNormalizer()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => false, s => s.ToLower(), "default");
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("a1", "NonEmptyAgain");
            Assert.AreEqual(1, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //Trivial case with mixed data types
        [TestMethod]
        public void PublicGetNamesOfAllNonEmptyCellsMixedDataTypes()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "NonEmpty");
            a1.SetContentsOfCell("A2", "12.1234");
            a1.SetContentsOfCell("A3", "=A1 + 1 + B2");
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
                a1.SetContentsOfCell("A" + i, "SomeString");
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE-1; i >= 0; i--)
            {
                
                a1.SetContentsOfCell("A" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
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
                a1.SetContentsOfCell("A" + i, "12.34");
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE-1; i >= 0; i--)
            {
                
                a1.SetContentsOfCell("A" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
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
                a1.SetContentsOfCell("A" + i, "=1 + A1");
                Assert.IsTrue(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
            }
            //Remove values
            for (int i = MAXSIZE-1; i >= 0; i--)
            {
                
                a1.SetContentsOfCell("A" + i, "");
                Assert.IsFalse(a1.GetNamesOfAllNonemptyCells().Contains("_" + i));
                Assert.AreEqual(i, a1.GetNamesOfAllNonemptyCells().Count());
            }
        }

        //GetCellContents Exception Tests *******************************************************************

        //GetCellContents must throw an exception if passed an invalid variable name (Ex. Empty String)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVarEmptyString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("");
        }

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

        //GetCellContents variables can't start with underscores
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsValidVarStartUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("_A1");

        }

        //GetCellContents variables can't contain underscores at all
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A_1");
        }

        //Lone underscores aren't valid characters
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsSingleUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("_");
        }

        //Letters, then numbers, then letters in a variable ought to throw exception
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsLettersNumbersLetters()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A1a");
        }

        //GetCellContents needs to respect the passed Validator.
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsRespectValidator()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => false, s => s, "YardeHarHAR");
            a1.GetCellContents("A1");
        }

        //GetCellContents Normal Tests *******************************************************************

        //GetCellContents variables can start with letters
        [TestMethod]
        public void PublicGetCellContentsValidVarStartLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A1");

        }

        //GetCellContents variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Digit Case)
        [TestMethod]
        public void PublicGetCellContentsDigits()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A12145689911111111141545145");
        }

        //GetCellContents variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Letter Case)
        [TestMethod]
        public void PublicGetCellContentsLetters()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("AASFDfadsfqadsfFeAFljkhpLKH");
        }

        //GetCellContents variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Combined Case)
        [TestMethod]
        public void PublicGetCellContentsCombined()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("AljkhpLsfqadsfFeAASFDf12145611445899");
        }

        //GetCellContents variables are valid for solitary valid characters (letter case)
        [TestMethod]
        public void PublicGetCellContentsSingleLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("A");
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
            a1.SetContentsOfCell("A1", "ThisISATestString1923%Ann\n");
            Assert.AreEqual("ThisISATestString1923%Ann\n", (string) a1.GetCellContents("A1"));
        }

        //GetCellContents: Cells that we initialize to doubles must return that same double
        [TestMethod]
        public void PublicGetCellContentsRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            Assert.AreEqual((double)978.248, (double) a1.GetCellContents("A1"), 1e-9);
        }

        //GetCellContents: Cells that we initialize to formulas must return that same formula
        [TestMethod]
        public void PublicGetCellContentsRetrieveStoredFormula()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula f1 = new Formula("B1 + a2 + 34/2");
            a1.SetContentsOfCell("A1", "=B1 + a2 + 34/2");
            Assert.AreEqual(f1, (Formula) a1.GetCellContents("A1"));
        }

        //GetCellContents: Cell names are case sensitive with an identity normalizer
        [TestMethod]
        public void PublicGetCellContentsCellNamesCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula f1 = new Formula("B1 + a2 + 34/2");
            a1.SetContentsOfCell("A1", "=B1 + a2 + 34/2");
            a1.SetContentsOfCell("a1", "lowercase");
            Assert.AreEqual(f1, (Formula) a1.GetCellContents("A1"));
            Assert.AreEqual("lowercase", (string) a1.GetCellContents("a1"));
        }

        //SetContentsOfCell() Exception Tests *******************************************************************

            //DOUBLES *****************************************************************************************

        //SetContentsOfCell(string, double) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetContentsOfCellStringDoubleExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetContentsOfCell("&", "0.0");
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetContentsOfCell(string, double) must throw an exception if passed an invalid variable name (Ex. An empty string)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringDoubleInvalidVarEmptyStr()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("", "0.0");
        }

        //SetContentsOfCell(string, double) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringDoubleInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("AA1&", "0.0");
        }

        //SetContentsOfCell(string, double) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringDoubleInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("25", "0.0");
        }

        //SetContentsOfCell(string, double) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringDoubleInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("1ABAA111", "0.0");
        }

        //SetContentsOfCell(string, double) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringDoubleInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell(null, "0.0");
        }

        //STRINGS *****************************************************************************************

        //SetContentsOfCell(string, string) must throw an exception if passed an invalid variable name (Ex. An empty string)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringStringInvalidVarEmptyStr()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("", "String");
        }

        //SetContentsOfCell(string, string) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetContentsOfCellStringStringExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetContentsOfCell("&", "String");
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetContentsOfCell(string, string) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringStringInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("AA1&", "SomeTextValue");
        }

        //SetContentsOfCell(string, string) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringStringInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("25", "SomeTextValue");
        }

        //SetContentsOfCell(string, string) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringStringInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("1ABAA111", "SomeTextValue");
        }

        //SetContentsOfCell(string, string) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringStringInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell(null, "SomeTextValue");
        }

        //SetContentsOfCell(string, string) must throw an (ArgumentNull) exception if passed a null text value
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublicSetContentsOfCellStringStringInvalidTextNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            string str = null;
            a1.SetContentsOfCell("A1", str);
        }

        //SetContentsOfCell(string, string) if an ArgumentNull exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetContentsOfCellStringStringArgNullExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                string str = null;
                a1.SetContentsOfCell("A1", str);
            }
            catch (ArgumentNullException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //FORMULAS************************************************************************************

        //SetContentsOfCell(string, formula) must throw an exception if passed an invalid variable name (Ex. An empty string)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringFormulaInvalidVarEmptyStr()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("", "=1 + 1");
        }

        //SetContentsOfCell(string, formula) if a name exception is thrown, the sheet must not change.
        [TestMethod]
        public void PublicSetContentsOfCellStringFormulaExceptionNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            try
            {
                a1.SetContentsOfCell("&", "=1 + 1");
            }
            catch (InvalidNameException e)
            {
                //Do nothing
            }
            Assert.AreEqual(0, a1.GetNamesOfAllNonemptyCells().Count());
        }

        //SetContentsOfCell(string, Formula) must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringFormulaInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("AA1&", "=1 + 2");
        }

        //SetContentsOfCell(string, Formula) must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringFormulaInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("25", "=1 + 2");
        }

        //SetContentsOfCell(string, Formula) must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringFormulaInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("1ABAA111", "=1 + 2");
        }

        //SetContentsOfCell(string, Formula) must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicSetContentsOfCellStringFormulaInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell(null, "=1 + 2");
        }

        //SetContentsOfCell(string, string): The method should throw a CircularException if the SetContentsOfCell method would cause
        //a circular dependency (Direct Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetContentsOfCellStrFormDirectDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A2 + 0");
            a1.SetContentsOfCell("A2", "=A1 + 10"); //Throw here
        }

        //SetContentsOfCell(string, string): The method should throw a CircularException if the SetContentsOfCell method would cause
        //a circular dependency (Indirect Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetContentsOfCellStrFormIndirectDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A4");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("A3", "=A2 + 10");
            a1.SetContentsOfCell("A4", "=A3 + 10"); //Throw here
        }

        //SetContentsOfCell(string, string): The method should throw a CircularException if the SetContentsOfCell method would cause
        //a circular dependency (Mixed Version)
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void PublicSetContentsOfCellStrFormMixedDependentsCircException()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A4");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("A3", "=A2 + 10");
            a1.SetContentsOfCell("A4", "=A1 + 10"); //Throw here
        }

        //If we fail with a CircularDependency, the sheet must not change.
        [TestMethod]
        public void PublicSetContentsOfCellStrFormCircularDependencyNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A2", "=A1");
            try
            {
                a1.SetContentsOfCell("A1", "=A2");
            }
            catch (CircularException e)
            {
                //Do nothing
            }
            Assert.AreEqual(1, a1.GetNamesOfAllNonemptyCells().Count()); //No size change
        }

        //SetContentsOfCell() Normal Tests *******************************************************************

        //DOUBLES **************************************************************************************

        //SetContentsOfCell(string, double): Cells that we initialize to doubles must return that same double
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetContentsOfCell(string, double): SetContentsOfCell must respect case sensitivity
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubStringDoubleRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            a1.SetContentsOfCell("a1", "-978.248");
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("a1"), 1e-9);
        }

        //SetContentsOfCell(string, double): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubOverwriteDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            Assert.AreEqual((double)978.248, (double)a1.GetCellContents("A1"), 1e-9);
            a1.SetContentsOfCell("A1", "-978.248");
            Assert.AreEqual((double)-978.248, (double)a1.GetCellContents("A1"), 1e-9);
        }

        //SetContentsOfCell(string, double): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A1 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248")));
        }

        //SetContentsOfCell(string, double): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=B1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248")));
        }

        //SetContentsOfCell(string, double): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248")));
        }

        //SetContentsOfCell(string, double): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubMixedDependentsCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            a1.SetContentsOfCell("a1", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=a1 + 15");
            a1.SetContentsOfCell("c3", "=A1 + 20");
            a1.SetContentsOfCell("D4", "=C3 + 25");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248"))); //We SHOULD NOT contain D4.
        }

        //SetContentsOfCell(string, double): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetContentsOfCellStrDoubSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "1.0");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            ISet<String> a1Set = (ISet<string>)a1.SetContentsOfCell("A1", "1.0");
            Assert.IsTrue(testSet.SetEquals(a1Set)); //We have to use a funky equals methods for sets.
        }

        //STRINGS **********************************************************************************************************

        //SetContentsOfCell(string, string): Cells that we initialize to string must return that same string
        [TestMethod]
        public void PublicSetContentsOfCellStrStrRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString");
            Assert.AreEqual("SomeString", (string)a1.GetCellContents("A1"));
        }

        //SetContentsOfCell(string, string): SetContentsOfCell must respect case sensitivity
        [TestMethod]
        public void PublicSetContentsOfCellStrStrRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString 1");
            a1.SetContentsOfCell("a1", "SomeString 2");
            Assert.AreEqual("SomeString 1", (string)a1.GetCellContents("A1"));
            Assert.AreEqual("SomeString 2", (string)a1.GetCellContents("a1"));
        }

        //SetContentsOfCell(string, string): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetContentsOfCellStrStrOverwriteString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString 1");
            Assert.AreEqual("SomeString 1", (string)a1.GetCellContents("A1"));
            a1.SetContentsOfCell("A1", "SomeString 2");
            Assert.AreEqual("SomeString 2", (string)a1.GetCellContents("A1"));
        }

        //SetContentsOfCell(string, string): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetContentsOfCellStrStrDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A1 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248")));
        }

        //SetContentsOfCell(string, string): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetContentsOfCellStrStrIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=B1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "-978.248")));
        }

        //SetContentsOfCell(string, string): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetContentsOfCellStrStrMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString 1");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "SomeString 2")));
        }

        //SetContentsOfCell(string, string): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetContentsOfCellStrStrMixedDependentsCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString 1");
            a1.SetContentsOfCell("a1", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=a1 + 15");
            a1.SetContentsOfCell("c3", "=A1 + 20");
            a1.SetContentsOfCell("D4", "=C3 + 25");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "SomeString 2"))); //We SHOULD NOT contain D4.
        }

        //SetContentsOfCell(string, string): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetContentsOfCellStrStrSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "SomeString");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "SomeString")));
        }

        //FORMULAS ******************************************************************************************************

        //SetContentsOfCell(string, formula): Cells that we initialize to string must return that same formula
        [TestMethod]
        public void PublicSetContentsOfCellStrFormRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=1 + 1");
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
        }

        //SetContentsOfCell(string, formula): SetContentsOfCell must respect case sensitivity
        [TestMethod]
        public void PublicSetContentsOfCellStrFormRespectCaseSensitivity()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=1 + 1");
            a1.SetContentsOfCell("a1", "=1 + 2");
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
            Assert.AreEqual(new Formula("1 + 2"), (Formula)a1.GetCellContents("a1"));
        }

        //SetContentsOfCell(string, formula): We should be able to overwrite a value, and retrieve the new value
        [TestMethod]
        public void PublicSetContentsOfCellStrFormOverwriteString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=1 + 1");
            Assert.AreEqual(new Formula("1 + 1"), (Formula)a1.GetCellContents("A1"));
            a1.SetContentsOfCell("A1", "=1 + 2");
            Assert.AreEqual(new Formula("1 + 2"), (Formula)a1.GetCellContents("A1"));
        }

        //SetContentsOfCell(string, formula): The method should return a set of cells that depend on this one. (Direct version)
        [TestMethod]
        public void PublicSetContentsOfCellStrFormDirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=1 + 1");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A1 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "=1 + 2")));
        }

        //SetContentsOfCell(string, formula): The method should return a set of cells that depend on this one. (Indirect version)
        [TestMethod]
        public void PublicSetContentsOfCellStrFormIndirectDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "1 + 1");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=B1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "=1 + 2")));
        }

        //SetContentsOfCell(string, formula): The method should return a set of cells that depend on this one. (Mixed version)
        [TestMethod]
        public void PublicSetContentsOfCellStrFormMixedDependents()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "1 + 1");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "=1 + 2")));
        }

        //SetContentsOfCell(string, formula): The set returned must respect case sensitivity 
        [TestMethod]
        public void PublicSetContentsOfCellStrFormMixedDependentsCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "1 + 1");
            a1.SetContentsOfCell("a1", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=a1 + 15");
            a1.SetContentsOfCell("c3", "=A1 + 20");
            a1.SetContentsOfCell("D4", "=C3 + 25");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("a1");
            testSet.Add("B1");
            testSet.Add("c3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "=1 + 2"))); //We SHOULD NOT contain D4.
        }

        //SetContentsOfCell(string, formula): We should get a set of changed cells, regardless if the cell has a new value or not.
        [TestMethod]
        public void PublicSetContentsOfCellStrFormSetNoChange()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "1 + 1");
            a1.SetContentsOfCell("A2", "=A1 + 10");
            a1.SetContentsOfCell("B1", "=A2 + 15");
            a1.SetContentsOfCell("C3", "=A1 + 20");
            //Set up a set w/ cell names
            ISet<String> testSet = new HashSet<String>();
            testSet.Add("A1");//We have to remember to update ourselves
            testSet.Add("A2");
            testSet.Add("B1");
            testSet.Add("C3");
            Assert.IsTrue(testSet.SetEquals((ISet<string>)a1.SetContentsOfCell("A1", "=1 + 2")));
        }

        //SetContentsOfCell(string, formula): We should be able to depend on empty cells
        [TestMethod]
        public void PublicSetContentsOfCellStrFormDependOnEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A2");
        }

        //SetContentsOfCell(string, formula): We should be able to depend on cells with strings
        [TestMethod]
        public void PublicSetContentsOfCellStrFormDependOnString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A2", "STRING!");
            a1.SetContentsOfCell("A1", "=A2");
        }

        //SetContentsOfCell(string, formula): We should be able to depend on cells with doubles
        [TestMethod]
        public void PublicSetContentsOfCellStrFormDependOnDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A2", "1.346");
            a1.SetContentsOfCell("A1", "=A2");
        }

        //GetCellValue Exception Tests **********************************************************************************

        //GetCellValue must throw an exception if passed an invalid variable name (Ex. Empty String)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueInvalidVarEmptyString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("");
        }

        //GetCellValue must throw an exception if passed an invalid variable name (Ex. An illegal character)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueInvalidVarIllegalChar()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("AA1&");
        }

        //GetCellValue must throw an exception if passed an invalid variable name (Ex. just a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueInvalidVarNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("25");
        }

        //GetCellValue must throw an exception if passed an invalid variable name (Ex. Starting w/ a number)
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueInvalidVarStartNum()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("1ABAA111");
        }

        //GetCellValue must throw an exception if passed a null variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueInvalidVarNull()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue(null);
        }

        //GetCellValue variables can't start with underscores
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueValidVarStartUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("_A1");

        }

        //GetCellValue variables can't contain underscores at all
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("A_1");
        }

        //Lone underscores aren't valid characters
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueSingleUnderscore()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("_");
        }

        //Letters, then numbers, then letters in a variable ought to throw exception
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueLettersNumbersLetters()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("A1a");
        }

        //GetCellValue needs to respect the passed Validator.
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellValueRespectValidator()
        {
            AbstractSpreadsheet a1 = new Spreadsheet(s => false, s => s, "YardeHarHAR");
            a1.GetCellValue("A1");
        }

        //GetCellValue Normal Tests **********************************************************************************

        //GetCellValue variables can start with letters
        [TestMethod]
        public void PublicGetCellValueValidVarStartLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("A1");

        }

        //GetCellValue variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Digit Case)
        [TestMethod]
        public void PublicGetCellValueDigits()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("A12145689911111111141545145");
        }

        //GetCellValue variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Letter Case)
        [TestMethod]
        public void PublicGetCellValueLetters()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("AASFDfadsfqadsfFeAFljkhpLKH");
        }

        //GetCellValue variables are valid so long as all remaining characters are numbers or letters (but not numbers then letters). (Combined Case)
        [TestMethod]
        public void PublicGetCellValueCombined()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("AljkhpLsfqadsfFeAASFDf12145611445899");
        }

        //GetCellValue variables are valid for solitary valid characters (letter case)
        [TestMethod]
        public void PublicGetCellValueSingleLetter()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellValue("A");
        }

        //GetCellValue: Cells that have not been changed must have empty values
        [TestMethod]
        public void PublicGetCellValueNonInitializedCellsEmpty()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Assert.AreEqual("", (string)a1.GetCellValue("A1"));
        }

        //GetCellValue: Cells that we initialize to strings must return that same string
        [TestMethod]
        public void PublicGetCellValueRetrieveStoredString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "ThisISATestString1923%Ann\n");
            Assert.AreEqual("ThisISATestString1923%Ann\n", (string)a1.GetCellValue("A1"));
        }

        //GetCellValue: Cells that we initialize to doubles must return that same double
        [TestMethod]
        public void PublicGetCellValueRetrieveStoredDouble()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "978.248");
            Assert.AreEqual((double)978.248, (double)a1.GetCellValue("A1"), 1e-9);
        }

        //GetCellValue: Cells that we initialize to invalid formulas must return FormulaErrors (divide by zero)
        [TestMethod]
        public void PublicGetCellValueDivideByZero()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=1 / (1 - 1)");
            Assert.IsTrue(a1.GetCellValue("A1").GetType().Equals(typeof(FormulaFormatException)));
        }

        //GetCellValue: Cells that we initialize to invalid formulas must return FormulaErrors (empty cell)
        [TestMethod]
        public void PublicGetCellValueInvalidEmptyCell()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A2");
            Assert.IsTrue(a1.GetCellValue("A1").GetType().Equals(typeof(FormulaFormatException)));
        }

        //GetCellValue: Cells that we initialize to invalid formulas must return FormulaErrors (strings cell)
        [TestMethod]
        public void PublicGetCellValueInvalidDependOnString()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A2", "STRING!");
            a1.SetContentsOfCell("A1", "=A2");
            Assert.IsTrue(a1.GetCellValue("A1").GetType().Equals(typeof(FormulaFormatException)));
        }

        //GetCellValue: Cells that we initialize to invalid formulas must return FormulaErrors (Other invalid formulas)
        [TestMethod]
        public void PublicGetCellValueDependOnOtherInvalidFormula()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.SetContentsOfCell("A1", "=A2");
            a1.SetContentsOfCell("A2", "1 / (1 - 1)");
            Assert.IsTrue(a1.GetCellValue("A1").GetType().Equals(typeof(FormulaFormatException)));
        }



        //GetCellValue: Cell names are case sensitive with an identity normalizer
        [TestMethod]
        public void PublicGetCellValueCellNamesCaseSensitive()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            Formula f1 = new Formula("B1 + a2 + 34/2");
            a1.SetContentsOfCell("A1", "=B1 + a2 + 34/2");
            a1.SetContentsOfCell("a1", "lowercase");
            Assert.AreEqual(f1, (Formula)a1.GetCellValue("A1"));
            Assert.AreEqual("lowercase", (string)a1.GetCellValue("a1"));
        }

        //GetCellValue formula tests ******************************************************************************************
        //As this logic is almost entirely handled by the Formula class, the majority of the testing should 
        //be handled by that class. However, I will do limited testing here, as I am technically responsible
        //for the validity of the results.


    }
}
