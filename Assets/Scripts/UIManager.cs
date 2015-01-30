using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager instance { get; private set; }

	List<GameObject> UIMenus = new List<GameObject>();

	public GameObject MainMenuUI;
	public GameObject PreLevelUI;
	public GameObject MapUI;

	public GameObject MapRef;

	int SelectedLevel;

	public List<GameObject> TitleLetters = new List<GameObject>();

	//PRESCREEN VARS
	public Text LevelTitleTxt;
	public Text LevelTypeTxt;
	public Button PlayBtn;
	public Button MapBtn;


	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {

		GameManager.instance.KillMainMenu += KillMainMenu;

		UIMenus.Add(MainMenuUI);
		UIMenus.Add(PreLevelUI);
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

	void BuildLevel ()
	{
		GameManager.instance.CurrentGameState = GameManager.GameState.Game;
		GameManager.instance.BuildBoard (SelectedLevel - 1);
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
