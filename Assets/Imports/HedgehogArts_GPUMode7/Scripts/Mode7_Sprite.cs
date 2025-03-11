/*
 * Mode7_Sprite.cs
 * 
 * This component properly rotates sprites to the correct orientation.
 * This script also sets parameters for an optional shadow sprite, and
 * can have billboarding settings tweaked. A 9x8 texture sheet can
 * also be used for pseudo-rotations and animation.
 * (i.e. pre-rendered Mario Kart 64 sprites anyone? ;-) )
 * 
 * (C) FuzzyLogic Studios 2015. All Rights Reserved.
 */

using UnityEngine;
using System.Collections;

public class Mode7_Sprite : MonoBehaviour {
    [System.Serializable]
    public struct animState
    {
        public string name; //name of animation.
        public int fps; //frmae rate of animation.
        public int frames; //number of frames.
        public Texture2D sheet; //which sprite sheet to use.
    }

    //sprite look-up table
    int[] LUT = { 0, 1, 2, 3, 4, 4, 5, 6, 7, 8,
                  8, 7, 6, 5, 4, 4, 3, 2, 1, 0 };

	public Transform parent; //since billboard itself is auto-oriented, this vector is used instead.
	public Renderer sprite; //renderer to use for pseudo-3D
    public Renderer shadow; //sprite to use for shadows.

    public int selectedAnimation;
    public animState[] animations;

    public void Play(string name)
    {
        for (int i = 0; i < animations.Length; ++i)
        {
            if (animations[i].name == name)
            {
                selectedAnimation = i;
                break;
            }
        }
        selectedAnimation = Mathf.Clamp(selectedAnimation, 0, animations.Length - 1);
    }

    public bool shadowB;
	public bool isMultiView; //is this a sprite sheet for multiple views? (Like F-ZERO/Super Mario Kart)
	public bool usePerspective; //use this to have rotations affected by perspective.
	float currentSprite; //current rotation offset.
    float currentFrame; //current animation offset.
    float currFrameTime; //current frame time.
    public float animSpeed; //speed to play frames back.
	int cx;
	int cy;

    Vector3 oldScale;
	
	public float currentRotation;
    public float extraRotation = 0; //additional rotation for things such as simulating turning.
	
    void Start ()
    {
        oldScale = sprite.transform.localScale;
    }

    void Update ()
    {
        currFrameTime += Time.deltaTime;
        if (currFrameTime >= 1.0f/animSpeed)
        {
            currFrameTime = 0.0f;
            if (currentFrame+1 > animations[selectedAnimation].frames-1)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame += 1;
            }
        }
    }

	// Update is called once per frame
	void OnWillRenderObject () {
		transform.rotation = Camera.current.transform.rotation;
        if (shadowB && !parent.gameObject.isStatic) {
            RaycastHit point;
            Physics.Raycast(parent.transform.position, Vector3.down, out point);
            shadow.transform.position = point.point;
        }
        //if car falls out, put it behind first BG, but in front of second BG. 
        if (parent.transform.position.y < 0)
        {
            sprite.material.SetInt("_Order", 2);
            if (shadowB)
                shadow.material.SetInt("_Order", 0);
        }
        else
        {
            sprite.material.SetInt("_Order", 4);
            if (shadowB)
                shadow.material.SetInt("_Order", 3);
        }
        /*-------------------------------------------*
		 * The below code only works with 4x4 sprite
		 * sheets. The images are laid out in order:
		 * first row is images 0-3
		 * second row is images 4-7
		 * third row is images 8-11
		 * fourth row is images 12-15
		 * 
		 * first image starts from front view, and
		 * "rotates" 22.5 degrees with each step.
		 *-------------------------------------------*/
        if (isMultiView) {
			if (usePerspective) {
				Quaternion globalAngleCalc = Quaternion.LookRotation(Vector3.Normalize(parent.transform.position - Camera.current.transform.position), Vector3.up);
				Quaternion localAngleCalc = Quaternion.LookRotation(parent.transform.forward, parent.transform.up);
				//the 180* is to spin the back round so +Z is forward. (facing away)
				currentSprite = (360-(Mathf.Repeat((180.0f+globalAngleCalc.eulerAngles.y-localAngleCalc.eulerAngles.y)-extraRotation - 10, 360))) / 360;
			}
			else {
				//the 180* is to spin the back round so +Z is forward. (facing away)
				currentSprite = (360-(Mathf.Repeat((180.0f+Camera.current.transform.rotation.eulerAngles.y-parent.transform.rotation.eulerAngles.y)-extraRotation - 10, 360))) / 360;
			}

            if (currentSprite >= 0.5f)
            {
                sprite.transform.localScale = new Vector3(-oldScale.x, oldScale.y, oldScale.z);
            }
            else
            {
                sprite.transform.localScale = new Vector3(oldScale.x, oldScale.y, oldScale.z);
            }

            cx = LUT[(int)Mathf.Clamp((currentSprite*(LUT.Length-1)), 0.0f, LUT.Length-1)];
            cy = (int)currentFrame;
			
			float ix = cx;
			float iy = cy;

            sprite.material.SetTexture("_MainTex", animations[selectedAnimation].sheet);
			sprite.material.SetVector("_MainTex_ST", new Vector4(1.0f / 9.0f, 1.0f / animations[selectedAnimation].frames, ix/9, iy/animations[selectedAnimation].frames));
		}
	}
}
