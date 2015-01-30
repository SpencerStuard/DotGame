using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance { get; private set; }

	List<GameObject> UIMenus = new List<GameObject>();

	public GameObject MainMenuUI;
	public GameObject PreLevelUI;
	public GameObject SettingsUI;
	public GameObject MapUI;

	public GameObject MapRef;

	int SelectedLevel;

	public List<GameObject> TitleLetters = new List<GameObject>();

	//MainMenuVars
	public Button SettingsBtn;

	//PRESCREEN VARS
	public Text LevelTitleTxt;
	public Text LevelTypeTxt;
	public Button PlayBtn;
	public Button MapBtn;

	//SettingVars
	public GameObject HomeSettingBtn;

	//StartDots
	public GameObject StartDots;
	public GameObject StartDotsRef;
	

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.KillMainMenu += KillMainMenu;

		UIMenus.Add(MainMenuUI);
		UIMenus.Add(PreLevelUI);
		UIMenus.Add(SettingsUI);
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

		GameManager.instance.CurrentGameState = GameManager.GameState.MainMenu;

		
		//place dots
		GameObject StartDotsRef  = Instantiate(StartDots,Vector3.up,Quaternion.Euler(0f,0f,0f))as GameObject;

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
			G.GetComponent<FadeText>().DoFade(0f,1f,Random.Range(.25f,1f),Random.Range(.25f,1f),G);
		}

		Color SetCol = ColorManager.instance.PossibleColors[Random.Range( (int)NodeColors.First + 1, GameManager.instance.NumberOfColors)];
		SetCol.a = 0;
		SettingsBtn.image.color = SetCol;

		SettingsBtn.gameObject.AddComponent<FadeText>();
		SettingsBtn.GetComponent<FadeText>().DoFade(0f,1f,Random.Range(.25f,1f),Random.Range(.25f,1f),SettingsBtn.gameObject);



		//Fade in Letters Quickly

		//Fade in teh three bottom dots super quick
	}

	public void LaunchMapUI ()
	{
		DisableAllUIMenus();

		//Enable map stuff
		MapRef = Instantiate(MapUI,Vector3.up,Quaternion.Euler(0,0,0)) as GameObject;
		MapRef.GetComponent<MapManager>().SetUpVars();
		MapRef.GetComponent<MapManager>().LaunchMap();

	}

	void KillMainMenu ()
	{
		foreach(GameObject G in TitleLetters)
		{
			G.AddComponent<FadeText>();
			G.GetComponent<FadeText>().DoFade(1f,0f,Random.Range(.5f,1.5f),0,G);
		}

		SettingsBtn.gameObject.AddComponent<FadeText>().DoFade(1,0,.5f,0f,SettingsBtn.gameObject);

		if(GameManager.instance.CurrentGameState == GameManager.GameState.Map)
		{
			object[] parms = new object[2]{"LaunchMapUI", 1.5f};
			StartCoroutine( "DelayFunctionCall",parms);
		}
		if(GameManager.instance.CurrentGameState == GameManager.GameState.Settings)
		{

		}
		else{

		}
		object[] parms2 = new object[2]{"DisableMainMenu", 1.5f};
		StartCoroutine( "DelayFunctionCall",parms2);
	}

	void DisableMainMenu()
	{
		MainMenuUI.SetActive(false);
	}

	void KillMainMenuAndDots ()
	{
		foreach(GameObject G in TitleLetters)
		{
			G.AddComponent<FadeText>();
			G.GetComponent<FadeText>().DoFade(1f,0f,Random.Range(.5f,1.5f),0,G);
		}
		object[] parms = new object[2]{"LaunchMapUI", 1.5f};
		StartCoroutine( "DelayFunctionCall",parms);

		StartDotsRef.GetComponent<StartDots>().FadeAndKillUs();
	}

	public void ClickedOnLevel (int LevNum)
	{
		SelectedLevel = LevNum;
		MapRef.GetComponent<MapManager>().DisableMap();
		//ADD SOUND
		GameManager.instance.CurrentGameState = GameManager.GameState.PreScreen;
		//LoadPreScreen(LevNum);
	}

	public void LoadPreScreen ()
	{
		DisableAllUIMenus();
		PreLevelUI.SetActive(true);

		//SetUpLevelTitle
		LevelTitleTxt.text = "LEVEL " + SelectedLevel.ToString();
		//SetUp Level Type
		switch(LevelManaer.instance.Levels[SelectedLevel - 1].MyLevelType)
		{
		case LevelType.Collect:
			LevelTypeTxt.text = "COLLECT";
			break;
		case LevelType.Survive:
			LevelTypeTxt.text = "SURVIVE";
			break;
		}
		//Fade In Texts
		Color TitleCol = ColorManager.instance.PossibleColors[Random.Range((int)NodeColors.First + 1,GameManager.instance.NumberOfColors)];
		TitleCol.a = 0;
		Color TypeCol = ColorManager.instance.PossibleColors[Random.Range((int)NodeColors.First + 1,GameManager.instance.NumberOfColors)];
		TypeCol.a = 0;

		LevelTitleTxt.color = TitleCol;
		LevelTypeTxt.color = TypeCol;
		LevelTitleTxt.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,0f,LevelTitleTxt.gameObject);
		LevelTypeTxt.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.25f,LevelTypeTxt.gameObject);

		//Color btns;
		Color WhiteA = Color.white;
		WhiteA.a = 0f;
		Color PlayBtnCol = ColorManager.instance.PossibleColors[Random.Range((int)NodeColors.First + 1,GameManager.instance.NumberOfColors)];
		PlayBtnCol.a = 0;
		Color MapBtnCol = ColorManager.instance.PossibleColors[Random.Range((int)NodeColors.First + 1,GameManager.instance.NumberOfColors)];
		MapBtnCol.a = 0;


		PlayBtn.image.color = PlayBtnCol;
		MapBtn.image.color = MapBtnCol;
		foreach(Transform T in PlayBtn.transform)
		{
			T.GetComponent<Text>().color = WhiteA;
			T.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.75f,T.gameObject);
		}
		foreach(Transform T in MapBtn.transform)
		{
			T.GetComponent<Text>().color = WhiteA;
			T.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.75f,T.gameObject);
		}
		PlayBtn.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.5f,PlayBtn.gameObject);
		MapBtn.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.5f,MapBtn.gameObject);
	}

	public void LaunchLevel()
	{
		//FadeEverything Out in .5 Seconds
		LevelTitleTxt.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,0f,LevelTitleTxt.gameObject);
		LevelTypeTxt.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,.0f,LevelTypeTxt.gameObject);

		foreach(Transform T in PlayBtn.transform)
		{
			T.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,.0f,T.gameObject);
		}
		foreach(Transform T in MapBtn.transform)
		{
			T.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,.0f,T.gameObject);
		}
		PlayBtn.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,0f,PlayBtn.gameObject);
		MapBtn.gameObject.AddComponent<FadeText>().DoFade(1f,0f,.4f,0f,MapBtn.gameObject);

		//DelayLaunch Level by .6 seconds

		object[] parms = new object[2]{"BuildLevel", .6f};
		StartCoroutine( "DelayFunctionCall",parms);
	}

	public void LaunchSettingMenu ()
	{

		SettingsUI.SetActive(true);
		HomeSettingBtn.GetComponent<Image>().color = GetColorWithNoAlpha();
		HomeSettingBtn.gameObject.AddComponent<FadeText>().DoFade(0f,1f,.5f,.5f,HomeSettingBtn.gameObject);
		GameManager.instance.CurrentGameState = GameManager.GameState.Settings;
		GameManager.instance.GoToSettingsMenu();

	}

	void BuildLevel ()
	{
		PreLevelUI.SetActive(false);

		GameManager.instance.CurrentGameState = GameManager.GameState.Game;
		GameManager.instance.BuildBoard (SelectedLevel - 1);
	}

	Color GetColorWithNoAlpha ()
	{
		Color TitleCol = ColorManager.instance.PossibleColors[Random.Range((int)NodeColors.First + 1,GameManager.instance.NumberOfColors)];
		TitleCol.a = 0;
		return(TitleCol);
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
