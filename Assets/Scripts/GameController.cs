using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	// TODO: size of level

	// TODO: lights flashing as level ends
		// background color could create this effect :)

	// TODO: point system, reward completing level quickly...
			// points turn into "orbs" that you can assign to different level-ups, can reassign at will

	// TODO: clicking on an object (menu button, element, etc) brings up panel that describes it

	// TODO: undo button
		// lists of previous things, elems, barriers, elempositions, etc.
		// needs to clear undo lists when explosion happens!

	// TODO: keep working on entire ui and level select system
		// 2 game modes, map size, barr num, time to finish, which elements present
	// TODO: other glitches?
	public Object scene;
	public int seed;

	public GameObject lastDestroyedElemTest;

	public List<GameObject> elementsToAssign = new List<GameObject> ();
	public List<GameObject> barriersToAssign;
	GOShuffleBag objsToAssign;

	LevelData levelData;

	// Barriers
	public GameObject earthBarrier;
	public GameObject airBarrier;
	public GameObject fireBarrier;
	public GameObject waterBarrier;
	public GameObject woodBarrier;
	public GameObject metalBarrier;
	public GameObject sunBarrier;
	public GameObject moonBarrier;

	public List<Material> elemMats = new List<Material>();


	int initialBarrierCount;

	public GameObject explosion;

	public Scrollbar timeLeftBar;


	// Should move all this into level data eventaully:
	// In order to change dEA, bounds need to be changed as well
	public decimal dEA = 0.6m;
	//Distance each element is apart from other neighboring elements
	public float zLayer = -2f;

	public Dictionary<int, List<decimal>> XBounds = new Dictionary<int, List<decimal>> ()
	{
		{4, new List<decimal> () {-.9m, .9m}},
		{5, new List<decimal> () {-1.2m, 1.2m}},
		{6, new List<decimal> () {-1.5m, 1.5m}},
		{7, new List<decimal> () {-1.8m, 1.8m}},
		{8, new List<decimal> () {-2.1m, 2.1m}}
	};

	public Dictionary<int, List<decimal>> YBounds = new Dictionary<int, List<decimal>> ()
	{
		{6, new List<decimal> () {-1.8m, 1.2m}},
		{7, new List<decimal> () {-2.1m, 1.5m}},
		{8, new List<decimal> () {-2.4m, 1.8m}},
		{9, new List<decimal> () {-2.7m, 2.1m}},
		{10, new List<decimal> () {-3.0m, 2.4m}},
		{11, new List<decimal> () {-3.3m, 2.7m}},
		{12, new List<decimal> () {-3.6m, 3.0m}},
		{13, new List<decimal> () {-3.9m, 3.3m}},
		{14, new List<decimal> () {-4.2m, 3.6m}}
	};

	// 8x14 
	/*
	public decimal negYBound = -4.2m;
	public decimal posYBound = 3.6m;
	public decimal negXBound = -2.1m;
	public decimal posXBound = 2.1m;

	// 7x14
	public decimal negYBound = -4.2m;
	public decimal posYBound = 3.6m;
	public decimal negXBound = -1.8m;
	public decimal posXBound = 1.8m;
	// 4x6

	public decimal negYBound = -1m;
	public decimal posYBound = 2m;
	public decimal negXBound = -.9m;
	public decimal posXBound = .9m;
	*/
	// 1x13
	public decimal negYBound;
	public decimal posYBound;
	public decimal negXBound;
	public decimal posXBound;


	public int elemsPerColumn;
	public int elemsPerRow;
	public decimal totalElems;
	// end of border stuff


	public Dictionary<Vector3, GameObject> coordElemDict = new Dictionary<Vector3, GameObject> ();
	public List<GameObject> elemList = new List<GameObject> ();
	public Dictionary<Vector3, GameObject> previousCoordElemDict = new Dictionary<Vector3, GameObject> ();
	public List<GameObject> previousElemList = new List<GameObject> ();

	public Dictionary<Vector3, GameObject> coordBarrDict = new Dictionary<Vector3, GameObject> ();
	public List<GameObject> barrList = new List<GameObject> ();
	public Dictionary<Vector3, GameObject> previousCoordBarrDict = new Dictionary<Vector3, GameObject> ();
	public List<GameObject> previousBarrList = new List<GameObject> ();

	// Keeps track of the columns of exploded elems
	// I think this list could just be an int, number of objects destroyed...
	List<float> emptyColumnList;
	Dictionary<float, int> emptyColumnDict;

	// UI STUFF

	// menu
	public GameObject menuUI;
	public GameObject gameUI;

	public Text scoreText;
	public int score;
	public List<string> elemsToScore = new List<string>();
	// This gets renewed after new elems are instantiated

	public Text goalText;
	public int goal;

	public float totalTimeToFinish;
	float timeLeft = 0f;


	// UNDO STUFF
	public List<GameObject> previousElems = new List<GameObject> ();
	public List<GameObject> lastCreatedElems = new List<GameObject> ();
	public List<GameObject> elemsMoved = new List<GameObject> ();
	public List<GameObject> previousBarrs = new List<GameObject> ();
	public List<GameObject> lastCreatedBarrs = new List<GameObject> ();
	public int previousScore;


	// Use this for initialization
	// Iphone 5 screen size is unity 2.814
	void Start ()
	{
		levelData = this.gameObject.GetComponent<LevelData> ();

		barriersToAssign = new List<GameObject> {
			woodBarrier,
			metalBarrier,
			sunBarrier,
			moonBarrier
		};

		// 7m <= y <= 14m
		elemsPerColumn = levelData.elemsPerColumn;
		// 4m <= x <= 8m
		elemsPerRow = levelData.elemsPerRow;
		Debug.Log (elemsPerColumn);
		negYBound = YBounds[elemsPerColumn][0];
		posYBound = YBounds[elemsPerColumn][1];
		negXBound = XBounds[elemsPerRow][0];
		posXBound = XBounds[elemsPerRow][1];

		//elemsPerColumn = (decimal)Mathf.FloorToInt((float)((posYBound + (decimal)Mathf.Abs ((float)negYBound)) / dEA)) + 1m;
		//elemsPerRow = (decimal)Mathf.FloorToInt((float)((posXBound + (decimal)Mathf.Abs ((float)negXBound)) / dEA)) + 1m;
		Debug.Log ("Number of Elems per column: " + elemsPerColumn.ToString ());
		Debug.Log ("Number of elems per row: " + elemsPerRow.ToString ());
		totalElems = (decimal)elemsPerColumn * (decimal)elemsPerRow;
		// 22m determines the ratio of barriers to elems
		// This will likely get overriden in LevelData
		initialBarrierCount = Mathf.RoundToInt((float)(totalElems / 30m));

	}

	public void StartGame ()
	{
		// Get's started in level data

		SetLevelData ();

		// UI Initialization

		timeLeft = 0;
		score = 0;
		previousScore = score;
		UpdateScore ();


		// ShuffleBag initialization, right now just used for barriers
		objsToAssign = new GOShuffleBag (System.Decimal.ToInt32(totalElems), this);

		objsToAssign.AddList (elementsToAssign, System.Decimal.ToInt32(totalElems - initialBarrierCount));
		objsToAssign.AddList (barriersToAssign, initialBarrierCount);

		emptyColumnList = new List<float> ();
		emptyColumnDict = new Dictionary<float, int> ();

		// Instantiation initialization
		// right now elems are assigned randomly in create elem, not from shuffle bag
			// bag just determines whether it is a barrier or element :)
		for (decimal y = negYBound; y <= posYBound; y += dEA) {
			for (decimal x = negXBound; x <= posXBound; x += dEA) {
				GameObject toAssign = objsToAssign.Next ();
				if (toAssign.tag == "Barrier")
				{
					CreateBarrier (new Vector3 ((float)x, (float)y, zLayer), 
					               toAssign);
				}
				else
				{
					CreateElement (new Vector3 ((float)x, (float)y, zLayer));
				}
			}
		}


	}

	public void SetLevelData ()
	{
		//TODO: SetLevelData here
		goalText.text = levelData.dataGoalText;
		totalTimeToFinish = levelData.dataTimeToFinish;


	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Time Left stuff
		timeLeft += (Time.deltaTime * ((barrList.Count * 0.1f) + 1));
		if (timeLeft >= totalTimeToFinish)
		{
			goalText.text = "YOU LOST";
			timeLeft = totalTimeToFinish;
		}


		// Timeleft Bar stuff
		Vector3 barXScale = new Vector3 (1 - (timeLeft/totalTimeToFinish), 1.0f, 1.0f);
		timeLeftBar.handleRect.localScale = barXScale;

		float totalWidth = ((timeLeftBar.GetComponent<RectTransform> ().rect.width / 2) - 2) * -1;
		Vector3 barXPos = new Vector3 ((totalWidth * (timeLeft / totalTimeToFinish)), 0f, 0f);
		timeLeftBar.handleRect.localPosition = barXPos;

		timeLeftBar.size = 1f;
		//timeLeftBar.size = (timeLeft/totalTimeToFinish);

	}

	/// <summary>
	///  This section is about the process of creating new elements and those falling into place!
	///  It is the controller for the 3 functions below it.
	/// </summary>
	public void TriggerElemsFall (Element gameObjectToDestroy)
	{
		// Needs to destroy gameObject after this is triggered, so this makes the call
		//gameObjectToDestroy.DestroyElem (gameObjectToDestroy.gameObject, true);

		CheckOpenSpots ();

		for (decimal x = negXBound; x <= posXBound; x += dEA) 
		{
			if (emptyColumnDict.ContainsKey ((float)x)) 
			{
				// Do stuff with this column!
				InstantiateAboveElems ((float)x, emptyColumnDict [(float)x]);
			}
		}
		CascadeDownwards ();
	}

	public void CheckOpenSpots ()
	{
		//Debug.Log (lastDestroyedElemTest);
		for (decimal x = negXBound; x <= posXBound; x += dEA) 
		{
			int numToGen = 0;
			if (emptyColumnList.Contains((float)x)) 
			{
				for (decimal y = negYBound; y <= posYBound; y += dEA) 
				{
					Vector3 dictVect = new Vector3 ((float)x, (float)y, zLayer);

					if (coordElemDict.ContainsKey (dictVect))
					{
						// just skip it if it is an element
					}
					else if (coordBarrDict.ContainsKey (dictVect))
					{
						numToGen = 0;
					}
					else
					{
						// this happens if there is an empty spot
						numToGen += 1;
					}

				}
				emptyColumnDict.Add ((float)x, numToGen);
			}
		}

	}

	public void InstantiateAboveElems (float xColumn, int numToGen)
	{
		// TODO: this needs to be changed in order for new deas to fall
		for (decimal y = posYBound + dEA; y <= posYBound + (dEA * numToGen); y += dEA) 
		{
			CreateElement (new Vector3 (xColumn, (float)y, zLayer));
		}
		// resets the elemsToScore list after updating the score
		UpdateScore ();
		elemsToScore = new List<string>();
	}


	public void CascadeDownwards ()
	{
		for (decimal x = negXBound; x <= posXBound; x += dEA) 
		{
			// check out this columndict...
			if (emptyColumnDict.ContainsKey ((float)x)) 
			{
				List<Vector3> emptySpots = new List<Vector3> ();
				for (decimal y = negYBound; y <= posYBound + (dEA * elemsPerColumn); y += dEA)
				{
					Vector3 dictVect = new Vector3 ((float)x, (float)y, zLayer);
					if (coordElemDict.ContainsKey (dictVect))
					{
						
						if (emptySpots.Count != 0)
						{
							//Debug.Log ("cascading happens");
							GameObject curElem = coordElemDict [dictVect];
							//Undo.RecordObject (curElem, "moving elem");
							// add to undo lists
							elemsMoved.Add(curElem);
							// add spot of to be moved elem to emptys
							emptySpots.Add (curElem.GetComponent<Element> ().initialPos);
							// change initialPos of elem and update previousPos
							curElem.GetComponent<Element> ().previousPos = curElem.GetComponent<Element> ().initialPos;
							curElem.GetComponent<Element> ().initialPos = emptySpots [0];
							curElem.GetComponent<Element> ().StartCoroutine ("MoveToInitialPos");
							// change key in dict
							coordElemDict.Remove (dictVect);
							coordElemDict.Add (emptySpots [0], curElem);
							// delete vect from emptySpots
							emptySpots.RemoveAt (0);
						}
					}
					else if (coordBarrDict.ContainsKey (dictVect))
					{
						emptySpots = new List<Vector3> ();
					}
					else
					{
						emptySpots.Add (new Vector3 ((float)x, (float)y, zLayer));
					}
				}
			}
		}
		// after cascading, emptyColumnList gets renewed
		emptyColumnList = new List<float> ();
		emptyColumnDict = new Dictionary<float, int> ();
		//Debug.Log ("renewed");

	}

	public void AddToXColumn (GameObject curObj)
	{
		// This adds columns to emptyColumnList when an element in them is destroyed
		Vector3 curVec;
		if (curObj.GetComponent<Element> () != null)
		{
			curVec = curObj.GetComponent<Element> ().initialPos;
		}
		else if (curObj.GetComponent<Barrier> () != null)
		{
			curVec = curObj.GetComponent<Barrier> ().initialPos;
		}
		else
		{
			curVec = curObj.transform.position;
		}
		if (!emptyColumnList.Contains (curVec.x)) 
		{
			emptyColumnList.Add (curVec.x);
		}
	}



	public void CreateElement (Vector3 creationLoc)
	{
		//Random.seed = seed;
		//Debug.Log (Random.seed);
		GameObject newElement = GameObject.Instantiate (elementsToAssign [Random.Range (0, elementsToAssign.Count)], 
		                                                creationLoc, Quaternion.identity) as GameObject;
		//Undo.RegisterCreatedObjectUndo (newElement, "create element");

		// Adds gamecontroller to element.cs, now this is in element.cs
		//newElement.GetComponent<Element> ().gameController = this;

		// adds to undo lists
		lastCreatedElems.Add(newElement);

		// adds element to dict and list
		coordElemDict.Add (newElement.GetComponent<Element>().initialPos, newElement);
		elemList.Add (newElement);
	}

	public void CreateBarrier (Vector3 creationLoc, GameObject barr)
	{
		
		GameObject newBarrier = GameObject.Instantiate (barr, creationLoc, barr.transform.rotation) as GameObject;
		//Undo.RegisterCreatedObjectUndo (newBarrier, "create barrier");

		// add to undo lists
		if (!lastCreatedBarrs.Contains(newBarrier))
			lastCreatedBarrs.Add(newBarrier);

		//need to add gamecontroller to barrier.cs
		if (coordBarrDict.ContainsKey(newBarrier.GetComponent<Barrier>().initialPos))
			Debug.Log(coordBarrDict[newBarrier.GetComponent<Barrier>().initialPos].name);
		coordBarrDict.Add (newBarrier.GetComponent<Barrier>().initialPos, newBarrier);
		barrList.Add (newBarrier);


	}

	public void DestroyBarrier (GameObject barrToDestroy)
	{
		// add to undo list
		if (!previousBarrs.Contains(barrToDestroy))
			previousBarrs.Add (barrToDestroy);

		AddToXColumn (barrToDestroy);
		elemsToScore.Insert(0, "barrier");

		coordBarrDict.Remove (barrToDestroy.GetComponent<Barrier>().initialPos);
		barrList.Remove (barrToDestroy);

		//Undo.DestroyObjectImmediate (barrToDestroy);
		Destroy (barrToDestroy);
	}

	public void UpdateScore ()
	{
		previousScore = score;
		if (elemsToScore.Count != 0)
		{
			string initialType = elemsToScore [0];
			float sameTypeCount = 1;
			foreach (string type in elemsToScore)
			{
				// right now elemstoscore doesn't count in intuitive order, 
				// instead goes all the way along one path of destruction, then the other
				if (type == initialType)
				{
					score += (int)Mathf.Pow (sameTypeCount, 2f) * 50;
					sameTypeCount++;
				}
				else if (type == "barrier")
				{
					score += 750;
				}
				else if (type == Opp (initialType))
				{
					score -= 1000;
				}
				else
				{
					score += 25;
				}
			}
		}
			
		scoreText.text = "Score: " + score.ToString ();
	}

	public string Opp (string eT)
	{
		if (eT == "fire")
			return "water";
		else if (eT == "water")
			return "fire";
		else if (eT == "sun")
			return "moon";
		else if (eT == "moon")
			return "sun";
		else if (eT == "wood")
			return "metal";
		else if (eT == "metal")
			return "wood";
		// add more here!
		else if (eT == "earth")
			return "air";
		else
			return "earth";
	}

	public bool CheckInBounds (Vector3 curVec)
	{
		if (((decimal)curVec.x >= negXBound && (decimal)curVec.x <= posXBound) &&
		    ((decimal)curVec.y >= negYBound && (decimal)curVec.y <= posYBound)) {
			return true;
		} else {
			return false;
		}
	}

	public void Alt1Undo ()
	{
		/*
		foreach (GameObject createdElem in lastCreatedElems)
		{
			coordElemDict.Remove (createdElem.GetComponent<Element>().initialPos);
			elemList.Remove (createdElem);
		}
		foreach (GameObject createdBarr in lastCreatedBarrs)
		{
			coordBarrDict.Remove (createdBarr.GetComponent<Barrier>().initialPos);
			barrList.Remove (createdBarr);
		}*/

		Undo.PerformUndo ();

		coordElemDict = previousCoordElemDict;
		elemList = previousElemList;
		coordBarrDict = previousCoordBarrDict;
		barrList = previousBarrList;

		foreach (GameObject elem in elemList)
		{
			if (elem == null)
			{
				Debug.Log ("keljfakljfkl");
			}
		}

		// sets up list to be sorted
		List<GameObject> elemsMovedReal = new List<GameObject> ();
		foreach (GameObject curElem in elemsMoved)
		{
			if (curElem != null)
			{
				elemsMovedReal.Add (curElem);
			}
		}
		var elemsMovedtest = from element in elemsMovedReal
			orderby element.GetComponent<Element>().initialPos.y descending
			select element;

		foreach (GameObject curElem in elemsMovedtest)
		{
			Vector3 movedDictVect = curElem.GetComponent<Element> ().initialPos;
			curElem.GetComponent<Element> ().initialPos = curElem.GetComponent<Element> ().previousPos;
			curElem.GetComponent<Element> ().StartCoroutine ("MoveToInitialPos");
			// change key in dict
			//coordElemDict.Remove (movedDictVect);
			//coordElemDict.Add (curElem.GetComponent<Element> ().initialPos, curElem);
		}

		// restores lists :)...
		foreach (GameObject removedElem in previousElems)
		{
			// TODO: need to delete/refresh list of previousElems correctly (i.e. deletion)
			Debug.Log (removedElem.transform.position);
			//removedElem.GetComponent<Element> ().ResetHighlight ();
			removedElem.SetActive (true);
			//Debug.Log (removedElem.GetComponent<Element> ().initialPos);

			// adds element to dict and list
			//coordElemDict.Add (removedElem.GetComponent<Element>().initialPos, removedElem);
			//elemList.Add (removedElem);
		}/*
		foreach (GameObject removedBarr in previousBarrs)
		{
			if (coordBarrDict.ContainsKey(removedBarr.GetComponent<Barrier>().initialPos))
				Debug.Log(coordBarrDict[removedBarr.GetComponent<Barrier>().initialPos].name);
			coordBarrDict.Add (removedBarr.GetComponent<Barrier>().initialPos, removedBarr);
			barrList.Add (removedBarr);
		}*/

		//ClearUndoLists ();
		// should we erase the undo lists here?
	}

	public void AltUndo ()
	{
		foreach (GameObject curElem in lastCreatedElems)
		{
			curElem.GetComponent<Element>().DestroyElem(curElem, false);
		}
		foreach (GameObject curBarr in lastCreatedBarrs)
		{
			DestroyBarrier (curBarr);
		}
		foreach (GameObject curElem in elemsMoved)
		{
			Vector3 movedDictVect = curElem.GetComponent<Element> ().initialPos;
			curElem.GetComponent<Element> ().initialPos = curElem.GetComponent<Element> ().previousPos;
			curElem.GetComponent<Element> ().StartCoroutine ("MoveToInitialPos");
			// change key in dict
			coordElemDict.Remove (movedDictVect);
			coordElemDict.Add (curElem.GetComponent<Element> ().initialPos, curElem);
		}
		foreach (GameObject curElem in previousElems)
		{
			GameObject newElement = GameObject.Instantiate (curElem, curElem.GetComponent<Element> ().initialPos, 
			                                                Quaternion.identity) as GameObject;
			// adds element to dict and list
			coordElemDict.Add (newElement.GetComponent<Element>().initialPos, newElement);
			elemList.Add (newElement);
		}

		foreach (GameObject curBarr in previousBarrs)
		{
			CreateBarrier (curBarr.GetComponent<Barrier> ().initialPos, curBarr);
		}

		// update score also
		score = previousScore;
		scoreText.text = "Score: " + score.ToString ();

	}

	public void ClearUndoLists ()
	{
		previousElems = new List<GameObject> ();
		lastCreatedElems = new List<GameObject> ();
		elemsMoved = new List<GameObject> ();
		previousBarrs = new List<GameObject> ();
		lastCreatedBarrs = new List<GameObject> ();

	}

	public void RestartLevel ()
	{
		foreach (GameObject elem in previousElems)
			Debug.Log(elem.GetComponent<Element> ().initialPos);
		foreach (GameObject toDelete in elemList)
		{
			if (toDelete.GetComponent<Element> () != null)
				Debug.Log (toDelete.GetComponent<Element> ().initialPos);
			Destroy (toDelete);

		}
		foreach (GameObject toDelete in barrList)
			Destroy (toDelete);
		
		coordElemDict = new Dictionary<Vector3, GameObject> ();
		elemList = new List<GameObject> ();
		coordBarrDict = new Dictionary<Vector3, GameObject> ();
		barrList = new List<GameObject> ();

		StartGame ();
	}

}



