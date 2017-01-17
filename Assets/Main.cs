using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	public Player player;
	public Haze hazePrototype;
	public float hazeCreateInterval = 1;  // seconds
	public GameObject gameOverText;
	public int hazeBucketRows = 4;
	public int hazeBucketColumns = 3;

	bool[] hazeBucketEmptyFlags;
	int[] emptyHazeBuckets;

	public static Main instance;
	static Vector3 playerStartPosition;
	static Stack<Haze> freeHazeList = new Stack<Haze>();
	static Dictionary<Haze, int> activeHazeMap = new Dictionary<Haze, int> ();

	float deltaTimeAccumulated = 0;

	// Use this for initialization
	void Start () {
		instance = this;
		playerStartPosition = player.transform.position;
		hazeBucketEmptyFlags = new bool[hazeBucketRows * hazeBucketColumns];
		for (int i = 0; i < hazeBucketEmptyFlags.Length; ++i) {
			hazeBucketEmptyFlags [i] = true;
		}
		emptyHazeBuckets = new int[hazeBucketRows * hazeBucketColumns];
		GameRestart ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameOverText.activeSelf && (Input.GetMouseButtonDown (0) || Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)) {
			GameRestart ();
		}

		if (!gameOverText.activeSelf) {
			deltaTimeAccumulated += Time.deltaTime;
			if (deltaTimeAccumulated >= hazeCreateInterval) {
				int emptyHazeBucketCount = 0;
				for (int i = 0; i < hazeBucketEmptyFlags.Length; ++i) {
					if (hazeBucketEmptyFlags [i]) {
						emptyHazeBuckets [emptyHazeBucketCount++] = i;
					}
				}

				if (emptyHazeBucketCount > 0) {
					int hazeBucket = emptyHazeBuckets[Random.Range (0, emptyHazeBucketCount)];
					hazeBucketEmptyFlags [hazeBucket] = false;
					int row = hazeBucket / hazeBucketColumns;
					int col = hazeBucket % hazeBucketColumns;
					Vector2 minScreenPos, maxScreenPos;
					minScreenPos.x = col * (Camera.main.pixelWidth / hazeBucketColumns);
					minScreenPos.y = row * (Camera.main.pixelHeight / hazeBucketRows);
					maxScreenPos.x = minScreenPos.x + Camera.main.pixelWidth / hazeBucketColumns;
					maxScreenPos.y = minScreenPos.y + Camera.main.pixelHeight / hazeBucketRows;
					Haze haze = freeHazeList.Count > 0 ? freeHazeList.Pop () : Object.Instantiate (hazePrototype.gameObject).GetComponent<Haze> ();
					haze.enabled = true;
					float screenX = Random.Range (minScreenPos.x, maxScreenPos.x);
					float screenY = Random.Range (minScreenPos.y, maxScreenPos.y);
					Vector3 position = Camera.main.ScreenToWorldPoint (new Vector2 (screenX, screenY));
					position.z = haze.transform.position.z;
					haze.transform.position = position;
					haze.gameObject.SetActive (true);
					activeHazeMap.Add (haze, hazeBucket);
					deltaTimeAccumulated = 0;
				}
			}
		}
	}

	public void GameOver() {
		foreach (var haze in activeHazeMap) {
			haze.Key.enabled = false;
		}
		player.enabled = false;
		gameOverText.SetActive (true);
		deltaTimeAccumulated = 0;
	}

	void GameRestart() {
		foreach (var haze in activeHazeMap) {
			haze.Key.gameObject.SetActive (false);
			haze.Key.Reset ();
			freeHazeList.Push (haze.Key);
		}
		activeHazeMap.Clear ();

		for (int i = 0; i < hazeBucketEmptyFlags.Length; ++i) {
			hazeBucketEmptyFlags [i] = true;
		}

		gameOverText.SetActive (false);
		player.transform.position = playerStartPosition;
		player.enabled = true;
	}

	public void RemoveHaze(Haze haze) {
		haze.gameObject.SetActive (false);
		haze.Reset ();
		hazeBucketEmptyFlags [activeHazeMap [haze]] = true;
		activeHazeMap.Remove (haze);
		freeHazeList.Push (haze);
	}
}
