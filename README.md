# unity-panini-camera-utils

Fixes ScreenPointToRay method when using Panini post process.

# Installation
Download the csharp file and place in your scripts directory.

# Usage
Instead of using Camera.ScreenPointToRay use Camera.ScreenPointToRayPanini, you will need to pass in the distance and cropToFit float properties.
