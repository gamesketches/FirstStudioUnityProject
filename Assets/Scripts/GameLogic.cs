using UnityEngine;
using System.Collections;
using UnityEngine.UI; //this allows us to address the text object

public class GameLogic : MonoBehaviour {

	//public variable and functions can be seen, used, and modfied (for vars) outside the class
	//public variables can also be set/modified  in the inspector
	public GameObject player;
	public GameObject ring;
	int score;
	int highscore;
	bool gameStarted ;

	public Text scoreText;
	public Text highScoreText;
	
	public GameObject enemyPrefab;
	public Transform enemyContainer; //to hold new enemies and stop from cluttering our editor.

	float timeSinceLastEnemyCreated = 0;
	float enemySpawnDelay = 0;
	int enemySpawnCount = 0;
	public int largeEnemySpawnCount = 10;
	public float largeEnemyScale = 1.0f;
	public float largeEnemySpawnDistance = 20.0f;
	public float startingScale = 1.0f;
	public float scaleDecay = 0.2f;
	int curMode = 0;
	float scaleLevel;

	// Use this for initialization
	void Start () {
		highscore = 0;
		Reset();
	}
	
	// Update is called once per frame
	void Update () {

		highScoreText.text = "" + highscore;  //set the high score text

		if (gameStarted){
			scoreText.text = "" + score;

			timeSinceLastEnemyCreated += Time.deltaTime; //increase this timer by the time since the last frame
			if (timeSinceLastEnemyCreated > enemySpawnDelay){ //if enough time has passed, time to spawn a new enemy
				//create a new enemy and reset the timer
				//we want to create the enemy at a fixed distance from the player
				float enemySpawnDistance = 11.0f; 

				Vector3 enemyScale = new Vector3(0.0f, 0.0f);

				newEnemy(enemySpawnDistance, enemyScale);

				if(enemySpawnCount > largeEnemySpawnCount){ 
					enemyScale = new Vector3(largeEnemyScale, largeEnemyScale);

					newEnemy(largeEnemySpawnDistance, enemyScale);

					enemySpawnCount = 0;
				}
			}
		} else {
			scoreText.text = "click dot";
		}

		//if the ring is larger than normal, animate it back
		if (ring.transform.localScale.x > scaleLevel) {
			//we can't change the transform x,y or z direct, so we make a copy
			Vector3 newScale = ring.transform.localScale;
			newScale.x /= 1.01f;
			newScale.y /= 1.01f;
			
			//now copy it back
			ring.transform.localScale = newScale;
		}
	}

	//function is called by PlayerControl when the circle is clicked.
	public void StartGame(){
		gameStarted = true;
	}

	public void Reset() { //Tell the game manager to reset everything

		gameStarted = false;

		//Kill any living enemies
		for(int i = 0; i < enemyContainer.childCount; i++){
			Transform enemy = enemyContainer.GetChild(i);
		  	Destroy(enemy.gameObject);
		}

		ring.GetComponent<SpriteRenderer>().sprite = 
			Resources.Load("WhiteRing", typeof(Sprite)) as Sprite;

		ring.transform.localScale += new Vector3(startingScale, startingScale, 0);
		scaleLevel = ring.transform.localScale.x;
		
		//Put the player and ring back in the center of the world
		PlayerControl playerControl = player.GetComponent<PlayerControl>();
		playerControl.Reset();
		ring.transform.position = Vector3.zero;

		//Stop any motion on the ring. First we need a reference to its rigidbody
		Rigidbody2D ringBody = ring.GetComponent<Rigidbody2D>();
		ringBody.velocity = Vector2.zero;

		if (score > highscore){
			highscore = score; //store high score
		}

		score = 0;
		curMode = 0;
		timeSinceLastEnemyCreated = 0f;
		enemySpawnDelay = 0.5f;

		//play the start sound
		GetComponent<AudioSource>().Play();
	}

	//this function is called when an enemy touches the ring and sends a message to call this function
	public void EnemyTouch(GameObject sender){
		if(sender.transform.localScale.x > 1.0f) {
			switch(curMode) {
			case 0:
				ring.GetComponent<SpriteRenderer>().sprite = 
					Resources.Load("WhiteTriangle", typeof(Sprite)) as Sprite;
				curMode = 1;
				break;
			case 1:
				ring.GetComponent<SpriteRenderer>().sprite = 
					Resources.Load("WhiteSquare", typeof(Sprite)) as Sprite;
				curMode = 2;
				break;
			case 2:
				ring.GetComponent<SpriteRenderer>().sprite = 
					Resources.Load("WhiteRing", typeof(Sprite)) as Sprite;
				curMode = 0;
				break;
			}
			ring.transform.localScale -= new Vector3(scaleDecay, scaleDecay, 0);
		}
		//they say 'don't kill the messenger' but in this case we have to
		Destroy(sender);

		score += 1;
		
		//give the ring a little reaction when it gets hit, by increasing its size
		ring.transform.localScale += new Vector3(.05f,.05f,0); 

	}

	public void newEnemy(float spawnDistance, Vector3 scale){
			//and at a random angle to the player (in Radians rather than degree)
			float enemySpawnAngle = Random.Range(-Mathf.PI, Mathf.PI);
			
			//start by getting the ring position and then add an offset from the random angle
			Vector2 newEnemyPosition = ring.transform.position; //we'll use the ring as the center
			
			//Again, if your trig is rusty, don't worry about this. We'll talk about it in Code Lab 0
			newEnemyPosition.x += spawnDistance * Mathf.Cos(enemySpawnAngle);
			newEnemyPosition.y += spawnDistance * Mathf.Sin(enemySpawnAngle);
			
			//Make a new enemy from a Prefab
			GameObject newEnemy = Instantiate(enemyPrefab,newEnemyPosition,Quaternion.identity) as GameObject;
			newEnemy.transform.parent = enemyContainer;
			newEnemy.transform.localScale += scale;
			
			timeSinceLastEnemyCreated = 0; //reset the timer
			enemySpawnDelay *= 0.97f; //get faster
			enemySpawnCount += 1;
		}
}
