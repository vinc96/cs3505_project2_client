///Written by Josh Christensen u0978248
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class SpreadsheetGUI : Form
    {
        /// <summary>
        /// The base window title for our spreadsheet program. Modifiers are concatenated to this to show the status of the program.
        /// </summary>
        private const String WINDOWTITLE = "Super Spreadsheet";
        /// <summary>
        /// The version we use when saving or loading spreadsheets.
        /// </summary>
        private const String VERSION = "ps6";
        
        /// <summary>
        /// The OpenFileDialog we'll be using to open any files we'll need.
        /// </summary>
        private OpenFileDialog openDialog;

        /// <summary>
        /// The SaveFileDialog we'll be using to save all the files we want.
        /// </summary>
        private SaveFileDialog saveDialog;

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

        /// <summary>
        /// The location where the sheet we're working on was last saved. Null if it hasn't been saved yet.
        /// </summary>
        private string lastSaveLocation;

        public SpreadsheetGUI()
        {
            InitializeComponent();

            //Set up lastCol and lastRow: the "last" items selected in this case are the starting values.
            spreadsheetPanel1.GetSelection(out lastCol, out lastRow);
            //Set up the open dialog, and add our file open listener.
            openDialog = new OpenFileDialog();
            openDialog.Filter = "Spreadsheet Files (.sprd)| *.sprd|All Files|*";
            openDialog.FileOk += OpenFileListener;
            //Set up the save dialog, and add our file save listener.
            saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Spreadsheet Files (.sprd)| *.sprd|All Files|*";
            saveDialog.FileOk += SaveFileListener;

            //Set up our empty modelSheet.
            modelSheet = new Spreadsheet(isValid, normalizer, VERSION);

            grabNewDisplayedData(); //Populate the UI for the current cell.
        }

        /// <summary>
        /// The validator we use for our spreadsheet objects. Returns true if the cell is within the spreadsheet grid, false otherwise.
        /// </summary>
        /// <param name="s">The string input. This validator returns true regardless.</param>
        /// <returns></returns>
        private bool isValid(String s)
        {
            int integerPart;
            int.TryParse(s.Substring(1), out integerPart);
            //In order to be valid, the first character has to be an uppercase letter, and the second and third together should parse between 1 and 99 (inclusive)
            if (Char.IsUpper(s[0]) && (1 <= integerPart && integerPart <= 99))
            {
                return true;
            }
            else
            {
                return false;
            }
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

        /// <summary>
        /// Fired when we click the "Open" option in the file dialog. Opens a file open dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                potentialDataLoss();
                string fileLocation = ((OpenFileDialog)sender).FileName;
                //If the fileLocation is null, open an empty sheet. Else, open the sheet at fileLocation.
                modelSheet = new Spreadsheet(fileLocation, isValid, normalizer, VERSION);
                lastSaveLocation = fileLocation;
                updateWindowTitle();

                //Replace the spreadsheet panel, so that we don't have any lingering data values
                spreadsheetPanel1.Clear();
                //Update all the values on load.
                updateCells(modelSheet.GetNamesOfAllNonemptyCells());
                //Pull the data from our current location
                grabNewDisplayedData();
            }
            else
            {
                //Else, this delegate was used incorrectly, and we should do nothing.
            }
        }

        /// <summary>
        /// Fired when we hit the Save As button in the file menu. Opens a file save dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveDialog.ShowDialog();
        }

        /// <summary>
        /// Fired when we hit the save button in the file menu. If the file hasn't been saved before, opens up a "save as" dialog.
        /// If it has been saved before, overwrites the file previously saved to/opened from.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ReferenceEquals(lastSaveLocation, null))
            {
                saveDialog.ShowDialog();
            }
            else
            {
                modelSheet.Save(lastSaveLocation);
                updateWindowTitle();
            }
        }

        /// <summary>
        /// This is the listener that we use to save files. Should be reigstered with our saveDialog.FileOK listener. If
        /// it's not used in this fashion, does nothing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileListener(object sender, CancelEventArgs e)
        {
            //If we're saving a file, perform actions required to do that.
            if (sender.GetType().Equals(typeof(SaveFileDialog)))
            {
                modelSheet.Save(((SaveFileDialog) sender).FileName);
                lastSaveLocation = ((SaveFileDialog) sender).FileName;
                updateWindowTitle();//Update the window title, so that we can remove a "*" if needed.
            }
            else
            {
                //Else, this delegate was used incorrectly, and we should do nothing.
            }
        }

        /// <summary>
        /// Fired when a new cell is selected in the spreadsheet panel. Takes the data from the view, pushes it to the model, and 
        /// updates the view to the model's new values.
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {

            saveOldInputs(); //Put whatever was in the input box into the form, and update relevant cells.

            grabNewDisplayedData(); //Grab the data from the new selection, and put it where it needs to go in the view. 
        }

        /// <summary>
        /// Fired when someone clicks the enter button. Shifts the selection down, which fires the SelectionChanged listener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enterButton_Click(object sender, EventArgs e)
        {
            spreadsheetPanel1.selectDown();
        }

        /// <summary>
        /// Grabs the data from the newly selected cell, and pushes it to the relevent sections in the GUI.
        /// </summary>
        private void grabNewDisplayedData()
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);
            string cellNameString = coordsToCellName(col, row);
            //Update the location text.
            cellNameBox.Text = cellNameString;
            //Update the value text.
            cellValueBox.Text = modelSheet.GetCellValue(cellNameString).ToString();
            //Update the content text. Be careful, if we're grabbing a formula, we need to preappend a "="
            if (modelSheet.GetCellContents(cellNameString).GetType().Equals(typeof(Formula)))
            {
                cellContentsBox.Text = "=" + modelSheet.GetCellContents(cellNameString).ToString();
            }
            else
            {
                cellContentsBox.Text = modelSheet.GetCellContents(cellNameString).ToString();
            }

            //Give the contents box user focus. 
            cellContentsBox.Focus();
            cellContentsBox.SelectAll();
            
            //Update lastCol and lastRow to our current position.
            lastCol = col;
            lastRow = row;
        }

        /// <summary>
        /// Takes the data from the input boxes, and if it's valid, saves it to the spreadsheet.
        /// </summary>
        private void saveOldInputs()
        {
            //Take the current contents of the cellContents box, and if it's different, add it to the spreadsheet.
            string cellNameString = coordsToCellName(lastCol, lastRow);
            //Determine if the cell contets have changed. Special case for formulas, because of the preappended "=".
            bool cellChanged;
            if (modelSheet.GetCellContents(cellNameString).GetType().Equals(typeof(Formula)))
            {
                //Special case for making empty cells
                if (cellContentsBox.Text.Length == 0)
                {
                    cellChanged = true;
                }
                else
                {
                    //Compare the contents of the spreadsheet, with the contents of the cellContentsBox, minus the "=".
                    cellChanged = !modelSheet.GetCellContents(cellNameString).ToString().Equals(cellContentsBox.Text.Substring(1));
                }
            }
            else
            {
                cellChanged = !modelSheet.GetCellContents(cellNameString).ToString().Equals(cellContentsBox.Text);
            }
            //If we've changed the cell contents, change the model, and update our front end.
            if (cellChanged)
            {
                try
                {
                    ISet<String> cellsToUpdate = modelSheet.SetContentsOfCell(cellNameString, cellContentsBox.Text);
                    updateCells(cellsToUpdate); //Update the cells that need re-evaluation.

                    //Run the updateWindowTitle method, so we'll indicate that the spreadsheet changed.
                    updateWindowTitle();
                    
                }
                catch (FormulaFormatException)//If we catch an invalid formula error, inform the user.
                {
                    MessageBox.Show("Error: Invalid Formula!"); 
                }
                catch (CircularException)//If we catch a circular exception error, inform the user.
                {
                    MessageBox.Show("Error: You've entered a formula that has a circular dependency!");
                }
            }
            
        }

        /// <summary>
        /// Goes through the passed enumerable and runs updateCell on each cell named.
        /// </summary>
        /// <param name="cellsToUpdate"></param>
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
        /// In the case that the cellName does not consist of one uppercase letter followed by one or more integers, throws
        /// InvalidArgumentException.
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void cellNameToCoords(string cellName, out int col, out int row)
        {
            string LETTERBANK = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            col = 0;

            //Go through and convert each letter to a number. We only handle cell names with a single starting letter.
            char c = cellName[0];
            col += (LETTERBANK.IndexOf(c));
            cellName = cellName.Substring(1);
            
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

        /// <summary>
        /// If we try to close before the file is saved, warn the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            potentialDataLoss();
        }

        /// <summary>
        /// A method designed to be called when there is the potential for data loss in our form, so that we can interupt the process, and
        /// ensure the user has a chance to save. 
        /// </summary>
        private void potentialDataLoss()
        {
            if (modelSheet.Changed)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save?", "Save file?", MessageBoxButtons.YesNo);
                if (result.Equals(DialogResult.Yes))
                {
                    saveDialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Fired when you click on the "close" menu item in the file menu. Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Fired when you click on the "New" menu item in the file menu. Opens a new empty file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(() => { Application.Run(new SpreadsheetGUI()); }));
            newThread.SetApartmentState(ApartmentState.STA); //Open/Save dialogs require STA threads.
            newThread.Start();
        }
        /// <summary>
        /// Sets the window title to be up to date, given the current state of the sheet. Grabs the edit state and file
        /// location and concatenates them together into a valid title.
        /// </summary>
        private void updateWindowTitle()
        {
            //Add the file location (if it exists)
            if (ReferenceEquals(lastSaveLocation, null))
            {
                this.Text = WINDOWTITLE;
            }
            else
            {
                this.Text = WINDOWTITLE + ": " + lastSaveLocation;
            }

            //Add the save state indicator
            if (modelSheet.Changed)
            {
                this.Text += "*";
            }
        }
        /// <summary>
        /// Fires when the main window is shown. Used here in order to ensure that when the window is shown, the editable cell contents box is in focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            cellContentsBox.Focus();
        }
        /// <summary>
        /// Fired when the help menu button is clicked. Opens a help window that explains how to use the spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //If we're getting data from arrow keys, move stuff around
            switch (e.KeyCode)
            {
                case Keys.Down:
                    spreadsheetPanel1.selectDown();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    spreadsheetPanel1.SelectUp();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Left:
                    spreadsheetPanel1.selectLeft();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Right:
                    spreadsheetPanel1.selectRight();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                    spreadsheetPanel1.selectDown();
                    e.SuppressKeyPress = true; //Keeps the form from making a "bing" sound whenever you press enter.
                    break;
            }

            base.OnKeyDown(e);

        }
    }
}
