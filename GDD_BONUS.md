# Meteor Command: GDD Bonus

Everything we built **on top of** the approved game design document (`GDD.md`, first approved as v2.0).
The GDD says what we planned. This file says what we added or changed while building and playtesting, and why.

If something here differs from `GDD.md`, **this file describes what the game really does**.

---

## 1. New gameplay rules

| Addition | How it works | Why we added it |
|---|---|---|
| **Reload delay** | After every shot the battery needs `0.5 s` to reload. Clicks during the reload do nothing, and a short reload sound plays. | Stops the player from spamming shots and makes each shot feel like a decision. |
| **Turret hit points** | The turret has **3 hit points**. A meteor that lands within `1.5` units of it is a hit. Each hit **doubles the reload time** (`0.5 s`, then `1 s`, then `2 s`). The third hit destroys the turret and ends the game. | Meteors that landed on the turret used to do nothing, which made no sense. Now the turret is something to protect, like the cities. |
| **Turret repair** | Clearing a wave repairs **one** hit point (never above 3). | Without it, slow reloading causes more hits and the run is lost for sure. |
| **Two ways to lose** | The game ends when the last city is destroyed (`ALL CITIES LOST`) **or** when the turret is destroyed (`TURRET DESTROYED`). The Game Over screen shows which one. | A consequence of the turret hit points. |
| **Ammo grows with the wave** | The shots for a wave are the number of meteors in that wave plus 25 percent, rounded up. The GDD had a fixed 20. | From about wave 6 a wave had more meteors than 20 shots. The ammo icon row stops growing at 50. |
| **Meteors aimed at targets** | 55 percent of the meteors are aimed at a city that is still standing, 25 percent at the turret, and the other 20 percent land anywhere along the ground, as the GDD describes. All three numbers are in the Inspector of `WaveSpawner`. | A friend who tested the game found it strange that many meteors landed on empty ground and nothing happened. Now most meteors threaten something, including the turret, and the player still has to choose which ones to stop. |
| **Pause** | `Escape` or the pause button in the HUD. Time and sound freeze together. Resume or go back to the main menu. | A basic feature every game needs. |

## 2. New feedback for the player

| Addition | What the player sees or hears |
|---|---|
| **Combo words** | One explosion that destroys 2, 3, 4 or 5+ meteors shows `DOUBLE!`, `TRIPLE!`, `MEGA COMBO!`, `INSANE COMBO!` in the middle of the screen. The colour gets hotter with the combo. |
| **Slow motion** | A short slow motion moment on a 3 meteor combo, when a city is lost, and when the turret is destroyed. It uses real time, so it is the same length every time, and a pause during it stays paused. |
| **Danger glow** | The edges of the screen pulse red when only one city is left, or the turret is on its last hit point. |
| **Crosshair** | In the game the mouse arrow is replaced by a crosshair. In the menus and on the pause screen the normal arrow comes back. |
| **Target marker** | A ring with an X shows where each missile will explode, from the moment it is fired until it arrives. |
| **Muzzle flash and recoil** | A bright flash and sparks at the end of the barrel, and the turret kicks back for a moment, on every shot. |
| **Warning siren** | A red rotating light on a post next to the turret, like the beacon on a real air defence launcher. It flashes brightest when it points at the camera. |
| **Turret damage look** | One hit: light smoke. Two hits: heavy smoke. Three hits: the gun blows up and leaves a burning wreck. |
| **Turret hit points in the HUD** | Three small round lights next to the ammo. The last one turns red. They are hidden on the main menu. |
| **Marks on the ground** | Every meteor that lands leaves a ring that spreads out, a hot orange glow that cools down, and a dark scorch mark that fades away after a few seconds. So the player can see that a meteor really landed, also when it was harmless. The menu says it too: "Meteors that land between the cities do no damage." |
| **Rising kill sound** | Each extra kill in one explosion plays the kill sound a little higher. The sound slot in `AudioManager` is empty for now, so it is silent until a clip is added. |
| **More sound** | A reload sound, two blast sounds that alternate, a click on any mouse click while not playing (also on the pause screen), and the sounds have separate volume sliders. |

## 3. Visuals beyond the plan

- **Meteors:** real rock models that tumble in the air, with a faint red tint, a flame trail and a smoke trail. The rock turns around its own centre, so the flame always starts from the middle of the rock.
- **Missile:** built from four rocket parts. A short exhaust flame at the back and a thick white smoke trail that stays in the sky for a couple of seconds and fades. The smoke is separate from the missile, because the missile goes back into the pool the moment it arrives.
- **Explosions:** layered particle effects made in code (sparks, ring, fireball, smoke, debris), plus a custom shader for the bubble of the blast (`BlastBubble.shader`). The GDD planned the free Unity Particle Pack.
- **Cities:** six different towers built in code with neon windows (`BuildingMesh`). A destroyed city shakes, sinks, breaks into a pile of rubble and burns with smoke, embers and a flickering light.
- **Night look:** dark camera, moonlight, bloom, and a night sky and lake picture made with AI image generation (credited in the README).
- **Menu:** logo with spaced letters, a tagline, the three rules on screen, and fonts Orbitron and Bebas Neue.
- **Everything drawn in code:** the crosshair, the target marker, the red danger glow and the turret lights are small pictures drawn by `ProceduralSprite`, so the project needs no extra image files for them.

## 4. Technical extras

- **Object pooling** goes further than the plan: also the score pop ups and explosion effects, not only meteors, interceptors and blasts.
- **Observer pattern** (C# events) between almost all scripts, for example `TurretHealth.Hit`, `TurretHealth.Destroyed`, `City.Destroyed`, `GameManager.GameOver`. Every script that subscribes also unsubscribes.
- **Gizmos:** in the Scene view the spawn area, the target ground, and the hit radius of the cities and the turret are drawn (`OnDrawGizmos` in `WaveSpawner`, `CityManager` and `TurretHealth`).
- **Speed:** the crosshair and the danger glow each have their own UI canvas, so Unity does not rebuild the whole HUD every frame when they move. The missile smoke is limited to a few hundred puffs.
- **Audio trimming** (`AudioTrim`): leading silence is cut and long clips are capped, so a sound starts the moment it is triggered.
- **Tested without opening the game by hand:** new effects were checked with scripted runs of Unity that save screenshots, so we could see how they look before playing.

## 5. Things that changed from the plan

| Plan in `GDD.md` | What we did | Why |
|---|---|---|
| Layered low poly mountains with distance fog | One night sky and lake picture | The fog could not be seen against one flat picture, so we removed the idea. |
| Kenney city kit buildings | Buildings built in code | We liked the look, and each tower is different. |
| Unity Particle Pack explosions | Our own layered particle code | More control over the look and no extra asset. |
| Sounds from Freesound.org | Sounds from Pixabay | Easier to download, licence is free to use. |
| `AudioManager` as a singleton | A normal scene object that listens to events | Nothing needs to call it directly, so it does not need a global access point. |
| **Out of scope: destructible or repairable batteries** | The turret can be damaged and is repaired between waves | See section 1. It makes the middle of the map matter. We kept a **single** battery, as planned. |

## 6. Not done, on purpose

- **ScriptableObjects:** the numbers live in `[SerializeField]` fields on `GameManager`, `WaveSpawner` and the other scripts, as the GDD explains.
- **Mobile, gamepad, online:** still out of scope.
- **Cinemachine, Animation Controller:** the camera never moves except for the shake, and the animations are done in code.
