# Witchi - 2D C# Game

Witchi is a 2D platformer video game created using C# and the SFML library. The player controls a tiny witch that can run, jump and shoot projectiles. The goal of the game is to collect gems and defeat enemies in order to gain score points and reach the end of the level. The game's "engine" is mostly made from scratch, leveraging SFML's API to create the game window, draw images, play sounds etc. This application is primarily a demo created to serve as my bachelor's degree project.

![Witchi gameplay](https://i.imgur.com/Znlee7Z.gif)

## Features
- Momentum-based character physics;
- Parallax scrolling and support for multiple drawing layers;
- Support for level layouts made with [Tiled](https://www.mapeditor.org/) level editor;
- Multiple languages (English, Romanian);
- High score leaderboards, both local and global (the global score database is currently closed);
- 3D sound effects;
- XInput controller support.

## Credits

All the assets are free-to-use, most of them were downloaded from [OpenGameArt.org](https://opengameart.org/); the source of each individual asset can be found in the game's **Credits** menu. I would like to thank Georgi Kanchev for creating the [TransformableHitbox2D](https://github.com/georgi-kanchev/TransformableHitbox2D) NuGet package that was used in this project's collision code, as well as for helping me with debugging SFML-related issues.
