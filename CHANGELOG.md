# Changelog
All notable changes to this project will be documented in this file using the standards as defined at [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0).

### Version 1.0.2 *(2024-09-19)*
Improvements:
- Modified the `MainThreadDispatcher` initializer `RuntimeInitializeOnLoadMethod` from `BeforeSceneLoad` to `SubsystemRegistration` to initialize as early as possible.

### Version 1.0.1 *(2024-08-01)*

Improvements:
- `MainThreadDispatcher` better exception handling
- Utilizing centralized logging for better log handling. 

### Version 1.0.0 *(2024-01-26)*

First version of Chartboost Threading Utilities package for Unity.

Improvements:
- Added `MainThreadDispatcher` class.
- Added `TaskExtensionTest` class.
- Structured project as a UPM compatible package.
