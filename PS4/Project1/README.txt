Written by Josh Christensen, 9/28/16
*********************************IMPLEMENTATION*****************************************
-Initialy, I'm planning on creating a Spreadsheet object with a DependencyGraph from PS2, and 
a dictionary of Cell objects.
-Each cell object can either contain a string, a Formula, or a double.
-When we retrieve the contents of a cell, we return whatever value that cell contains.
-Variable name validation will be done by one method.
*********************************LIBRARIES**********************************************
-This solution will be built with V1.0 of both the Formula and DependencyGraph items. These versions
were compiled 9/28/2016, and at compillation time, was passing all provided tests.
********************************NOTES***************************************************
-The majority of tests were written before the code was implemented, in a Black Box style. This should
explain any percieved over-redundancy in tests, as I was operating with absoloutly no knowledge of implementation.
-Also included are tests that were written to test specific bugs discovered during development, and 
target potential weak points in implementation.