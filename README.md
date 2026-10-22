# Dino Runner

## Project Overview
Dino Runner is a 2D endless-runner game built in Unity. The player controls a cartoon dinosaur running across a scrolling ground, jumping over ground obstacles, and ducking under low-flying birds. The goal is to survive as long as possible and earn a higher score.

The visual direction is a colourful, detailed cartoon illustration suitable for a children's game (NOT grayscale or pixel art). Assets should feature a consistent art style, character proportions, clean edges, and transparent backgrounds where appropriate.

---

## 1. Current Development Environment
- **Engine**: Unity 6.3 LTS (`6000.3.20f1`).
- **Project Name**: `DinoRunner`.
- **Main Scene**: `GameScene`.
- **Target Platforms**: Windows desktop and Android.
- **Game Type**: 2D side-view endless runner.
- **Rendering**: 2D sprites and physics using `Rigidbody2D` and `Collider2D` components.
- **UI Text**: TextMeshPro (`TMP_Text`).
- **Input Supported**:
  - Jump: `Space`, `Up Arrow`, mouse click, screen touch.
  - Duck: `Down Arrow` or `S`.
- **Art Direction**: Colourful, detailed cartoon style.

---

## 2. Project Folders
Expected folders under `Assets/`:
```text
Assets/
  Art/          Artwork and source art
  Audio/        Audio assets (e.g. Assets/Audio/SFX/)
  Prefabs/      Reusable configured GameObjects
  Scenes/       Unity scenes, including GameScene
  Scripts/      C# scripts
  Settings/     Unity settings
  Sprites/      Imported sprite sheets and sliced sprites
```

---

## 3. Scene Structure
Key objects in `GameScene` Hierarchy:
- `Main Camera`
- `Global Light 2D`
- `Dino`
- `Ground` & `Ground2`
- `GameManager`
- `ObstacleSpawner`
- `Canvas`
  - `ScoreText`
  - `GameOverPanel`
- `EventSystem`

---

## 4. Core Gameplay Loop
1. The dinosaur runs in place while ground and obstacles move from right to left.
2. The score increases over time while the game is active (default: 10 score units/sec, displayed as 5 digits, e.g. `SCORE: 00017`).
3. The player jumps to avoid ground obstacles (cacti, stones, dust bowls).
4. The player ducks to avoid low-flying birds.
5. Collision with any obstacle ends the run.
6. The Game Over panel appears and allows restarting the scene.

---

## 5. Player and Dinosaur Setup
- **GameObject**: `Dino` tagged as `Player`.
- **Transform (Typical)**: Position `(-5.13, -2.135, 0)`, Scale `(0.9, 0.9, 1)`.
- **Components**:
  - `SpriteRenderer`
  - `Rigidbody2D` (Dynamic, Gravity Scale ~3, Freeze Rotation Z enabled)
  - `BoxCollider2D` (`Is Trigger = OFF`)
  - `DinoController`
  - `DinoAnimationController`

---

## 6. Player Input and Physics
Managed by `DinoController.cs`:
- **Jump**: `Space`, `Up Arrow`, mouse click, or screen touch.
- **Duck**: Hold `Down Arrow` or `S` while grounded.
- Jump is blocked while ducking.
- Ducking reduces `BoxCollider2D` height to ~55% and lowers the collider offset.
- Ground contact detected via collision with objects tagged `Ground`.
- Uses `Rigidbody2D.linearVelocity` (Unity 6).

---

## 7. Dinosaur Animation
Managed by `DinoAnimationController.cs`:
- Switches between Run frames (`Sprite[]`), Jump frame (`Sprite`), and Duck frames (`Sprite[]`).
- **Art Import & Alignment Guidelines**:
  - Keep cell sizes, Pixels Per Unit (PPU), and pivots consistent (`Bottom Center` recommended).
  - Align character feet/baseline across all frame sets.
  - Do not dynamically alter `Transform.localScale` per frame to compensate for uneven artwork.
  - Use `Bilinear` filtering for smooth cartoon rendering (`Point` only for pixel art).

---

## 8. Ground Scrolling
Managed by `GroundScroller.cs`:
- Two tiles (`Ground` and `Ground2`) move left and loop continuously.
- Default settings: `Speed = 7`, `Tile Width = 25`.
- `Ground2` placed exactly `tileWidth` (25 units) to the right of `Ground`.
- Both objects require `Ground` tag and solid colliders (`Is Trigger = OFF`).

---

## 9. Obstacle Types and Prefab Workflow
- **Ground Obstacles**: Small Cactus, Medium Cactus, Large Cactus, Stone/Rock, Rolling Dust Bowl/Tumbleweed.
- **Flying Obstacles**: Bird (low and high flight altitudes).

### Prefab Creation Steps
1. Drag sprite into Scene.
2. Rename (e.g. `Cactus_Small`).
3. Add `BoxCollider2D` or `CircleCollider2D` fitting the visible artwork.
4. Set `Is Trigger = ON`.
5. Attach `ObstacleMovement.cs`.
6. Save prefab to `Assets/Prefabs/`.

---

## 10. Obstacle Movement and Collisions
Managed by `ObstacleMovement.cs`:
- Moves obstacle left (`Speed = 7`), destroys past `Destroy X = -12`.
- Detects player overlap via `OnTriggerEnter2D` checking `CompareTag("Player")`.
- Triggers `GameManager.Instance.GameOver()`.

---

## 11. Bird Flight Heights and Direction
- Bird moves right-to-left and faces left.
- Use `SpriteRenderer.flipX` to face left (avoid 180° prefab rotation).
- **Intended Heights**:
  - **Low Bird**: Hits standing Dino; dodged by ducking.
  - **High Bird**: Runs underneath safely; hits jumping Dino.

---

## 12. ObstacleSpawner
Managed by `ObstacleSpawner.cs`:
- Spawns configured ground obstacles and birds at designated intervals.
- Default parameters: `Spawn X = 10`, `Ground Y = -2.1`, `Min Interval = 1.8`, `Max Interval = 2.8`, `Bird Chance = 0.15`.

---

## 13. GameManager, Score, and UI
Managed by `GameManager.cs`:
- Singleton `GameManager.Instance`.
- Manages `IsPlaying`, updates score, activates `GameOverPanel`.
- `RestartGame()` restores `Time.timeScale = 1` and reloads `GameScene`.

---

## 14. Audio Plan
Planned sound effects (`Assets/Audio/SFX/`):
- Jump, Landing (optional)
- Obstacle impact / collision, Game Over
- Score milestone / reward
- Button click, Bird flap (optional), Duck whoosh (optional)
- Track licensing (CC0 / Kenney / Mixkit).

---

## 15. Art and Asset Standards
- Consistent cartoon palette, line weight, and lighting.
- Genuine alpha transparency (no baked checkerboard patterns).
- Sufficient sprite sheet padding to prevent bleed when slicing.

---

## 16. Script Responsibilities
| Script | Responsibility |
|---|---|
| `GameManager.cs` | Game state, score accumulation, Game Over, and scene restart |
| `DinoController.cs` | Player input, jumping, ducking, and collider resizing |
| `DinoAnimationController.cs` | Frame animation state machine (run, jump, duck) |
| `GroundScroller.cs` | Endless looping of ground tiles |
| `ObstacleMovement.cs` | Obstacle translation, out-of-bounds destruction, trigger collision |
| `ObstacleSpawner.cs` | Procedural obstacle and bird instantiation |
| `BirdMovement.cs` | Bird flight mechanics, animation, and trigger collision |

---

## 17. Testing Checklist
- [ ] `GameScene` runs without console errors.
- [ ] Dino stands solidly on the ground.
- [ ] `Ground` and `Ground2` loop seamlessly without gaps.
- [ ] Dino animation maintains consistent size and baseline.
- [ ] Jump and duck inputs function properly.
- [ ] Ducking shrinks player collider and restores on release.
- [ ] Collisions with all obstacles trigger Game Over.
- [ ] Bird flies left; low bird dodged by ducking, high bird cleared standing.
- [ ] Game Over panel halts gameplay and score; restart reloads scene.

---

## 18. Troubleshooting Guide
- **Dino doesn't die on obstacle**: Verify `Is Trigger = ON` on obstacle collider, Dino has `Player` tag, `Rigidbody2D` is active, and `ObstacleMovement` is attached.
- **Dino falls through floor**: Check `Ground` tag and ensure ground colliders have `Is Trigger = OFF`.
- **Dino changes size during animations**: Verify uniform sprite canvas slicing and consistent pivots (`Bottom Center`).
- **Bird rotation reset on spawn**: Use `SpriteRenderer.flipX` instead of rotating prefab transform.

---

## 19. Development Milestones
1. **Milestone 1**: Stabilise the core loop (ground looping, jump/duck, collision, game over, restart).
2. **Milestone 2**: Standardise character assets (consistent sprites, pivots, animations).
3. **Milestone 3**: Complete obstacle prefabs (cacti, stone, dust bowl, birds).
4. **Milestone 4**: Spawning tuning and difficulty progression.
5. **Milestone 5**: Audio integration and visual polish.
6. **Milestone 6**: Multi-platform builds (Windows & Android testing).
