using UnityEngine;
using System.Collections;
using UnityEditor;

public class PlayerControl : MonoBehaviour {

	//a reference to the cursor object is set using the inspector
	//since it's public it'll show up there
	public Transform cursor;
	public Transform ring;

	//these variables are not public thus won't show up in the inspector or be accessible to other classes
	GameObject game;
	bool gameStarted;
	bool frozen=false;
	
	public float playerSpeed;
	public float thisRadius = 0.84f;
	public float ringRadius = 2.56f;

	public AudioSource dangerAudio;
	public AudioSource gameOverAudio;

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
			if(!frozen){
			    transform.position = thisPosition + directionToMouse * playerSpeed * distanceToMouse;
			}
			WarnPlayer(); //call the "WarnPlayer" script on 103

		}

	}

	//OnMouseDown is a built-in function that gets called when this gameobject is clicked
	void OnMouseDown(){
		GameLogic gameLogic = game.GetComponent<GameLogic>(); //get the GameLogic component from the game GameObject
		if(frozen) {
		gameLogic.Reset(); //Call the "StartGame" function on line 78 of the GameLogic script
		frozen = false;
		}
		else {
		gameLogic.StartGame();
			gameStarted = true;
		}
	}

	public void Reset(){
		transform.position = Vector3.zero; //Vector3.zero is a shortcut for "new Vector3(0,0,0);"
		gameStarted = false;
	}
	
	//Automatically called by Unity on a collision of 2D Colliders
	void OnCollisionEnter2D (Collision2D thisCollision){
		Debug.Log("HIT THE RING!");
		dangerAudio.volume = 0;
		GameLogic gameLogic = game.GetComponent<GameLogic>(); //get the GameLogic component from the game GameObject
		gameLogic.freeze();
		frozen = true;
		gameStarted = false;
		gameOverAudio.Play();
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
