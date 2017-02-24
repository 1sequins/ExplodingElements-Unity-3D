using UnityEngine;
using System.Collections;

public class Barrier : MonoBehaviour {

	public Material initialMat;
	public Material highMat;

	public Vector3 initialPos;

	public bool highlighted;

	// Use this for initialization
	void Awake () {

		initialPos = transform.position;

		initialMat = GetComponent<Renderer> ().material;

		Color newColor = GetComponent<Renderer> ().material.GetColor ("_EmissionColor") + new Color(0.20f, 0.20f, 0.13f, 0f);
		highMat = new Material (GetComponent<Renderer>().material.shader);
		highMat.EnableKeyword ("_EMISSION");
		highMat.SetColor ("_EmissionColor", newColor);

		highlighted = false;

	}

	// Different than HighlightElem, this always calls on its own gameobject :)
	public void HighlightBarr ()
	{
		highlighted = true;
		//GetComponent<Renderer> ().material = highMat;
		StartCoroutine ("StressJig");
	}
		
	public IEnumerator StressJig (){
		Debug.Log ("stress jig");
		while (highlighted == true)
		{
			transform.position = new Vector3 (Mathf.PingPong (Time.time / 2f, 0.05f) + initialPos.x - 0.025f, transform.position.y, transform.position.z);
			yield return new WaitForEndOfFrame ();
		}
	}

}
