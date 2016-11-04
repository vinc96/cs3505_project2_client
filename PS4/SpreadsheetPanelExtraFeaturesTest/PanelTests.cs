///Written by Josh Christensen, u0978248
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

/// <summary>
/// A class designed to test the extra features I added the the SpreadsheetPanel, outside the context of a GUI.
/// </summary>
namespace SpreadsheetPanelExtraFeaturesTest
{
    [TestClass]
    public class UnitTests
    {

        [TestMethod]
        public void TestCanMoveDown()
        {
            int row, col;
            SpreadsheetPanel p = new SpreadsheetPanel();
            p.selectDown();
            p.GetSelection(out row, out col);
            Assert.AreEqual(1, col);
        }
    }
}
