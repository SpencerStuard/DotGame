using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManaer : MonoBehaviour {

	public static LevelManaer instance { get; private set; }
	
	public List<LevelData> Levels = new List<LevelData>();
	
	// Use this for initialization
	void Awake () {
		instance = this;
	}
}
