using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameControllerScript : MonoBehaviour {

	public GameObject cubePrefab;
	public GameObject NextCube;
	public GameObject activeCube;
	public Text UI;
	private GameObject[,] allCubes;
	bool GameOver = false;
	bool isActive;
	bool scoreHasChanged = false;
	int totalRows = 5;
	int totalColumns = 8;
	int homoPoints = 10;
	int heteroPoints = 5;
	int noTouchPoints = -1;
	int score = 0;
	float timeToAct = 0.0f;
	float spawnFrequency = 2.0f;
	float gameEndTime = 60.0f;

	//this method gives us a random cube
	public GameObject RandomCube (int x)	{
		return allCubes [Random.Range (0, totalColumns), x];
	}

	//this list and function are used to look for available, random cubes in the row. 

	public bool TryToPlace (Color color, int x)	{
		GameObject cubeToCheck;
		List <GameObject> listOfCubes = new List <GameObject> ();
		while (listOfCubes.Count < totalColumns) {
			cubeToCheck = RandomCube (x);
			if (cubeToCheck.GetComponent<Renderer>().material.color == Color.white)	{
				cubeToCheck.GetComponent<Renderer>().material.color = color;
				return true;
			}
			else if (listOfCubes.Contains (cubeToCheck) == false) {
				listOfCubes.Add (cubeToCheck);
			}
		}
		return false;
	}

	//this is the code that translates what movement keys do; I'm using keyArrayNumber to give the KeyDowns
	//their accompanying key in the array. It also helps to minimize the code written here.
	public void KeyboardInput ()	{
		int keyArrayNumber = -1;
		if (Input.GetKeyDown ("1")) {
			keyArrayNumber = 0;
		} 
		else if (Input.GetKeyDown ("2")) {
			keyArrayNumber = 1;
		} 
		else if (Input.GetKeyDown ("3")) {
			keyArrayNumber = 2;
		} 
		else if (Input.GetKeyDown ("4")) {
			keyArrayNumber = 3;
		} 
		else if (Input.GetKeyDown ("5")) {
			keyArrayNumber = 4;
		}
		if (keyArrayNumber != -1) {
			Color turnNext = NextCube.GetComponent<Renderer> ().material.color;
			if (TryToPlace (turnNext, keyArrayNumber) == true) {
				NextCube.SetActive (false);
			} 
			else {
				print ("Try again, sweatie. :)");
				GameOver = true;
			}
		}
		//this second piece allows for black cubes to be placed if nothing is pressed after two seconds.
		else {
			if (Time.time >= timeToAct)	{
				if (TryToPlace (Color.black, Random.Range (0, totalRows)) == true) {
					if (score >= 1)	{
						score += noTouchPoints;
						scoreHasChanged = true;
					}
					NextCube.SetActive (false);
				}
				else {
					print ("Try again, sweatie. :)");
					GameOver = true;
				}
			}
		}
	}

	//this allows for any cubes that aren't white/black to be activated and deactivated through clicks.
	public void Activate (GameObject oneCube)	{

		if (oneCube.GetComponent<Renderer> ().material.color != Color.white &&
			oneCube.GetComponent<Renderer> ().material.color != Color.black) {
			if (isActive == false) {
				oneCube.transform.localScale += new Vector3 (0.5f, 0.5f, 0);
				isActive = true;
				activeCube = oneCube;
			} 
			else if (oneCube != activeCube)	{
				oneCube.transform.localScale += new Vector3 (+0.5f, +0.5f, 0);
				activeCube.transform.localScale += new Vector3 (-0.5f, -0.5f, 0);
				activeCube = oneCube;
			}
			else {
				oneCube.transform.localScale += new Vector3 (-0.5f, -0.5f, 0);
				isActive = false;
				activeCube = oneCube;
			}
		}

	}

	//this method allows for the movement of a cube if an adjacent cube is clicked.
	public void Movement (GameObject oneCube)	{
		int clickedx = oneCube.GetComponent<CubeBehavior> ().x;
		int newx = activeCube.GetComponent<CubeBehavior> ().x;
		int clickedz = oneCube.GetComponent<CubeBehavior> ().z;
		int newz = activeCube.GetComponent<CubeBehavior> ().z;

		if (oneCube.GetComponent<Renderer>().material.color == Color.white && isActive == true && 
		(clickedx != newx || clickedz != newz)) {
			//this checks for adjacent cubes to the center activated cube
			if (Mathf.Abs (clickedx - newx) <= 1 && Mathf.Abs (clickedz - newz) <= 1)	{
				oneCube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
				activeCube.GetComponent<Renderer>().material.color = Color.white;
				oneCube.transform.localScale += new Vector3 (+0.5f, +0.5f, 0);
				activeCube.transform.localScale += new Vector3 (-0.5f, -0.5f, 0);
				activeCube = oneCube;
			}
		}
	}

	//this checks for pluses that have hemogenous color schemes
	public bool CheckForHomoPlus (int x, int z)	{

		Color centerColor = allCubes [x, z].GetComponent<Renderer>().material.color;

		if (centerColor == Color.white || centerColor == Color.black) {
			return false;
		}
		else if (centerColor == allCubes [x+1, z].GetComponent<Renderer>().material.color &&
		    centerColor == allCubes [x-1, z].GetComponent<Renderer>().material.color &&
		    centerColor == allCubes [x, z+1].GetComponent<Renderer>().material.color &&
		         centerColor == allCubes [x, z-1].GetComponent<Renderer>().material.color)	{
			GameObject [] tempCubes = {allCubes [x, z], allCubes [x +1, z], allCubes [x - 1, z], allCubes [x, z + 1], allCubes [x, z - 1]};
			foreach (GameObject somecube in tempCubes)	{
				somecube.GetComponent<Renderer>().material.color = Color.black;
				if (activeCube == somecube)	{
					isActive = false;
					activeCube.transform.localScale += new Vector3 (-0.5f, -0.5f, 0);
				}
			}
			return true;
		}
		return false;
	}

	//this checks for pluses that have heterogeneous color schemes
	public bool CheckForHeteroPlus (int x, int z)	{
		Color color1 = allCubes [x, z].GetComponent<Renderer>().material.color;
		Color color2 = allCubes [x+1, z].GetComponent<Renderer> ().material.color;
		Color color3 = allCubes [x-1, z].GetComponent<Renderer> ().material.color;
		Color color4 = allCubes [x, z+1].GetComponent<Renderer> ().material.color;
		Color color5 = allCubes [x, z-1].GetComponent<Renderer> ().material.color;
		Color [] tempColors = {color1, color2, color3, color4, color5};
		GameObject [] tempCubes = {allCubes [x, z], allCubes [x + 1, z], allCubes [x - 1, z], allCubes [x, z + 1], allCubes [x, z - 1]};
		if (ColorValues (tempColors)) {
			foreach (GameObject somecube in tempCubes)	{
				somecube.GetComponent<Renderer>().material.color = Color.black;
				if (activeCube == somecube)	{
					isActive = false;
					activeCube.transform.localScale += new Vector3 (-0.5f, -0.5f, 0);
				}
			}
			return true;
		}
		return false;
	}

	//this is an assistant method that checks to make sure all five colors are different
	public bool ColorValues (Color [] newTempColors)	{

		int colorValue = 0;

		foreach (Color colorused in newTempColors) {
			if (colorused == Color.yellow)	{
				colorValue += 1;
			}
			else if (colorused == Color.blue)	{
				colorValue += 10;
			}
			else if (colorused == Color.green)	{
				colorValue += 100;
			}
			else if (colorused == Color.red)	{
				colorValue += 1000;
			}
			else if (colorused == Color.magenta)	{
				colorValue += 10000;
			}
		}

		if (colorValue == 11111) {
			return true;
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		timeToAct += spawnFrequency;

		//this gives us the first random cube;
		NextCube.GetComponent<Renderer>().material.color = colors[ Random.Range (0, colors.Length) ];

		//this creates the grid through a 2D array
		allCubes = new GameObject[totalColumns, totalRows];
		for (int x = 0; x < totalColumns; x++) {
			for (int z = 0; z < totalRows; z++)	{
				allCubes [x,z] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * 2 -14, z * 2 - 14, 10), Quaternion.identity);
				allCubes [x,z].GetComponent<CubeBehavior>().x = x;
				allCubes [x,z].GetComponent<CubeBehavior>().z = z;
				allCubes [x,z].GetComponent<CubeBehavior>().GameController = this;
			}
		}
	}
	
	Color[] colors = {Color.yellow, Color.blue, Color.green, Color.red, Color.magenta};

	// Update is called once per frame
	void Update () {
		//this runs the plus checks constantly because it's out of the turn time loop
		if (Time.time < gameEndTime && GameOver == false) {
			for (int x = 1; x < totalColumns -1; x++) {
				for (int z = 1; z < totalRows -1; z++)	{
					if (CheckForHomoPlus (x,z)) {
						score += homoPoints;
						scoreHasChanged = true;
					}
					else if (CheckForHeteroPlus (x,z))	{
						score += heteroPoints;
						scoreHasChanged = true;
					}
				}
			}
			if (NextCube.activeSelf) {
				KeyboardInput ();
			}
			if (Time.time >= timeToAct)	{
				//this controls events that occur inside of the turn function - random cube spawns, score showing, etc.
				NextCube.SetActive (true);
				NextCube.GetComponent<Renderer>().material.color = colors[ Random.Range (0, colors.Length) ];
				timeToAct += spawnFrequency;
				if (scoreHasChanged == true)	{
				print ("Your score is " + score + ".");
				}
				scoreHasChanged = false;
			}
		}
		else {
			GameOver = true;
			if (score >= 1)	{
				print ("Congratulations, you've won!");
			}
			else {
				print ("Try again sweatie :)");
			}
		}
	}
}