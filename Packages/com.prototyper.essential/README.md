# Prototyper Essential Unity Package - Essential framework

The Prototyper Essential Unity Package provides a foundational framework for Unity developers, offering a collection of utilities, extensions, and systems to streamline development workflows. It is designed to enhance productivity and simplify common tasks in Unity projects.

### Key Features

- **Animator Utilities**: Simplify and extend Unity's Animator functionality.
- **Custom Attributes**: Add custom attributes to enhance Unity's inspector.
- **Dialog System**: Create and manage in-game dialogues with ease.
- **Audio System**: Manage and control audio playback with advanced features.
- **Text Table Utilities**: Work with localization and text tables for multilingual support.
- **Timeline Extensions**: Extend Unity's Timeline capabilities.
- **UI Utilities**: Simplify UI development with reusable components.
- **Visual Scripting Tools**: Enable visual scripting for non-programmers.

This package is part of the Prototyper suite, which includes additional tools and utilities to further enhance Unity development.


## Installation

To use this package in your Unity project:

1. Use NPM scope registry in Unity
    - Open the `Edit` menu in Unity.
    - Select `Project Settings`.
    - Go to the `Package Manager` section.
    - Add the following scope to the `Name` field: `Prototyper`. (You can use any name you prefer.)
    - Add the following URL to the `URL` field: `https://registry.npmjs.org/`.
    - Add the following scope(s) to the `Scopes` field:
      - `com.prototyper.utility` (dependency)
      - `com.prototyper.buildpipeline` (dependency)
      - `com.prototyper.essential`
2. Add the package to your project:
    - Open the `Window` menu in Unity.
    - Select `Package Manager`.
    - Add packages you need in `My Registries` section.

### Samples

This package includes the following samples:

- **Audio Visualization Example**: Demonstrates audio visualization techniques.
- **Async Await Example**: Shows how to use async/await in Unity.

You can import these samples via the Unity Package Manager.

## License

This package is licensed under the MIT License. See the [LICENSE](LICENSE/LICENSE.md) file for details.