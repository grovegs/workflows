# Workflows

[![Tests](https://github.com/grovegs/workflows/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/workflows/actions/workflows/tests.yml)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/workflows)](https://github.com/grovegs/workflows/releases/latest)

---

## Overview

A comprehensive collection of reusable GitHub Actions workflows designed specifically for game development pipelines. These workflows provide seamless automation for building, testing, formatting, and releasing Unity, Godot, and .NET projects across multiple platforms including Android, iOS, Windows, macOS, and Linux.

**Key Differentiators:**

- **Game Engine Focused**: Purpose-built workflows for Unity and Godot with deep integration support
- **Cross-Platform**: Native support for mobile (Android, iOS), desktop (Windows, macOS, Linux), and web platforms
- **Production Ready**: Battle-tested workflows with extensive error handling, caching, and retry logic
- **Unified Workflows**: Single workflows handle Godot and Unity projects with consistent inputs

**Ideal For:**

- Game studios automating their CI/CD pipeline
- Independent developers needing reliable build automation
- Teams requiring consistent cross-platform builds
- Projects targeting mobile platforms with automated distribution

---

## Features

- **Multi-Engine Support**: Seamless integration with Unity, Godot, and .NET projects
- **Mobile Builds**: Full Android and iOS build support with code signing
- **Automated Testing**: Run unit tests and validation across all platforms
- **Code Formatting**: Automatic formatting for C#, YAML, JSON, Markdown, and Shell scripts
- **Package Management**: Create NuGet packages, Unity packages, and Godot addons
- **Distribution**: Publish to Firebase, TestFlight, NuGet, and GitHub Releases
- **Smart Caching**: Intelligent caching for Unity, Godot, .NET, and build dependencies
- **Version Management**: Automatic version bumping and changelog generation

---

## Requirements

**GitHub Runners:**

- `ubuntu-latest` - For .NET, Android, and Godot Linux builds
- `macos-latest` - Required for iOS, macOS, and Godot addon builds

**Additional Requirements:**

- Secrets configured for signing (keystores, certificates, provisioning profiles)
- Unity license keys for engine-specific builds
- `global.json` file for .NET and Godot C# projects

---

## Installation

These workflows are designed to be used directly in your GitHub repositories. No installation is required - simply reference the workflows in your workflow files.

### Basic Usage

Add workflows to your `.github/workflows` directory:

```yaml
name: Release

on:
  push:
    branches: [main]

jobs:
  release:
    uses: grovegs/workflows/.github/workflows/project-release.yml@v1.0.0
    with:
      project-name: "My Game"
      godot-project: games/MyGame
      version-type: minor
      global-json-file: global.json
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      firebase-credentials: ${{ secrets.FIREBASE_CREDENTIALS }}
      firebase-app-id-android: ${{ secrets.FIREBASE_APP_ID_ANDROID }}
      android-keystore: ${{ secrets.ANDROID_KEYSTORE }}
      android-keystore-user: ${{ secrets.ANDROID_KEYSTORE_USER }}
      android-keystore-password: ${{ secrets.ANDROID_KEYSTORE_PASSWORD }}
```

---

## Quick Start

### Godot Game Project Example

Build and release a Godot project:

```yaml
name: Release Godot Game

on:
  workflow_dispatch:
    inputs:
      version-type:
        description: "Version bump type"
        required: true
        type: choice
        options:
          - major
          - minor
          - patch

permissions:
  contents: write

jobs:
  release:
    uses: grovegs/workflows/.github/workflows/project-release.yml@v1.0.0
    with:
      project-name: My Game
      godot-project: games/MyGame
      version-type: ${{ inputs.version-type }}
      global-json-file: global.json
      tester-groups: qa-team
      publish-github: true
      publish-firebase: true
      publish-discord: true
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      discord-webhook: ${{ secrets.DISCORD_WEBHOOK }}
      firebase-credentials: ${{ secrets.FIREBASE_CREDENTIALS }}
      firebase-app-id-android: ${{ secrets.FIREBASE_APP_ID_ANDROID }}
      firebase-app-id-ios: ${{ secrets.FIREBASE_APP_ID_IOS }}
      android-keystore: ${{ secrets.ANDROID_KEYSTORE }}
      android-keystore-user: ${{ secrets.ANDROID_KEYSTORE_USER }}
      android-keystore-password: ${{ secrets.ANDROID_KEYSTORE_PASSWORD }}
      ios-team-id: ${{ secrets.IOS_TEAM_ID }}
      ios-certificate: ${{ secrets.IOS_CERTIFICATE }}
      ios-certificate-password: ${{ secrets.IOS_CERTIFICATE_PASSWORD }}
      ios-provisioning-profile: ${{ secrets.IOS_PROVISIONING_PROFILE }}
      ios-api-key: ${{ secrets.IOS_API_KEY }}
      ios-api-key-id: ${{ secrets.IOS_API_KEY_ID }}
      ios-api-issuer-id: ${{ secrets.IOS_API_ISSUER_ID }}
```

### Unity Game Project Example

Build and release a Unity project:

```yaml
name: Release Unity Game

on:
  workflow_dispatch:
    inputs:
      version-type:
        required: true
        type: choice
        options: [major, minor, patch]

permissions:
  contents: write

jobs:
  release:
    uses: grovegs/workflows/.github/workflows/project-release.yml@v1.0.0
    with:
      project-name: My Unity Game
      unity-project: projects/MainGame
      version-type: ${{ inputs.version-type }}
      publish-github: true
      publish-firebase: true
      publish-testflight: true
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      unity-email: ${{ secrets.UNITY_EMAIL }}
      unity-password: ${{ secrets.UNITY_PASSWORD }}
      unity-license-key: ${{ secrets.UNITY_LICENSE }}
      firebase-credentials: ${{ secrets.FIREBASE_CREDENTIALS }}
      firebase-app-id-android: ${{ secrets.FIREBASE_APP_ID_ANDROID }}
      firebase-app-id-ios: ${{ secrets.FIREBASE_APP_ID_IOS }}
      android-keystore: ${{ secrets.ANDROID_KEYSTORE }}
      android-keystore-user: ${{ secrets.ANDROID_KEYSTORE_USER }}
      android-keystore-password: ${{ secrets.ANDROID_KEYSTORE_PASSWORD }}
      ios-team-id: ${{ secrets.IOS_TEAM_ID }}
      ios-certificate: ${{ secrets.IOS_CERTIFICATE }}
      ios-certificate-password: ${{ secrets.IOS_CERTIFICATE_PASSWORD }}
      ios-provisioning-profile: ${{ secrets.IOS_PROVISIONING_PROFILE }}
      ios-api-key: ${{ secrets.IOS_API_KEY }}
      ios-api-key-id: ${{ secrets.IOS_API_KEY_ID }}
      ios-api-issuer-id: ${{ secrets.IOS_API_ISSUER_ID }}
```

### .NET Package Example

Build and publish a NuGet package:

```yaml
name: Release Package

on:
  workflow_dispatch:
    inputs:
      version-type:
        required: true
        type: choice
        options: [major, minor, patch]

permissions:
  contents: write

jobs:
  release:
    uses: grovegs/workflows/.github/workflows/package-release.yml@v1.0.0
    with:
      package-name: My Library
      dotnet-project: src/Core
      version-type: ${{ inputs.version-type }}
      global-json-file: global.json
      publish-github: true
      publish-nuget: true
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      nuget-api-key: ${{ secrets.NUGET_API_KEY }}
```

### Godot Addon Package Example

Package and release a Godot addon:

```yaml
name: Release Godot Package

on:
  workflow_dispatch:
    inputs:
      version-type:
        required: true
        type: choice
        options: [major, minor, patch]

permissions:
  contents: write

jobs:
  release:
    uses: grovegs/workflows/.github/workflows/package-release.yml@v1.0.0
    with:
      package-name: My Godot Plugin
      dotnet-project: src/Core
      godot-addon: addons/my-plugin
      version-type: ${{ inputs.version-type }}
      global-json-file: global.json
      publish-github: true
      publish-nuget: true
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      nuget-api-key: ${{ secrets.NUGET_API_KEY }}
```

---

## Available Workflows

### Project Workflows

**project-release.yml** - Build and release game projects

- Supports: Godot and Unity projects
- Platforms: Android (APK/AAB), iOS (IPA)
- Publishing: Firebase, TestFlight, GitHub Releases
- Inputs:
  - `project-name` - Project name
  - `version-type` - major, minor, or patch
  - `godot-project` - Path to Godot project
  - `unity-project` - Path to Unity project
  - `global-json-file` - Path to global.json
  - `tester-groups` - Firebase tester groups
  - `build-android` - Enable Android builds
  - `build-ios` - Enable iOS builds
  - `publish-github` - Publish to GitHub
  - `publish-firebase` - Publish to Firebase
  - `publish-testflight` - Publish to TestFlight
  - `publish-discord` - Send Discord notifications

**project-tests.yml** - Run tests for game projects

- Inputs:
  - `dotnet-project` - Path to .NET test project
  - `godot-project` - Path to Godot project
  - `unity-project` - Path to Unity project
  - `global-json-file` - Path to global.json

**project-format.yml** - Format game project code

- Inputs:
  - `files` - File patterns to format using Prettier
  - `dotnet-project` - Path to .NET project
  - `global-json-file` - Path to global.json

### Package Workflows

**package-release.yml** - Build and publish packages

- Supports: .NET projects, Godot addons, Unity packages
- Publishing: NuGet, GitHub Releases
- Inputs:
  - `package-name` - Package name
  - `version-type` - major, minor, or patch
  - `dotnet-project` - Path to .NET project
  - `godot-addon` - Path to Godot addon
  - `unity-package` - Path to Unity package
  - `global-json-file` - Path to global.json
  - `directory-build-props` - Path to Directory.Build.props
  - `publish-github` - Publish to GitHub
  - `publish-nuget` - Publish to NuGet
  - `publish-discord` - Send Discord notifications

**package-tests.yml** - Run tests for packages

- Inputs:
  - `dotnet-project` - Path to .NET test project
  - `global-json-file` - Path to global.json

**package-format.yml** - Format package code

- Inputs:
  - `files` - File patterns to format using Prettier
  - `dotnet-solution` - Path to .NET solution directory
  - `global-json-file` - Path to global.json

---

## Advanced Usage

### Conditional Platform Builds

Disable specific platforms:

```yaml
jobs:
  release:
    uses: grovegs/workflows/.github/workflows/project-release.yml@v1.0.0
    with:
      project-name: My Game
      godot-project: games/MyGame
      version-type: patch
      build-android: true
      build-ios: false
```

### Selective Publishing

Choose specific publishing targets:

```yaml
jobs:
  release:
    uses: grovegs/workflows/.github/workflows/project-release.yml@v1.0.0
    with:
      project-name: My Game
      godot-project: games/MyGame
      version-type: minor
      publish-firebase: true
      publish-testflight: false
```

### Package with Godot Addon

```yaml
jobs:
  release:
    uses: grovegs/workflows/.github/workflows/package-release.yml@v1.0.0
    with:
      package-name: My SDK
      dotnet-project: src/Core
      godot-addon: addons/my-plugin
      unity-package: Packages/com.example.package
      version-type: minor
      global-json-file: global.json
```

### Automated Testing on Pull Requests

```yaml
name: Test

on:
  pull_request:
    branches: [main, develop]

jobs:
  test:
    uses: grovegs/workflows/.github/workflows/project-tests.yml@v1.0.0
    with:
      godot-project: games/MyGame
      dotnet-project: tests/MyGame.Tests
      global-json-file: global.json
```

### Code Formatting on Push

```yaml
name: Format

on:
  push:
    branches: [main, develop]

permissions:
  contents: write

jobs:
  format:
    uses: grovegs/workflows/.github/workflows/project-format.yml@v1.0.0
    with:
      dotnet-project: src/MyProject
      global-json-file: global.json
```

---

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
    "Godot.NET.Sdk": "4.3.0"
  }
}
```

### Unity Build Profiles

Unity projects must use Build Profiles for each platform and environment:

- `Android Development`
- `Android Production`
- `iOS Development`
- `iOS Production`

### Godot Export Presets

Godot projects must have export presets configured in `export_presets.cfg`:

- `Android Development`
- `Android Production`
- `iOS Development`
- `iOS Production`

---

## Workflow Outputs Pattern

All workflows follow a consistent pattern:

### Project Workflow Pattern

1. **Prepare Release** - Bump version and generate changelog
2. **Build Projects** - Build each project for each platform using matrix strategy
3. **Upload Artifacts** - Upload build files to GitHub artifacts
4. **Publish** - Distribute builds to Firebase/TestFlight and create GitHub release
5. **Notify** - Send Discord notifications

### Package Workflow Pattern

1. **Prepare Release** - Bump version and generate changelog
2. **Pack Projects** - Build and package each project using matrix strategy
3. **Upload Artifacts** - Upload packages to GitHub artifacts
4. **Publish** - Publish packages to NuGet and create GitHub release
5. **Commit Changes** - Commit version changes back to repository
6. **Notify** - Send Discord notifications

---

## Troubleshooting

### Common Issues

#### Android build fails with keystore error

Keystore must be base64 encoded:

```bash
base64 -i your-keystore.jks | pbcopy  # macOS
base64 -i your-keystore.jks          # Linux
```

Then add as a secret: `ANDROID_KEYSTORE`

---

#### iOS build fails with provisioning profile error

Missing or incorrect provisioning profile configuration.

Ensure all iOS secrets are properly configured:

```yaml
IOS_TEAM_ID: Your Apple Developer Team ID
IOS_CERTIFICATE: Base64-encoded .p12 certificate
IOS_CERTIFICATE_PASSWORD: Certificate password
IOS_PROVISIONING_PROFILE: Base64-encoded .mobileprovision file
```

For TestFlight uploads, also configure:

```yaml
IOS_API_KEY: Base64-encoded App Store Connect API .p8 key
IOS_API_KEY_ID: API Key ID
IOS_API_ISSUER_ID: API Issuer ID
```

---

#### Godot addon packaging fails on Linux runner

Godot addon packaging requires macOS runner.

The `pack-godot-addons` job automatically runs on `macos-latest`. No action needed.

---

#### Version not bumping correctly

Missing or incorrect Git tags.

Ensure your repository has at least one Git tag in semantic versioning format:

```bash
git tag v0.1.0
git push origin v0.1.0
```

---

#### Cache not working or builds are slow

Caching is automatically enabled for main/develop branches. Ensure your branch name matches:

```yaml
cache: ${{ github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop' }}
```

---

### Getting More Help

If you encounter issues not listed here:

1. Check [existing issues](https://github.com/grovegs/workflows/issues)
2. Open a [new issue](https://github.com/grovegs/workflows/issues/new) with:
   - Workflow name and version
   - Complete workflow file
   - Error logs
3. Contact us at [support@grove.gs](mailto:support@grove.gs)

---

## Contributing

Contributions are welcome! We appreciate your help in making this project better.

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Write or update tests in `tests.yml`
5. Test against sandbox applications
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Contribution Guidelines

- Follow existing code style and conventions
- Test workflows against sandbox applications
- Update documentation as needed
- Keep commits focused and atomic
- Write clear commit messages following [Conventional Commits](https://www.conventionalcommits.org/)
- Ensure bash scripts are compatible with bash 3.2+ (macOS compatibility)

### Development Setup

```bash
# Clone the repository
git clone https://github.com/grovegs/workflows.git

# Navigate to directory
cd workflows

# Test workflows using sandbox applications
gh workflow run tests.yml
```

### Testing Workflows

All workflow changes must be tested using the sandbox applications:

- `sandbox/GodotApplication/` - Godot C# project
- `sandbox/ConsoleApplication/` - .NET console application

The `tests.yml` workflow validates all workflows automatically.

---

## Support

- 🐛 [Issue Tracker](https://github.com/grovegs/workflows/issues) - Report bugs or request features
- 📧 [support@grove.gs](mailto:support@grove.gs) - Email support
- 🌐 [grove.gs](https://grove.gs) - Official website

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
