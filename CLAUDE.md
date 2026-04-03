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
- `PlayerMovement` — WASD keyboard input → CharacterController movement with gravity. Handles FBX Z-up → Y-up axis compensation via `AxisCompensation` quaternion applied to the model child transform.
- `CameraFollow` — Smooth third-person camera following the player with configurable offset and LookAt.

### Character Asset
- `Assets/Character/Powerroo.fbx` — Main character model (Git LFS, ~122MB)
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
- **FBX Axis**: Blender-exported FBX uses Z-up. A static `Quaternion.Euler(-90, 0, 0)` compensates in code. New character scripts must account for this.
- **Serialized Fields**: Use `[SerializeField] private` pattern for inspector-exposed values (not `public`).
