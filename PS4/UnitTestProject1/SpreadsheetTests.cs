using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

namespace UnitTestProject1
{
    [TestClass]
    public class SpreadsheetTests
    {
        //GetNamesOfAllNonEmptyCells Tests *******************************************************************

        //GetCellContents Exception Tests *******************************************************************
        //GetCellContents must throw an exception if passed an invalid variable name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void PublicGetCellContentsInvalidVar1()
        {
            AbstractSpreadsheet a1 = new Spreadsheet();
            a1.GetCellContents("&");

        }
        //GetCellContents Normal Tests *******************************************************************

        //SetCellContents(string, double) Exception Tests *******************************************************************

        //SetCellContents(string, double) Normal Tests *******************************************************************

        //SetCellContents(string, string) Exception Tests *******************************************************************

        //SetCellContents(string, string) Normal Tests *******************************************************************

        //SetCellContents(string, formula) Exception Tests *******************************************************************

        //SetCellContents(string, formula) Normal Tests *******************************************************************

        


    }
}
