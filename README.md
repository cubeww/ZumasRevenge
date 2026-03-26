# Zuma's Revenge!

This repository contains a modernized C# rebuild/porting effort for *Zuma's Revenge!* on top of MonoGame and .NET 10.

The codebase is being adapted from an older XNA-style implementation and updated to run on current platforms. The main goals of this project are:

- keep the original gameplay logic intact
- replace legacy platform/runtime assumptions with modern .NET and MonoGame equivalents
- make the project buildable on Windows again
- bring up an Android version from the same gameplay codebase

This is an unofficial project and is not affiliated with PopCap or EA.

## Current Status

- Windows build target: `net10.0-windows`
- Android build target: `net10.0-android`
- Rendering/input backend: MonoGame `3.8.4.1`
- Shared engine layer: `SexyFramework`

The Windows version is the primary development target and has already received a number of compatibility and input fixes. Android support is present in the repository and is being actively adapted for mobile rendering, lifecycle, and touch behavior.

## Repository Layout

- `Zuma's Revenge!` - Windows game project and gameplay code
- `Zuma's Revenge.Android` - Android launcher/project
- `SexyFramework` - shared framework and rendering/audio/platform abstraction layer
- `tools` - utility tools used during investigation and porting work

## Building

### Windows

Requirements:

- .NET 10 SDK
- Windows desktop development support

Build command:

```powershell
dotnet build "Zuma's Revenge!\Zuma's Revenge!.csproj" -v minimal
```

### Android

Requirements:

- .NET 10 SDK
- Android workload: `dotnet workload install android`
- Android SDK/platform tools configured on the machine

Build command:

```powershell
dotnet build "Zuma's Revenge.Android\Zuma's Revenge.Android.csproj" -f net10.0-android -v minimal
```

## Game Content

The project expects game assets under `Zuma's Revenge!/Content`.

Depending on how this repository is shared, original commercial assets may need to be supplied separately. If you are setting up the project on a new machine, make sure the required content files are available before running the game.

## Notes

- The codebase is focused on compatibility restoration and platform adaptation.
- Some subsystems were originally written around older XNA/mobile assumptions and are being corrected incrementally.
- If you are extending the port, it is usually best to verify changes on Windows first, then validate Android-specific behavior separately.

## Android Startup Intro Notes

The PopCap startup intro on Android previously had multiple rendering failures that did not reproduce on Windows:

- the lightning animation could render as a solid yellow block
- logo/lightning frames could sample the wrong atlas region and flash corrupted textures
- the black fade and white flash transitions could expose bad intermediate frames
- the transition from the lightning animation into the loading screen could feel abrupt

The final fix was not a single change. It required both low-level texture/atlas fixes and a local `LoadingScreen` workaround path.

Key files:

- `SexyFramework/SexyFramework.Drivers.Graphics/BaseXNARenderDevice.cs`
- `SexyFramework/SexyFramework.Graphics/MemoryImage.cs`
- `Zuma's Revenge!/ZumasRevenge/LoadingScreen.cs`

What each file is responsible for:

- `BaseXNARenderDevice.cs`: fixes Android texture-data access, atlas UV calculation, and sub-image materialization so atlas-backed images sample the correct texture region.
- `MemoryImage.cs`: fixes `GetBits()` so atlas sub-images are copied correctly, including rotated atlas entries.
- `LoadingScreen.cs`: adds Android-specific safe handling for the startup intro by materializing logo/lightning frames into standalone textures, using texture overlays for black/white full-screen fades, and running a controlled Android lightning sequence.

Important maintenance guidance:

- Do not assume this issue is only caused by the semi-transparent black fade. A "black fade only" fix was tested and was not sufficient.
- Avoid global viewport/projection/render-area changes when touching this intro. Earlier attempts in that direction caused the intro to collapse into the top-left corner on Android.
- Prefer keeping Android intro fixes local to `LoadingScreen` unless there is clear evidence of a shared renderer bug.
- When changing the intro again, always test both Windows and Android. Windows often looks correct even when Android atlas sampling is still broken.
- If the Android intro regresses back to yellow blocks or corrupted frames, re-check the three files above before trying cosmetic transition tweaks.

Known-good outcome after the current fix:

- logo fade-in is stable on Android
- lightning frames render correctly instead of yellow blocks
- corrupted intermediate intro frames are removed
- the lightning-to-loading transition includes a white fade-out instead of a hard cut

## Iron Frog / Heroic Frog Notes

The original game unlocks two post-game modes:

- `Iron Frog`: a separate chapter/zone
- `Heroic Frog`: a harder replay of Adventure mode

This port still contains important pieces of both systems, but they are not in the same state.

### Iron Frog

Current findings:

- Iron Frog level content is present in `Content/levels`.
- `levels.xml.xnb` defines a dedicated zone 7 starting at `ironfrog1`.
- `ironfrog1` through `ironfrog10` are present and marked with `ironfrog="true"`.
- `LevelMgr` still remaps Iron Frog levels into zone 7 and tracks first/last Iron Frog level.
- `GameApp.StartIronFrogMode()` still exists, and board/gameplay code still contains Iron Frog-specific flow, stats, win handling, and UI text.

Important caveat:

- The visible main-menu button set currently does not appear to expose a normal Iron Frog entry.
- `MainMenu.cs` still contains leftover logic for an Iron Frog button (`id=15`), but the currently constructed menu widgets do not obviously create that button.

Practical conclusion:

- Iron Frog is not missing as gameplay content.
- It looks much more like an "entry/UI wiring is incomplete or removed" problem than a missing-data problem.

### Heroic Frog

Current findings:

- Heroic mode has separate profile/state storage (`mHeroicModeVars`, `mHeroicStats`, heroic save-game naming, heroic beta stats).
- Map and stats code still contain heroic-specific branches.
- The repository contains many `_hard.dat` level files under `Content/levels`, which strongly suggests a dedicated harder curve/path set still exists.
- Both `LevelMgr` and `LevelsXmlReader` contain logic that appends `_hard` to curve paths when `mIsHardConfig` is enabled.
- `GameApp` still defines `mHardLevelXML = "levels/levels_hard"`.
- Multiple bosses still contain hard-mode branches that remove beginner/tutorial leniency or otherwise make behavior stricter.

Examples of remaining hard-mode evidence:

- Boss tutorial/leniency branches still check `IsHardMode()`.
- Some boss logic also treats "already beat this zone once" as equivalent to harder behavior, which suggests the original code path was designed around replay/hard-mode escalation.

Important caveats:

- `GameApp.IsHardMode()` currently returns `false` unconditionally.
- No current menu path was found that sets `mClickedHardMode = true`.
- No active code path was found that sets `LevelMgr.mIsHardConfig = true`.
- No separate `levels_hard.xnb` content file was found in `Content/levels`; only `levels.xnb` and `levels.xml.xnb` are present.

Practical conclusion:

- Heroic Frog is not just a name left in UI text. There is real evidence of intended hard-mode data and gameplay differences.
- However, the activation chain appears broken/incomplete in the current port.
- In its current state, Heroic looks like a partially preserved system rather than a fully usable mode.

### Future Implementation Guidance

- If Iron Frog is restored, start from menu/flow wiring first, not from level data.
- If Heroic is restored, verify all of the following together:
  - how the mode is selected from UI
  - how `mClickedHardMode` is set
  - how `IsHardMode()` should report runtime state
  - how `LevelMgr.mIsHardConfig` is supposed to be enabled
  - whether a real `levels_hard` content asset must be generated or loaded differently in this port
- Do not assume `_hard.dat` files alone are enough to make Heroic work. The mode-selection, save-game, level-manager, and content-loading chain all need to agree.
- Before implementing either mode, test both a fresh profile and a post-game-unlocked profile, because some unlock logic depends on zone-completion counters.
