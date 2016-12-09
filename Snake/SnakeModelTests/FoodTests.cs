using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeModel;

namespace SnakeModelTests
{
    [TestClass]
    public class FoodTests
    {
        /// <summary>
        /// Tests the food constructor to ensure that it sets the correct values
        /// </summary>
        [TestMethod]
        public void FoodConstructor()
        {
            Food f = new Food(12565, new Point(12, 156));
            Assert.AreEqual(12565, f.ID);
            Assert.AreEqual(new Point(12, 156), f.loc);
        }

        [TestMethod]
        public void TestFoodEat()
        {
            Food f = new Food(12565, new Point(12, 156));
            f.eat();
            Assert.AreEqual(new Point(-1, -1), f.loc);
        }
    }
}
