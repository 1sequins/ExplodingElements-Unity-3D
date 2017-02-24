using UnityEngine;
using System.Collections;

public class MoonElement : AdvElement {

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "moon";

	}

	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{
		//Debug.Log (initDir);
		int moonExpCount = 2;
		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		MoonExp (initDir, secPos, moonExpCount);
		// Positive iteration of offset
		/*locExp1 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x) + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * -1)))
				, (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y) + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * -1)))
				, secPos.z);
		
		// Negative iteration of offset
		locExp2 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x) + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * 1)))
				, (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y) + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * 1)))
				, secPos.z);
		*/
	}

	public void MoonExp (Vector2 initDir, Vector3 prevElemLoc, int moonExpCount)
	{
		//Debug.Log (moonExpCount);
		if (moonExpCount == 0)
			return;

		Vector3 nextExp = new Vector3 
			((float)((decimal)prevElemLoc.x + (gameController.dEA * (decimal)initDir.x))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * -1)))
			 , (float)((decimal)prevElemLoc.y + (gameController.dEA * (decimal)initDir.y))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * -1)))
			 , prevElemLoc.z);

		if (gameController.coordElemDict.ContainsKey (nextExp) == false)
		{
			Debug.Log ("no elem");
			// check if there is a barrier or else there must be nothing
			if (gameController.coordBarrDict.ContainsKey (nextExp))
			{
				if (AttackBarrier (nextExp))
				{
					attackedBarrs.Add (gameController.coordBarrDict [nextExp]);
					gameController.coordBarrDict [nextExp].GetComponent<Barrier> ().HighlightBarr ();
				}
			}
			MoonExp (initDir, nextExp, moonExpCount - 1);
		}
		else 
		{
			GameObject newElem = gameController.coordElemDict [nextExp];
			//Debug.Log (newElem.GetComponent<Element> ().initialPos);
			// Assign highlighted material
			HighlightElem (newElem);
			newElem.GetComponent<Element> ().StartCoroutine ("StressJig");

			if (gameController.Opp (newElem.GetComponent<Element> ().elemType) == elemType)
			{
				attackedBarrToBe.Add (newElem);
				SetOppJig (newElem);
				MoonExp (initDir, nextExp, moonExpCount - 1);
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
			}
			else if (!SameType(newElem)) 
			{
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
				MoonExp (initDir, nextExp, moonExpCount - 1);
				attackedElems.Add (newElem);
			} 
			else 
			{
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
				MoonExp (initDir, nextExp, moonExpCount); //Make this +1 to make moon more powerful
				attackedElems.Add (newElem);
			}
		}


	}


}
