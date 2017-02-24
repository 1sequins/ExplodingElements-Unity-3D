using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterElement : Element {

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "water";

	}

	/* Explosions in water happen very differently in code.
	 * They all originate from the second explosion (the elem that the first elem is dragged onto).
	 * The script that makes it all happen is attached to that elem.
	 * 
	 */

	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{
		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		int waterElemExp = 0;
		locExp1 = new Vector3 ((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x)), 
			(float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y)), 
			secPos.z);
		locExp2 = new Vector3 ((float)((decimal)secPos.x + (gameController.dEA * 2 * (decimal)initDir.x)), 
			(float)((decimal)secPos.y + (gameController.dEA * 2 * (decimal)initDir.y)), 
			secPos.z);

		List<Vector3> locExps = new List<Vector3> {
			locExp1,
			locExp2
		};

		foreach (Vector3 locExp in locExps)
		{
			if (gameController.coordElemDict.ContainsKey (locExp))
			{
				GameObject curElem = gameController.coordElemDict [locExp];
				//Debug.Log (curElem.GetComponent<Element> ().initialPos);
				HighlightElem (curElem);
				curElem.GetComponent<Element> ().StartCoroutine ("StressJig");

				if (SameType(curElem))
				{
					waterElemExp += 1;
					attackedElems.Add(curElem);
				}
				else if (gameController.Opp (curElem.GetComponent<Element> ().elemType) == elemType)
				{
					attackedBarrToBe.Add (curElem);
					SetOppJig (curElem);
				}
				/*if (gameController.coordElemDict [locExp1].GetComponent<Element> ().elemType != elemType) 
				{
					waterElemExp -= 1;
				}
				else 
				{
					waterElemExp += 1;
				}*/// Can add this to make water more powerful, need to add below as well (2 places)
				else
				{
					attackedElems.Add(curElem);
				}
				// To continue explosion
				if (locExp == locExp2)
				{
					TriggerWaterExp (locExp2, initDir, waterElemExp);
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

	}

	void TriggerWaterExp (Vector3 prevElemLoc, Vector2 initDir, int waterElemExp)
	{
		Vector3 locExp = new Vector3 ((float)((decimal)prevElemLoc.x + (gameController.dEA * (decimal)initDir.x)), 
			(float)((decimal)prevElemLoc.y + (gameController.dEA * (decimal)initDir.y)), 
			prevElemLoc.z);
		if (waterElemExp == 0)
		{
			return;
		}
		// the case where there is still water to burn, but no element
		else if (gameController.coordElemDict.ContainsKey (locExp) == false)
		{
			// check if there is a barrier
			if (gameController.coordBarrDict.ContainsKey (locExp))
			{
				if (AttackBarrier (locExp))
				{
					attackedBarrs.Add (gameController.coordBarrDict [locExp]);
					gameController.coordBarrDict [locExp].GetComponent<Barrier> ().HighlightBarr ();
				}
			}
			TriggerWaterExp (locExp, initDir, waterElemExp - 1);
		}
		else 
		{
			GameObject newElem = gameController.coordElemDict [locExp];
			//Debug.Log (newElem.GetComponent<Element> ().initialPos);
			// Assign highlighted material
			HighlightElem (newElem);
			newElem.GetComponent<Element> ().StartCoroutine ("StressJig");

			if (gameController.Opp (newElem.GetComponent<Element> ().elemType) == elemType)
			{
				attackedBarrToBe.Add (newElem);
				SetOppJig (newElem);
				TriggerWaterExp (locExp, initDir, waterElemExp - 1);
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
			}
			else if (SameType(newElem)) 
			{
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
				TriggerWaterExp (locExp, initDir, waterElemExp);
				attackedElems.Add (newElem);
			} 
			else 
			{
				//Debug.Log (newElem.GetComponent<Element> ().initialPos);
				TriggerWaterExp (locExp, initDir, waterElemExp - 1); //Make this +1 to make water more powerful
				attackedElems.Add (newElem);
			}
		}

	}


}
