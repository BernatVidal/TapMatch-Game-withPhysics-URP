# TapMatch-Game-withPhysics-URP
A simple mobile tap match game, with some physics related. Developed in under 18 hours as a test.

!! IMPORTANT !!
To set it up properly, you will need to first create a Unity 2D Template with URP and then download the repo
!!!!!!!!!!!!!!!

Tap Match game approach, by Bernat Vidal

About

This is a Tap Match game developed in Unity2D, that uses physics to provide a funnier and more satisfying experience to the user, as well as simplify grid related systems, while approaching DreamBlast’s game features. Just set the rows, columns and color variety you want on the Game Manager, and enjoy!

The approach implements a game events system to avoid instances the necessity to know each other, while laying the foundations for a modular architecture.

Multiple pooling-handling system is used to improve overall performance of the game, while providing scalability.

The algorithm used to detect Matches is a simplified Deep-First Search algorithm.

Matchable popping animations and user punctuation feedback are added to improve game enjoyability.

--> Main game classes:

![alt text](https://github.com/BernatVidal/TapMatch-Game-withPhysics-URP/blob/main/mainClassesUML.png?raw=true)

--> To Play:

Just set the game settings you want on the GameManager:

![alt text](https://github.com/BernatVidal/TapMatch-Game-withPhysics-URP/blob/main/setupPlay.png?raw=true)

And have fun!

![alt text](https://github.com/BernatVidal/TapMatch-Game-withPhysics-URP/blob/main/gameplayImg.png?raw=true)
