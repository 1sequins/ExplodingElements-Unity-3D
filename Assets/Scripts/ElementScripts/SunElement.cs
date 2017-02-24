using UnityEngine;
using System.Collections;

public class SunElement : AdvElement {

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "sun";

	}

	// Sun operates differently than fire, its more like a controlled blast, wheras fire spreads out to the sides more in a traditional cone
		// can keep the way it is, or decide that one is better through playtesting and change to correct

	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{

		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		// starts at 1 to attack the locExp2
		int sunElemExp = 1;
		locExp1 = new Vector3 ((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x)), 
		                       (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y)), 
		                       secPos.z);

		AttackSunElem (locExp1);
		if (gameController.coordElemDict.ContainsKey (locExp1))
		{
			GameObject curElem = gameController.coordElemDict [locExp1];
			if (SameType(curElem))
			{
				sunElemExp += 1;
			}
		}
		//Debug.Log (secPos);
		//Debug.Log (fireElemExp);
		TriggerSunExp (secPos, secPos, new Vector3 (initDir.x, initDir.y, 0), sunElemExp);
	}

	public void TriggerSunExp (Vector3 prevElemLoc, Vector3 secExpElem, Vector3 initDir3, int sunElemCount)
	{
		//Debug.Log ("triggerfire " + fireElemExp.ToString ());

		//Debug.Log (sunElemCount);
		if (sunElemCount <= 0)
		{
			return;
		}

		// Terminology is for horizontal initial directions
		decimal xAway;
		decimal yAway;
		xAway = (decimal)Mathf.Abs((float)(((decimal)secExpElem.x - (decimal)prevElemLoc.x)/0.6m));
		yAway = (decimal)Mathf.Abs((float)(((decimal)secExpElem.y - (decimal)prevElemLoc.y)/0.6m));

		//Debug.Log (initDir3);
		//Debug.Log (secExpElem);
		//Debug.Log (prevElemLoc);
		//Debug.Log (sidewaysFromSecElem);


		Vector3 nextElemLoc;

		if (secExpElem == prevElemLoc)
		{
			nextElemLoc = new Vector3 
				((float)((decimal)secExpElem.x + (gameController.dEA * (decimal)initDir3.x * 2m))
				 , (float)((decimal)secExpElem.y + (gameController.dEA * (decimal)initDir3.y * 2m))
				 , secExpElem.z);
		}
		else if (xAway == yAway)
		{
			// Does this when it is even diagonally
			//Debug.Log("diagonal");
			nextElemLoc = new Vector3 
				((float)((decimal)prevElemLoc.x - (gameController.dEA * (decimal)initDir3.x))
				 , (float)((decimal)prevElemLoc.y - (gameController.dEA * (decimal)initDir3.y * 2m))
				 , prevElemLoc.z);
		}
		else if (xAway > yAway)
		{
			// Does this when it is on the lower y side of the beam
			//Debug.Log("x greater than y");
			nextElemLoc = new Vector3 
				((float)((decimal)prevElemLoc.x - (gameController.dEA * (decimal)initDir3.x * (xAway - yAway)))
				 , (float)((decimal)prevElemLoc.y + (gameController.dEA * (decimal)initDir3.y * (xAway - yAway)))
				 , prevElemLoc.z);
		}

		else if (yAway - xAway== 1m)
		{
			// Does this when it is on the upper side of y, but not time to jump forward
			//Debug.Log ("first upper side");
			nextElemLoc = new Vector3 
				((float)((decimal)prevElemLoc.x + (gameController.dEA * (decimal)initDir3.x * 2m))
				 , (float)((decimal)prevElemLoc.y - (gameController.dEA * (decimal)initDir3.y))
				 , prevElemLoc.z);
		}
		else
		{
			// When on the upper y side, of the upper two elems there, and needs to jump forwards
			//Debug.Log ("second upper side");
			nextElemLoc = new Vector3 
				((float)((decimal)prevElemLoc.x + (gameController.dEA * (decimal)initDir3.x * 3m))
				 , (float)((decimal)prevElemLoc.y + (gameController.dEA * (decimal)initDir3.y))
				 , prevElemLoc.z);
		}
		//Debug.Log ("lay the attack");
		//Debug.Log (nextElemLoc);
		sunElemCount = TriggerSunCount (nextElemLoc, sunElemCount);
		AttackSunElem (nextElemLoc);
		TriggerSunExp (nextElemLoc, secExpElem, initDir3, sunElemCount);

	}




	public void AttackSunElem (Vector3 locExp)
	{
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

	int TriggerSunCount (Vector3 nextElem, int sunElemExp)
	{
		if (gameController.coordElemDict.ContainsKey (nextElem))
		{
			GameObject curElem = gameController.coordElemDict [nextElem];
			if (SameType(curElem))
			{
				return sunElemExp;
			}
		}
		return sunElemExp - 1;
	}

}
