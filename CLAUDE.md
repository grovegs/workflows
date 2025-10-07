# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is Grove Games' collection of reusable GitHub Actions workflows for building, testing, formatting, and releasing game projects across multiple platforms and engines.

## Project Structure

### Core Workflows

- `.github/workflows/` - Main reusable workflow definitions
  - `project-*` - Unified workflows for Godot and Unity game projects
  - `package-*` - Unified workflows for .NET and Godot packages
  - `release.yml` - Main release orchestration workflow
  - `tests.yml` - Automated testing of workflows using sandbox applications

### Sandbox Applications

- `sandbox/GodotApplication/` - Sample Godot C# project for testing workflows
- `sandbox/ConsoleApplication/` - Sample .NET console application for testing

## Workflow Types and Commands

### Project Workflows

Unified workflows for both Godot and Unity game projects using array-based inputs:

- **Release**: `project-release.yml` - Builds Android/iOS releases for multiple projects, publishes to Firebase/GitHub/TestFlight
- **Testing**: `project-tests.yml` - Runs .NET unit tests and engine-specific tests for multiple projects
- **Formatting**: `project-format.yml` - Formats C# code and project files for multiple projects

These workflows use array-based inputs to support any number of projects:

- `godot-projects` - Array of Godot project paths containing project.godot files
- `godot-tests` - Array of Godot test project paths containing .csproj files
- `unity-projects` - Array of Unity project paths containing ProjectSettings folders

Key requirements:

- **Godot projects**: `global.json` file with SDK version and Godot.NET.Sdk reference, `project.godot` file
- **Unity projects**: Unity license credentials and platform-specific signing certificates
- Separate test projects for unit testing

### Package Workflows

- **Release**: `package-release.yml` - Builds and publishes NuGet packages for both .NET and Godot projects
- **Testing**: `package-tests.yml` - Runs .NET unit tests with support for multiple project types
- **Formatting**: `package-format.yml` - Formats C# code for .NET and Godot projects

These unified workflows use array-based inputs to support any number of projects:

- `dotnet-projects` - Array of .NET project paths to build/format/package
- `dotnet-tests` - Array of .NET test project paths to test
- `godot-addons` - Array of Godot addon paths to package

## Version Management

All workflows use semantic versioning with these version types:

- `major` - Breaking changes (X.0.0)
- `minor` - New features (0.X.0)
- `patch` - Bug fixes (0.0.X)

Versions are automatically bumped using the `grovegs/actions` bump-version action.

## Configuration Files

### global.json

Required for all .NET-based projects (Godot C#, .NET packages):

```json
{
  "sdk": {
    "rollForward": "major",
    "version": "8.0.401"
  },
  "msbuild-sdks": {
    "Godot.NET.Sdk": "4.3.0" // Only for Godot projects
  }
}
```

## External Dependencies

Workflows depend on custom actions from the `grovegs/actions` repository:

- Setup actions: `setup-dotnet`, `setup-godot`, `setup-unity`, `setup-android`, `setup-xcode`
- Build actions: `build-dotnet`, `build-godot`, `build-unity`
- Test actions: `test-dotnet`, `test-godot`
- Publishing actions: `publish-firebase`, `publish-github`, `publish-nuget`, `publish-testflight`
- Utility actions: `bump-version`, `generate-changelog`, `format-dotnet`

## Usage Examples

### Using Project Workflows

For releasing multiple Godot projects:

```yaml
uses: ./.github/workflows/project-release.yml
with:
  name: "My Game Collection"
  godot-projects: '["games/PuzzleGame", "games/ActionGame"]'
  version-type: "minor"
  environment: "Development"
  global-json-file: "global.json"
```

For releasing Unity projects:

```yaml
uses: ./.github/workflows/project-release.yml
with:
  name: "My Unity Game"
  unity-projects: '["projects/MainGame", "projects/DemoLevel"]'
  version-type: "patch"
  environment: "Production"
```

For testing multiple projects:

```yaml
uses: ./.github/workflows/project-tests.yml
with:
  godot-projects: '["games/PuzzleGame", "games/ActionGame"]'
  godot-tests: '["tests/PuzzleGame.Tests", "tests/ActionGame.Tests"]'
  unity-projects: '["projects/MainGame"]'
  global-json-file: "global.json"
```

### Using Package Workflows

For a simple .NET package:

```yaml
uses: ./.github/workflows/package-release.yml
with:
  name: "My Package"
  dotnet-projects: '["src/MyPackage"]'
  version-type: "minor"
  global-json-file: "global.json"
```

For a complex multi-project package:

```yaml
uses: ./.github/workflows/package-release.yml
with:
  name: "My Godot Package"
  dotnet-projects: '["src/Core", "src/Godot", "src/Extensions"]'
  godot-addons: '["addons/my-plugin", "addons/another-plugin"]'
  version-type: "patch"
  global-json-file: "global.json"
```

For testing multiple packages:

```yaml
uses: ./.github/workflows/package-tests.yml
with:
  dotnet-tests: '["tests/Core.Tests", "tests/Godot.Tests", "tests/Integration.Tests"]'
  global-json-file: "global.json"
```

## Development Notes

- All workflows are designed as reusable (`workflow_call`) for use by other repositories
- Both project and package workflows use array-based inputs with GitHub Actions matrix strategy for scalability
- Conditional job execution - only runs jobs for provided inputs (e.g., if `godot-projects` array is empty, Godot jobs are skipped)
- Environment-specific builds support both `Development` and `Production` configurations
- Caching is enabled for main/develop branches to improve build performance
- Firebase publishing is only enabled for Development builds, TestFlight for Production
- GitHub releases include downloadable build artifacts from all successful platform builds
- Use `tests.yml` to validate workflow changes against sandbox applications
- Project workflows support mixed engine types (can build both Godot and Unity projects in same release)
