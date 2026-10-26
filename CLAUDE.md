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

Unified workflows for both Godot and Unity game projects:

- **Release**: `project-release.yml` - Builds Android/iOS releases, publishes to Firebase/GitHub/TestFlight
- **Testing**: `project-tests.yml` - Runs .NET unit tests and engine-specific tests
- **Formatting**: `project-format.yml` - Formats C# code and project files

Each workflow handles a single project; use a matrix in the caller for several:

- `godot-project` - Godot project path containing a project.godot file
- `unity-project` - Unity project path containing a ProjectSettings folder
- `dotnet-project` - .NET test project path containing a .csproj file

Key requirements:

- **Godot projects**: `global.json` file with SDK version and Godot.NET.Sdk reference, `project.godot` file
- **Unity projects**: Unity license credentials and platform-specific signing certificates
- Separate test projects for unit testing

### Package Workflows

- **Release**: `package-release.yml` - Builds .NET, Godot and Unity packages, publishes to GitHub Packages, nuget.org and GitHub releases
- **Testing**: `package-tests.yml` - Runs .NET unit tests
- **Formatting**: `package-format.yml` - Formats C# code for .NET and Godot projects

Each workflow handles a single package:

- `dotnet-project` - .NET project path to build, format, package or test
- `godot-addon` - Godot addon path to package
- `unity-package` - Unity package path to package

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

Each workflow handles a single project. Development and Production are chosen by the caller through the `publish-*` flags:

```yaml
uses: grovegs/workflows/.github/workflows/project-release.yml@v1
with:
  project-name: "My Game"
  godot-project: "projects/Game"
  version-type: "minor"
  global-json-file: "global.json"
  publish-firebase: ${{ inputs.environment == 'Development' }}
  publish-testflight: ${{ inputs.environment == 'Production' }}
```

For testing a project:

```yaml
uses: grovegs/workflows/.github/workflows/project-tests.yml@v1
with:
  dotnet-project: "tests/Game.Tests"
  godot-project: "projects/Game"
  global-json-file: "global.json"
```

### Using Package Workflows

Development releases go to GitHub Packages, Production releases go to nuget.org. Consumers list both feeds in their `nuget.config`, so they can restore either. The caller must grant `packages: write` for GitHub Packages:

```yaml
jobs:
  release:
    uses: grovegs/workflows/.github/workflows/package-release.yml@v1
    permissions:
      contents: write
      packages: write
    with:
      package-name: "My Package"
      dotnet-project: "src/MyPackage"
      version-type: ${{ inputs.version-type }}
      global-json-file: "global.json"
      publish-github-packages: ${{ inputs.environment == 'Development' }}
      publish-nuget: ${{ inputs.environment == 'Production' }}
      publish-github: ${{ inputs.environment == 'Production' }}
    secrets:
      github-token: ${{ secrets.GITHUB_TOKEN }}
      nuget-api-key: ${{ secrets.NUGET_API_KEY }}
```

Consumer `nuget.config`. Package source mapping limits the `github` feed to `GroveGames.*`; list `GroveGames.*` under nuget.org too, because the most specific pattern wins and released versions live there. Every workflow exports `GITHUB_TOKEN`, so restores from GitHub Packages work in any repo without extra setup; locally, set `GITHUB_TOKEN` to a classic token with `read:packages`:

```xml
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/grovegs/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="nuget.org">
      <package pattern="*" />
      <package pattern="GroveGames.*" />
    </packageSource>
    <packageSource key="github">
      <package pattern="GroveGames.*" />
    </packageSource>
  </packageSourceMapping>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="%GITHUB_ACTOR%" />
      <add key="ClearTextPassword" value="%GITHUB_TOKEN%" />
    </github>
  </packageSourceCredentials>
</configuration>
```

For testing a package:

```yaml
uses: grovegs/workflows/.github/workflows/package-tests.yml@v1
with:
  dotnet-project: "tests/MyPackage.Tests"
  global-json-file: "global.json"
```

## Development Notes

- All workflows are designed as reusable (`workflow_call`) for use by other repositories
- Each workflow handles a single project or package; jobs only run for the inputs that are provided
- Development and Production are selected by the caller through `publish-*` flags, not a dedicated input
- Projects: Firebase for Development, TestFlight for Production
- Packages: GitHub Packages for Development, nuget.org for Production
- Caching is only used on GitHub-hosted runners; self-hosted runners reuse locally installed tools
- GitHub releases include downloadable artifacts from all successful builds
- Use `tests.yml` to validate workflow changes against sandbox applications

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
