/*
 * Mode7.cs
 * 
 * This component serves as a means of feeding the BGs the scroll parameters.
 * The size of the BG specified in total scale must be just right, otherwise,
 * sprite positions/projections won't match up.
 * 
 * Pitch is also locked to 0, due to projection issues if the pitch is anything but 0.
 * 
 * ChangeLog:
 * 11/10/16: Cleaned up code, and removed unused variables.
 * 25/07/17: Another code cleanup + rerelease on Unity Forum
 * 
 * (C) Hedgehog Arts 2015-2017. All Rights Reserved.
 */

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Mode7 : MonoBehaviour {
	public Texture2D map; //our map to use. Can be anything.
    public float totalRotation; //small rotation to rotate the map itself with.
	
	public float fov; //field of view.
	public float distanceFromGround; //how high-low the floor is from the origin. (Y = 0)
    public float totalScale; //actual scale set. (scale submitted is modified to accomodate moving up and down)
	
	public Renderer Mode7Layer; //renderer to use as Mode7 BG layer. (can be 2D or 3D, as screen coords are used in shader regardless)

	//empty start function to restore checkbox to turn this off.
	void Start () {
		
	}

	// Update is called once per frame
	void OnPreRender () {
		//set roll and pitch to 0, as Mode7 shader can't handle those properly.
		//(roll isn't even taken into account, changing pitch causes 'sprite swimming')
		transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);

		//setup rotation.
		//Must be between 360 and 0, can't be more or less, 
		//as it breaks mode7
        float rotation = Mathf.Repeat(transform.rotation.eulerAngles.y, 360);

        //set map texture.
        Mode7Layer.sharedMaterial.SetTexture("_Mode7Map", map);

		//regenerate the rotation again, this time adding the total rotation as well.
		//total rotation rotates the 'map' itself, thus giving the designers a way to
		//correct the map rotation if required.
		float newRot = Mathf.Repeat (-rotation + totalRotation, 360);

		//pre-generate the sin/cos values for the shader.
		//this is an optimization measure, as a sin/cos on every pixel is
		//very expensive to render. (It also allows SM2.0 hardware to run this)
		Mode7Layer.sharedMaterial.SetFloat("_sine", (float)System.Math.Sin(newRot*Mathf.PI/180));
		Mode7Layer.sharedMaterial.SetFloat("_cosine", (float)System.Math.Cos(newRot*Mathf.PI/180));
        
        //set parameters for projection.
		float horizon = -transform.forward.y; //this isn't technically used anyway, but...
		Mode7Layer.sharedMaterial.SetFloat("_fov", fov); //field of view (in millimeters) (Yet to be confirmed)
		Mode7Layer.sharedMaterial.SetFloat("_horizon", horizon); //pitch/horizon control.

		//camera Y axis + floor distance from Y = 0
		Mode7Layer.sharedMaterial.SetFloat("_floorHeight", (transform.position.y*0.1f+distanceFromGround));
		//rotation around Y axis.
		Mode7Layer.sharedMaterial.SetFloat("_rot", transform.rotation.eulerAngles.y);
		//total scale of floor layer being rendered.
		Mode7Layer.sharedMaterial.SetFloat("_scaling", Mathf.Clamp(totalScale, 0.0f, 100000.0f));
		
		//Set map offset. (aka. moves 'camera' to correct position)
		Mode7Layer.sharedMaterial.SetVector("_offset", new Vector4(-transform.position.x, -transform.position.z, transform.position.y, 1)); 
	}
}
