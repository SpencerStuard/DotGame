using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour {

	public static ColorManager instance { get; private set; }

	new public List<Color> PossibleColors = new List<Color>();

	// Use this for initialization
	void Awake () {
		instance = this;
	}

}
