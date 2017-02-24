using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;



public class WildElement : Element {

	List<Color> allElemColors = new List<Color>();

	int curColor;

	Material newMat;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "wild";

		foreach (Material mat in gameController.elemMats)
		{
			//Debug.Log (mat);
			//Debug.Log (mat.GetColor ("_EmissionColor"));
			allElemColors.Add(mat.GetColor ("_EmissionColor"));
		}



		curColor = 0;
		StartCoroutine ("ColorShift");
	}

	public override void Update ()
	{
		//StartCoroutine ("ColorShift");
	}


	IEnumerator ColorShift ()
	{
		newMat = new Material (GetComponent<Renderer>().material.shader);
		newMat.EnableKeyword ("_EMISSION");
		newMat.SetColor ("_EmissionColor", allElemColors[curColor]);

		gameObject.GetComponent<Renderer> ().material = newMat;

		if (curColor == allElemColors.Count - 1)
			curColor = 0;
		else
			curColor += 1;
		yield return new WaitForSeconds(0.5f);
		StartCoroutine ("ColorShift");

	}

	public override void OnTriggerStay (Collider newColl)
	{
		
		//Debug.Log (callOnce);
		GameObject collElem = newColl.gameObject;
		if (collElem.tag == "Element" && activated && callOnce)
		{
			//Debug.Log(IsNeighElem (initialPos, collElem.GetComponent<Element> ().initialPos));
			if (IsNeighElem (initialPos, collElem.GetComponent<Element> ().initialPos))
			{
				compatibleColl = true;
				compatibleElem = collElem;
				//Debug.Log (compatibleElem.GetComponent<Element>());
				Debug.Log (compatibleElem.GetComponent<Element> ().elemType);
				if (compatibleElem.GetComponent<Element> ().elemType == "wild")
				{
					TriggerSecWild (compatibleElem.gameObject.transform.position);
				}
				else
				{
					TriggerSecExp (dirCoords8 (initialPos, compatibleElem.GetComponent<Element> ().initialPos),
					              Vector3.zero, Vector3.zero, compatibleElem);
				}
			}
		}
		//Debug.Log ("callonce set");
		callOnce = false;
	}

	public void TriggerSecWild (Vector3 secElem)
	{
		decimal d = gameController.dEA;
		for (decimal x = -1; x <= 1; x++)
		{
			for (decimal y = -1; y <= 1; y++)
			{
				AttackWildElem (new Vector3 ((float)((decimal)secElem.x + (x*d)), (float)((decimal)secElem.y + (y*d)), secElem.z));
				if (x == 0 || y == 0)
				{
					AttackWildElem (new Vector3 ((float)((decimal)secElem.x + (x*2*d)), 
					                             (float)((decimal)secElem.y + (y*2*d)), secElem.z));
				}
			}
		}
	}

	public void AttackWildElem (Vector3 locExp)
	{
		if (gameController.coordElemDict.ContainsKey (locExp))
		{
			GameObject curElem = gameController.coordElemDict [locExp];
			//Debug.Log (curElem.GetComponent<Element> ().initialPos);
			HighlightElem (curElem);
			curElem.GetComponent<Element> ().StartCoroutine ("StressJig");
			attackedElems.Add (curElem);
		}
		else if (gameController.coordBarrDict.ContainsKey (locExp))
		{
			attackedBarrs.Add (gameController.coordBarrDict [locExp]);
			gameController.coordBarrDict [locExp].GetComponent<Barrier> ().HighlightBarr ();
		}
		else
		{
			// the case where there is no elem or barr
		}
	}



	public override bool IsNeighElem (Vector3 curElem, Vector3 possNeighElem)
	{
		decimal curDEA = gameController.dEA;
		GameObject possElem = gameController.coordElemDict [possNeighElem];
		string possType = possElem.GetComponent<Element> ().elemType;

		if (possType == "wood" || possType == "metal" || possType == "moon" || possType == "sun")
		{
			// downleft
			if ((new Vector3 ((float)((decimal)curElem.x - curDEA), (float)((decimal)curElem.y - curDEA), curElem.z) == possNeighElem) ||
		    // downright
			    (new Vector3 ((float)((decimal)curElem.x + curDEA), (float)((decimal)curElem.y - curDEA), curElem.z) == possNeighElem) ||
		    // upleft
			    (new Vector3 ((float)((decimal)curElem.x - curDEA), (float)((decimal)curElem.y + curDEA), curElem.z) == possNeighElem) ||
		    // upright
			    (new Vector3 ((float)((decimal)curElem.x + curDEA), (float)((decimal)curElem.y + curDEA), curElem.z) == possNeighElem))
			{
				return true;

			}
			else
				return false;
		}
		else if ((new Vector3 ((float)((decimal)curElem.x - curDEA), curElem.y, curElem.z) == possNeighElem) ||
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


}
