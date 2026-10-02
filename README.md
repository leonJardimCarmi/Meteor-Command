# Meteor Command

A small arcade game we made in Unity for the final project of the Unity 101 course at MTA.
Meteors fall from the sky toward six cities. You click to fire an interceptor, it explodes where you clicked,
and everything inside the explosion is destroyed. Protect the cities as long as you can!

By **Ilan** and **Leon**.

## How to play

- **Left mouse click** - fire an interceptor at the crosshair. A ring with an X shows where it will explode.
- **Escape** (or the pause button) - pause and resume.
- The battery needs a moment to **reload** between shots, so every shot counts.
- Each wave gives you just enough shots to clear it, with a little extra - the number grows as the waves do, so don't waste them. Some meteors land on empty ground and are harmless.
- **Large meteors split** into two smaller ones when you hit them.
- Destroying **several meteors with one explosion** gives a big combo bonus (2 meteors = 400 points, 3 = 900, ...), and the screen shows `DOUBLE!`, `TRIPLE!` and so on.
- Shots you did not use at the end of a wave give a **bonus**, and then your ammo is refilled.
- From wave 4 a fast **purple scout meteor** shows up. It does not split.
- **The turret can be hit too.** A meteor that lands next to it damages it: the reload gets twice as slow with every hit, and the third hit destroys it. Clearing a wave repairs one hit.
- The game ends when the **last city** or the **turret** is destroyed. Your best score is saved.

## Screens

- **Main menu** - Play, Quit, short rules and your best score.
- **HUD** - wave number and progress bar, score, remaining shots, the six cities and the three turret lights.
- **Pause** - Resume and Main Menu.
- **Game over** - why it ended (`ALL CITIES LOST` or `TURRET DESTROYED`), final score, `NEW BEST!` if you beat your record, Restart and Main Menu.

## What we used from the course

- **Object pooling** - meteors, interceptors, explosions, effects and score pop-ups are reused instead of created and destroyed all the time.
- **Coroutines** - the explosion (grow, hold, shrink), the waves, the wave banner, the camera shake, the slow motion, the combo words and the turret kick.
- **Singleton** - `GameManager`, `PoolManager` and `CityManager`.
- **Events (observer pattern)** - scripts talk to each other with C# events (for example `City.Destroyed`, `TurretHealth.Hit`, `GameManager.GameOver`), so the UI, the audio and the effects do not need to know about the game logic.
- **Physics** - the explosion is a trigger collider that destroys the meteors it touches.
- **UI** - a Canvas that scales with the screen size, TextMeshPro, and buttons that call the game manager.
- **Audio** - `AudioManager` plays all the sounds and the music, with a volume slider for each in the Inspector.
- **PlayerPrefs** - the best score is saved between runs.
- **Gizmos** - in the Scene view you can see where meteors spawn and how close an impact must be to hit a city or the turret.
- **Particle effects** - explosions, smoke and fire are particle systems made in code.
- **Prefabs and the Inspector** - almost every number (speeds, wave sizes, blast radius, etc.) is a `[SerializeField]` so we can tune the game without touching the code.

## How to run

1. Install **Unity 6000.3.20f1** (Unity 6.3 LTS) with the Universal Render Pipeline.
2. Clone the repository and open the folder with Unity Hub.
3. Open the scene `Assets/Scenes/Game.unity` and press **Play**.

The game is made for PC (Windows) and is played only with the mouse.

## Project structure

```
Assets/
  Scripts/     all the code (one small script per job)
  Prefabs/     meteor, interceptor, explosion, effects
  Materials/   glowing materials, city and ground
  models/      Kenney 3D models: turret, missile, meteor rocks, roads and trees
  Art/         background, icons, textures
  Audio/       sound effects and music
  Fonts/       Orbitron and Bebas Neue
  Scenes/      Game.unity (the only scene)
GDD.md         the game design document (images are in the images folder)
GDD_BONUS.md   everything we added or changed after the GDD was approved
```

Main scripts: `GameManager` (game states and score), `WaveSpawner` (the waves), `Meteor`, `Interceptor`, `Blast`,
`City`, `Battery` and `TurretHealth` (the turret), `PoolManager`, `UIManager` and `AudioManager`.

## Documents

- [`GDD.md`](GDD.md) - the approved game design document: what we planned.
- [`GDD_BONUS.md`](GDD_BONUS.md) - what we added or changed on top of it while building the game, and why.

## Credits

- **3D models** by [Kenney](https://kenney.nl/) (CC0): the turret (Blaster Kit), the missile parts (Space Kit), the meteor rocks and trees (Nature Kit), and the roads, signs and street lamp (City Kit Roads). The city buildings themselves are built in code.
- **Sound effects and music** from [Pixabay](https://pixabay.com/) by `dennish18`, `dragon-studio`, `freesound_community` and `chrysalyn`.
- **Fonts**: [Orbitron](https://fonts.google.com/specimen/Orbitron) and [Bebas Neue](https://fonts.google.com/specimen/Bebas+Neue), both under the SIL Open Font License (license files are in `Assets/Fonts/Licenses`).
- **Background image** generated with ChatGPT image generation.
- **`images/reference-missile-command.png`** (used in the GDD only, for comparison) is a photo by Brett L. from San Francisco, California, USA, licensed [CC BY-SA 2.0](https://creativecommons.org/licenses/by-sa/2.0/), via [Wikimedia Commons](https://commons.wikimedia.org/wiki/File:MISSILE_COMMAND_(2392548208).jpg).
- We used **Claude** (an AI assistant) to help with the code and to learn Unity while we built the game.
- The original idea comes from the arcade game *Missile Command* (Atari, 1980).
