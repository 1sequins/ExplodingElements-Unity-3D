using UnityEngine;
using System.Collections;

public class WoodElement : AdvElement {

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "wood";

	}

	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{
		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		// Positive iteration of offset
		locExp1 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * -1)))
			 , (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y * -1))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * -1)))
			 , secPos.z);
		// Negative iteration of offset
		locExp2 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)initDir.x * -1))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * 1)))
			 , (float)((decimal)secPos.y + (gameController.dEA * (decimal)initDir.y))// + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * 1)))
			 , secPos.z);

		base.TriggerSecExp (initDir, locExp1, locExp2, secExpElem);

	}


}
