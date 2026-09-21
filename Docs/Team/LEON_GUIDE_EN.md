# Meteor Command: Team Guide for Leon

> **Who this is for:** Leon, and the AI assistant Leon works with.
> **Written:** 2026-09-21, by Ilan's AI assistant (Claude), on Ilan's request.
> **Project:** *Meteor Command*, the Unity 101 End Project (MTA). Unity 6000.3.20f1, URP, Windows. **Deadline: 4 October 2026.**
> **Hebrew version:** `LEON_GUIDE_HE.md` (same content, same structure).

## How to use this file

- **Leon:** read Parts 1 to 5 first (about 20 minutes). They cover the game, the team, the project and **your mission**. Come back to the other parts when you need them.
- **AI assistant:** read the **whole** file before your first answer. **Part 9 is your operating briefing**: it says how to help Leon and how to talk with Leon, based on what worked well on Ilan's first two Unity projects (Flappy Bird, then this one).
- **Starter prompt.** Leon pastes this into the AI assistant together with the file:

  > Hi! I'm Leon. Ilan and I are building a Unity game called Meteor Command for our course. The attached guide explains the project. Please read all of it before you answer, follow Part 9 as your operating instructions, and start with the calibration questions. My job in the project is Part 5 (the 3D models).

## Contents

1. The game in two minutes
2. The team, the roles and how we worked
3. Project map
4. How the game works
5. **Leon's mission: the 3D models**
6. How every feature was built (build log)
7. Numbers you can tune
8. Team rules: Git, Unity, code style
9. **Briefing for the AI assistant**
10. Status, what is left, timeline

Appendix A: event map. Appendix B: Unity gotchas we already paid for. Appendix C: five-minute Unity vocabulary.

---

## 1. The game in two minutes

*Meteor Command* is a 3D arcade game for Windows, inspired by *Missile Command* (Atari, 1980). Six cities stand on flat ground with one missile battery in the middle. Meteors fall from the sky toward the cities.

The player **left-clicks** and an interceptor flies from the battery to that point at constant speed. It **passes through everything on the way and explodes only when it arrives**. The explosion is an expanding sphere that destroys every meteor it touches. A **large meteor does not die: it splits into two small ones** that keep falling. Ammo is limited, so the player decides what to protect and what to let land. Destroying several meteors with one blast is worth far more (N kills in one blast = 100 x N x N points).

The mouse is the only input (plus `Esc` to pause). There is one scene and one battery.

### The rules with their numbers

| Rule | Value |
|---|---|
| Arena | One invisible vertical **play plane at Z = 0**. Ground top surface at Y = 0, X from -20 to +20. The camera never moves. |
| Cities | 6, at X = -16, -11, -6, +6, +11, +16 |
| Battery | 1, at X = 0. Shots leave from 1.5 units above its position. |
| A shot | Left click, ray to the play plane, interceptor flies at **30 units/s**, explodes only on arrival |
| Blast | Sphere, **radius 3**. Grows 0.35 s, holds 0.15 s, shrinks 0.35 s (0.85 s in total). Destroys every meteor it touches. |
| Ammo | **20 shots per wave**, spent when the shot is fired (not when it explodes) |
| Meteor path | Spawns at **Y = 28** (above the top of the screen), random X in [-18, 18], flies in a straight line at constant speed to a random ground X in [-16, 16] |
| Meteor tiers | **Large** (scale 4) splits into two **Small** (scale 2), turned +25 and -25 degrees. **Scout** (scale 1, from wave 4) is twice as fast and does not split. |
| Wave size | 6 meteors on wave 1, +3 per wave |
| Wave speed | 6 units/s on wave 1, +0.5 per wave |
| Large mix | From wave 3: 25 %, +10 % per wave, capped at 60 % |
| Scout mix | From wave 4: 10 %, +5 % per wave, capped at 30 % |
| Score | The Nth kill of one blast is worth 100 x (2N - 1), so N kills in one blast = 100 x N x N |
| Wave clear | Everything spawned and nothing alive: +25 points per unused shot, then ammo refills to 20 |
| Losing a city | A meteor whose **bottom edge** reaches Y = 0 destroys the nearest living city that is within 2 units on the X axis. Otherwise it is harmless. |
| Game over | The sixth city is destroyed. The best score is saved with `PlayerPrefs`. |

### The three design pillars (from the approved GDD)

1. **Every shot is a commitment.** Limited ammo, real travel time, no undo. *Rules out homing missiles and unlimited ammo.*
2. **Breaking a meteor does not solve it.** A large meteor always splits. *Rules out screen-clearing bombs.*
3. **Readable at a glance.** One fixed plane, constant speed within a wave, the same blast radius every time, a camera that never moves.

### Screens

Main menu (Play, Quit, best score, three rule lines) - HUD (score, wave number with a progress bar, ammo as `18 / 20` with missile icons, six city icons, a pause button) - Pause panel (Resume, Main Menu) - Game over (final score, `NEW BEST!`, Restart, Main Menu).

### Look

Night sky, flat-shaded low poly, glowing (emissive + Bloom) blasts, trails and city windows. The target picture is `images/concept-target.png`. **Open it and look at it before you model anything.**

### How we are graded

| Criterion | Weight |
|---|---|
| Playability | 40 % |
| Code readability | 20 % |
| Implemented patterns (pooling, coroutines, singleton, events) | 20 % |
| WOW factor (look, effects, polish) | 20 % |

Your models are the biggest single lever on the WOW factor.

---

## 2. The team, the roles and how we worked

### Roles

| Who | What they did / do |
|---|---|
| **Ilan** | Owns the Unity project and every Unity Editor action: created the project from the URP 3D template, built the scene, wired every Inspector reference, built the whole Canvas UI (menu, HUD, game over, pause), imported audio, fonts and images, created materials (in the Editor, or as files together with the AI), tested every single step in Play mode and reported back with screenshots. Took the design decisions (look, what to cut, what to keep), chose the sounds (Pixabay) and fonts, produced the AI background image with ChatGPT image generation, and handles the course administration. The **first scripts were typed by Ilan personally**, step by step, while the AI explained each line. |
| **Leon** | Authored the design document (`GDD.md`, v2.0, approved by the teacher) and the first README. **Owner of the 3D models**: rocks, buildings, turret, trees (see Part 5). Also owns the GitHub repository (`leonJardimCarmi/Meteor-Command`). |
| **AI assistant (Claude)** | Coding partner for Ilan. Explained every Unity step with exact click locations, later wrote and reviewed the scripts, edited prefabs and materials as files, generated simple images (icons, window textures), checked the real state of the project on disk after every step, made the commits, wrote the README and this guide. |
| **The teacher** | Approved the GDD. Grades with the four criteria above, and checks technical items such as *Canvas Scaler = Scale With Screen Size* and *all scenes in Build Settings* (both are done and verified). |

### How we worked (the loop that worked)

1. **One small section per message.** A short numbered list of exact clicks, and what Ilan should see afterwards.
2. Ilan does it in Unity and replies with a **screenshot**.
3. The AI **verifies the real state on disk** (see Part 9): scene and prefab files, compile errors, whether Unity really recompiled. It does not trust "it works" or a screenshot alone.
4. The AI says what is right or wrong, then **commits** a small, described change.
5. Next small section.

Two rules came out of this way of working: the AI writes the code but *explains* it, and nothing is pushed to GitHub unless Ilan asks.

### Timeline so far

| Date | What happened |
|---|---|
| 2026-09-09 | Repository created, GDD added |
| 2026-09-19 | Unity project set up (URP 3D), arena with grey primitives, click-to-world-point, object pooling, interceptor and battery, blast (coroutine + trigger), meteors that split, first wave spawner |
| 2026-09-20 | Cities and city loss, game over, score with combos, ammo per wave, full wave loop, Canvas and HUD |
| 2026-09-21 | Menu and game states, high score, night look and glow, trails, audio, explosion particles, camera shake, city icons, wave banner, city collapse, score pop-ups, scout meteor, AI background, blast light, fonts and HUD redesign to match the mockup, pause menu, procedural missile, smoke trails, smoking ruins, README |

Everything that is *not* a 3D model works. The game is fully playable with grey primitives.

---

## 3. Project map

### Getting the project

1. Install **Unity Hub**, then the editor version **6000.3.20f1** (exactly this one, with the *Universal Render Pipeline* template packages already in the project).
2. `git clone https://github.com/leonJardimCarmi/Meteor-Command.git`
3. Unity Hub ▸ **Add** ▸ *Add project from disk* ▸ pick the cloned folder ▸ open it. The first open takes a few minutes while Unity builds its `Library` folder.
4. Open `Assets/Scenes/Game.unity` and press **Play**. Click **Play** on the menu. Play for ten minutes before you change anything.
5. Set your own identity so your commits are yours: `git config user.name "Leon"` and `git config user.email "<your github email>"` (inside the project folder).

### Folders

```
Meteor Command/                  the repository root is also the Unity project root
  Assets/
    Scripts/      29 scripts, one small script per job (Part 4)
    Prefabs/      Meteor, Interceptor, Blast, ExplosionEffect, ScorePopup
    Materials/    glow materials, city and ground materials, the BlastBubble shader
    Art/          background image, icons, window textures, button frame, soft dot
    Audio/        8 mp3 files (Pixabay)
    Fonts/        Orbitron and Bebas Neue, plus their TextMeshPro font assets and licenses
    Scenes/       Game.unity  (the only scene)
    Settings/     URP assets (PC_RPAsset is the active one) and the volume profile (Bloom)
    TextMesh Pro/ generated by Unity
    (TutorialInfo, Readme.asset, InputSystem_Actions.inputactions are leftovers of the Unity template)
  Packages/  ProjectSettings/     Unity settings, always committed
  GDD.md         the approved design document (frozen, do not edit)
  README.md      how to play, how to run, credits
  images/        concept-target.png, screens-layout.png (the GDD points to them)
  Docs/Team/     this guide, English and Hebrew
```

### The scene `Game.unity` (what is in the Hierarchy)

| Object | What it is |
|---|---|
| `Main Camera` | Perspective, field of view 60, at (0, 11, -24) looking along +Z. Dark blue background. Has `CameraShake`. The visible part of the play plane is about X = +-24.6 and Y = -2.9 to 24.9 at 16:9. |
| `Directional Light` | The moonlight: intensity 0.8, cool blue colour, soft shadows. Ambient light is dark blue. |
| `Global Volume` | URP post-processing. **Bloom** with threshold 1, intensity 1, scatter 0.6: anything with an emission brighter than 1 glows. |
| `Ground` | A cube scaled (40, 1, 6) at (0, -0.5, 0) with the `Ground_Night` material. Its top is at Y = 0. |
| `City_1` ... `City_6` | Cubes scaled (2, 2, 2) at Y = 1 (so the base sits on the ground), with the `City` script and the `City_Windows` material. |
| `Battery` | A cube scaled (2, 2, 2) at (0, 1, 0) with the `Battery` script. |
| `BackgroundFit` | A Quad with the night-sky picture, placed and sized by code far behind the play area (60 units from the camera). |
| `GameManager`, `CityManager`, `PoolManager`, `WaveSpawner`, `PlayerAim`, `EffectSpawner`, `AudioManager`, `UIManager`, `PauseMenu` | Empty objects that only carry one script each (the "managers"). |
| `Canvas` | Screen Space Overlay, **Canvas Scaler = Scale With Screen Size**, reference 1920 x 1080, Match 0.5. Holds the HUD (`ScoreText`, `WaveText`, `WaveBar`, `AmmoText`, `AmmoIcons`, `CityIcons`, `PauseButton`, `WaveBannerText`) and three panels (`MainMenuPanel`, `GameOverPanel`, `PausePanel`) that are switched on and off. |
| `EventSystem` | Needed for UI clicks. |

### Prefabs

| Prefab | Contents | Pool size |
|---|---|---|
| `Meteor` | Unity Sphere mesh, `Meteor_Glow` material, SphereCollider (radius 0.5), TrailRenderer, `Meteor` script. A smoke-trail particle child is added by code. | 80 |
| `Interceptor` | A Sphere mesh that the script replaces at start with the code-built missile mesh, `Interceptor_Glow` material, TrailRenderer, `Interceptor` script | 25 |
| `Blast` | Sphere mesh with the `Blast_Bubble` material, SphereCollider (**trigger**, radius 0.5), **kinematic** Rigidbody, `Blast` and `BlastLight` scripts | 25 |
| `ExplosionEffect` | ParticleSystem with `Spark_Additive`, `ExplosionEffect` script (four more particle layers are created by code) | 30 |
| `ScorePopup` | World-space TextMeshPro with the `ScorePopup` script | 20 |

### Materials

| Material | Used for | Key values |
|---|---|---|
| `Meteor_Glow` | Meteors | dark brown base (0.23, 0.07, 0.03), orange emission (4, 1.57, 0.31) |
| `Scout_Glow` | Scout meteors | dark purple base, purple emission (2.67, 0.94, 4) |
| `Interceptor_Glow` | Missile | pale blue-white base, cyan-white emission (0.9, 1.6, 2.6) |
| `Blast_Bubble` | Blast sphere | custom shader `MeteorCommand/BlastBubble`: transparent, almost clear in the middle, glowing cyan rim |
| `City_Windows` | Cities | base + emission textures `city_windows_base.png` and `city_windows_emission.png` (lit windows), emission 4 |
| `City_Rubble` | Destroyed cities | dark blue-grey (0.11, 0.125, 0.19), no emission |
| `Ground_Night` | Ground | dark blue (0.125, 0.15, 0.235) |
| `Background_Night` | The sky picture | `Assets/Art/background_night_2.png` |
| `Spark_Additive`, `Trail_Additive` | Explosion particles and trails (additive = adds light, looks like glow) | |
| `Blast_Glow` | An old material that nothing uses any more (safe to delete in the final cleanup) | |
| `Smoke_Sprite` | Smoke trails and smoke over ruined cities | soft dot texture, normal alpha blending |

### Credits already in the README

Sounds and music from Pixabay (`dennish18`, `dragon-studio`, `freesound_community`, `chrysalyn`), fonts Orbitron and Bebas Neue (SIL Open Font License), the background image generated with ChatGPT image generation, the code written with Claude. **When your models arrive, add their credits there.**

---
## 4. How the game works

### 4.1 The 29 scripts (all in `Assets/Scripts`, one small script per job)

| Group | Script | Responsibility |
|---|---|---|
| **Core** | `GameManager` | Game states (`Menu`, `Playing`, `Paused`, `GameOver`), score and combo points, high score, pause (time and audio), Restart / Main Menu by reloading the scene, ends the run when the last city dies, refills ammo each wave. |
| | `WaveSpawner` | One coroutine runs every wave: announce it, wait 2 s, spawn N meteors 1.5 s apart, wait until the sky is empty, complete the wave. Picks each meteor's type, publishes wave progress. |
| | `PoolManager` | Holds one `ObjectPool` per prefab type. `Get`, `Release`, `CountActive`, `HasPool`. |
| | `ObjectPool` | Pre-creates a fixed number of one prefab, hands them out and takes them back, grows only if it runs dry. |
| | `CityManager` | Knows the six cities: which are alive, and which one a ground impact destroys. |
| | `PlayerAim` | Turns a left click into a point on the play plane and asks the battery to fire. Ignores clicks outside play, during the wave banner and on buttons. |
| **Gameplay objects** | `Battery` | Ammo, `TryFire`, takes an interceptor from the pool and launches it. |
| | `Interceptor` | Flies to its target point, then spawns a blast there and returns itself to the pool. Builds its missile mesh with `MissileMesh`. |
| | `Blast` | Expand, hold, shrink coroutine (it scales itself); kills meteors on trigger enter; counts kills for the combo. |
| | `Meteor` | Moves in a straight line, hits the ground, dies (a Large splits into two Small), its look and trails depend on its type. |
| | `City` | Collapses into rubble, then smoulders. |
| **UI** | `UIManager` | HUD numbers, main menu and game over panels, button listeners. |
| | `PauseMenu` | Pause button and the pause panel. |
| | `WaveBanner` | The big "WAVE N" text that fades out. |
| | `WaveProgress` | The thin bar under the wave label that empties as the wave is dealt with. |
| | `CityIcons`, `AmmoIcons` | One HUD icon per city / per shot, cloned from one template icon by a layout group. |
| | `ScorePopup` | The floating "+300" at a kill, bigger and warmer for combos. |
| **Audio** | `AudioManager` | Plays a sound for each game event (it only listens to events), starts the music loop. |
| | `AudioTrim` | Cuts the silence at the start of a clip and caps its length, so sounds are instant. |
| **Effects and look** | `EffectSpawner` | Listens to events and spawns pooled explosion effects and score pop-ups. |
| | `ExplosionEffect` | Five particle layers built in code: sparks, ring, fireball, smoke, embers. |
| | `BlastLight` | A point light that grows and fades with the blast and lights the cities. |
| | `TrailStyle` | The shared look of all trails (a tapered, fading gradient). |
| | `SmokeTrail`, `SmokePlume` | Smoke behind meteors, and rising smoke from ruined cities. |
| | `MissileMesh` | Builds the low-poly missile mesh in code (nose, body, tail, four fins). |
| | `CameraShake` | Shakes the camera on a ground hit or a lost city. |
| | `BackgroundFit` | Sizes the sky picture so it always fills the view. |

### 4.2 The patterns we implemented (and where)

| Pattern | Where | Why |
|---|---|---|
| **Object pooling** | `PoolManager`, `ObjectPool`. Pools: 80 meteors, 25 interceptors, 25 blasts, 30 effects, 20 pop-ups. | Late waves spawn ~24 meteors and every Large adds two fragments, so `Instantiate`/`Destroy` would peak exactly when the screen is busiest. A garbage-collector hitch while the player leads a shot is a death they did not earn. |
| **Coroutines** | `Blast` (three timed phases), `WaveSpawner` (wave loop), `CameraShake`, `WaveBanner`, `ScorePopup`, `City`, `UIManager` (button lockout) | Timed sequences read as straight-line code instead of timer variables. |
| **Singleton** | `GameManager`, `PoolManager`, `CityManager` (`Instance` set in `Awake`, no `DontDestroyOnLoad`) | Pooled meteors hold no references, so they need a global route to the score. Reloading the scene resets everything. |
| **Observer (C# events)** | Static events, see Appendix A | The UI, audio, effects and camera listen to the game; the game logic knows nothing about them. |
| **State** | `GameState` enum in `GameManager` | One place decides what is allowed (for example, no shooting outside `Playing`). |
| **Prefabs + Inspector tuning** | `[SerializeField]` fields under `[Header]` | Balance can be changed in Play mode without recompiling. |

### 4.3 The life of one shot

1. **Click.** `PlayerAim.Update` sees the mouse press. `CanFire()` must be true: the game is `Playing`, the wave banner is not showing, and the pointer is not over a button.
2. **Point.** The screen point becomes a ray (`Camera.ScreenPointToRay`) that is intersected with the play plane `Plane(Vector3.forward, Vector3.zero)`. In a 3D scene this makes aiming unambiguous.
3. **Fire.** `Battery.TryFire(point)`: if `Ammo > 0`, ammo goes down (event `AmmoChanged`), an interceptor is taken from the pool at the launch point and given the target, event `Fired` (launch sound).
4. **Fly.** `Interceptor.Update` moves it toward the target with `Vector3.MoveTowards` at 30 units/s and points it nose-first. It passes through everything.
5. **Arrive.** At the target it takes a `Blast` from the pool at that point, returns itself to the pool, and raises `Arrived` (blast sound, spark effect).
6. **Blast.** `Blast.OnEnable` gives itself a new unique `Id` and starts the coroutine: scale 0 to 6 in 0.35 s (a scale of 6 is a radius of 3), hold 0.15 s, back to 0 in 0.35 s, then return to the pool.
7. **Kill.** The blast has a trigger SphereCollider. `OnTriggerEnter` finds a `Meteor` and calls `meteor.Kill(Id)`. If it returns true, `GameManager.AddKill(killNumber, position)` adds `100 x (2N - 1)` points and raises `ScoreChanged` and `KillScored` (pop-up).
8. **Meteor death.** `Meteor.Kill` returns the meteor to the pool and raises `Destroyed` (spark and fire effect). A **Large** first spawns two **Small** fragments from the pool, turned +25 and -25 degrees, and those fragments are **immune to the blast that split their parent** (they remember its `Id`), otherwise one blast would wipe out its own fragments and pillar 2 would be false.

### 4.4 Game states and flow

`Menu` -> (Play) -> `Playing` <-> (`Esc` or Pause button) `Paused`; `Playing` -> (last city destroyed) `GameOver`.

- Pausing sets `Time.timeScale = 0` **and** `AudioListener.pause = true`, so a pause is really a pause.
- **Restart** and **Main Menu** reload the scene. A static flag `_skipMenu` survives the reload so Restart goes straight into play. There is deliberately one scene and no `DontDestroyOnLoad`.
- Game Over buttons are locked for 0.5 s so the click that lost the last city cannot skip the result screen.
- During the "WAVE N" banner (about 2 s) shooting is blocked, so no ammo is wasted before the sky is visible.

### 4.5 Waves and city loss in detail

- `WaveSpawner.RunWaves` loops forever: wave number up, raise `WaveStarted` (banner, HUD), wait 2 s, raise `WaveSpawning`, spawn the meteors, wait until the pool has no active meteor, then `GameManager.CompleteWave()` gives the unused-ammo bonus and refills the battery.
- A meteor's type is one random roll: below the scout chance it is a Scout, otherwise below scout + large chance a Large, otherwise a Small.
- A meteor moves with `transform.position += direction * speed * Time.deltaTime`. It has reached the ground when `position.y - scale * 0.5 <= 0` (its **bottom edge**, using the scale as its diameter). Then `CityManager.HitAt` collapses the nearest living city whose X is within 2 units, and the meteor is returned to the pool (`Impacted` event: bigger explosion, camera shake).

### 4.6 Hit detection

The blast is a **trigger** sphere collider on a **kinematic Rigidbody**; meteors have a normal SphereCollider and no Rigidbody (they are moved with the transform). At least one side needs a Rigidbody for trigger events, and the blast has it.

We stress-tested this: a throw-away headless Unity run spawned **600 random meteor/blast pairs** (random positions, directions, speeds, both sizes, pooled blasts reused). Every meteor that was clearly inside a blast was destroyed within two physics steps. **Zero misses.** So if a meteor survives, it was not inside the blast.

### 4.7 Why the missile can fly through a meteor (this is by design)

The GDD says the interceptor *"passes through everything on the way, and detonates only on arrival"*, and pillar 1 rules out homing. The missile needs 0.3 to 0.8 s to reach the clicked point, and the meteor keeps moving during that time, so the blast often appears **behind** the meteor. To hit reliably the player must click **ahead of the meteor along its path** (lead the target). We calculated what happens if the player clicks exactly on a small meteor with the current numbers (30 units/s missile, blast that needs 0.35 s to grow):

| Wave | Chance that a click exactly on a small meteor destroys it |
|---|---|
| 1 | about 24 % |
| 3 | about 10 % |
| 5 | about 4 % |

If this feels too harsh in playtests, the fix is **tuning, not a new rule**: raise the interceptor speed and/or shorten the blast's grow time (both are Inspector values, see Part 7). For example speed 40 and grow time 0.25 s gives about 67 % on wave 1 and 27 % on wave 5. **Do not add "explode on contact" without asking Ilan**: it would break a GDD rule and pillar 1. This decision is open and belongs to Ilan.

### 4.8 Rendering and look

- **URP** with `PC_RPAsset`. Real-time lighting only, no baked lighting (everything meaningful moves).
- Night look: dark-blue camera background, dark-blue ambient light, one cool directional "moon", **Bloom** (threshold 1). **Glow = emission brighter than 1** (our glow materials use emission values between 1 and 4). Additive particle materials also glow.
- Palette: deep blue night (`#060A1C` background), warm orange (meteors, fire), cyan / ice blue (interceptors, blasts, UI accents), purple (scouts), warm yellow windows.
- The sky is a flat picture on a Quad because the camera never moves.
- Meshes: flat-shaded low poly, under 500 triangles each, so 60 FPS is easy.

---

## 5. Leon's mission: the 3D models

### 5.1 The goal in one sentence

Replace the grey placeholder shapes (sphere meteors, cube cities, cube battery) with **real low-poly 3D models** that look like `images/concept-target.png`, **without breaking gameplay**, and land them in the repository **under your own name** with proper credits. This is the biggest lever for the WOW factor (20 % of the grade), and your commits are the visible proof of your part of the project.

### 5.2 What to deliver

| # | Asset | Replaces | How many | Budget |
|---|---|---|---|---|
| 1 | **Meteor rock** | Sphere on the `Meteor` prefab | 1 mesh (the game scales it to make Large, Small and Scout). 2 or 3 variants are a bonus. | under 500 triangles |
| 2 | **Buildings** | The six cubes `City_1` to `City_6` | 6 variants (they may differ) | under 500 triangles each |
| 3 | **Rubble** | Squashed dark building | 1 (optional, see 5.4) | under 200 triangles |
| 4 | **Turret / launcher** | The cube `Battery` | 1 | under 500 triangles |
| 5 | **Trees and scenery** | Nothing yet (new) | a few variants, placed many times | under 150 triangles each |
| 6 | Missile (optional) | The missile built in code | 1 | under 200 triangles |

**Sources.** The GDD planned CC0 kits by Kenney (*Nature Kit* for rocks and trees, *City Kit (Commercial)* for buildings, *Tower Defense Kit* for the turret). You can use them, model in Blender, or mix. Rules: everything must be **CC0 or your own work**, **credited in the README**, and if any model was AI-generated it must be **cleaned up and disclosed** in the README. Nothing from image search.

### 5.3 Style guide

- **Flat-shaded low poly** (hard edges, no smoothing), simple shapes, no tiny details.
- **Night palette** (see 4.8). The scene is dark, so make base colours a little lighter than you would in daylight, and use **emission** for what should glow: city windows above all.
- Prefer **one material per model**, using a flat colour or a tiny palette texture (64 x 64 is plenty). No normal maps. Textures no bigger than 512 x 512.
- Look at the concept image and at the game running. Match the mood, not just the shapes.

### 5.4 The contracts (what the code expects from your models)

These come from how the current scripts use the placeholder shapes. **Follow them and no code has to change.** If a contract cannot work for a model, do not bend the model: tell Ilan (and your AI); the fix is usually a two-line script change.

**Meteor**
- The mesh must fit a **sphere of diameter 1** (bounds about 1 x 1 x 1), **pivot at the centre**. The game makes the rock bigger with the scale: **4** for Large, **2** for Small, **1** for Scout, always uniform.
- Keep it roughly round. The game treats every meteor as a sphere of that diameter for collisions **and** for touching the ground, so a long, pointy rock would visibly hit at the wrong place.
- **One material slot only.** The Scout look is made by swapping the material in slot 0. (More slots need a change in `Meteor.ApplyLook`.)
- Do not add trails or smoke: they are added by code and by the prefab.
- Bonus (needs a tiny script from the AI): a slow random tumble while falling, which sells the rock instantly.

**City (building)**
- The scene scales each city by (2, 2, 2) and puts it at Y = 1 so that its base sits on the ground. So the mesh must be **exactly 1 unit tall with the pivot at the centre** (Y from -0.5 to +0.5), and up to 1 wide and 1 deep.
- When a city dies, the code shrinks its height to 0.4 and keeps the base on the ground, and swaps the material to the dark `City_Rubble`. A model squashed to 40 % height looks like a low ruin, which is acceptable. A **real rubble mesh** looks better: it needs `City.cs` to swap the mesh (a few lines), ask the AI.
- Design **emissive windows**: an emission map (or an emissive material) with values around 2 to 4 so Bloom makes them glow. Look at `city_windows_emission.png` and the `City_Windows` material to see what the cubes do now.
- The six cities are 5 units apart on X (three on each side of the battery) and only the `City` script matters; the cubes' BoxColliders are not used by the game.

**Battery (turret)**
- Now a cube scaled (2, 2, 2) at (0, 1, 0). Keep the footprint within about 2 x 2 units at the centre of the arena (the nearest cities are at X = +-6, so there is room), and the pivot so the base touches the ground.
- Shots start at the battery position plus the `Launch Offset` field of the `Battery` component (now (0, 1.5, 0)). **Move that value to the barrel tip** when the model is in.
- Bonus (a small script from the AI): the barrel turns toward the click.

**Missile (optional)**
- The missile mesh is built in code by `MissileMesh` (nose, body, tail, four fins), about 1.6 units long, 0.32 wide, pointing along **+Z**. A model of your own must point along +Z too, and the line that assigns `MissileMesh.Shared` in `Interceptor.Awake` must be removed.

**Trees and scenery**
- No colliders, no scripts. Keep the play area clear: the action happens on the plane Z = 0 between X = -20 and +20, Y = 0 to 25. Put scenery **behind** the plane (Z from about +3 to +12) or on the far left and right, below the horizon of the sky picture. Do not put anything between the camera and the play area.
- Up to about 30 objects, sharing materials, is fine for performance.

### 5.5 Where files go

```
Assets/Models/<AssetName>/     the FBX (or OBJ/GLB) and its textures
Assets/Materials/              any new materials, named like the existing ones (Thing_Glow, Thing_Rubble)
Assets/Prefabs/                prefabs, if you make any
```

Name assets in English with `PascalCase` (`Rock01`, `BuildingA`, `TurretBase`). **Commit the `.meta` files with their assets**, and rename or move assets **inside Unity** (never in Windows Explorer), otherwise references break.

### 5.6 Integration, step by step

1. **Import.** Drag the model into `Assets/Models/...` in the Unity **Project** window (bottom of the screen). Select it and, in the **Inspector** (right panel), check the *Model* tab: scale factor so that it is the right size, *Read/Write* off, *Generate Colliders* off. In the *Materials* tab use the project's materials instead of the embedded ones. Before exporting from Blender: apply transforms (scale 1, rotation 0) and put the origin where the pivot should be.
2. **Check the size** next to the placeholder: drag the model into the **Scene** view beside `City_1` (or a meteor) and compare. Delete this test copy afterwards.
3. **Meteor:** open `Assets/Prefabs/Meteor.prefab` (double click). Select the root `Meteor` in the Hierarchy. In the Inspector find **Mesh Filter** and drag your rock mesh into its **Mesh** field. Leave the SphereCollider (radius 0.5) as it is. Leave prefab mode with the back arrow at the top left of the Scene view.
4. **City, battery, scenery:** these live in `Game.unity`, see the **scene baton rule** below. Select `City_1`, in **Mesh Filter** replace the **Mesh** with your building, and swap the material if the model needs its own.
5. **Test in Play mode**: click Play, play a few waves. Check that meteors hit the ground where they visually touch it, that cities sit on the ground and collapse cleanly, and that the shot starts at the barrel.
6. **Save and commit** (Part 8).

### 5.7 Who edits what (to avoid merge conflicts)

| Area | Owner | Rule |
|---|---|---|
| `Assets/Models`, new materials, new art | Leon | Free to add. |
| `Assets/Prefabs/*.prefab` | Leon while integrating models | Tell Ilan before you start; Ilan does not touch prefabs meanwhile. |
| `Assets/Scripts` | Ilan + AI | Ask (or agree first) before changing a script. Contract problems are solved here. |
| **`Assets/Scenes/Game.unity`** | **Whoever holds the baton** | **Only one person edits the scene at a time.** Scene files are hard to merge. Say "I'm taking the scene", edit, save, commit, push, say "scene is free". |
| `GDD.md` | Nobody | The approved GDD stays as it is. |

### 5.8 Definition of done (for each asset)

- [ ] Triangle count under the budget, flat shaded, correct size and pivot per the contract.
- [ ] Looks right **in the game at night**, not only in the model viewer.
- [ ] Meteors hit the ground where they visually touch it; cities sit on the ground; the shot starts from the barrel.
- [ ] No errors in the Console (bottom panel) while playing.
- [ ] Frame rate is still smooth.
- [ ] Credit line added to the README if the model is not 100 % your own.
- [ ] Committed under your name with a clear message; `.meta` files included.

### 5.9 Extra things you can take (if you want more of your name on the project)

Meteor tumble, turret aiming, more meteor and building variants, scenery, a real rubble mesh, the decorative taglines from the mockup on the menu, playtest reports with numbers (which wave you lose, how, is it fun), and checking the build on a second computer.

---
## 6. How every feature was built (build log)

Each entry says **what it is**, **how it works** and **what we learned**. The order is roughly the order we built them. The commit history (`git log --oneline`) shows each step as a small commit.

### Phase 1: the game first, with grey boxes (2026-09-19 to 09-20)

**Arena and camera.** One invisible vertical plane at Z = 0 holds all gameplay; the third dimension is only for meshes, light and depth. A perspective camera at (0, 11, -24) looks at it and never moves. Ground, six cities and the battery are Unity cubes. *Lesson:* a fixed plane makes aiming unambiguous in 3D, and grey boxes let us make the game fun before making it pretty. The project also needed *Active Input Handling = Both*, because the default blocked the legacy `Input` class.

**Click to world point (`PlayerAim`).** `Camera.ScreenPointToRay(Input.mousePosition)` gives a ray; `Plane.Raycast` finds where it crosses the play plane; `ray.GetPoint(distance)` is the target. Later we added `CanFire()` (playing, not in the wave intro, not over a button).

**Object pooling (`ObjectPool`, `PoolManager`).** `ObjectPool` keeps a `Queue` of ready objects and a `HashSet` of all its objects. `Get` places the object and activates it, `Release` deactivates it and queues it again, and it creates a new one only if it runs dry. `PoolManager` keeps a dictionary from `PoolType` to pool, so callers write `PoolManager.Instance.Get(PoolType.Meteor, position, rotation)`. Effects and pop-ups are optional pools (`HasPool`), so the game still runs without those prefabs. `CountActive(PoolType.Meteor)` is also how the wave knows the sky is empty.

**Interceptor and Battery.** `Battery.TryFire` spends ammo, takes an interceptor from the pool and launches it. `Interceptor.Update` uses `Vector3.MoveTowards` at a constant 30 units/s and checks for arrival with an exact position comparison (`MoveTowards` lands exactly on the target). On arrival it spawns the blast, releases itself and raises `Arrived`.

**Blast.** Built in three steps: (1) a coroutine that scales a sphere up, holds and shrinks it (visual only, it killed nothing yet); (2) the prefab got a **trigger** SphereCollider plus a **kinematic Rigidbody**, because trigger events need a Rigidbody on at least one of the two colliders; (3) `OnTriggerEnter` calls `Meteor.Kill(Id)`, with a **unique `Id` per blast** and a kill counter. The scale is twice the radius, so the collider radius (0.5) times the scale equals the blast radius. *Lesson:* a kill has to happen at the moment the growing sphere touches the meteor, which is exactly what a trigger gives us.

**Meteor.** Moves in a straight line; dies by `Kill(blastId)`. A Large spawns two Small fragments turned +-25 degrees; fragments store the `Id` of the blast that split their parent and ignore that one blast. Ilan tuned the sizes to 4 (Large) and 2 (Small) after the first ones looked too small. The ground test uses the **bottom edge** (`y - scale * 0.5 <= 0`); using the centre looked wrong.

**Waves (`WaveSpawner`).** Version 1 spawned one random meteor every 1.5 s. Version 2 is a coroutine wave loop with growing count, speed and Large share, then a Scout share. Progress is published every frame so the HUD bar can follow it.

**Cities (`City`, `CityManager`).** `CityManager.HitAt(point)` picks the nearest **living** city by distance **along X only** (the play is flat) within 2 units. `City.Collapse()` raises a static `Destroyed` event; `GameManager` listens and ends the run when none is left.

**Score and combos.** The Nth kill of one blast is worth `100 x (2N - 1)`. That way every kill can show its own pop-up value and the total of N kills is exactly `100 x N x N` as the GDD says.

**First UI.** A Canvas with **Scale With Screen Size (1920 x 1080, Match 0.5)** (the teacher checks this), TextMeshPro texts, and `UIManager` that only listens to events.

### Phase 2: a complete game (2026-09-21)

**Game states, menu, game over, high score.** `GameState` enum; the menu is a panel switched on and off. Restart and Main Menu reload the scene; a static `_skipMenu` flag makes Restart go straight to play. The best score lives in `PlayerPrefs`. Game Over buttons are locked for 0.5 s.

**Night look and glow.** Dark-blue camera background and ambient light, one cool directional moonlight, URP **Bloom** (threshold 1). Emissive materials (emission colours brighter than 1) glow; the values are in Part 3. Cities use a generated facade texture with lit windows (`city_windows_base.png` plus an emission map).

**Trails.** A `TrailRenderer` on meteors and interceptors, styled by `TrailStyle`: a width curve that tapers to zero and a gradient that runs from a bright head through warm colours into dark ash. Meteors get a flame gradient, scouts a purple glow, interceptors a pale blue smoke. Meteors also carry a `SmokeTrail`: a particle emitter (created in code) that leaves grey puffs in world space.

**Explosion effects.** `EffectSpawner` listens to events and takes a pooled `ExplosionEffect`. Each effect is one prefab (its own particle system makes the sparks) plus four child particle systems created in code: an expanding ring, a fireball, glowing smoke and embers. All use the additive material, so they glow on the dark sky. `CameraShake` (a coroutine; the strongest shake wins) fires on ground hits and lost cities.

**The blast itself.** A custom URP shader, `MeteorCommand/BlastBubble`, draws a transparent bubble that is almost clear in the middle with a glowing cyan rim (a fresnel term), so meteors inside the blast stay visible. `BlastLight` adds a point light that follows the blast and lights the cities; URP lights fall off with the square of the distance, so it needed intensity 80 and a position 4 units in front of the plane.

**Audio.** `AudioManager` is event-driven: it subscribes to the game events and plays a clip for each. `AudioTrim` reads only the start of each mp3, removes leading silence and caps clips at 2 s (with a short fade), so sounds play the instant the event happens. The game-over jingle is uncapped. Music loops on a second `AudioSource`. The blast sound is triggered by `Interceptor.Arrived`. At first the AI generated placeholder sounds in code; Ilan then added real mp3 files from Pixabay, and on 2026-09-21 the last generated sound was removed so the game plays **only Ilan's audio** (the "meteor destroyed" slot is empty on purpose and can take a clip later).

**HUD polish.** Wave number, progress bar (`WaveProgress`), ammo as text plus one missile icon per shot (`AmmoIcons`), one building icon per city (`CityIcons`, dark when the city is lost), a big "WAVE N" banner (`WaveBanner`), a floating "+300" per kill (`ScorePopup`, bigger and warmer for combos). Fonts: Orbitron and Bebas Neue as TextMeshPro font assets. The layout was matched to Ilan's mockup image.

**City collapse and ruins.** `City.Crumble` shrinks the city's height to 0.4 in 0.4 s and swaps to the dark rubble material; `City.Smoulder` runs a `SmokePlume` (a rising column of smoke, created in code) for 14 s.

**Scout meteor.** Wave 4 and later: small, double speed, own purple material and trail, does not split.

**Background.** An AI-generated night picture (lake, moon, mountains) on a Quad. `BackgroundFit` computes the size from the camera: `viewHeight = 2 * distance * tan(fov / 2)`, `viewWidth = viewHeight * aspect`, then scales with a margin so the picture always covers the screen at any aspect ratio.

**Pause.** Pause button and `Esc` both call `GameManager.Pause()`, which freezes time and audio and shows the panel.

**The missile.** `MissileMesh` builds the mesh in code: a body made by rotating a profile (radius against length) in 8 sides, plus four double-sided fins. Every triangle has its own vertices, so the shading is flat like the rest of the game. The interceptor turns nose-first toward its target.

**Performance tidying.** Gradients and wait objects are created once, HUD values are only redrawn when they change, everything that appears many times is pooled.

### Problems we hit and what we learned

| Problem | Cause | Fix and lesson |
|---|---|---|
| Unity did not recompile after saving a script | Unity only recompiles when its window is focused | Click the Unity window. If still stuck: right-click the script in the Project window, **Reimport**. |
| Changes "disappeared" | The scene or material was not saved (title shows `Game*`), or was edited **in Play mode** (Play-mode edits are thrown away) | Stop Play before editing; File > Save (and Save Project); the AI checks the file on disk. |
| Code pasted into the wrong script (`CityManager` got `GameManager` code) | The wrong tab was open in the code editor | Always name the file; the AI reads the file back. Restore from git if needed. |
| A "boom" sound at the start of the game | A pooled blast raised an event when it was first activated | The blast sound now comes from `Interceptor.Arrived`. |
| Clicks sometimes did nothing | Invisible HUD texts with **Raycast Target** on sat under the cursor and swallowed the click | Only real buttons block a shot now (`Selectable` check); labels should have Raycast Target off. |
| Unity **froze** | The `AmmoIcons` script was attached to the template icon itself, so it cloned itself forever | The script must sit on the **parent** of the template. A guard now logs an error and stops. Unity leaves an `Assets/_Recovery` folder after a crash: it is git-ignored. |
| Blast light too weak | URP point lights use inverse-square falloff | Intensity 80, 4 units in front of the play plane. |
| Smoke invisible | Dark grey smoke on a dark sky | Warm-lit, glowing smoke that fades to grey. |
| Duplicated UI objects landed in the wrong place | Copies keep their old `anchoredPosition` | Reset anchors and check the numbers in the Inspector. |
| Pause panel misplaced | Created outside the `Canvas` | Every UI object must be a child of `Canvas`. |
| A commit contained a huge TextMesh Pro fallback asset | Unity rewrites `LiberationSans SDF - Fallback.asset` by itself | **Never commit that file.** |
| "The missile goes through the meteor" | By design plus travel time | See 4.7. |

### How we verify (this is the method Part 9 asks the AI to keep using)

- Search the scene and prefab files (they are YAML text) for the value that should have changed.
- Compare the timestamp of `Library/ScriptAssemblies/Assembly-CSharp.dll` with the script's, and search the DLL for a new method name, to know Unity really recompiled.
- Search `Editor.log` for `error CS` (compile errors).
- For risky behaviour, measure instead of guessing: hit detection was checked with the 600-trial headless test (4.6), and the hit-chance table (4.7) comes from a simulation with the game's real numbers.

---

## 7. Numbers you can tune

These are `[SerializeField]` fields: change them in the Inspector, no code needed. Values as of 2026-09-21; **the prefab or scene is always the source of truth**. To change a prefab value permanently, open the prefab (double click it in the Project window), edit, and save.

| What | Where (object > script > field) | Now | Effect |
|---|---|---|---|
| Interceptor speed | `Interceptor` prefab > Interceptor > Speed | 30 | How far ahead the player must lead |
| Blast radius | `Blast` prefab > Blast > Radius | 3 | How forgiving aiming is, how reachable combos are |
| Blast grow / hold / shrink | `Blast` prefab > Blast > Expand / Hold / Shrink Time | 0.35 / 0.15 / 0.35 | Grow time decides whether a blast catches a fast meteor |
| Meteor sizes | `Meteor` prefab > Meteor > Large / Small / Scout Scale | 4 / 2 / 1 | Also the collision size and the ground-contact size |
| Fragment spread | `Meteor` prefab > Meteor > Fragment Spread Angle | 25 | How far fragments fly apart |
| Scout speed | `Meteor` prefab > Meteor > Scout Speed Multiplier | 2 | |
| Points per kill | `GameManager` > Kill Points | 100 | |
| Ammo per wave | `GameManager` > Ammo Per Wave | 20 | The strategy dial |
| Unused shot bonus | `GameManager` > Unused Ammo Bonus | 25 | |
| City hit radius | `CityManager` > Hit Radius | 2 | How close an impact must land to kill a city |
| Wave size | `WaveSpawner` > Meteors Base / Meteors Per Wave | 6 / 3 | |
| Spawn pace | `WaveSpawner` > Spawn Interval / Pause Between Waves | 1.5 / 2 | |
| Wave speed | `WaveSpawner` > Meteor Speed Base / Per Wave | 6 / 0.5 | Main difficulty dial |
| Large mix | `WaveSpawner` > First Large Wave / Large Chance Base / Per Wave / Cap | 3 / 0.25 / 0.1 / 0.6 | |
| Scout mix | `WaveSpawner` > First Scout Wave / Scout Chance Base / Per Wave / Cap | 4 / 0.1 / 0.05 / 0.3 | |
| Spawn area | `WaveSpawner` > Spawn Height / Spawn Range X / Target Range X | 28 / 18 / 16 | |
| Low ammo warning | `UIManager` > Low Ammo | 5 | Ammo turns red below this |
| Game over lockout | `UIManager` > Restart Lockout | 0.5 | |
| Wave banner | `WaveBanner` > Hold Seconds / Fade Seconds | 1.5 / 0.5 | Should match the 2 s pause |
| Camera shake | `CameraShake` > Duration / Impact Strength Per Size / City Lost Strength | 0.35 / 0.06 / 0.5 | |
| Sound | `AudioManager` > Volume / Max Clip Seconds / Music Volume | 0.6 / 2 / 0.743 | |
| Blast light | `Blast` prefab > Blast Light > Max Intensity / Range / Distance In Front | 80 / 14 / 4 | |
| Trails | `Meteor` prefab > Trail Time / Width Per Size; `Interceptor` prefab > Trail Time / Width | 0.55 / 0.38; 1.0 / 0.55 | |
| Pool sizes | `PoolManager` > Meteor / Interceptor / Blast / Effect / Popup Count | 80 / 25 / 25 / 30 / 20 | Pools grow if starved, but keep them big enough |

---
## 8. Team rules: Git, Unity, code style

### 8.1 Git

- **Remote:** `origin` = `https://github.com/leonJardimCarmi/Meteor-Command.git`, branch `main`.
- **Rhythm:** `git pull` when you start; a **small commit after each verified step**; `git pull` again right before `git push`; push when a piece is finished and tested, and at the end of a work session.
- **Commit messages:** one line, imperative, what changed and why. Good: `Add rock mesh and use it on the Meteor prefab`. Bad: `update`, `fix`, `wip`.
- **Your identity:** `git config user.name` and `user.email` must be **yours** (see Part 3), so the history shows who did what.
- **Never commit:** `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Builds/` (already in `.gitignore`), `Assets/_Recovery*`, and Unity's auto-rewritten `Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset` (a 2,000-line change that means nothing).
- **Always commit:** `.meta` files together with their assets, `ProjectSettings/`, `Packages/manifest.json`.
- **Do not** force-push, rewrite history, run `git reset --hard` or `git clean`, reformat whole files, or delete `.meta` files unless the team agreed first.
- **Conflicts:** in `.cs` files they are easy. In `.unity` and `.prefab` files they are painful. That is why Part 5.7 has ownership rules and the **scene baton**. If you ever see `<<<<<<<` in a scene or prefab, **stop and ask Ilan**. Do not guess.
- **Keep large files out:** models and textures should stay small (a few hundred KB each). No screen recordings, no source `.blend` files bigger than a few MB in the repo (keep those elsewhere).

### 8.2 Unity

- The editor version is **6000.3.20f1**. Do not open the project with another version, and do not accept an "upgrade" prompt.
- **Save before you commit:** `File > Save` for the scene or prefab, and `File > Save Project`. A `*` in the title bar means unsaved.
- **Do not edit in Play mode.** Changes made while the game is playing are thrown away when it stops.
- After a `git pull`, wait for Unity to finish compiling (spinner at the bottom right). The **Console** must have no red errors before you continue.
- Move and rename assets **inside Unity**, so the `.meta` GUIDs keep the references alive.
- Keep the two graded settings: **Canvas Scaler = Scale With Screen Size**, and **`Game.unity` enabled in Build Settings**.
- Turn off **Raycast Target** on any UI text or image that is not a button (it blocks clicks).

### 8.3 Code style (the teacher grades readability, 20 %)

The code follows the course's style: many small, single-purpose methods and scripts. Read three scripts (`Blast`, `Battery`, `WaveSpawner`) before you write any.

- One class per file, the file name equals the class name. `PascalCase` for types, methods and properties, `_camelCase` for private fields, explicit types (no `var`), Allman braces (opening brace on its own line), 4 spaces, no trailing whitespace.
- Inspector fields are `[SerializeField] private` (never `public` fields), grouped with `[Header("...")]`. Read-only public state is a property: `public int Ammo { get; private set; }`.
- **No magic numbers:** every tunable number is a named field or constant.
- **Small methods** with one job and a name that says it (`SpawnFragment`, `HasReachedGround`). Prefer early returns to deep nesting.
- **Comments explain why, not what**, in plain English, and only where the reason is not obvious. No commented-out code, no dead code, no unused `using` lines.
- **Events:** `public static event Action<...> Name;` named as something that happened (`Destroyed`, `Arrived`, `GameStarted`). Subscribe in `OnEnable`, unsubscribe in `OnDisable`. Publishers do not know their listeners.
- **Singletons:** `public static X Instance { get; private set; }` assigned in `Awake`.
- **Awake** for references to itself, **Start** for references to other objects.
- Never `Instantiate` or `Destroy` in gameplay: use the pool. No `Find...` in `Update`. Cache `WaitForSeconds`, gradients and components.
- Public methods before private ones.

---

## 9. Briefing for the AI assistant

> **This part is addressed to you, the AI assistant.** Follow it for the whole session. It is not a description of a person: it is the working method that made Ilan's project succeed, adapted for Leon. If Leon asks you to do something that conflicts with it, do what Leon needs and state the trade-off in one sentence.

### 9.1 Your situation

- You are the mentor and pair-programmer of **Leon**, a student on a two-person team (Ilan and Leon) finishing the graded Unity End Project **Meteor Command**. The deadline is **4 October 2026**; a feature freeze around **1 October** is proposed.
- **Leon's job is the 3D models** (Part 5). The game itself already works and is being polished by Ilan and Ilan's AI. Your priority is: models that fit the contracts, imported cleanly, integrated safely, committed under Leon's name.
- **You do not know Leon's level.** Do not assume. Ilan was a complete beginner and the method below is calibrated for that. If Leon turns out to be more advanced, go faster, but keep the safety rules.
- The approved **GDD is frozen**. The game's core rules are in Part 1 and Part 4.7. Do not change them.

### 9.2 Your first message in every session

1. **Read the state before you speak** (if you can access the project): `git status`, `git log -5 --oneline`, the Console or `Editor.log` for compile errors, and whether Leon has uncommitted changes.
2. **First session only, ask at most five short questions:**
   - Which language do you prefer (Hebrew or English)?
   - How much Unity have you used? (never / the course / some projects)
   - Which 3D tool will you use, or will you start from Kenney kits or generated models? Have you used Blender?
   - Do you want to **type the code yourself** (you explain each line) or **have me write it** (I explain what I wrote)?
   - Mouse-only instructions with menu paths, or are keyboard shortcuts fine?
3. Say **what you understood the goal of today's session to be** in one sentence, propose the **first small step**, and wait for Leon.

Remember Leon's answers and keep to them for the session.

### 9.3 The ten rules

1. **One small step per message.** A goal in one sentence, then at most about six numbered actions, then *what Leon should see*, then *what to send you* (usually a screenshot). Never two unrelated tasks in one message.
2. **Exact locations, using the words on the screen.** "In the **Project** window (bottom of the screen), open `Assets` > `Models`", not "add the file to the project". Name the panel, the menu path and the button as they are written. Describe where a panel is (top left, right side, bottom) the first time you use it.
3. **Verify instead of assuming.** Before advising, look at the real state (files, Console, screenshots). After each step, check that it really happened. See 9.5.
4. **Explain, briefly.** One or two plain sentences on what a step does and why. Teach a concept once, when it is needed, with an example from this game. Leon should understand what is in the project.
5. **Protect the design.** Never change a core rule (the missile passes through, blast radius and timing, combo scoring, no homing, one scene). You may *suggest* tuning values and say who decides (Ilan).
6. **Small, clean, consistent code** (Part 8.3). If you write code, say which files you touched and what each change does. Match the surrounding code exactly.
7. **Safe by default.** Ask before you delete, overwrite, move, reset, force-push, change `ProjectSettings` or packages, or push anything to GitHub. Before you delete or overwrite, look at what is there and say what it is. Never put secrets in a file.
8. **Report honestly.** Say what you verified and how. Say what you *could not* verify (for example how something looks) and tell Leon exactly what to look at. Never say "done" without evidence. If you made a mistake, say so plainly and fix it.
9. **Recommend, do not survey.** When there are options, pick one and give the reason in a sentence. Tie it to the four grading criteria or the deadline when relevant.
10. **Respect Leon's time and morale.** Short messages. No lectures, no flattery, no hype. Celebrate a finished milestone in one line. When Leon is stuck or frustrated, make the next step smaller, not longer.

### 9.4 Anatomy of a good step message

Template:

```
Step N of M: <goal in one sentence>

1. <exact action with location>
2. <exact action with location>
3. <exact action with location>

You should see: <what appears>.
Send me: <screenshot of X>. (Optional: what to do if it looks different.)
```

Bad: *"Add a Rigidbody to the prefab and make it kinematic."*

Good:

> **Step 2 of 4: check that the rock has the right size.**
> 1. In the **Project** window (bottom of the screen) open `Assets` > `Models` > `Rock01`, and click the model file once.
> 2. In the **Inspector** (right panel) click the **Model** tab and look at **Scale Factor**.
> 3. Drag the model from the Project window into the **Scene** view (the big window in the middle) and drop it next to the grey cube `City_1`.
>
> You should see a rock about as wide as half of the cube.
> Send me a screenshot of the Scene view with the Inspector visible.

After the screenshot arrives: say what is right, say what is wrong (if anything) with the exact fix, verify on disk if you can, commit, then the next step.

### 9.5 Verification protocol

**If you can run commands** (Windows; Git Bash or PowerShell), use these checks:

| Question | Check |
|---|---|
| Did Unity recompile after a script change? | Compare timestamps of `Library/ScriptAssemblies/Assembly-CSharp.dll` and the script. If the DLL is older, click the Unity window; if still old, right-click the script in Unity > **Reimport**. To be sure, search the DLL for a new method name. |
| Are there compile errors? | Search `%LOCALAPPDATA%\Unity\Editor\Editor.log` for `error CS`. |
| Was the scene or prefab really saved with the change? | The files are text (YAML). Search `Assets/Scenes/Game.unity` or the `.prefab` for the field or the mesh name. |
| Which asset does a reference point to? | Open the asset's `.meta` file, read its `guid`, search for it in the scene or prefab. |
| What changed? | `git status`, `git diff --stat`. **Nothing** unexpected may be in the list (see 8.1 for files never to commit). |

Do not run a second Unity instance on the project while Leon has it open. If you need a test run, use a copy of the project without `Library`.

**If you cannot access the files** (chat-only AI): ask for screenshots and pasted text. Ask for the **Console** panel (red errors), the **Inspector** of the object you changed, and the **Hierarchy**. Tell Leon which checks need no tools: a `*` in the title means unsaved; a spinner at the bottom right means Unity is still compiling; a red message in the Console means stop. Ask Leon to paste the full text of any error.

**Traps:**
- A screenshot taken **outside Play mode** shows static Inspector values, not behaviour. To test behaviour, Leon must press Play.
- "It works" often means "it worked in Play mode and was then lost". Ask whether the scene/prefab was saved.
- Code pasted into the **wrong file** happens. Always name the exact file and check the tab.
- When Leon reports a bug, **read the current file or scene first**. Most "bugs" were an unsaved script, a missed earlier step, or a forgotten `Save`.

### 9.6 How to talk to Leon

- **Mirror Leon's language** (Hebrew or English). In Hebrew keep Unity words in English exactly as they are on screen (Inspector, Hierarchy, Prefab, Mesh Filter).
- **Never guess pronouns or forms of address from a name.** Use Leon's name or "you". If the language forces a choice (Hebrew verbs, for example), use neutral phrasing (infinitives, plurals) and ask once which form Leon prefers.
- Short paragraphs. Numbered steps. **Bold** for names of things on screen and `code font` for file names and values.
- Friendly, direct and calm. Leon is a capable adult who is new to this tool. Never talk down, never over-praise.
- Teach the **reason** in one sentence ("the code treats every meteor as a sphere, so the rock must fit a sphere").
- Prefer a **screenshot** to a long description. Say which panel you want in it.
- When Leon makes a mistake, treat it as normal, find out what actually happened, and fix it without blame.
- Do not bury Leon in options. Recommend one.
- Keep track of the clock: name the next milestone and how it helps the grade.

### 9.7 What you may and may not do

| You may | Ask first | Never |
|---|---|---|
| Explain, review, suggest | Change any script under `Assets/Scripts` (contract fixes are Ilan's area, so agree with Leon and note it for Ilan) | Change the GDD or a core rule |
| Create new assets in `Assets/Models` and new materials | Edit `Game.unity` (baton rule, 5.7) | Commit Unity's TextMesh Pro fallback asset noise |
| Edit a `.mat` or `.prefab` text file when it is far simpler than the Editor **and** you can verify it | Delete or rename anything that already exists | Hand-edit GUIDs in `.meta` files |
| Small, well-explained commits to `main` | `git push`, changing packages, changing `ProjectSettings` | Force-push, rewrite history, `reset --hard`, `clean` |
| Add credit lines to the README | Reformat or restructure existing code | Add assets with unclear licences |

### 9.8 Model integration protocol

For each asset, in this order:

1. **Contract check** (5.4): size, pivot, triangles, materials, forward axis. Ask for the Blender statistics or Unity's numbers if you cannot see them (in Unity, select the model: the preview area at the bottom of the Inspector shows vertex and triangle counts, or use the **Stats** button in the Game view).
2. **Import settings** (5.6, step 1).
3. **Look in context:** drop it in the Scene next to the placeholder, compare, then Play.
4. **Integrate** by the ownership rules (5.7): prefab first, scene only with the baton.
5. **Test behaviour:** meteors touch the ground where they visually do; cities stand on the ground and collapse; the shot leaves the barrel; no red Console lines; smooth frame rate.
6. **Credits:** add to the README if it is not fully Leon's own work.
7. **Commit** with a clear message, including `.meta` files. Then tell Leon what is done, what was verified, and what comes next.

If a contract cannot be met, do not distort the model. Write down the exact problem and the smallest code change that would fix it, and let Leon pass it to Ilan.

### 9.9 Common problems and first checks

| Symptom | First checks |
|---|---|
| Model is huge, tiny, or lies on its side | Scale Factor in the import settings; apply transforms and axes in the 3D tool before export |
| Pink or black material | The material's shader must be a URP one (`Universal Render Pipeline/Lit` or `Simple Lit`); do not use Built-in shaders |
| Model looks smooth, not flat-shaded | Export with hard edges (flat shading), Normals set to **Import** in Unity |
| Model is invisible in Game view but visible in Scene view | It is behind the background, inside another object, or outside the camera view; check its Z (the play plane is Z = 0, the camera looks along +Z from Z = -24) |
| Rock sinks into or floats above the ground | Pivot is not at the centre (meteor) or the mesh is not exactly 1 unit tall (city) |
| Meteors disappear before touching the ground or after passing it | The mesh is not roughly round or not 1 unit across; the game uses a sphere |
| Nothing glows | Emission colour must be brighter than 1 (HDR); Bloom threshold is 1 |
| Clicks do nothing | Something with Raycast Target sits over the screen, or the game is not in `Playing` |
| Unity froze | A script cloned itself endlessly; wait, do not kill the editor unless needed; check the last script or object added. Unity may create `Assets/_Recovery` (it is git-ignored) |
| Console shows errors after a pull | Wait for the compile to finish; if the errors stay, do not fix blindly: read them, and ask Ilan if they are in scripts |

### 9.10 Escalate to Ilan when

A contract cannot be met; a script must change; two people need the scene; a balance question comes up (whether the missile is fast enough, whether waves are too hard); anything touches the GDD; the game breaks after a pull; or it is time to push or submit.

### 9.11 Session end

Confirm together: everything saved (scene, prefab, project), the Console is clean, the work is committed (and pushed if Leon agreed), and give a three-line summary: **done**, **verified how**, **next step**.

---
## 10. Status, what is left, timeline

### 10.1 Status on 2026-09-21

- **Working and committed:** the whole game loop, all screens including pause, all effects, audio (only Ilan's files), high score, the night look. Playable from the menu to game over with grey-box art.
- **Written, compiles, waiting for Ilan's check in Unity:** the code-built missile, smoke trails behind meteors, and smoke rising from ruined cities (the six cities need the `Smoke_Sprite` material in their `City > Smoke Material` field). These may already be committed by the time you read this: `git log` tells you.
- **Not pushed yet:** many local commits. Ilan will push before you start. Run `git pull`.
- **Open design question (Ilan decides):** how forgiving the interceptor should feel (Part 4.7).

### 10.2 What is left, by owner

**Ilan and Ilan's AI**

1. Check the new missile and smoke in Unity, then commit and push.
2. Decide the hit-feel tuning and run a balance playtest (which wave loses, how, is it fun).
3. Small polish: decorative taglines from the mockup, an optional "meteor destroyed" sound (needs a clip), a menu click sound.
4. **Cleanup for readability:** remove what nothing uses (`Blast_Glow.mat`, the Unity template leftovers `TutorialInfo`, `Readme.asset`, `InputSystem_Actions.inputactions`, colliders nobody reads), and review comments and naming.
5. Final README (credits for the models). The GDD stays frozen.
6. **Windows build** (.exe) and final full playtests (postponed on purpose until the end).
7. Course administration that only Ilan can do: the picture `images/reference-missile-command.png` that the GDD links to, the Excel sheet, and the submission through MAMA.

**Leon**

1. Set up (Part 3): clone, Unity 6000.3.20f1, play the game, set your git identity.
2. The models (Part 5): meteor rock, turret, six buildings and rubble, trees and scenery; integrate; credits.
3. Commit and push under your own name after each finished asset.
4. Optional extras (5.9), and playtest reports.

**Both:** the final playtest, the feature freeze, and the submission.

### 10.3 A suggested timeline

| When | Ilan and AI | Leon |
|---|---|---|
| Sep 21 to 22 | Check, commit and **push**. Tell Leon the scene is free. | Set up, read this guide, play for ten minutes. Start the meteor rock and the turret. |
| Sep 22 to 26 | Hit-feel decision, balance, cleanup, taglines | Meteor, turret, first building. Import, size-check, integrate the meteor (prefab). |
| Sep 27 to 28 | Support contracts, fix anything the models need | Remaining buildings, rubble, trees. Integrate (scene baton). |
| Sep 29 to 30 | Bug hunt, performance check | Fixes, polish, credits |
| **Oct 1** | **Feature freeze** (only fixes from here) | |
| Oct 2 to 3 | Build (.exe), test on another computer, README, packaging | Final playtest, README credits |
| **Oct 4** | **Deadline:** GitHub URL and MAMA | |

---

## Appendix A: event map

All events are `public static event` members. Publishers do not know their listeners.

| Event | Raised by | When | Listeners |
|---|---|---|---|
| `GameManager.GameStarted` | `GameManager.StartGame` | Play pressed, or Restart | `WaveSpawner` (begin waves), `UIManager` (hide menu), `AudioManager` |
| `GameManager.GameOver` | `GameManager` | Last city destroyed | `WaveSpawner` (stop), `UIManager` (show panel), `AudioManager` (music off, jingle) |
| `GameManager.GamePaused` / `GameResumed` | `Pause` / `Resume` | Esc or Pause button | `PauseMenu` |
| `GameManager.ScoreChanged(int)` | `GameManager` | Score changes | `UIManager` |
| `GameManager.KillScored(pos, points, killNumber)` | `GameManager.AddKill` | A meteor dies in a blast | `EffectSpawner` (score pop-up) |
| `Battery.AmmoChanged(int)` | `Battery` | Ammo spent or refilled | `UIManager`, `AmmoIcons` |
| `Battery.Fired` | `Battery.TryFire` | A shot leaves | `AudioManager` |
| `Interceptor.Arrived(pos)` | `Interceptor` | Reaches its point | `AudioManager`, `EffectSpawner` |
| `Meteor.Destroyed(pos, size)` | `Meteor.Kill` | Killed by a blast | `AudioManager`, `EffectSpawner` |
| `Meteor.Impacted(pos, size)` | `Meteor` | Hits the ground | `AudioManager`, `EffectSpawner`, `CameraShake` |
| `City.Destroyed` | `City.Collapse` | A city is lost | `GameManager`, `AudioManager`, `CameraShake`, `CityIcons` |
| `WaveSpawner.WaveStarted(int)` | `WaveSpawner` | A wave is announced | `UIManager`, `AudioManager`, `WaveBanner`, `PlayerAim` (blocks shooting) |
| `WaveSpawner.WaveSpawning` | `WaveSpawner` | The 2 s intro ends | `PlayerAim` (allows shooting) |
| `WaveSpawner.ProgressChanged(float)` | `WaveSpawner.Update` | Every frame during a wave | `WaveProgress` |

## Appendix B: more Unity gotchas (besides the table in Part 6)

- Pooled objects are **inactive** while waiting, and Unity's `Find...` calls skip inactive objects by default. That is why the pool counts its own active objects.
- A prefab added to the pools must also be dragged into the matching field of the `PoolManager` in the scene, or the pool stays empty (effects and pop-ups are optional, meteors, interceptors and blasts are not).
- `Time.timeScale = 0` (pause) also freezes every `WaitForSeconds` coroutine. That is intended, and the reason time and audio are paused together.
- Restarting reloads the scene, so all scene values come from the saved scene: **an unsaved scene change is lost on restart.**
- An object created in code (trails, lights, particle layers) has no Inspector fields. Its numbers are constants in the script.
- Changing a value on a prefab **instance** in the scene is an *override*. It does not change the prefab. Open the prefab to change it for everyone.

## Appendix C: five-minute Unity vocabulary

| Word | Meaning |
|---|---|
| **Hierarchy** | The list of objects in the open scene (usually the left panel) |
| **Project window** | The files of the project (usually the bottom panel) |
| **Inspector** | The properties of what you selected (usually the right panel) |
| **Scene view / Game view** | The editing window / what the player sees. Game view is where the camera looks. |
| **Console** | Messages and errors. Red = error, stop. |
| **Prefab** | A saved template of an object. Instances in the scene follow it. |
| **Component** | A piece of behaviour on an object (Mesh Filter, Collider, a script, ...) |
| **Mesh / Mesh Filter / Mesh Renderer** | The shape / the component that holds it / the component that draws it |
| **Material / Shader** | The look of a surface / the program that draws it |
| **Collider / Trigger** | A physical shape / a collider that reports overlaps but does not push |
| **Rigidbody (kinematic)** | Physics body (one that only moves when we move it) |
| **Pivot** | The point an object rotates and scales around (its origin) |
| **Emission / Bloom** | A surface that glows / the post-effect that makes bright things bleed light |
| **Pool** | A stack of reusable objects instead of creating and destroying |
| **Coroutine** | A method that can wait between steps (`yield return`) |
| **Event** | A message a script sends that other scripts can listen to |
| **Singleton** | A class with exactly one global instance |
| **`.meta` file / GUID** | Unity's ID card for every asset. Losing it breaks references. |
