using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{

    /// <summary>
    /// An Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// A DependencyGraph object recording all the dependency pairs for this object.
        /// </summary>
        private DependencyGraph dependencies = new DependencyGraph();
        /// <summary>
        /// A dictionary containing all of the non-empty cells in this object.
        /// </summary>
        private Dictionary<String, Cell> nonEmptyCells = new Dictionary<string, Cell>();

        /// <summary>
        /// Creates a new empty spreadsheet object, where by default, all cells are empty.
        /// </summary>
        public Spreadsheet()
        {
            //Do nothing
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Return all the names of our cells in our dictionary
            return nonEmptyCells.Keys;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            IsNameInvalidOrNull(name); //If we get passed this, our name is valid
            //If this isn't a NonEmptyCell, it's value is the empty string.
            if(!GetNamesOfAllNonemptyCells().Contains(name))
            {
                return "";
            }
            else
            {
                return nonEmptyCells[name].Contents;
            }
            
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            HashSet<string> returnValue = new HashSet<String>(GetCellsToRecalculate(name));//Get our list of cells to recalculate

            MakeEmpty(name);//Make our cell empty.

            nonEmptyCells.Add(name, new Cell(number)); //Add our new cell

            return returnValue; //Return all the cells to recalculate.
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            if (text==null)
            {
                throw new ArgumentNullException();
            }

            MakeEmpty(name); //Make the cell empty.

            //If the cell's value is going to be empty, don't re-add to the dictionary
            if (text.Equals(""))
            {
                return new HashSet<String>(GetCellsToRecalculate(name)); //Return all the cells to recalculate.
            }
            else
            {
                nonEmptyCells.Add(name, new Cell(text)); //Add our new cell
                return new HashSet<String>(GetCellsToRecalculate(name)); //Return all the cells to recalculate.
            }
            
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            if (formula == null)
            {
                throw new ArgumentNullException();
            }

            //Perserve the old value.
            object oldValue = GetCellContents(name);

            MakeEmpty(name); //Make the cell empty.

            //Add all our dependencies.
            foreach (string varName in formula.GetVariables())
            {
                dependencies.AddDependency(varName, name);
            }

            nonEmptyCells.Add(name, new Cell(formula)); //Add our new cell

            IEnumerable<string> returnValue = null; //This should never exit the method as null. We throw an exeption, or replace it.

            //Check to see if the new cell will create a circular dependency
            try
            {
                returnValue = GetCellsToRecalculate(name);
            }
            catch (CircularException e)
            {
                //We have a circular dependency. Restore the old values.
                if (oldValue.GetType().Equals(typeof(double)))
                {
                    this.SetCellContents(name, (double) oldValue);
                }
                else if (oldValue.GetType().Equals(typeof(string)))
                {
                    this.SetCellContents(name, (string) oldValue);
                }
                else
                {
                    this.SetCellContents(name, (Formula) oldValue);
                }

                throw e;
            }

            return new HashSet<String>(returnValue); //All is well, return.
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name==null)
            {
                throw new ArgumentNullException();
            }

            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            return dependencies.GetDependents(name); //Return our dependents.
        }
        /// <summary>
        /// If the passed name is invalid (does not follow the established rules of variable names)
        /// or null, throws an InvalidNameException with an apropriate name.
        /// </summary>
        /// <param name="name">The name to check the validity of.</param>
        private void IsNameInvalidOrNull(string varname)
        {
            if (varname == null || varname.Length == 0)
            {
                throw new InvalidNameException();
            }

            if (!(varname[0].Equals('_') || Char.IsLetter(varname[0])))
            {
                throw new InvalidNameException(); //The variable must start with an underscore or letter.

            }

            for (int i = 1; i < varname.Length; i++)
            {
                if (!(varname[i].Equals('_') || Char.IsLetterOrDigit(varname[i])))
                {
                    throw new InvalidNameException(); //Return false if we encounter anything that's not a letter, digit, or underscore.
                }
            }
            //If we haven't broken the rules, its valid.
        }

        /// <summary>
        /// Makes the specified cell empty. If it exists and depends on cells, removes those dependencies. Has no error checking.
        /// </summary>
        /// <param name="name"></param>
        private void MakeEmpty(string name)
        {
            //If the cell exists
            if (GetNamesOfAllNonemptyCells().Contains(name))
            {
                //If the cell is a formula.
                if (nonEmptyCells[name].Contents.GetType() == typeof(Formula))
                {
                    Formula toBeRemoved = (Formula) nonEmptyCells[name].Contents;
                    //Remove the dependencies for every varName in this cell.
                    foreach (string varName in toBeRemoved.GetVariables())
                    {
                        dependencies.RemoveDependency(varName, name);
                    }
                }
                nonEmptyCells.Remove(name); //Remove it from our dictionary
            }
        }

        /// <summary>
        /// Represents a cell on a spreadsheet. 
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.
        /// 
        /// Considering all returned types are immutable, at this time, cells are immutable.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// The contents of this cell. Must be a string, a double, or a Formula.
            /// </summary>
            object contents;

            /// <summary>
            /// Creates a new Cell object, using the parameter as Contents. 
            /// The parameter must fit the definition of Contents, else an ArgumentException is thrown.
            /// </summary>
            /// <param name="contents">The value to set to be contents.</param>
            public Cell(object contents)
            {
                this.Contents = contents;
            }

            /// <summary>
            /// The contents of this cell. Must be a string, a double, or a Formula. If this property is set to anything else, 
            /// an ArgumentException is thrown.
            /// </summary>
            public object Contents
            {
                set {
                    if (value.GetType().Equals(typeof(string)))
                    {
                        this.contents = value;
                    }
                    else if (value.GetType().Equals(typeof(double)))
                    {
                        this.contents = value;
                    }
                    else if (value.GetType().Equals(typeof(Formula)))
                    {
                        this.contents = value;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                get {
                    return contents;
                }
            }

        }
    }

    
}
