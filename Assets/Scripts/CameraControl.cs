using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	public Transform playerTransform;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//get a point that's the average of the camera position and the player position
		Vector3 newPosition = 0.5f * (transform.position + playerTransform.position);
		
		//make sure the camera is at z=-10 so it can see the playing field
		newPosition.z = -10f;

		//move to that point
		transform.position = newPosition;
	}
}
