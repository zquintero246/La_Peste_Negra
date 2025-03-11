=====================================================
	Hedgehog Arts' GPU Mode 7
   Bringing old-school fx into modern gaming!	
=====================================================

Thank you developer for downloading this pack. It's nice
to see that someone else can find a use for this special
set of scripts and shaders I made to see if I could do it.

Please note that any included assets for demonstration purposes
are the intellectual property of Hedgehog Arts. (FuzzyQuills on
the Unity3D forums) there are two; a track surface and a 
car sprite sheet.

For making your own, I'll have some special guidelines and recommendations
to make sure you get the best experience with this pack :)

Let's get started on basic setup:
- first, unzip the pack to your project's assets folder.
- Next, create a new camera, and set it up as follows:
	- 80 degree FOV. Required to keep sprite "positions" consistent.
		(I may be adding an automatic FOV calculation in future to 
		make this step redundant)
	- near plane: 0.3, far plane: 500
	- Solid clear colour (the backdrop rendering is handled in the next step)

NOTE: The following steps may be completely eliminated in the future by moving to a 
full-screen blitting system. This will make it much easier to setup a scene, as hand-setting up a quad can be tedious.
- Goto GameObject > 3D Object > Quad, and parent the new quad to the camera.
	- Set the quad translation to (0, 0, 0.32)
	- Make a new Material, and specify the Hedgehog Arts/Mode7_Bg shader, and assign it to the Quad
	- On the camera itself, add a Mode7 component.
	- On the mode7 component, assign the map texture you're going to use to the Map slot,
	  and assign the quad to Mode7_Layer
	- Set the FOV value to 2.4, and totalScale to 0.019

Script is setup, rendering system setup... now for ease of editing, we're going to do the next steps:
- goto GameObject > 3D Object > Plane, center the plane on Unity's origin, and set the X and Z scale to 51.2
- Create a new material with the Hedghehog Arts/General_Bg shader, and assign your map texture to the texture slot
- Assign the material to the plane. Now you have a visible track to use as a reference when making maps. :)
  To play the game, disable the General_Bg object, and hit play to test. 
	(it can be left on, but to avoid artifacts, its better to turn it off first)

And there's basic setup done.
To add a background, just repeat the process, but instead of setting map, go to the material itself and set map and backdrop explicity.
There can be more than one Mode7 object on a camera. 
Because the process of calculating sin and cos can be CPU heavy, refrain from 
using insande amounts of layers in your scene... please. :D

For a free-standing sprite...
- Make two new Empty GameObjects, and assign a SpriteRenderer to one of them. Parent the sprite to the other empty
- On the spriteRenderer, setup a material with the Hedgehog Arts/Mode7_Spr shader.
- add the SpriteDraw script to the spriterenderer, and set spr to the spriterenderer on it.
- Set Order In Layer to whichever works, it's mostly for exposing the order to scripting. 
  Example: making it look like a car has fallen out of the world by sitting the sprite layer behind the Mode7 layer in a script.
- You may have noticed the "Is Multi Sprite" boolean. This will be explained in the next section ;)

Now, onto getting sprites working like Super Mario Kart/F-ZERO...

This requires that a special sprite sheet with 16 images be made. 
(technically, it can be lower or higher, but 16 is a good happy medium between texture size and number of frames)
As an example, I'll be using a car. The example pre-render was made in blender using 
360/16 as the angle difference between frames.

Essentially, image 0 is the back view, image 9 is the front view. the images in between 
"rotate" the car at various angles.

To setup a UI/2D sprite for use with the "is multi sprite" parameter:
- First, set your texture as a UI/Sprite, and set the mode to multiple.
  I use 1024x64 but other sizes can be used, just make sure there is 16 images in a long row viewing the 
  car/object from various angles.
- In the sprite packer, select "slice"
- Set slicing mode to "grid by cell size" and set the size to 64x64. Leave the other values as is.
- Select "Slice" and now under your sprites main asset should be 16 sub-images. Click apply in the sprite editor and exit
- Now, on your Mode7_Spr, tick Is Multi Sprite
- Open the frames array, set the size to 16, and assign your frames to the array, making sure they're in order
- You're done, now in the editor viewport you should see a sprite "rotating" as your rotate it or the editor view. :)

Any questions? Just send me a PM on the Unity forum, or email me (neubot321@gmail.com)


Requirements:
- CPU features: whatever Unity needs, this isn't that much of a heavyweight :)
- GPU features:
	- Support stencil buffer (needed for sprite/BG draw ordering)
	- Support at least shader model 2.0 (GMA950 users aren't left out! This depends on Unity support however)
- Unity 5.x required.
- Tested on Windows and Linux. if OSX has any bugs, please let me know.
- D3D9/D3D11 9x tested. OpenGL tested. GLES2/3 should work, but not tested.

For shader nerds, here's some stats:
Mode7_Bg:
- Shader Model VS/PS 2.0
- 15 math in vertex shader
- 55/57 math, 3 texture in pixel shader
Mode7_Spr:
- Shader Model VS/PS 2.0
- 5 math in vertex shader
- 3 math, 2 texture in pixel shader

For more, use Unity's "view compiled shader" option :)

Known issues:
- Can't adjust pitch or roll, this is a limitation of the shader. May be fixed, may be not fixed.
- If the scaling values are set wrong, the background shader may look real inconsistent. The best way 
  to get a good look is to keep General_Bg active and fiddle ass around until you don't see double-mapping 
  anymore, or sprites stop "swimming." (aka, moving faster/slower than the ground or projecting wrong)
  This can take some adjustment, hence I hope to automate this soon to save you guys the pain.
- The multi-sprite animations are not projection based, to match how the SNES/GBA would do it in most cases.
  I can try adding projection-based rotations, but this can be hard to get right. 
  (Sometimes it snaps to the wrong frame doing this)

Alright time for the boring licensing stuff...

LICENSE:
All scripts and shaders can be used for both commercial and non-commercial use, as well as educational use.
All example sprite/BG assets, being IP of Hedgehog Arts, are licensed for non-commercial use only.
DO NOT SELL THIS ASSET PACK IN ANY CIRCUMSTANCES. This asset pack is provided as-is. 
I will try to provide support where I can, but otherwise, this pack comes without any warranty. Proceed at your own risk.
Also, give credit where it is due please, don't claim you made it, and we're good. :)

Boring legal stuff over... Happy developing :)