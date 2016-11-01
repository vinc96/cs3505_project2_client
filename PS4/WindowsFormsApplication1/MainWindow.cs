using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// The version we use when saving or loading spreadsheets.
        /// </summary>
        private const String version = "ps6";
        
        /// <summary>
        /// The OpenFileDialog we'll be using to open any files we'll need.
        /// </summary>
        private OpenFileDialog openDialog;

        /// <summary>
        /// The spreadsheet we use as the model in our MVC paradigm. Contains all the data for our spreadsheet.
        /// </summary>
        private Spreadsheet modelSheet;

        /// <summary>
        /// The column that was selected before the current one. Allows us to do cell updates whenever focus leaves any particular cell.
        /// </summary>
        private int lastCol;
        /// <summary>
        /// The row that was selected before the current one. Allows us to do cell updates whenever focus leaves any particular cell.
        /// </summary>
        private int lastRow;
        
        public MainWindow(string fileLocation)
        {
            InitializeComponent();

            //Set up lastCol and lastRow: the "last" items selected in this case are the starting values.
            spreadsheetPanel1.GetSelection(out lastCol, out lastRow);
            //Set up the open dialog, and add our file open listener.
            openDialog = new OpenFileDialog();
            openDialog.AddExtension = true;
            openDialog.FileOk += OpenFileListener;
            //


            //If the fileLocation is null, open an empty sheet. Else, open the sheet at fileLocation.
            if (ReferenceEquals(fileLocation, null))
            {
                modelSheet = new Spreadsheet(isValid, normalizer, version);
            }
            else
            {
                modelSheet = new Spreadsheet(fileLocation, isValid, normalizer, version);
            }

            //Update all the values on load.
            updateCells(modelSheet.GetNamesOfAllNonemptyCells());

        }
        
        public MainWindow() : this(null)
        {
            
        }

        /// <summary>
        /// The validator we use for our spreadsheet objects. Returns true regardless.
        /// </summary>
        /// <param name="s">The string input. This validator returns true regardless.</param>
        /// <returns></returns>
        private bool isValid(String s)
        {
            return true;
        }

        /// <summary>
        /// The normalizer we use in our spreadsheet objects. Normalizes all strings to uppercase.
        /// </summary>
        /// <param name="stringToBeNormalized">The string to be normalized. Returns this string, 
        /// shifted to uppercase.</param>
        /// <returns>The input string, turned uppercase.</returns>
        private string normalizer(String stringToBeNormalized)
        {
            return stringToBeNormalized.ToUpper();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openDialog.ShowDialog();
        }

        /// <summary>
        /// The listener that we use to open files. Should be registered with our openDialog.FileOK Listener. If it's not used
        /// in this fashion, does nothing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileListener(object sender, CancelEventArgs e)
        {
            //If we're opening a file, perform actions required to do that.
            if (sender.GetType().Equals(typeof(OpenFileDialog)))
            {

            }
            else
            {
                //Else, this delegate was used incorrectly, and we should do nothing.
            }
        }

        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {

            saveOldData(); //Put whatever was in the input box into the form, and update relevant cells.

            grabNewData(); //Grab the data from the new selection, and put it where it needs to go in the view. 
        }

        private void grabNewData()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            string cellNameString = coordsToCellName(col, row);
            //Update the location text.
            cellName.Text = cellNameString;
            //Update the value text.
            cellValue.Text = modelSheet.GetCellValue(cellNameString).ToString();
            //Update the content text. Be careful, if we're grabbing a formula, we need to preappend a "="
            if (modelSheet.GetCellContents(cellNameString).GetType().Equals(typeof(Formula)))
            {
                cellContents.Text = "=" + modelSheet.GetCellContents(cellNameString).ToString();
            }
            else
            {
                cellContents.Text = modelSheet.GetCellContents(cellNameString).ToString();
            }
            
            //Update lastCol and lastRow to our current position.
            lastCol = col;
            lastRow = row;
        }

        private void saveOldData()
        {
            //Take the current contents of the cellContents box, and add it to the spreadsheet.
            string cellNameString = coordsToCellName(lastCol, lastRow);
            ISet<String> cellsToUpdate = modelSheet.SetContentsOfCell(cellNameString, cellContents.Text);
            updateCells(cellsToUpdate); //Update the cells that need re-evaluation.


            
        }

        private void updateCells(IEnumerable<string> cellsToUpdate)
        {
            foreach (string name in cellsToUpdate)
            {
                updateCell(name);
            }
        }

        /// <summary>
        /// Updates the displayed value of a cell in our spreadsheetPanel, specified by it's name, rather than coordinates. 
        /// </summary>
        /// <param name="cellName"></param>
        private void updateCell(string cellName)
        {
            int col, row;
            cellNameToCoords(cellName, out col, out row);
            spreadsheetPanel1.SetValue(col, row, modelSheet.GetCellValue(cellName).ToString());
        }
        /// <summary>
        /// Takes a cell name from our sheet in the form of a name, and returns the col and row corresponding to that cell name.
        /// In the case that the cellName does not consist of one or more uppercase letters followed by one or more integers, throws
        /// InvalidArgumentException.
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void cellNameToCoords(string cellName, out int col, out int row)
        {
            string LETTERBANK = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            col = 0;

            //Go through and convert each letter to a number. This is essentially a little endian base26 system, and ought to be treated as such. 
            int baseValue = 1;
            while(Char.IsUpper(cellName[0]))
            {
                char c = cellName[0];
                col += (LETTERBANK.IndexOf(c) + baseValue);
                cellName = cellName.Substring(1);
                baseValue *= 26;
            }
            //Subtract 1 from the column #, to make it zero based.
            col -= 1;
            //Take the remaining string, which ought to just be a number.
            int.TryParse(cellName, out row);
            //We should never have a zero parsed result. If we get a zero, we either failed to parse (meaning we had an invalid cell name), 
            //or we parsed a zero (which is an invalid cell name).
            if (row == 0)
            {
                throw new ArgumentException("Not a valid cellName: " + cellName);
            }
            //Subtract 1 from the row number, to make it zero based
            row -= 1;
        }

        /// <summary>
        /// Takes a column and row, and converts it to a string corresponding to those coordinates' location
        /// in a standard spreadsheet layout. col = 0, row = 0 is converted to "A1", col = 1, row = 0 is converted to "A2", 
        /// col = 1, row = 1 is converted to "B2", etc. Returns the string containing this name. Returns uppercase strings.
        /// </summary>
        /// <param name="col">The column to convert</param>
        /// <param name="row">the row to convert</param>
        /// <returns></returns>
        private string coordsToCellName(int col, int row)
        {
            string LETTERBANK = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string cellname = "";
            //Construct the letter portion of the address.
            while (col >= 0)
            {
                cellname += LETTERBANK[col % 26];
                col -= 26;
            }
            //Add the number portion of the address.
            cellname += (row + 1);
            return cellname;
        }
    }
}
