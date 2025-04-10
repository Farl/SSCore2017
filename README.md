# Prototyper Essenial Unity Packages

This repository contains the some useful packages, which provides essential tools and utilities for prototyping in Unity Engine.


## Packages Included
The following packages are included in this repository:

```
/com.prototyper.utility/
/com.prototyper.buildpipeline/
/com.prototyper.essential/
/com.prototyper.packagehelper/      Create and manage Unity packages easily
/com.prototyper.tools/
/com.prototyper.xrsetup/            XR setup (for Vive Focus and Meta Quest) utilities
```

## Installation

To use this package in your Unity project:

1. Use NPM scope registry in Unity
    - Open the `Edit` menu in Unity.
    - Select `Project Settings`.
    - Go to the `Package Manager` section.
    - Add the following scope to the `Name` field: `Prototyper`. (You can use any name you prefer.)
    - Add the following URL to the `URL` field: `https://registry.npmjs.org/`.
    - Add the following scope(s) to the `Scopes` field:
      - `com.prototyper.utility`
      - `com.prototyper.buildpipeline`
      - `com.prototyper.essential`
      - `com.prototyper.tools`
      - `com.prototyper.xrsetup`
      - `com.prototyper.packagehelper`
2. Add the package to your project:
    - Open the `Window` menu in Unity.
    - Select `Package Manager`.
    - Add packages you need in `My Registries` section.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE/LICENSE.md) file for details.