<p align="center">
  <img src="./images/logo.png" />
</p>

## OZMap VR Version Experimental Branch 

This branch contains changes to Coreality implemented as part of the [OZMap project](https://github.com/WearableComputerLab/OZMap/):

- Extension of the HoloAvatar to enable representation of either XR controllers or tracked hands. 
- Addition of a "AvatarNetBehaviour" component system enabling custom modular network behaviour on the HoloAvatar GameObject.
- Map based behaviours from OZMap included for reference (requires OZMap to compile):
  - **MapAvatarBehaviour**: Applies custom positioning/scaling of the HoloAvatar based on each user's relative map position/zoom. 
  - **MapPointerAvatarBehaviour**: Syncs remote HoloAvatar hand ControllerPointer components with the user's local ControllerPointers (laser pointer function). 
  - **MapAnnotationAvatarBehaviour**: Syncs user's local AnnotationModule state so that freehand annotation drawings are synced across each client. Annotation line segments are anchored to the map and are map position/zoom relative.
- Prefabs:
  - VR Holo Avatar
  - VR Left/Right Controllers
  - VR Remote Left/Right Controllers
  - Drawing Segment (AnnotationModule)
  - Ground Indicator
  - Controller Pointer

---

## Requirements

Unity version 2019.4 LTS or greater

This project was built on the 2019.4 LTS, if you want to ensure 100% compatability, use that one.

MRTK version 2.7.2 +

Download and import all the packages from the [latest realease of MRTK](https://github.com/microsoft/MixedRealityToolkit-Unity/releases)

Vuforia 9.8.8 +

Download and import the [latest version of vuforia for unity](https://developer.vuforia.com/downloads/SDK)

Photon 2

Download and add the [PUN 2 - Free asset from the asset store](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)

# Setup

### Install MRTK + Vuforia + Photon

[MRTK + Vuforia Tutorial](https://library.vuforia.com/articles/Solution/Working-with-the-HoloLens-sample-in-Unity.html)


## Setting up HoloAvatar rigged hands

TBD

## Example Scenes

### 1 Networked Object

Simple scene that demonstrates spawning and manipulation of a networked object across a photon network.

### 2 Hololens Avatars

Demonstrates the Avatar system which allows users to see each other in virtual space. Users will be able to see other player's heads and hands including their current hand pose. The scene also demonstrates use of stencil shaders on the user's hands so they only appear 'infront' of other objects.

### 3 Desktop and Hololens

Demonstrates a multi-scene project that lets desktop and hololens builds communicate with eachother. 

#### To run

Put all three scenes in the build output, the first scene `Startup` will automatically detect the target platform and load the Hololens or Desktop scene accordingly. Desktop users can use the middle mouse button to rotate the scene.

### 4 Hololens Mini Me

*WIP*

Demonstrates the ability to scale player size in realtime to make the scene appear bigger or smaller.

## Modules

### Network Module

Module for connecting / disconnecting from the photon server.

### Avatar Module

Module for creating user avatars and syncronizing them between all users.

