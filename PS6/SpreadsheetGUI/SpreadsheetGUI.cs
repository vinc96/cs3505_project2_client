﻿using SpreadsheetClient;
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
        /// A list of all Users who access the spreadsheet.
        /// The users are mapped to unique colors so that the cells they edit can be highlighted
        /// </summary>
        private Dictionary<string, Color> Users;

        /// <summary>
        /// <author>AtShar</author>
        /// Network controller to send and receive requests to and from the server
        /// </summary>
        private ClientController clientController;

        /// <summary>
        /// The location where the sheet we're working on was last saved. Null if it hasn't been saved yet.
        /// </summary>
        private string lastSaveLocation;
        /// <summary>
        /// The message to be displayed when the user clicks on the help button.
        /// </summary>
        private string helpMessage = "How to Use This Spreadsheet: \n\n" +
            "Entering Data:\n\n" +

            "In order to enter data, simply type it into the editable box at the top of the window. " +
            "The box ought to be already selected if there's something in the form, allowing you to overwrite it with a single keystroke.\n\n" +

            "In order to save data, type something into the top box, and do one of the following:\n" +
            "\t Hit the Enter button\n" +
            "\t Hit the Enter key\n" +
            "\t Select another box with the arrow keys\n" +
            "\t Select another box with the mouse\n\n" +

            "Saving/Loading:\n\n" +
            "Saving and loading works the same as in any other windows application. There is Save, Save As, and load functionality.\n\n" +
            "In order open a new file, either hit Ctrl+N, or select \"New\" from the file menu.\n" +
            "In order to open a file, either hit Ctrl+O, or select \"Open\" from the file menu.\n" +
            "In order to save the form, either hit Ctrl+S, or select \"Save\" from the file menu.\n" +
            "In order to Save As , either hit Ctrl+Shift+S, or select \"Save As\" from the file menu.\n\n" +

            "Other Assorted Sugar:\n\n" +
            "When a file is loaded, its file location is in the title bar.\n" +
            "When a file that's open has unsaved changes, an asterisk (*) appears in the title bar.\n";

        public SpreadsheetGUI()
        {
            //AtShar: initalize fields
            clientController = new ClientController();
            Users = new Dictionary<string, Color>();

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
            saveFile();
        }

        /// <summary>
        /// Saves the current file. If the saveLocation is null, shows a dialog. If saveLocation is non-null, saves to the previously 
        /// saved location.
        /// </summary>
        private void saveFile()
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
            openNewWindow();
        }
        /// <summary>
        /// Opens a new window by creating another version of this one in a new thread.
        /// </summary>
        private void openNewWindow()
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
            MessageBox.Show(helpMessage);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Shift:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //If we're getting data from arrow keys, move stuff around
            switch (e.KeyCode)
            {
                case Keys.Down: //Down arrow
                    spreadsheetPanel1.selectDown();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up: //Up arrow
                    spreadsheetPanel1.SelectUp();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Left: //Left arrow
                    spreadsheetPanel1.selectLeft();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Right: //Right arrow
                    spreadsheetPanel1.selectRight();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter: //Enter key
                    spreadsheetPanel1.selectDown();
                    e.SuppressKeyPress = true; //Keeps the form from making a "bing" sound whenever you press enter.
                    break;
                case Keys.N: //New shortcut
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        openNewWindow();
                    }
                    break;
                case Keys.O: //Open shortcut
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        openDialog.ShowDialog();
                    }
                    break;
                case Keys.S: //Save/Save As shortcut
                    if (ModifierKeys.HasFlag(Keys.Control) && !ModifierKeys.HasFlag(Keys.Shift))
                    {
                        saveFile();//Save
                        e.SuppressKeyPress = true; //Keeps the form from making a "bing" sound whenever you press this key combo.
                    }

                    if (ModifierKeys.HasFlag(Keys.Control) && ModifierKeys.HasFlag(Keys.Shift))
                    {
                        saveDialog.ShowDialog();//Save As
                        e.SuppressKeyPress = true; //Keeps the form from making a "bing" sound whenever you press this key combo.
                    }
                    break;
                case Keys.F4: //Open shortcut
                    if (ModifierKeys.HasFlag(Keys.Alt))
                    {
                        this.Close();
                    }
                    break;

            }

            base.OnKeyDown(e);

        }

        /// <summary>
        /// A callback to handle a successful server connection.
        /// Starts Data Listening loop.
        /// <author>AtShar<author/>
        /// </summary>
        /// <param name="initData"></param>
        private void handleHandshakeSuccess(ClientController.StartupData setupData)
        {
            if (setupData.ErrorOccured)
            {
                Invoke(new MethodInvoker(() => { handleSocketError(setupData.ErrorMessage); }));
                return;
            }

            Invoke(new MethodInvoker(() => {
                this.spreadsheetPanel1.Focus();
            }));

            //AtShar: Populate the spreadsheet after it has been instantiated
            modelSheet = new Spreadsheet();
            foreach(string cell in setupData.Cells.Keys)
            {
                modelSheet.SetContentsOfCell(cell,setupData.Cells[cell]);
            }

            clientController.startDataListenerLoop(recievedData);
        }

        /// <summary>
        /// <author>AtShar</author>
        /// Handle recieving data from the socket. Update the apropriate parts of the model.
        /// Only change the parts of the spreadsheet required OR Retrieve the information requested.
        /// </summary>
        /// <param name="data"></param>
        private void recievedData(IList<string> data)
        {
            if (data.FirstOrDefault() == "ERROR")
            {
                Invoke(new MethodInvoker(() => { handleSocketError(data[1]); }));
                return;
            }

            foreach (string message in data)
            {
                //AtShar+vinc: get contents of the message (Type + cell&value pairs)
                string[] messageComponents = message.Trim().Split('\t');
                lock (modelSheet)
                {

                    switch (messageComponents[0])
                    {
                        case "Startup":
                            {
                                //AtShar: Clear spreadsheet
                                modelSheet = new Spreadsheet();

                                //vinc: Need to clear panel
                                spreadsheetPanel1.Clear();

                                Invalidate();

                                if (messageComponents.Length % 2 != 1)
                                {
                                    //AtShar: The number of components in the startup should be odd
                                    //MessageType+Pairs of cell+value
                                    MessageBox.Show("Invalid startup message received from server.");
                                }
                                for (int i = 1; i < messageComponents.Length; i+=2)
                                {
                                    modelSheet.SetContentsOfCell(messageComponents[i],messageComponents[i+1]);
                                }
                                break;
                            }
                        case "Change":
                            {
                                if (messageComponents.Length != 3)
                                {
                                    //AtShar: The number of components in the startup should be three
                                    //MessageType+One pair of cell+value
                                    MessageBox.Show("Incomplete change request.");
                                }
                                modelSheet.SetContentsOfCell(messageComponents[1],messageComponents[2]);
                                break;
                            }
                        case "IsTyping":
                            {
                                if (messageComponents.Length != 3)
                                {
                                    //AtShar: The number of components in the startup should be three
                                    //MessageType+One pair of cell+value
                                    MessageBox.Show("Incomplete Typing-Status request.");
                                }

                                if (Users.ContainsKey(messageComponents[1]))
                                {
                                    //change the panel background color
                                }
                                else
                                {
                                    //AtShar: Assign random color to each new user
                                    Users.Add(messageComponents[1],RandomColorObject(messageComponents[1]));
                                }

                                break;
                            }
                        case "DoneTyping":
                            {

                                break;
                            }
                        default:
                            {
                                //AtShar:Recommendation: Remove message box and do nothing for invalid requests.
                                MessageBox.Show("The server is sending messages that are not understood.");
                                break;
                            }
                    }
                }
            }
            Invoke(new MethodInvoker(updateView));
        }


        /// <summary>
        /// <author>AtShar</author>
        /// Updates the view. Called on a per-tick basis, whenever there's new data.
        /// Updating the state of the spreadsheet.
        /// </summary>
        private void updateView()
        {
            //Update game display
            spreadsheetPanel1.Update();
        }

        private Color RandomColorObject(string ID)
        {
            int hashCode = (ID.ToString() + "SaltyMcSaltPants").GetHashCode();
            return Color.FromArgb(255, (hashCode & 0x00FF0000) >> 16, (hashCode & 0x0000FF00) >> 8, hashCode & 0x000000FF);
        }


        /// <summary>
        /// Displays error message on an unsuccesful initial connect attempt
        /// <author>AtShar<author/>
        /// </summary>
        /// <param name="message"></param>
        private void handleSocketError(string message)
        {
            MessageBox.Show(message);
            //Need to implement commented buttons
            //DO NOT DELETE!!!!!!!!!!!!!!!!
            //inpHostname.Enabled = true;
            //inpPlayerName.Enabled = true;
            //Need to clear spreadsheet
            //btnConnectToServer.Enabled = true;
        }


        //private class User
        //{
        //    Color assignedColor;
        //    string clientID;
        //    public User(string uniqueID)
        //    {
        //        clientID = uniqueID;
        //    }
        //}

    }


}
