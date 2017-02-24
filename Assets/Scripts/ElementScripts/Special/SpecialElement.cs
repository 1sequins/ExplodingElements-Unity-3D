using UnityEngine;
using System.Collections;

public class SpecialElement : Element {

	// Not currently used

	public override bool IsNeighElem (Vector3 curElem, Vector3 possNeighElem)
	{
		decimal curDEA = gameController.dEA;
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
		{
			//Debug.Log (curElem);
			//Debug.Log (possNeighElem);
			//Debug.Log (gameController.dEA);
			//Debug.Log ("not a neigh");
			return false;
		}
	}
}
