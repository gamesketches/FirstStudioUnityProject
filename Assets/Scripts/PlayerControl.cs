using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	//a reference to the cursor object is set using the inspector
	//since it's public it'll show up there
	public Transform cursor;
	public Transform ring;

	//these variables are not public thus won't show up in the inspector or be accessible to other classes
	GameObject game;
	bool gameStarted;
	
	public float playerSpeed;
	public float thisRadius = 0.84f;
	public float ringRadius = 2.56f;

	public AudioSource dangerAudio;

	SpriteRenderer sprite;

	// Use this for initialization
	void Start () {

		//initialize a reference to the game manager
		game = GameObject.Find("GameManager");

		//initialize a reference to the sprite renderer
		sprite = GetComponent<SpriteRenderer>();

		//move to the center of the world at the start of the game.
		transform.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		if (gameStarted){ //if the game has started

			//get the 3D position of the mouse and the player
			//don't be scared of vectors! they're just objects that have x, y and z values, so we can 
			//use them to represent positions 3D space (like PVectors in Processing!)
			Vector3 mousePosition = cursor.position;
			Vector3 thisPosition = transform.position;

			//we need a vector that points to the mouse. To do this we just subtract the player
			//position from the mouse position, and then 'normalize it' (set the vector length to 1)
			Vector3 vectorToMouse = mousePosition - thisPosition;
			Vector3 directionToMouse = vectorToMouse.normalized;

			//we can also use vectorToMouse to figure out how far away the player is from the mouse
			float distanceToMouse = vectorToMouse.magnitude;
			
			//now just move toward the mouse at a speed controlled by playerSpeed and the distance to the Mouse
			transform.position = thisPosition + directionToMouse * playerSpeed * distanceToMouse;

			WarnPlayer(); //call the "WarnPlayer" script on 103

			//let's see if we're too close to the ring, and end the game if so.
			if (CheckForRingCollision()) {
				Debug.Log("HIT THE RING!");
				dangerAudio.volume = 0;
				GameLogic gameLogic = game.GetComponent<GameLogic>(); //get the GameLogic component from the game GameObject
				gameLogic.Reset(); //Call the "Reset" function on line 82 of the GameLogic script
			}
		}

	}

	//OnMouseDown is a built-in function that gets called when this gameobject is clicked
	void OnMouseDown(){
		gameStarted = true;
		GameLogic gameLogic = game.GetComponent<GameLogic>(); //get the GameLogic component from the game GameObject
		gameLogic.StartGame(); //Call the "StartGame" function on line 78 of the GameLogic script
	}

	public void Reset(){
		transform.position = Vector3.zero; //Vector3.zero is a shortcut for "new Vector3(0,0,0);"
		gameStarted = false;
	}

	bool CheckForRingCollision(){

		//measure the distance between the player and the ring
		//if it's above a certain value we know we're touching the ring

		Vector3 thisPosition = transform.position;
		Vector3 ringPosition = ring.position;
		Vector3 vectorToRing = thisPosition - ringPosition;
		float distanceToRingCenter = vectorToRing.magnitude;

		//we must be touching the ring if the distance is larger than the difference in the radius
		//between the ring and the player.
		if (distanceToRingCenter > (ringRadius - thisRadius)){
			return true;
		} else {
			return false;
		}
	}

	void WarnPlayer(){ //juice for letting the player know they are in danger
		Vector3 thisPosition = transform.position;
		Vector3 ringPosition = ring.position;
		Vector3 vectorToRing = thisPosition - ringPosition;
		
		//set the volume of the danger noise as we approach the edge of the ring
		float distanceToRingCenter = vectorToRing.magnitude;
		float distanceToEdge = (ringRadius - thisRadius) - distanceToRingCenter;
		float volume = (ringRadius - distanceToEdge)/ringRadius;

		dangerAudio.volume = volume;

		//also set the player color to warn
		Color newColor = sprite.color;
		newColor.g = Mathf.Clamp(distanceToEdge, 0.0f, 0.9f);
		sprite.color = newColor;
	}
}
