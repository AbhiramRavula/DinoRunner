# Dino Runner

[![Play in Browser](https://img.shields.io/badge/Play_Now-Unity_Play-blue?style=for-the-badge&logo=unity)](https://play.unity.com/en/games/62c5b2b3-846d-4ba3-9836-ed9dde4ee4a6/dino-runner)

> 🎮 **Play Live in Your Browser**: [Play Dino Runner on Unity Play](https://play.unity.com/en/games/62c5b2b3-846d-4ba3-9836-ed9dde4ee4a6/dino-runner)

## Project Overview
**Dino Runner** is a colourful, vibrant 2D endless-runner game built in Unity 6. The player controls a cartoon dinosaur sprinting through an endless prehistoric desert, jumping over ground obstacles (cacti, stones, tumbleweeds), and ducking under flying pterodactyls (birds). The objective is to survive as long as possible, achieve the highest score, and beat personal records.

The game features rich cartoon illustration, responsive multi-platform controls (desktop keyboard/mouse and mobile swipe/tap), procedural 8-bit retro audio, animated UI menus, and persistent high scores.

---

## 1. Development Environment & Specifications
* **Engine**: Unity 6.3 LTS (`6000.3.20f1`)
* **Project Name**: `DinoRunner`
* **Main Scene**: `GameScene` (`Assets/Scenes/GameScene.unity`)
* **Target Platforms**: WebGL / Web Browser (Unity Play), Windows Desktop, Android (APK), and iOS
* **Target Frame Rate**: 60 FPS (`Application.targetFrameRate = 60`)
* **Physics & Colliders**: 2D Physics (`Rigidbody2D`, `BoxCollider2D`, `CircleCollider2D`)
* **UI Framework**: Unity UI (uGUI) with TextMeshPro (`TextMeshProUGUI`)
* **Input System**: Set to **Both** (`activeInputHandler: 2`) in ProjectSettings:
  * Supports legacy `UnityEngine.Input` for gameplay controls
  * Supports `InputSystemUIInputModule` for menu buttons and mobile touch
* **Audio**: Custom procedural chiptune 8-bit sound effects and seamless background music (WAV, 44.1 kHz, 16-bit PCM)

---

## 2. Project Directory Structure
```text
Assets/
  Art/               Source artwork, spritesheets, backgrounds, and slicing metadata
  Audio/             Audio assets (.wav and .meta)
    bg_music.wav     Seamless 130 BPM retro chiptune theme
    jump.wav         Upward pitch sweep SFX
    hit.wav          Crash / impact SFX
    score.wav        100-point milestone chime
    click.wav        UI button click SFX
  Prefabs/           Reusable obstacle and actor prefabs
    Bird.prefab           Flying pterodactyl obstacle (flapping animation)
    Cactus.prefab         Standard ground cactus
    Cactus_small.prefab   Small single cactus
    Cactus_Medium.prefab  Medium multi-stem cactus
    Cactus_large.prefab   Tall cactus
    stone_obstacle.prefab Rock obstacle
    Roller_obsctacle.prefab Rolling tumbleweed / dust bowl
  Scenes/            Unity scenes (GameScene.unity)
  Scripts/           C# gameplay and system scripts
  Settings/          Universal Render Pipeline and project configuration assets
```

---

## 3. Scene Architecture & Hierarchy
Key GameObjects configured in `GameScene`:
* **Main Camera**: Orthographic 2D camera (Size: 5)
* **Global Light 2D**: Ambient illumination for 2D sprites
* **BackgroundEnvironment**:
  * `Background_1` & `Background_2`: Parallax background tiles driven by `BackgroundScroller.cs`
* **Ground & Ground2**: Looping foreground platform colliders driven by `GroundScroller.cs`
* **Dino**: The player character with `Rigidbody2D`, `BoxCollider2D`, `DinoController.cs`, and `DinoAnimationController.cs`
* **ObstacleSpawner**: Spawns ground obstacles and flying birds at randomized intervals
* **GameManager**: Central coordinator managing game loop, UI panels, scoring, and `AudioManager`
* **Canvas** (Screen Space - Overlay):
  * **InGameHUD**: Real-time `ScoreText`, `HighScoreText`, and `PauseButton`
  * **StartMenuPanel**: Title header, Play button, and Settings button
  * **GameOverPanel**: Game Over header, `FinalScoreText`, `HighScoreText`, and Restart button
  * **PausePanel**: Pause header, Resume button, and Settings button
  * **SettingsPanel**: Music Toggle, SFX Toggle, and Back / Resume button
* **EventSystem**: Event processor with `InputSystemUIInputModule` for cross-platform UI interaction

---

## 4. Controls & Input Handling

The game provides unified controls for desktop and mobile devices:

| Action | Desktop Controls | Mobile Touch / Gestures |
| :--- | :--- | :--- |
| **Jump** | `Space`, `Up Arrow`, or Left Mouse Click | **Tap screen** or **Swipe Up** |
| **Duck** | Hold `Down Arrow` or `S` | **Swipe Down & Hold** |
| **Menus / Pause** | Mouse click on UI buttons | Tap on UI buttons *(gameplay touch is rejected while interacting with UI)* |

---

## 5. Core Systems & Scripts

### `GameManager.cs`
* **Singleton Lifecycle**: Accessible globally via `GameManager.Instance`.
* **State Management**:
  * `StartGame()`: Begins gameplay from the Start Menu (`IsPlaying = true`).
  * `PauseGame()`: Freezes time (`Time.timeScale = 0f`) and presents the Pause menu.
  * `ResumeGame()`: Restores time (`Time.timeScale = 1f`) and returns to gameplay.
  * `OpenSettings()`: Pauses gameplay and opens the Settings panel.
  * `CloseSettings()`: Restores previous pause or playing state.
  * `GameOver()`: Halts gameplay, plays hit SFX, compares score to High Score, saves to `PlayerPrefs`, and displays Game Over panel.
  * `RestartGame()`: Resets `Time.timeScale = 1f` and reloads the active scene.
* **Scoring**: Accumulates 10 points per second. Plays a reward chime every 100 points (`score.wav`). Saves `HighScore` persistently in `PlayerPrefs`.
* **Dynamic Speed Scaling**:
  * Starts at `baseSpeed = 7.0`.
  * Increases by `+0.5` every 100 points (`speedIncreasePer100Points = 0.5`).
  * Capped at `maxSpeed = 14.0`.
  * Synchronously accelerates the Ground, Obstacles, Birds, Background parallax layer, Dino running animation, and scales down obstacle spawn intervals to maintain consistent jumping distances.

### `AudioManager.cs`
* Dedicated audio singleton controlling separate `musicSource` and `sfxSource` components.
* Persists volume and mute preferences across play sessions using `PlayerPrefs`:
  * `Music_Muted` (0 = enabled, 1 = muted)
  * `SFX_Muted` (0 = enabled, 1 = muted)
* Fully wired to the UI toggles in `SettingsPanel`.

### `DinoController.cs`
* **Position Locking**: Strictly locks horizontal movement (`FixedUpdate` clamps `linearVelocity.x = 0` and enforces `transform.position.x = lockedX`) to eliminate character drift.
* **Ascending Jump Check**: Disables grounding when upward velocity $V_y > 0.1\text{ m/s}$ to ensure clean jump arcs without premature ground-snapping.
* **Contact Normal Grounding**: Evaluates ground collisions using collision contact normals ($Y > 0.1$) to prevent wall-cling bugs.
* **Dynamic Ducking**:
  * Standing: Full collider height.
  * Ducking: Scales collider height to `55%` and offsets the box downward so the player's base stays aligned with the ground.

### `DinoAnimationController.cs`
* Frame-based animation controller handling **Run** (looping sprites), **Jump** (single sprite), and **Duck** (looping frames).
* Frame rates are time-based (`Time.deltaTime`) to remain independent of framerate fluctuations.
* Maintains strict sprite scale consistency without runtime transform distortion.

### `ObstacleSpawner.cs`
* Spawns ground obstacles (cacti, stones, tumbleweeds) and flying birds.
* **Bird Altitudes**:
  * `birdLowY`: Low flight height. The Dino must **jump over** it.
  * `birdHighY`: High flight height. The Dino must **duck under** it.
* Randomizes spawn delay between `minInterval` and `maxInterval`.

### `BackgroundScroller.cs` & `GroundScroller.cs`
* **BackgroundScroller**: Loops background parallax layers seamlessly by automatically linking sibling tiles and snapping them relative to each other (`otherTile.position.x + tileWidth - 0.02f`) to prevent gaps.
* **GroundScroller**: Moves solid ground colliders left at obstacle speed (`speed: 7`) and recycles tiles endlessly.

### `ObstacleMovement.cs` & `BirdMovement.cs`
* Translates obstacles to the left across the screen.
* Automatically destroys obstacles once they reach `destroyX = -12`.
* `OnTriggerEnter2D` detects `Player` tag and notifies `GameManager.Instance.GameOver()`.
* `BirdMovement` includes sprite flapping animation cycling across flight frames.

---

## 6. Tuning Obstacles & Bird Altitudes

When configuring obstacle heights, use these reference coordinates:
* **Ground Surface Level**: $Y \approx -2.70$
* **Dino Standing Top**: $Y \approx -0.65$
* **Dino Ducking Top**: $Y \approx -1.57$
* **Dino Jump Apex (Feet)**: $Y \approx -0.60$

### Recommended Bird Spawner Settings:
* **Bird Scale**: `(1, 1, 1)` in [Assets/Prefabs/Bird.prefab](file:///c:/Users/abhir/SR_InterProjects/Repos/DinoRunner/Assets/Prefabs/Bird.prefab)
* **Bird BoxCollider2D**: Size `X = 2.0`, `Y = 1.2` (centered on bird body)
* **ObstacleSpawner Settings**:
  * **`Bird High Y`**: `0.5` *(Dino ducks cleanly under; standing Dino hits)*
  * **`Bird Low Y`**: `-1.6` *(Dino jumps cleanly over; standing/ducking Dino hits)*
  * **`Bird Chance`**: `0.2` (20% chance of an obstacle being a bird)
  * **`Spawn X`**: `10`

---

## 7. Audio Setup & Sound Effects
All sound effects and music are stored in `Assets/Audio/` and wired to the `AudioManager`:
* `bg_music.wav`: Ambient chiptune music (loops during gameplay, stops on Game Over).
* `jump.wav`: Triggered on jump press.
* `hit.wav`: Triggered on obstacle collision.
* `score.wav`: Triggered every 100 points scored.
* `click.wav`: Triggered when clicking or tapping menu buttons.

Volume and mute states can be toggled in the in-game **Settings** dialog.

---

## 8. Mobile Deployment (Android & iOS)

### Building for Android (APK)
1. In Unity, open **File $\rightarrow$ Build Profiles** (or **Build Settings**).
2. Select **Android** and click **Switch Platform**.
3. Under **Player Settings $\rightarrow$ Other Settings**:
   * **Package Name**: `com.YourStudio.DinoRunner`
   * **Minimum API Level**: Android 7.0 (API Level 24) or higher
   * **Target API Level**: Automatic (highest installed)
   * **Scripting Backend**: `IL2CPP` (recommended for Google Play 64-bit compliance)
   * **Target Architectures**: Check `ARMv7` and `ARM64`
4. Click **Build** to produce the `.apk` file.

---

## 9. Verification & Testing Checklist
- [x] Scene runs with zero console warnings or errors.
- [x] Dino starts on ground without sinking, floating, or X-axis drift.
- [x] `Ground` and `Ground2` loop infinitely with no visual seam or collider gap.
- [x] `Background_1` and `Background_2` loop smoothly in parallax without separation.
- [x] Jump arc allows clearing all cactus variations and low birds.
- [x] Ducking collider shrinks and clears high birds.
- [x] Mobile touch controls work for tap/swipe jumping and swipe ducking.
- [x] UI button clicks do not falsely trigger jumps or ducks.
- [x] Audio plays for jumps, hits, button clicks, milestones, and BGM.
- [x] Settings panel toggles mute/unmute audio and persists across reloads.
- [x] Pause menu freezes gameplay (`Time.timeScale = 0`) and resumes cleanly.
- [x] High score is saved in `PlayerPrefs` and updates when beaten.
