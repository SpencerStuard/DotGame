using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance { get; private set; }

	List<GameObject> UIMenus = new List<GameObject>();

	public GameObject MainMenuUI;
	public GameObject MapUI;

	public GameObject MapRef;

	public List<GameObject> TitleLetters = new List<GameObject>();


	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.KillMainMenu += KillMainMenu;

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
		DisableAllUIMenus();

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
			G.GetComponent<FadeText>().DoFade(0f,1f,Random.Range(.5f,1.5f),Random.Range(.5f,1.5f),G);
		}




		//Fade in Letters Quickly

		//Fade in teh three bottom dots super quick
	}

	void LaunchMapUI ()
	{
		DisableAllUIMenus();

		//Enable map stuff
		MapRef = Instantiate(MapUI,Vector3.up,Quaternion.Euler(0,0,0)) as GameObject;
		MapRef.GetComponent<MapManager>().SetUpVars();
		MapRef.GetComponent<MapManager>().LaunchMap();

		Debug.Log("Made it here");
	}

	void KillMainMenu ()
	{
		foreach(GameObject G in TitleLetters)
		{
			G.AddComponent<FadeText>();
			G.GetComponent<FadeText>().DoFade(1f,0f,Random.Range(.5f,1.5f),0,G);
		}
		object[] parms = new object[2]{"LaunchMapUI", 1.5f};
		StartCoroutine( "DelayFunctionCall",parms);
	}

	public void ClickedOnLevel (int LevNum)
	{
		MapRef.GetComponent<MapManager>().DisableMap();
		//ADD SOUND
		GameManager.instance.CurrentGameState = GameManager.GameState.PreScreen;
		LoadPreScreen(LevNum);
	}

	void LoadPreScreen (int LevNum)
	{

	}
	

	IEnumerator DelayFunctionCall (object[] parms)
	{

		string s = (string)parms[0]; 
		float dt = (float)parms[1];

		float timer = dt; 
		
		while (timer > 0)
		{
			//Debug.Log("Made it here");
			yield return null; 
			timer -= Time.deltaTime; 
		}
		
		this.gameObject.SendMessage(s); 
		 
	}

}
