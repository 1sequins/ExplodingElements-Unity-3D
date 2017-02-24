using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElement : Element {

	public override void Awake () {
		base.Awake ();
		elemType = "fire";

	}
	
	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{

		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		// starts at 1 to attack the locExp2
		int fireElemExp = 1;
		locExp1 = new Vector3 ((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x)), 
		                       (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y)), 
		                       secPos.z);
		locExp2 = new Vector3 ((float)((decimal)secPos.x + (gameController.dEA * 2 * (decimal)initDir.x)), 
		                       (float)((decimal)secPos.y + (gameController.dEA * 2 * (decimal)initDir.y)), 
		                       secPos.z);

		List<Vector3> locExps = new List<Vector3> {
			locExp1,
			//locExp2
		};


		AttackFireElem (locExp1);
		if (gameController.coordElemDict.ContainsKey (locExp1))
		{
			GameObject curElem = gameController.coordElemDict [locExp1];
			if (SameType(curElem))
			{
				fireElemExp += 1;
			}
		}
		//Debug.Log (secPos);
		//Debug.Log (fireElemExp);
		TriggerFireExp (secPos, secPos, new Vector3 (initDir.x, initDir.y, 0), fireElemExp);

		/*
		foreach (Vector3 locExp in locExps)
		{
			// TODO: change a lot of this code to make it more clear
			if (gameController.coordElemDict.ContainsKey (locExp))
			{
				GameObject curElem = gameController.coordElemDict [locExp];
				//Debug.Log (curElem.GetComponent<Element> ().initialPos);
				HighlightElem (curElem);
				curElem.GetComponent<Element> ().StartCoroutine ("StressJig");

				if (SameType(curElem))
				{
					fireElemExp += 1;
					attackedElems.Add(curElem);
				}
				else if (gameController.Opp (curElem.GetComponent<Element> ().elemType) == elemType)
				{
					attackedBarrToBe.Add (curElem);
					SetOppJig (curElem);
				}
				/*if (gameController.coordElemDict [locExp1].GetComponent<Element> ().elemType != elemType) 
				{
					fireElemExp -= 1;
				}
				else 
				{
					fireElemExp += 1;
				}
				// Can add this to make fire more powerful, need to add below as well (2 places)
				else
				{
					attackedElems.Add(curElem);
				}
				// To continue explosion
				if (locExp == locExp1)
				{
					// gives the second total explosion
					TriggerFireExp (secPos, secPos, initDir, fireElemExp);
				}
			}
			else if (gameController.coordBarrDict.ContainsKey (locExp))
			{
				if (AttackBarrier (locExp))
				{
					attackedBarrs.Add (gameController.coordBarrDict[locExp]);
					gameController.coordBarrDict[locExp].GetComponent<Barrier> ().HighlightBarr ();
					// will this implement the same below?
				}
			}
	
		}
		*/
	}

	void AttackFireElem (Vector3 elemLoc)
	{
		Vector3 locExp = elemLoc;
		if (gameController.coordElemDict.ContainsKey (locExp))
		{
			GameObject curElem = gameController.coordElemDict [locExp];
			//Debug.Log (curElem.GetComponent<Element> ().initialPos);
			HighlightElem (curElem);
			curElem.GetComponent<Element> ().StartCoroutine ("StressJig");

			if (gameController.Opp (curElem.GetComponent<Element> ().elemType) == elemType)
			{
				attackedBarrToBe.Add (curElem);
				SetOppJig (curElem);
			}
			else
			{
				attackedElems.Add (curElem);
			}
		}
		else if (gameController.coordBarrDict.ContainsKey (locExp))
		{
			if (AttackBarrier (locExp))
			{
				attackedBarrs.Add (gameController.coordBarrDict [locExp]);
				gameController.coordBarrDict [locExp].GetComponent<Barrier> ().HighlightBarr ();
			}
		}
		else
		{
			// the case where there is no elem
		}
	}

	public void TriggerFireExp (Vector3 prevElemLoc, Vector3 secExpElem, Vector3 initDir3, int fireElemExp)
	{
		//Debug.Log ("triggerfire " + fireElemExp.ToString ());

		//Debug.Log (fireElemExp);
		if (fireElemExp <= 0)
		{
			return;
		}

		// Terminology is for horizontal initial directions
		decimal forwardFromSecElem;
		decimal sidewaysFromSecElem;
		if (initDir3.y != 0)
		{
			//forwardFromSecElem = (decimal)Mathf.Abs((float)(((decimal)Mathf.Abs(secExpElem.y) - (decimal)Mathf.Abs(prevElemLoc.y))/0.6m));
			//sidewaysFromSecElem = (decimal)Mathf.Abs((float)(((decimal)Mathf.Abs(secExpElem.x) - (decimal)Mathf.Abs(prevElemLoc.x))/0.6m));
		
			forwardFromSecElem = (decimal)Mathf.Abs((float)(((decimal)secExpElem.y - (decimal)prevElemLoc.y)/0.6m));
			sidewaysFromSecElem = (decimal)Mathf.Abs((float)(((decimal)secExpElem.x - (decimal)prevElemLoc.x)/0.6m));
		}
		else {
			//forwardFromSecElem = (decimal)Mathf.Abs((float)(((decimal)Mathf.Abs(secExpElem.x) - (decimal)Mathf.Abs(prevElemLoc.x))/0.6m));
			//sidewaysFromSecElem = (decimal)Mathf.Abs((float)(((decimal)Mathf.Abs(secExpElem.y) - (decimal)Mathf.Abs(prevElemLoc.y))/0.6m));

			forwardFromSecElem = (decimal)Mathf.Abs((float)(((decimal)secExpElem.x - (decimal)prevElemLoc.x)/0.6m));
			sidewaysFromSecElem = (decimal)Mathf.Abs((float)(((decimal)secExpElem.y - (decimal)prevElemLoc.y)/0.6m));
		}
		//Debug.Log (initDir3);
		//Debug.Log (secExpElem);
		//Debug.Log (prevElemLoc);
		//Debug.Log (sidewaysFromSecElem);


		Vector3 nextElemLoc;
		Vector3 sideDir = new Vector3 (Mathf.Abs(initDir3.y), Mathf.Abs(initDir3.x), initDir3.z);

		// this says whether it is on the positive or negative side, and therefore what element should be next
		bool sidePositive = false;
		if ((sideDir.y == 1 && (dirCoords8 (secExpElem, prevElemLoc).y > 0)) || (sideDir.x == 1 && (dirCoords8 (secExpElem, 
		                                                                                                        prevElemLoc).x > 0)))
		{
			sidePositive = true;
		}

		if (forwardFromSecElem == sidewaysFromSecElem && (sidePositive || forwardFromSecElem == 0))
		{
			// Does this when it is at the apex of the current set
			//Debug.Log("apex");
			nextElemLoc = AddV3V3(secExpElem, (MultV3Dec(initDir3, (forwardFromSecElem + 2m) * gameController.dEA)));
		}
		else if (sidewaysFromSecElem == 0)
		{
			// Does this when it is level with the elem2
			nextElemLoc = SubV3V3(prevElemLoc, (MultV3Dec(initDir3, gameController.dEA)));
			nextElemLoc = SubV3V3(nextElemLoc, (MultV3Dec(sideDir, gameController.dEA)));
			//Debug.Log ("gets past sideways " + secExpElem.ToString());
		}

		else if (sidePositive)
		{
			//Debug.Log ("doing this?");
			nextElemLoc = SubV3V3 (prevElemLoc, (MultV3Dec(sideDir, (gameController.dEA * (sidewaysFromSecElem * 2m + 1m)))));
		}
		else
		{
			//Debug.Log ("doinggggg this");
			nextElemLoc = AddV3V3 (prevElemLoc, (MultV3Dec(sideDir, gameController.dEA * (sidewaysFromSecElem * 2m))));
		}
		//Debug.Log ("lay the attack");
		//Debug.Log (nextElemLoc);
		fireElemExp = TriggerFireCount (nextElemLoc, fireElemExp);
		AttackFireElem (nextElemLoc);
		TriggerFireExp (nextElemLoc, secExpElem, initDir3, fireElemExp);

	}

	int TriggerFireCount (Vector3 nextElem, int fireElemExp)
	{
		if (gameController.coordElemDict.ContainsKey (nextElem))
		{
			GameObject curElem = gameController.coordElemDict [nextElem];
			if (SameType(curElem))
			{
				return fireElemExp;
			}
		}
		return fireElemExp - 1;
	}

}
