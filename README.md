# Meteor Command

A small arcade game we made in Unity for the final project of the Unity 101 course at MTA.
Meteors fall from the sky toward six cities. You click to fire an interceptor, it explodes where you clicked,
and everything inside the explosion is destroyed. Protect the cities as long as you can!

By **Ilan** and **Leon**.

## How to play

- **Left mouse click** - fire an interceptor at that point in the sky.
- Every wave you get **20 shots**, so don't waste them. Some meteors land on empty ground and are harmless.
- **Large meteors split** into two smaller ones when you hit them.
- Destroying **several meteors with one explosion** gives a big combo bonus (2 meteors = 400 points, 3 = 900, ...).
- Shots you did not use at the end of a wave give a **bonus**, and then your ammo is refilled.
- From wave 4 a fast **purple scout meteor** shows up. It does not split.
- The game ends when the last city is destroyed. Your best score is saved.

## Screens

- **Main menu** - Play, Quit, short rules and your best score.
- **HUD** - wave number and progress bar, score, remaining shots and the six cities.
- **Game over** - final score, `NEW BEST!` if you beat your record, Restart and Main Menu.

## What we used from the course

- **Object pooling** - meteors, interceptors, explosions, effects and score pop-ups are reused instead of created and destroyed all the time.
- **Coroutines** - the explosion (grow, hold, shrink), the waves, the wave banner and the camera shake.
- **Singleton** - `GameManager`, `PoolManager` and `CityManager`.
- **Events** - scripts talk to each other with C# events (for example `City.Destroyed`, `GameManager.GameOver`), so the UI and the audio do not need to know about the game logic.
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
  Art/         background, icons, textures
  Audio/       sound effects and music
  Fonts/       Orbitron and Bebas Neue
  Scenes/      Game.unity (the only scene)
GDD.md         the game design document
```

Main scripts: `GameManager` (game states and score), `WaveSpawner` (the waves), `Meteor`, `Interceptor`, `Blast`,
`City`, `PoolManager`, `UIManager` and `AudioManager`.

## Credits

- **Sound effects and music** from [Pixabay](https://pixabay.com/) by `dennish18`, `dragon-studio`, `freesound_community` and `chrysalyn`.
- **Fonts**: [Orbitron](https://fonts.google.com/specimen/Orbitron) and [Bebas Neue](https://fonts.google.com/specimen/Bebas+Neue), both under the SIL Open Font License (license files are in `Assets/Fonts/Licenses`).
- **Background image** generated with ChatGPT image generation.
- We used **Claude** (an AI assistant) to help with the code and to learn Unity while we built the game.
- The original idea comes from the arcade game *Missile Command* (Atari, 1980).
