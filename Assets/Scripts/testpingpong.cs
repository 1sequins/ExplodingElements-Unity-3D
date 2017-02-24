using UnityEngine;
using System.Collections;

public class testpingpong : MonoBehaviour {

	Vector3 initialPos;

	// Use this for initialization
	void Awake () {
		initialPos = transform.position;
	}

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (Mathf.PingPong (Time.time, 1) + initialPos.x - 0.5f, transform.position.y, transform.position.z);
	}
}
