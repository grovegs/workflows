# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is Grove Games' collection of reusable GitHub Actions workflows for building, testing, formatting, and releasing game projects across multiple platforms and engines.

## Project Structure

### Core Workflows
- `.github/workflows/` - Main reusable workflow definitions
  - `godot-project-*` - Workflows for Godot Engine projects
  - `unity-project-*` - Workflows for Unity Engine projects
  - `package-*` - Unified workflows for .NET and Godot packages
  - `release.yml` - Main release orchestration workflow
  - `tests.yml` - Automated testing of workflows using sandbox applications

### Sandbox Applications
- `sandbox/GodotApplication/` - Sample Godot C# project for testing workflows
- `sandbox/ConsoleApplication/` - Sample .NET console application for testing

## Workflow Types and Commands

### Godot Project Workflows
- **Release**: `godot-project-release.yml` - Builds Android/iOS releases, publishes to Firebase/GitHub
- **Testing**: `godot-project-tests.yml` - Runs .NET unit tests and Godot-specific tests
- **Formatting**: `godot-project-format.yml` - Formats C# code using .NET formatting tools

Key requirements for Godot projects:
- `global.json` file with SDK version and Godot.NET.Sdk reference
- Project structure with `project.godot` file
- Separate test project for unit tests

### Unity Project Workflows
- **Release**: `unity-project-release.yml` - Builds Android/iOS releases with Unity Cloud Build
- Supports Firebase App Distribution and TestFlight publishing
- Requires Unity license credentials and platform-specific signing certificates

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
    "Godot.NET.Sdk": "4.3.0"  // Only for Godot projects
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

For testing multiple projects:
```yaml
uses: ./.github/workflows/package-tests.yml
with:
  dotnet-tests: '["tests/Core.Tests", "tests/Godot.Tests", "tests/Integration.Tests"]'
  global-json-file: "global.json"
```

## Development Notes

- All workflows are designed as reusable (`workflow_call`) for use by other repositories
- Package workflows use conditional job execution - only runs jobs for provided inputs
- Environment-specific builds support both `Development` and `Production` configurations
- Caching is enabled for main/develop branches to improve build performance
- Firebase publishing is only enabled for Development builds
- GitHub releases include downloadable build artifacts
- Use `tests.yml` to validate workflow changes against sandbox applications