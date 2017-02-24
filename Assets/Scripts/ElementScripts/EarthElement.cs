using UnityEngine;
using System.Collections;

public class EarthElement : Element {

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		elemType = "earth";

	}

	public override void TriggerSecExp (Vector2 initDir, Vector3 locExp1, Vector3 locExp2, GameObject secExpElem)
	{
		Vector3 secPos = secExpElem.GetComponent<Element> ().initialPos;
		// Negative iteration of offset
		locExp1 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)(Mathf.Abs (initDir.x) - 1)))
			 , (float)((decimal)secPos.y + (gameController.dEA * (decimal)(Mathf.Abs (initDir.y) - 1)))
			 , secPos.z);
		// Positive iteration of offset
		locExp2 = new Vector3 
			((float)((decimal)secPos.x + (gameController.dEA * (decimal)((Mathf.Abs (initDir.x) - 1) * -1)))
			 , (float)((decimal)secPos.y + (gameController.dEA * (decimal)((Mathf.Abs (initDir.y) - 1) * -1)))
			 , secPos.z);

		base.TriggerSecExp (initDir, locExp1, locExp2, secExpElem);

	}
}