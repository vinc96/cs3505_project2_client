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
        private const String version = "ps6";
        /// <summary>
        /// The OpenFileDialog we'll be using to open any files we'll need.
        /// </summary>
        private OpenFileDialog openDialog;

        /// <summary>
        /// The spreadsheet we use as the model in our MVC paradigm. Contains all the data for our spreadsheet.
        /// </summary>
        private Spreadsheet modelSheet;

        public MainWindow(string fileLocation)
        {
            InitializeComponent();
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

            //Grab all the values in the sheet.
            updateWholeSheet();
            
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

        /// <summary>
        /// Goes through and updates every single value in the entire sheet. Used only when loading.
        /// </summary>
        private void updateWholeSheet()
        {
            //IMPLEMENT
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
        }

        private void saveOldData()
        {
            throw new NotImplementedException();
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
            //Construct the letter portion of the address.
            return "";
        }
    }
}
