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

## Cross-Platform Compatibility

### macOS bash Compatibility

GitHub Actions macOS runners use bash 3.2.x (from 2006), which lacks many modern bash features. Always write scripts compatible with bash 3.2+.

#### Critical Compatibility Issues

#### ❌ mapfile (bash 4.0+) - NOT available on macOS

```bash
# ❌ WRONG - Will fail on macOS
mapfile -t ARRAY <<< "${INPUT}"

# ✅ CORRECT - Works on both Linux and macOS
declare -a ARRAY
while IFS= read -r line; do
  ARRAY+=("${line}")
done <<< "${INPUT}"
```

#### ❌ readarray (bash 4.0+) - NOT available on macOS

```bash
# ❌ WRONG - Will fail on macOS
readarray -t ARRAY <<< "${INPUT}"

# ✅ CORRECT - Use while-read loop instead
declare -a ARRAY
while IFS= read -r line; do
  ARRAY+=("${line}")
done <<< "${INPUT}"
```

#### ❌ Associative arrays (bash 4.0+) - NOT available on macOS

```bash
# ❌ WRONG - Will fail on macOS
declare -A MAP
MAP["key"]="value"

# ✅ CORRECT - Use indexed arrays or alternative approaches
declare -a KEYS
declare -a VALUES
KEYS+=("key")
VALUES+=("value")
```

#### ❌ shopt nullglob (bash 4.0+) - NOT available on macOS

```bash
# ❌ WRONG - Will fail on macOS
shopt -s nullglob
files=(*.txt)
shopt -u nullglob

# ✅ CORRECT - Use find with explicit handling
files=$(find . -maxdepth 1 -name "*.txt" 2>/dev/null | tr '\n' ' ' || echo "")
files=$(echo "${files}" | sed 's/[[:space:]]*$//')
```

#### ❌ ${var@Q} (bash 4.4+) - NOT available on macOS

```bash
# ❌ WRONG - Will fail on macOS
echo "${VAR@Q}"

# ✅ CORRECT - Use printf for quoting
printf '%q\n' "${VAR}"
```

#### Safe bash 3.2+ Features

#### ✅ Indexed arrays - SAFE to use

```bash
declare -a ITEMS
ITEMS+=("item1")
ITEMS+=("item2")
echo "${ITEMS[0]}"
echo "${#ITEMS[@]}"
```

#### ✅ Command substitution - SAFE to use

```bash
RESULT=$(command)
RESULT=`command`  # older style, but works
```

#### ✅ Parameter expansion - SAFE to use

```bash
${VAR:-default}
${VAR#pattern}
${VAR%pattern}
${VAR/pattern/replacement}
${VAR/#pattern/replacement}  # Replace at beginning
```

#### ✅ Process substitution - SAFE to use

```bash
while IFS= read -r line; do
  echo "${line}"
done < <(command)
```

### Path Handling Best Practices

#### Using HOME vs Tilde

#### In Shell Scripts - Use `${HOME}`

```bash
#!/usr/bin/env bash

# ✅ RECOMMENDED - Explicit and reliable
CACHE_DIR="${HOME}/.cache"
BUILD_DIR="${HOME}/.local/build"

# ❌ AVOID - Context-dependent expansion
CACHE_DIR="~/.cache"
```

#### In YAML Action Inputs - Use `~`

```yaml
# ✅ CORRECT - Action will expand tilde internally
- uses: ./.github/actions/upload-artifact
  with:
    path: |
      workspace-files/**
      ~/.nupkgs/package.nupkg
```

#### Expanding Tilde in Scripts

```bash
# When receiving path from user input, expand tilde immediately
USER_PATH="${USER_INPUT:-}"
USER_PATH="${USER_PATH/#\~/$HOME}"
```

#### Why Use ${HOME} in Scripts

1. **Explicit and Clear**: `"${HOME}/.nupkgs"` is immediately obvious
2. **Consistent Expansion**: Works in all contexts (quotes, assignments, etc.)
3. **Variable Substitution Safe**: Works with parameter expansion
4. **POSIX Compliant**: Defined in POSIX standard
5. **No Context Surprises**: Always expands, regardless of quoting

### Workspace Path Handling

Actions must handle files both inside and outside the workspace properly.

#### Understanding Workspace Boundaries

#### Inside workspace

- Files in `${GITHUB_WORKSPACE}` and subdirectories
- Source code, configuration files
- Can use relative paths for metadata tracking

#### Outside workspace

- Build artifacts in `${HOME}/.nupkgs`, `${HOME}/.godot/addons`, `${HOME}/.unity/packages`
- Temporary files in `${RUNNER_TEMP}`
- Cached dependencies in `${HOME}/.cache`, `${HOME}/.nuget`
- Must use absolute paths, no metadata tracking needed

#### Correct Path Handling Pattern

```bash
#!/usr/bin/env bash
set -euo pipefail

WORKSPACE="${GITHUB_WORKSPACE}"
STAGING_DIR="${WORKSPACE}/.staging"

# Expand tilde if present
path_pattern="${path_pattern/#\~/$HOME}"

for item in ${path_pattern}; do
  if [ -f "${item}" ]; then
    ITEM_DIR=$(cd "$(dirname "${item}")" && pwd)
    ITEM_NAME=$(basename "${item}")
    ABS_PATH="${ITEM_DIR}/${ITEM_NAME}"
  elif [ -d "${item}" ]; then
    ABS_PATH=$(cd "${item}" && pwd)
  else
    continue
  fi

  # Check if file is inside workspace
  if [[ "${ABS_PATH}" == "${WORKSPACE}"* ]]; then
    # Inside workspace - preserve directory structure
    REL_PATH="${ABS_PATH#"${WORKSPACE}"/}"
    TARGET_DIR="${STAGING_DIR}/$(dirname "${REL_PATH}")"
    mkdir -p "${TARGET_DIR}"
    cp -p "${ABS_PATH}" "${STAGING_DIR}/${REL_PATH}"
    echo "Copied (workspace): ${REL_PATH}"
  else
    # Outside workspace - copy to staging root with filename only
    FILENAME=$(basename "${ABS_PATH}")
    cp -p "${ABS_PATH}" "${STAGING_DIR}/${FILENAME}"
    echo "Copied (external): ${FILENAME}"
  fi
done
```

#### Why This Matters

#### Package/Build Actions

- Create artifacts outside workspace (`${HOME}/.nupkgs`, `${HOME}/.godot/addons`, etc.)
- Keeps workspace clean and prevents accidental commits
- Artifacts are meant to be uploaded, not tracked in git

#### Upload/Download Actions

- Must handle both workspace and external files
- Workspace files need path restoration (for modified source files)
- External files are uploaded as-is (packages, builds)

### Platform-Specific Commands

#### File Statistics

```bash
# ❌ WRONG - Linux-specific
FILE_SIZE=$(stat -c%s "${FILE}")

# ✅ CORRECT - Cross-platform
FILE_SIZE=$(stat -f%z "${FILE}" 2>/dev/null || stat -c%s "${FILE}" 2>/dev/null || echo "unknown")
```

#### Date Commands

```bash
# ❌ WRONG - GNU date specific
DATE=$(date -d "yesterday" +%Y-%m-%d)

# ✅ CORRECT - POSIX compatible
DATE=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
```

#### sed In-Place Editing

```bash
# ✅ CORRECT - Portable across Linux and macOS
sed -i.bak "s/old/new/g" "${FILE}"
rm -f "${FILE}.bak"
```

### Testing Compatibility

#### Always test scripts on the target platform

```yaml
jobs:
  test-linux:
    runs-on: ubuntu-latest
    steps:
      - run: ./scripts/my_script.sh

  test-macos:
    runs-on: macos-latest
    steps:
      - run: ./scripts/my_script.sh
```

#### Use shellcheck with POSIX mode

```bash
# Check for POSIX compatibility issues
shellcheck -s bash -e SC2039 script.sh

# Or check for bash 3.2 compatibility
shellcheck -s bash script.sh
```

### Compatibility Checklist

- [ ] No `mapfile` or `readarray` commands
- [ ] No associative arrays (`declare -A`)
- [ ] No `shopt` commands (especially `shopt -s nullglob`)
- [ ] No bash 4+ features (check with `shellcheck`)
- [ ] Use `while read` loop for array population
- [ ] Use `"${HOME}"` in shell scripts, not `~`
- [ ] Expand tilde from inputs: `path="${path/#\~/$HOME}"`
- [ ] Handle files both inside and outside workspace
- [ ] Use portable `stat` command with fallback
- [ ] Use portable `sed -i.bak` with cleanup
- [ ] Test on both Ubuntu and macOS runners
- [ ] Use `#!/usr/bin/env bash` not `#!/bin/bash`
- [ ] Quote all variables: `"${VAR}"` not `$VAR`

### Common Compatibility Errors and Solutions

| Error                                            | Cause                           | Solution                                         |
| ------------------------------------------------ | ------------------------------- | ------------------------------------------------ |
| `mapfile: command not found`                     | bash 3.2 on macOS               | Use `while read` loop                            |
| `declare: -A: invalid option`                    | Associative arrays (bash 4+)    | Use indexed arrays                               |
| `shopt: command not found`                       | bash 3.2 on macOS               | Use `find` with explicit handling                |
| `Skipping item outside workspace`                | File path validation too strict | Handle external paths separately                 |
| `stat: illegal option -- c`                      | GNU stat on macOS               | Add fallback: `stat -f%z ... \|\| stat -c%s ...` |
| `line 31: local: can only be used in a function` | `local` at script level         | Use `declare` instead                            |
| `~: No such file or directory`                   | Tilde not expanded              | Use `"${HOME}"` or expand: `${var/#\~/$HOME}`    |

### Reference: bash Version Features

#### bash 3.2 (macOS default)

- Indexed arrays
- Basic parameter expansion
- Process substitution
- Command substitution
- Functions with `local` variables

#### bash 4.0+ (NOT on macOS)

- `mapfile`/`readarray`
- Associative arrays
- `shopt -s nullglob`
- `;&` and `;;&` in case statements
- `**` globstar

#### bash 4.4+ (NOT on macOS)

- `${var@Q}` quoting
- Enhanced `mapfile` options
- Parameter transformation operators

**Always target bash 3.2 for maximum compatibility.**

### Summary: Path Handling Quick Reference

| Context                | Best Practice           | Example                         |
| ---------------------- | ----------------------- | ------------------------------- |
| Shell script variables | `"${HOME}"`             | `BUILD_DIR="${HOME}/.cache"`    |
| YAML action inputs     | `~`                     | `path: ~/.nupkgs/package.nupkg` |
| Expanding input paths  | `${var/#\~/$HOME}`      | `path="${path/#\~/$HOME}"`      |
| Workspace paths        | `"${GITHUB_WORKSPACE}"` | `SRC="${GITHUB_WORKSPACE}/src"` |
| Temp paths             | `"${RUNNER_TEMP}"`      | `TMP="${RUNNER_TEMP}/build"`    |
