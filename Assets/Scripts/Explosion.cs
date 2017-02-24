using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	private ParticleSystem ps;

	// Use this for initialization
	void Start (){
		Destroy (gameObject, GetComponent<ParticleSystem> ().duration);

	}
}
