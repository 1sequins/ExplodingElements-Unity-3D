using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GOShuffleBag {

	private System.Random random;
	private List<GameObject> data;

	GameController gameController;

	private GameObject currentItem;
	private int currentPosition = -1;

	private int Capacity { get { return data.Capacity; } }
	public int Size { get { return data.Count; } }

	public GOShuffleBag(int initCapacity, GameController gameCont)
	{
		data = new List<GameObject>(initCapacity);
		gameController = gameCont;
		random = new System.Random(gameController.seed);
	}


	public void Add(GameObject item, int amount)
	{
		for (int i = 0; i < amount; i++)
			data.Add(item);

		currentPosition = Size - 1;
	}

	public void AddList(List<GameObject> itemList, int amount)
	{
		Random.seed = gameController.seed;
		for (int i = 0; i < amount; i++)
			data.Add (itemList [Random.Range (0, itemList.Count)]);

		currentPosition = Size - 1;

	}


	public GameObject Next()
	{
		if (currentPosition < 1)
		{
			currentPosition = Size - 1;
			currentItem = data[0];

			return currentItem;
		}

		int pos = random.Next(currentPosition);

		currentItem = data[pos];
		data[pos] = data[currentPosition];
		data[currentPosition] = currentItem;
		currentPosition--;

		return currentItem;
	}

}
