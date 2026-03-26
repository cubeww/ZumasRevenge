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
