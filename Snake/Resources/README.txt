CLIENT****************************************************************************************

DESIGN***********************************************************
Given that the network protocol assumes a fairly "dumb" client, containing near 
no game logic whatsoever, our client has a very minimal Model. Essentially, all 
we do is: Recieve data, cache it in the world, draw it, and then wait for more data.
We store our snakes and food in Dictionary objects, indexed by ID (as this is often 
how they're referenced). We have an application agnostic Networking class to deal 
with most of the heavy lifting on the socket side, and a SnakeClient specific ClientSnakeNetworkController 
class that uses this Networking class to facilitate communication between the server and 
our client. Our view (and parts of our model) is contained in the MainWindow class,
consisting of a few custom panels that draw snakes and player names, with some 
interaction with the ClientSnakeController class, specifically to send movement commands.
EXTRA FEATURES:********************************************************
Most of our features are, rather than one large feature, smaller things that
incrementally improve the overall game. We have:
	Scoreboard Ranking:The players on the scoreboard are ranked according to score,
		as opposed to the default client's meaningless ranking.
	Scoreboard Player/Spectate focus: The player we're watching/controling is always
		displayed, underlined, at the top of the scoreboard.
	Consistent Snake Colors: Our snake colors are derived from each snake's unique ID, 
		so different players will see the snakes as the same color as their opponents.
	Spectate box: After the player dies, we give them the option to "spectate" another snake,
		viewing the game from their perspective.
	Resizable window: We address the pixels per cell count for both the X and Y directions independently,
		making our game view resizable.


SERVER**********************************************************************************************

DESIGN***********************************************************
Our server uses the same style of datastructures we discussed using in class (where snakes are functionally lists of verticies,
collision is done by checking points against verticies, etc.), but with an emphasis on good structure and OOP design. We chose
our classes and their scopes very carefully, with an emphasis on designing a well organized server that's easily modifiable and
extendible. For example, our game behavior is defined by a parameterless delegate that can be replaced or substituted easily, so 
creating custom game behaviors is as simple as writing a method that defines that behavior. Our settings data is well organized, 
as it's actually a class of its own that's constructed from the XML file, with the class consisting of several private writable, 
public readable properties to represent the various settings state of the server.
EXTRA FEATURES *******************************************************************
	Our gamemode that we implemented is TRON mode. In this mode, food does not spawn, rather your snake's tail
gets pinned to your starting position, and the head moves forward. 