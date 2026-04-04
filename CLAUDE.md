# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Powerroo is a Unity 6 (6000.4.1f1) 3D character action game featuring a kangaroo-like character "Powerroo". The project uses Universal Render Pipeline (URP) and the new Input System.

## Tech Stack

- **Unity**: 6000.4.1f1 (Unity 6)
- **Render Pipeline**: URP 17.4.0 (PC and Mobile profiles in `Assets/Settings/`)
- **Input**: Input System 1.19.0 (action map in `Assets/InputSystem_Actions.inputactions`)
- **Language**: C# (.NET managed by Unity)
- **LFS**: `*.fbx` files are tracked via Git LFS

## Architecture

### Game Loop
- `PlayerMovement` — WASD keyboard input → CharacterController movement with gravity. Auto-finds `Powerroo_born` as `modelTransform`. `AxisCompensation` is `Quaternion.identity` (Powerroo_born needs no facing correction). Walk/run speed with hold-time transition.
- `WalkAnimation` — Procedural walk/run animation via direct bone `localRotation` manipulation. Bone paths are `[SerializeField]` (default: `CharacterArmature/root/...` for Powerroo_born). Swings arms, legs, and tail with walk/run blend.
- `CameraFollow` — Smooth third-person camera following the player with configurable offset and LookAt.

### Character Asset
- `Assets/Character/Powerroo_born.fbx` — Active character model (SD-style, ~0.65 units tall). Bone root: `CharacterArmature/root`. Animator has no controller (procedural animation only), `ApplyRootMotion = false`.
- `Assets/Character/Powerroo.fbx` — Original character model (Git LFS, ~122MB, disabled in scene)
- `Assets/Character/textures/` — Per-part baked textures (body, head, arms, legs, horns, tail, eyes, mouth, ribbons)

### Scenes
- `Assets/Scenes/SampleScene.unity` — Main scene

### URP Settings
- `Assets/Settings/PC_RPAsset.asset` / `PC_Renderer.asset` — PC rendering profile
- `Assets/Settings/Mobile_RPAsset.asset` / `Mobile_Renderer.asset` — Mobile rendering profile

## Development with uloop MCP

This project uses `io.github.hatayama.uloopmcp` for Unity ↔ Claude Code integration. Available uloop skills:

- `/uloop-compile` — Compile and check for errors
- `/uloop-run-tests` — Run EditMode/PlayMode tests
- `/uloop-get-logs` — Read Unity Console logs
- `/uloop-screenshot` — Capture editor windows
- `/uloop-control-play-mode` — Start/stop/pause PlayMode
- `/uloop-get-hierarchy` — Inspect scene hierarchy
- `/uloop-find-game-objects` — Search for GameObjects
- `/uloop-execute-dynamic-code` — Run C# code in the editor at runtime

## Key Conventions

- **Input**: Currently reads `Keyboard.current` directly in `PlayerMovement`. The InputSystem_Actions asset defines Player/UI action maps but is not yet wired to the scripts.
- **FBX Axis**: Powerroo_born's `CharacterArmature` has built-in `-90° X` rotation for Z-up → Y-up. `PlayerMovement.AxisCompensation` is identity (no extra compensation needed). New character scripts must account for bone paths starting with `CharacterArmature/root/`.
- **Serialized Fields**: Use `[SerializeField] private` pattern for inspector-exposed values (not `public`).
