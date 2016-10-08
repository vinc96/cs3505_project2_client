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
