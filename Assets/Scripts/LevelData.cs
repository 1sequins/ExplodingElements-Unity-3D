using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelData : MonoBehaviour {

	GameController gameController;

	public GameObject menuUI;
	public GameObject gameUI;

	public string dataGoalText;
	public float dataTimeToFinish;

	public int elemsPerColumn = 14;
	public int elemsPerRow = 8;

	void Start ()
	{
		gameController = this.gameObject.GetComponent<GameController> ();
		gameUI.SetActive (false);
		gameController.enabled = false;

	}

	public void InitializeGame ()
	{
		gameController.enabled = true;
		menuUI.SetActive (false);
		gameUI.SetActive (true);
		gameController.StartGame ();
	}

	public void PointGoal ()
	{
		dataGoalText = "Goal: " + gameController.goal.ToString ();
		dataTimeToFinish = 40f;

	}

	public void NoBarrierGoal ()
	{
		dataGoalText = "Goal: Destroy all Barriers";
		dataTimeToFinish = 120f;

	}
	

}
