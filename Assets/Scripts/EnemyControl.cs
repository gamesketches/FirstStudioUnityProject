using UnityEngine;
using System.Collections;

public class EnemyControl : MonoBehaviour {

	//Unless specified as public variables and functions can only been read and written inside the class they are inside
	GameObject ring;
	GameObject game;

	float moveForce=4.00f;
	
	float timeSinceSpawn;
	float moveDelay = 0.5f; //wait 0.5 seconds before moving
	bool hasPlayedSound;

	bool frozen = false;

	// Use this for initialization
	void Start () {
		//we can't set the reference to ring in the inspector, because this is a prefab and it doesn't exist
		//when the game starts. So we use Find instead
		ring = GameObject.Find("Ring");
		game = GameObject.Find("GameManager");

		timeSinceSpawn = 0;
		hasPlayedSound = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		//we want this enemy to move toward the ring
		if(!frozen) {

		//transform.position is a Vector3 but we don't need its z coordinate so we convert it
		Vector2 ringPosition = ring.transform.position; 
		Vector2 thisPosition = transform.position;

		Vector2 vectorToRing = ringPosition - thisPosition;

		Vector2 directionToRing = vectorToRing.normalized;

		//to move and rotate the body we need a reference to the Rigidbody2D
		Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();

		//we want to rotate to point at the ring. First we need the angle from this to the ring
		//We use trigonometry to find the angle to the ring, if you don't remember it, don't worry about it 
		float angleToRing = Mathf.Atan2(directionToRing.y, directionToRing.x) * Mathf.Rad2Deg - 90;
				
		//We could just set the rotation on the transform, but 
		//it's better to call MoveRotation() on the rigidBody, as 
		//it helps to avoid physics glitches
		rigidBody.MoveRotation(angleToRing); //rotate to point at the ring

		//want to count the time since we created this enemy and then make it move after a delay
		timeSinceSpawn += Time.deltaTime; 

		//Once enough time has passed
		if (timeSinceSpawn > moveDelay){
			//now we can apply a force - we want it to be applied in the direction of the ring
			rigidBody.AddForce(directionToRing*moveForce);

			if (hasPlayedSound == false){ //only play the sound once
				GetComponent<AudioSource>().Play();
				hasPlayedSound = true;
			}
		}
	}
	}

	//Automatically called by Unity on a collision of 2D Colliders
	void OnCollisionEnter2D (Collision2D thisCollision){
		GameLogic gameLogic = game.GetComponent<GameLogic>(); //Get the GameLogic component off of the "game" gameobject
		gameLogic.EnemyTouch(gameObject); //call the "EnemyTouch" function with this game object (line 114 in the GameLogic script)
	}
}
