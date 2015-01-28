using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance { get; private set; }

	List<GameObject> UIMenus = new List<GameObject>();

	public GameObject MainMenuUI;

	public List<GameObject> TitleLetters = new List<GameObject>();


	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {

		UIMenus.Add(MainMenuUI);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisableAllUIMenus ()
	{
		foreach(GameObject G in UIMenus)
		{
			G.SetActive(false);
		}
	}

	public void LaunchMainMenuUI ()
	{
		MainMenuUI.SetActive(true);

		//Set all the letters of dots to 0
		//Set all their colors
		foreach(GameObject G in TitleLetters)
		{
			int c = Random.Range( (int)NodeColors.First + 1, GameManager.instance.NumberOfColors);
			Color col = ColorManager.instance.PossibleColors[c];
			col.a = 0;

			G.GetComponent<Text>().color = col;

			G.AddComponent<FadeText>();
			G.GetComponent<FadeText>().DoFadeText(0f,1f,Random.Range(.5f,1.5f),Random.Range(.5f,1.5f),G);
		}




		//Fade in Letters Quickly

		//Fade in teh three bottom dots super quick
	}
}
