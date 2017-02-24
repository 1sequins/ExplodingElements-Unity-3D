using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Element : MonoBehaviour 
{

	Camera mainCamera;

	public GameController gameController;

	public GameObject deathExplosion;

	public Vector3 initialPos;

	public Material initialMat;
	public Material highMat;

	public bool highlighted;
	public float jigSpeed;
	public float jigDist;
	public float origJigSpeed;
	public float origJigDist;

	public bool compatibleColl;
	public GameObject compatibleElem = null;

	public bool activated;
	public bool callOnce;

	// assigned from inspector
	public GameObject typeBarrier;

	public float fallingTime;

	public string elemType;

	public List<GameObject> attackedElems;
	public List<GameObject> attackedBarrToBe;
	public List<GameObject> attackedBarrs;

	// FOR UNDO
	public Vector3 previousPos;


	// Use this for initialization
	public virtual void Awake () 
	{
		attackedElems = new List<GameObject> ();
		attackedBarrs = new List<GameObject> ();
		attackedBarrToBe = new List<GameObject> ();
		fallingTime = 0.5f;
		compatibleColl = false;
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		initialPos = transform.position;

		// Material stuff!
		initialMat = GetComponent<Renderer> ().material;
		Color newColor = GetComponent<Renderer> ().material.GetColor ("_EmissionColor") + new Color(0.20f, 0.20f, 0.13f, 0f);
		highMat = new Material (GetComponent<Renderer>().material.shader);
		highMat.EnableKeyword ("_EMISSION");
		highMat.SetColor ("_EmissionColor", newColor);

		highlighted = false;
		jigSpeed = 3f; // the higher the slower it will jiggle! :)
		jigDist = 0.05f;
		origJigSpeed = jigSpeed;
		origJigDist = jigDist;

		deathExplosion = gameController.explosion;
		activated = false;
		callOnce = true;

	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		
	}

	public void OnMouseDrag () 
	{
		Vector3 mousePosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector3 (mousePosition.x, mousePosition.y, transform.position.z);
		activated = true;

	}

	public void OnMouseExit ()
	{
		if (!Input.GetMouseButton (0))
		{
			activated = false;
			callOnce = true;
		}
	}


	/*
	public void OnMouseOver ()
	{
		Debug.Log (compatibleColl);
		Debug.Log (initialPos.x);
		//Debug.Log (xNegNeigh);
	}*/


	public virtual void OnTriggerStay (Collider newColl)
	{
		//Debug.Log (callOnce);
		GameObject collElem = newColl.gameObject;
		if (collElem.tag == "Element" && activated && callOnce)
		{
			if (IsNeighElem (initialPos, collElem.GetComponent<Element> ().initialPos) && SameType(collElem))
			{
				compatibleColl = true;
				compatibleElem = collElem;
				//TODO: should this be here? line below?
				attackedElems.Add (gameObject);
				attackedElems.Add (compatibleElem);
				HighlightElem (gameObject);
				HighlightElem (compatibleElem);
				//Debug.Log ("triggers sec");
				if (!(compatibleElem.GetComponent<Element>().elemType == "wild" && gameObject.ToString().Substring(0,4) == "Wild"))
				{
					TriggerSecExp (dirCoords8 (initialPos, compatibleElem.GetComponent<Element> ().initialPos),
					              Vector3.zero, Vector3.zero, compatibleElem);
				}
			}
		}
		//Debug.Log ("callonce set");
		callOnce = false;
	}

	public void OnTriggerExit (Collider newColl)
	{
		if (compatibleColl == true)
		{
			compatibleColl = false;
		}
		if (activated && compatibleElem != null)
		{
			ResetHighlight ();
			ClearCompatLists (compatibleElem);
		}
		callOnce = true;
	}

	public void ClearCompatLists (GameObject otherObj)
	{
		compatibleColl = false;
		attackedBarrs = new List<GameObject> ();
		attackedElems = new List<GameObject> ();
		attackedBarrToBe = new List<GameObject> ();
		compatibleElem = null;

		otherObj.GetComponent<Element> ().compatibleColl = false;
		otherObj.GetComponent<Element> ().attackedBarrs = new List<GameObject> ();
		otherObj.GetComponent<Element> ().attackedElems = new List<GameObject> ();
		otherObj.GetComponent<Element> ().attackedBarrToBe = new List<GameObject> ();
		otherObj.GetComponent<Element> ().compatibleElem = null;
	}

	public void OnMouseUp ()
	{
		//Debug.Log ("onmouseup");
		if (compatibleColl == true) 
		{
			StartExplosion ();
			gameController.TriggerElemsFall (this);
		} 
		else 
		{
			transform.position = initialPos;
		}
		callOnce = true;
	}

	public void StartExplosion (){
		// When the explosion starts, clear the lists
		// save dicts for undo purposes:
		gameController.previousCoordElemDict = gameController.coordElemDict;
		gameController.previousElemList = gameController.elemList;
		gameController.previousCoordBarrDict = gameController.coordBarrDict;
		gameController.previousBarrList = gameController.barrList;

		gameController.ClearUndoLists ();
		foreach (GameObject curElem in attackedElems)
		{
			DestroyElem (curElem, true);
		}
		foreach (GameObject curElem in attackedBarrToBe)
		{
			//Debug.Log (curElem.GetComponent<Element> ().initialPos);
			gameController.CreateBarrier (curElem.GetComponent<Element>().initialPos, typeBarrier);
			DestroyElem (curElem, false);
		}
		foreach (GameObject curBarr in attackedBarrs)
		{
			gameController.DestroyBarrier (curBarr);
		}
	}
		

	public virtual void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{
		//Debug.Log (this);
		//Debug.Log (elemType);
		//Debug.Log ("trigger sec exp");
		List<Vector3> locExps = new List<Vector3> {
			locExp1,
			locExp2
		};
		// This happens in the first of the two initial blocks and the trigger from the second round of explosions
		// This is overridden by all elems, it is the 3rd and 4th total explosion elems
		foreach (Vector3 locExp in locExps)
		{
			if (gameController.coordElemDict.ContainsKey (locExp))
			{
				GameObject curElem = gameController.coordElemDict [locExp];
				//Debug.Log (curElem.GetComponent<Element> ().initialPos);
				// Assign highlighted state
				HighlightElem (curElem);
				curElem.GetComponent<Element> ().StartCoroutine ("StressJig");

				if (SameType(curElem))
				{
					TriggerTriExp (curElem, secExpElem);
					attackedElems.Add(curElem);
				}
				else if (gameController.Opp (curElem.GetComponent<Element> ().elemType) == elemType)
				{
					attackedBarrToBe.Add (curElem);
					SetOppJig (curElem);
				}
				else
				{
					attackedElems.Add(curElem);
				}
			}
			// set this up to detect if an explosion is going into a barrier
			else if (gameController.coordBarrDict.ContainsKey (locExp))
			{
				if (AttackBarrier (locExp))
				{
					attackedBarrs.Add (gameController.coordBarrDict [locExp]);
					gameController.coordBarrDict [locExp].GetComponent<Barrier> ().HighlightBarr ();
				}
			}
		}

	}

	// This is not "virtual" for now, but there may be more complicated explosions in the future...
	// Right now it just destroys blocks in a continued trigectory based on a prevoiusly destroyed blocks position
		// continues if block is the same element
	public virtual void TriggerTriExp (GameObject curElem, GameObject previousExpElem)
	{
		//Debug.Log ("triggertriexp");
		// Earth elem doesn't use this
		Vector2 contDir = dirCoords8 (previousExpElem.GetComponent<Element>().initialPos, curElem.GetComponent<Element>().initialPos);

		Vector3 nextElemVec = curElem.GetComponent<Element> ().initialPos;
		Vector3 locNextExpElem = new Vector3 ((float)((decimal)nextElemVec.x + (gameController.dEA * (decimal)contDir.x)) //x value
		                                      , (float)((decimal)nextElemVec.y + (gameController.dEA * (decimal)contDir.y)) //y value
		                                      , nextElemVec.z);
		
		if (gameController.coordElemDict.ContainsKey (locNextExpElem))
		{
			GameObject newElem = gameController.coordElemDict [locNextExpElem];
			//Debug.Log (newElem.GetComponent<Element> ().initialPos);
			// Assign highlighted material
			HighlightElem (newElem);
			newElem.GetComponent<Element> ().StartCoroutine ("StressJig");

			if (SameType(newElem))
			{
				TriggerTriExp (newElem, curElem);
				attackedElems.Add(newElem);
			}
			else if (gameController.Opp (newElem.GetComponent<Element> ().elemType) == elemType)
			{
				attackedBarrToBe.Add (newElem);
				SetOppJig (newElem);
			}
			else
			{
				attackedElems.Add(newElem);
			}
		}
		// set this up to detect if an explosion is going into a barrier
		else if (gameController.coordBarrDict.ContainsKey (locNextExpElem))
		{
			if (AttackBarrier (locNextExpElem))
			{
				attackedBarrs.Add (gameController.coordBarrDict [locNextExpElem]);
				gameController.coordBarrDict [locNextExpElem].GetComponent<Barrier> ().HighlightBarr ();
			}
		}

	}



	public IEnumerator MoveToInitialPos ()
	{
		float elapsedTime = 0;
		Vector3 startingPos = transform.position;
		while (elapsedTime < fallingTime)
		{
			transform.position = Vector3.Lerp(startingPos, initialPos, (elapsedTime / fallingTime));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		transform.position = initialPos;
	}



	public void HighlightElem (GameObject elem)
	{
		elem.GetComponent<Element> ().highlighted = true;
		elem.GetComponent<Renderer> ().material = elem.GetComponent<Element>().highMat;
		//elem.GetComponent<Element> ().StartCoroutine ("StressJig");
	}

	public IEnumerator StressJig ()
	{
		//Debug.Log ("stress jig");
		while (highlighted == true)
		{
			transform.position = new Vector3 (Mathf.PingPong (Time.time / jigSpeed, jigDist) + initialPos.x - (jigDist/2f), transform.position.y, transform.position.z);
			yield return new WaitForEndOfFrame ();
		}
	}

	public void SetOppJig (GameObject curElem)
	{
		curElem.GetComponent<Element> ().jigSpeed = 1f;
		curElem.GetComponent<Element> ().jigDist = 0.1f;
	}

	public void ResetMyHighlight ()
	{
		GetComponent<Renderer> ().material = initialMat;
		highlighted = false;
		transform.position = initialPos;
		jigSpeed = origJigSpeed;
		jigDist = origJigDist;
	}

	public void ResetHighlight ()
	{
		//Debug.Log ("reset dem highs");
		List<GameObject> resetList = attackedElems;
		resetList.AddRange (attackedBarrToBe);
		foreach (GameObject elemToReset in resetList)
		{
			//Debug.Log (elemToReset.GetComponent<Element> ().initialPos);
			elemToReset.GetComponent<Renderer> ().material = elemToReset.GetComponent<Element> ().initialMat;
			elemToReset.GetComponent<Element> ().highlighted = false;
			elemToReset.transform.position = elemToReset.GetComponent<Element> ().initialPos;
			elemToReset.GetComponent<Element> ().jigSpeed = origJigSpeed;
			elemToReset.GetComponent<Element> ().jigDist = origJigDist;
		}/*
		foreach (GameObject elemToReset in attackedBarrToBe)
		{
			elemToReset.GetComponent<Renderer> ().material = elemToReset.GetComponent<Element> ().initialMat;
			elemToReset.GetComponent<Element> ().highlighted = false;
			elemToReset.transform.position = elemToReset.GetComponent<Element> ().initialPos;
		}*/
		foreach (GameObject barrToReset in attackedBarrs)
		{
			barrToReset.GetComponent<Renderer>().material = barrToReset.GetComponent<Barrier> ().initialMat;
			barrToReset.GetComponent<Barrier> ().highlighted = false;
			barrToReset.transform.position = barrToReset.GetComponent<Barrier> ().initialPos;
		}

	}

	public void DestroyElem (GameObject elemToDestroy, bool replacement)
	{
		// add to undo list
		if (!gameController.previousElems.Contains(elemToDestroy))
			gameController.previousElems.Add (elemToDestroy);

		//Debug.Log ("destroying elem" + elemToDestroy.GetComponent<Element>().initialPos);
		GameObject exp = GameObject.Instantiate (deathExplosion, elemToDestroy.transform.position, Quaternion.identity) as GameObject;
		exp.GetComponent<ParticleSystemRenderer> ().material = elemToDestroy.GetComponent<Renderer> ().material;

		// adds this element to be scored
		gameController.elemsToScore.Insert(0, elemToDestroy.GetComponent<Element>().elemType);

		// test
		gameController.lastDestroyedElemTest = elemToDestroy;

		if (replacement)
		{
			gameController.AddToXColumn (elemToDestroy);
		}
		gameController.coordElemDict.Remove (elemToDestroy.GetComponent<Element>().initialPos);
		gameController.elemList.Remove (elemToDestroy);
		//TODO: Death animation goes here
		//Debug.Log("set active nooooot");
		elemToDestroy.GetComponent<Element>().ResetMyHighlight ();
		elemToDestroy.SetActive(false);
		//Undo.DestroyObjectImmediate(elemToDestroy);
		//Destroy (elemToDestroy);
	}

	public bool AttackBarrier (Vector3 locExp)
	{
		// This method checks to see if the barrier being attacked should be destroyed
		// Can change this later to reference the barrier script, if i have one!
		if (gameController.coordBarrDict [locExp].name == typeBarrier.name + "(Clone)")
		{
			return true;
		}
		else
			return false;
	}

	public bool SameType (GameObject checkElem)
	{
		string cType = checkElem.GetComponent<Element> ().elemType;
		if (elemType == cType || cType == "wild")
			return true;
		else
			return false;
	}

	public virtual bool IsNeighElem (Vector3 curElem, Vector3 possNeighElem)
	{
		decimal curDEA = gameController.dEA;
		if ((new Vector3 ((float)((decimal)curElem.x - curDEA), curElem.y, curElem.z) == possNeighElem) ||
		    (new Vector3 (curElem.x, (float)((decimal)curElem.y - curDEA), curElem.z) == possNeighElem) ||
		    (new Vector3 (curElem.x, (float)((decimal)curElem.y + curDEA), curElem.z) == possNeighElem) ||
		    (new Vector3 ((float)((decimal)curElem.x + curDEA), curElem.y, curElem.z) == possNeighElem))
		{
			return true;

		}
		else
		{
			//Debug.Log (curElem);
			//Debug.Log (possNeighElem);
			//Debug.Log (gameController.dEA);
			//Debug.Log ("not a neigh");
			return false;
		}
	}




	// the direction from the first elem to the second
	public Vector2 dirCoords8 (Vector3 firstElem, Vector3 secElem)
	{
		decimal xValue;
		decimal yValue;
		if (firstElem.x > secElem.x) {
			xValue = -1;
		} else if (firstElem.x < secElem.x) {
			xValue = 1;
		} else {
			xValue = 0;
		}

		if (firstElem.y > secElem.y) {
			yValue = -1;
		} else if (firstElem.y < secElem.y) {
			yValue = 1;
		} else {
			yValue = 0;
		}
		return new Vector2 ((float)xValue, (float)yValue);
	}

	public Vector3 v3Abs (Vector3 v)
	{
		return new Vector3 (Mathf.Abs (v.x), Mathf.Abs (v.y), Mathf.Abs (v.z));
	}

	public Vector3 MultV3Dec (Vector3 v, decimal d)
	{
		return new Vector3 ((float)((decimal)v.x * d), (float)((decimal)v.y * d), (float)((decimal)v.z * d));
	}


	public Vector3 AddV3V3 (Vector3 v, Vector3 v1)
	{
		return new Vector3 ((float)((decimal)v.x + (decimal)v1.x), (float)((decimal)v.y + (decimal)v1.y), (float)((decimal)v.z + (decimal)v1.z));
	}

	public Vector3 SubV3V3 (Vector3 v, Vector3 v1)
	{
		return new Vector3 ((float)((decimal)v.x - (decimal)v1.x), (float)((decimal)v.y - (decimal)v1.y), (float)((decimal)v.z - (decimal)v1.z));
	}
	
}
	