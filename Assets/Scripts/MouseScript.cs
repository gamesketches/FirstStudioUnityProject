using UnityEngine;
using System.Collections;

public class MouseScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//hide the mouse cursor
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		//move the object to the mouse

		//get the mouse position on screen (in pixels)
		Vector3 mouseScreenPosition = Input.mousePosition;

		//convert that to world coordinates
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
 		
		//set the z position (depth) to 0 
		mouseWorldPosition.z = 0f;

 		//move this object to the mouse
		transform.position = mouseWorldPosition;
	}
}
