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
