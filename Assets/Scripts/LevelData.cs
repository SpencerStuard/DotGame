using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData : MonoBehaviour {

	public static LevelData instance { get; private set; }

	public List<GameObject> Levels = new List<GameObject>();

	// Use this for initialization
	void Awake () {
		instance = this;
	}
}
