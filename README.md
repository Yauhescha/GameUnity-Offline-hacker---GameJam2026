# Offline Hacker: Daily Routine

A first-person Unity game created for GMTK Game Jam 2026. Explore an office, interact with computers and use a flash drive to collect and transfer the required files before time runs out.

[Play on itch.io](https://yauhescha.itch.io/offline-hacker-daily-routine)

## Gameplay

- First-person exploration and object interaction.
- An in-game desktop/file interface called Shindows.
- Flash-drive inventory and file collection.
- A mission checklist, countdown and success/failure screens.
- A comic prologue, office audio and interaction prompts.

The mission implementation checks for the `architecture`, `offer` and `roadmap` file IDs before accepting the final transfer.

## Controls

| Input | Action |
| --- | --- |
| WASD | Move |
| Mouse | Look around |
| E | Interact with the focused object |

Computer and modal interfaces use their on-screen controls.

## Open in Unity

1. Install **Unity 6000.3.20f1**, the version recorded in `ProjectSettings/ProjectVersion.txt`.
2. Add this repository as a project in Unity Hub.
3. Wait for packages and assets to import.
4. Open `Assets/Scenes/MainMenu.unity` and enter Play mode.

The configured scene order is **MainMenu → Prolog → Game**. Keep that order in Build Profiles when creating a player build. Install the target platform's Unity build support before switching platforms.

## Source layout

| Path or script | Responsibility |
| --- | --- |
| `Assets/Script/` | Gameplay and UI code |
| `PlayerController` | Movement, camera and interaction detection |
| `ShindowsComputer`, `Shindows9UI` | Computer interaction and desktop UI |
| `PlayerInventory`, `PlayerFlashDrive` | Inventory and file storage |
| `MissionObjective`, `CountdownTimer`, `EndGameManager` | Mission progression and results |
| `Assets/Scenes/` | Menu, prologue and game scenes |

The project includes external office assets under `Assets/external/`. Preserve their attribution and metadata when reorganizing the project.

This repository is an archived game-jam project.
