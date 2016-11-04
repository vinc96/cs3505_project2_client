
SPREADSHEET README-------------------------------------------------------------------------------------------------
Written by Josh Christensen, 10/7/16
*********************************IMPLEMENTATION*****************************************
-A Spreadsheet object has a DependencyGraph from PS2, a dictionary of Cell objects, and member variables
 for functors.
-Each cell object can either contain a string, a Formula, or a double.
-When we retrieve the contents of a cell, we return whatever value that cell contains.
-Variable name validation Is done by one method.
-Value is string if the item is a string, double if item is a double, or double/FormulaError if it's a formula.
*********************************LIBRARIES**********************************************
-This solution will be built with a debug version of both the Formula and DependencyGraph items. These versions
were compiled 9/28/2016, and at compillation time, was passing all provided tests. No code changes were made 
between when these assignments were turned in, and when they were compiled for this project.
********************************NOTES***************************************************
-In this assignment, there seemed to be very, very loose specification as to how the save/load functionality should 
 work. I asked clarifying questions, but I can't shake the feeling that I missed something.
-The majority of tests were written before the code was implemented, in a Black Box style. This should
explain any percieved over-redundancy in tests, as I was operating with absoloutly no knowledge of implementation.
-Also included are tests that were written to test specific bugs discovered during development, and 
target potential weak points in implementation.


SPREADSHEETGUI README--------------------------------------------------------------------------------------------------
Written by Josh Christensen 11/3/16
*********************************IMPLEMENTATION*****************************************
The SpreadsheetGUI is a standard Windows Forms project, as per the typical MVC design pattern.
It uses a Spreadsheet object as the model, and the SpreadsheetGUI acts as both the view, and the controller. 
The view is primarily designed using event driven programming, where button presses/user input will trigger comparitively short
snippets of code that interacts with the Model.
*********************************LIBRARIES**********************************************
I used the provided SpreadsheetPanel, but I was forced to modify it because of some shortcomings it had when it came to
button input. I added methods to select a cell above, below, left and right of the current one, and modified the SetSelection method
to update the scrollbars when the selected box moves outside the current view. 
********************************NOTES***************************************************
As for extra features, my primary goal was to create a spreadsheet with a high degree of polish. Essentially, what I did was try to use
my sheet in an actual, useful capacity, and every time I would attempt to do something that wouldn't work, I would record it so I could add it. To 
this end, I made the following extra changes:
	-Title bar lists the current file
	-Title bar has an appended * if the file has been changed, but not saved.
	-Added Save As, so I could have a save button that wouldn't constantly open a dialog.
	-Added keybindings for new, open, save, save as.
	-Added keyboard control, including the arrow keys and the enter key.
	-When selecting a cell that already has stuff in it, the contents are highlighted, so any new input will clear them (this is how it works in Google Sheets)
	-Named the window something other than "Form1" (I will be incredibly surprised if anyone didn't though...