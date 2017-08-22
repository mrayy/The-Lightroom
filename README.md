# The-Lightroom
![anim1](http://myamens.com/Uploads/TLR/TLR-intro.gif)

This is an expermintal project created for the application process at Teamlab Japan.
The concept of this room is the psudeo-immersion of visuals, sensory and interaction into a single space. The room consists of basic elements:
- Affectors: audio/light generators that can be added by the user.
- Walls: reflects the behaviour of the room based on the the affectors motion and colors. The shape of the room deforms following the attraction of the affectors.
- Particles: emitted from the walls represented the effect of attraction towards the affectors.

![anim1](http://myamens.com/Uploads/TLR/TLR-particles.gif)

The perspective of the rooms is changed based on user's head position, which is tracked using a webcamera. The rendering is done using off-axis projection, simulating looking-through a window. 

Requirements:
--------
- Leapmotion
- USB Webcamera: Camera index should be set by changing "Index" value in the settings file found in: Data\Settings.ini
- Projector (Optional)

Tested using the following PC: OS Windows 10 Pro, Intel(R) Core(TM) i7-6700, 16GB RAM, NVidia GeForce GTX 1080

Plugins Used:
--------
- UnityOpenCV: https://github.com/mrayy/UnityOpencv
- Leapmotion SDK


External repos:
--------
- Keijiro Takahashi's handful math utilities: https://github.com/keijiro  (DTween, Perlin)
- Keijiro Takahashi's Audio Spectrum utility: https://github.com/keijiro/unity-audio-spectrum
