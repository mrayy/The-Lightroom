# The-Lightroom
![anim1](http://myamens.com/Uploads/TLR/TLR-intro.gif)

This is an expermintal project created for the application process at Teamlab Japan.
The concept of this room is the psudeo-immersion of visuals, sensory and interaction into a single space. This work aims to remove the boundries between the physical and virtual space, and to create an extension of the human interaction into that virtual space.

The Lightroom consists of the following elements:
- Attractors: audio/light emitters that can be added by the user.
- Walls: reflects the behaviour of the room based on the the Attractors motion and colors. The shape of the room deforms following the attraction of the Attractors.
- Particles: emitted from the walls represented the effect of attraction towards the Attractors.

![anim1](http://myamens.com/Uploads/TLR/TLR-particles.gif)

Interactions:
----------
The perspective of the rooms is changed based on user's head position, which is tracked using a webcamera. The rendering is done using off-axis projection, simulating looking-through a window. 
The user can interact with the lightroom by using his hands motion and gestures. Pinching allows the user can add new attractors, and a long pinch to remove an existing attractor in the room. 


Requirements:
--------
- Leapmotion (In case not available, then set property value "NoLeapmotion" to "true" in the settings file found in: Data\Settings.ini)
- USB Webcamera: Camera index should be set by changing "Index" value in the settings file found in: Data\Settings.ini
   (In case not available, then set property value "NoWebcam" to "true" in the settings file found in: Data\Settings.ini)
- Projector (Optional, better for immersion)

Setup:
-----
Place the webcamera and leapmotion on front of the user at a table. The webcamera should be pointing to the user's face. 
If projector was used, then the user's face should be lit enough so the camera can detect his face. To check if the camera is tracking correctly, press F9 on keyboard to show debug information. The face icon on the bottom left corner should appear too if the face is being detected. 

Tested using the following PC: OS Windows 10 Pro, Intel(R) Core(TM) i7-6700, 16GB RAM, NVidia GeForce GTX 1080

Running:
-----
You can download a release version from github:
https://github.com/mrayy/The-Lightroom/releases
or you can run it in Unity3D (tested on Unity3D 5.6.2f1).

How it works:
-------
- The-Lightroom starts by calibrating the placement of the leapmotion using 3 points calibration. The user calibrates by pointing into the points and pinching his hand. If the calibration successeded, then the user should be able to point freely in the 2D space.
- The webcam tracks user's face in order to adjust camera's location. The generated effect is an off-axis projection.

Plugins Used:
--------
- UnityOpenCV: https://github.com/mrayy/UnityOpencv
- Leapmotion SDK


External repos:
--------
- Keijiro Takahashi's handful math utilities: https://github.com/keijiro  (DTween, Perlin)
- Keijiro Takahashi's Audio Spectrum utility: https://github.com/keijiro/unity-audio-spectrum
