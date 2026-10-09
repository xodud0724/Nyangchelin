# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

"Nyangchelin" is a 2D Unity game (Unity 6, editor 6000.3.10f1, URP 17.3, Input System 1.18). A cat player fishes at the Han River (hangang.png). Code comments and commit messages are written in Korean.

## Building / running

There is no CLI build, lint or test setup. Open the project in the Unity Editor and press Play. The Test Framework package is installed, but there are no tests yet.

- Only `Assets/Scenes/Scene_main.unity` is in the build settings. `Scene_TY` and `Scene_CB` are per-developer working scenes (initials), so add them to Build Settings before relying on them in a build.
- The project includes the `unity-mcp-server` package (com.akiojin.unity-mcp-server). The `mcp__unity-mcp-server__*` tools can drive the open Editor: compile state, console, play/stop, scene and GameObject edits.
- After editing a `.cs` file outside the Editor, check compilation with `get_compilation_state` / `read_console` instead of assuming it compiled.

## Code layout

All gameplay scripts currently live in `Assets/TutorialInfo/Scripts/` (leftover from the Unity template folder, not tutorial code):

- `FisingRod.cs`: the class is spelled `FisingRod`, and the file name must match the class for Unity. It cycles `fishingRodSprites[]` (FishingRod, FishingRod_bent, FishingRod_catching in `Assets/Sprites`) on Space and moves the rod's `localPosition` (x 0.5 when idle, x 5 otherwise). It uses the legacy `Input.GetKeyDown`.
- `Player.cs`, `FishingManager.cs`: empty stubs for the player and for fishing game flow.
- `Readme.cs` and `Editor/ReadmeEditor.cs` belong to the Unity template welcome asset and are unrelated to the game.

Input: `Assets/InputSystem_Actions.inputactions` is the new Input System asset, while the existing code polls the legacy `Input` class. Player Settings has Active Input Handling set to "Both" (`activeInputHandler: 2`), so either works.

## Git workflow

The main branch is `main`. Feature work happens on branches such as `feat/Player`. Commit `.meta` files together with their assets. `Library/`, `Temp/`, `Logs/` and `UserSettings/` are gitignored. `Assets/_Recovery` holds Unity's scene-recovery files.
