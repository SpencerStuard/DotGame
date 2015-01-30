using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LevelStatus {None,Locked,Unlocked,Beat1,Beat2,Beat3};

public class MapManager : MonoBehaviour {

	public GameObject MapNodes;
	public List<GameObject> ListOfLevelNodes = new List<GameObject>();

	public float StartZpos;
	public float VerticalSpacing;
	public float HorizontalSpacing;
	Vector3 lastPos;
	Vector3 delta;
	float ScrollAmount;
	Vector3 MoveAmount;
	float MapDampening;

	float BottomScrollLimit;
	float TopScrollLimit;
	Vector3 LimitSpring;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButtonDown(0) )
		{
			lastPos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
		}
		else if ( Input.GetMouseButton(0) )
		{
			delta = Camera.main.ScreenToWorldPoint( Input.mousePosition ) - lastPos;
			//delta = Camera.main.ScreenToWorldPoint(delta);
			//Debug.Log(delta);
			
			// Do Stuff here

			//Debug.Log( "delta X : " + delta.x );
			//Debug.Log( "delta Y : " + delta.y );
			
			// End do stuff

			lastPos = Camera.main.ScreenToWorldPoint( Input.mousePosition );


		}

		if(transform.position.z <= 0f && transform.position.z >= TopScrollLimit)
		{
			MoveAmount = new Vector3(transform.position.x, transform.position.y, transform.position.z + delta.z);
			transform.position = MoveAmount;
			delta = Vector3.Lerp(delta, Vector3.zero ,Time.deltaTime * MapDampening);
		}
		if(transform.position.z > 0f)
		{
			transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		}
		if(transform.position.z < TopScrollLimit)
		{
			transform.position = new Vector3 (transform.position.x, transform.position.y, TopScrollLimit);
		}
		
		/*
		//ToLow
		LimitSpring = Vector3.zero;
		if(transform.position.z > BottomScrollLimit)
		{
			LimitSpring = new Vector3(0,0, (-.1f * (BottomScrollLimit + Mathf.Abs(transform.position.z))));
			delta += LimitSpring;
		}
		//ToHigh
		if(transform.position.z < TopScrollLimit)
		{
			LimitSpring = new Vector3(0,0, .1f * (TopScrollLimit + Mathf.Abs( transform.position.z)));
			delta += LimitSpring;
		}
		*/
	}
	public void SetUpVars ()
	{
		VerticalSpacing = (GameManager.instance.TR.z - GameManager.instance.Origin.z) / 2; // Get a quarter of the screen height
		StartZpos = GameManager.instance.BL.z + VerticalSpacing;
		MapDampening = GameManager.instance.MapDamening;
		BottomScrollLimit = StartZpos + (VerticalSpacing);
		TopScrollLimit = -1 * ((BottomScrollLimit + (VerticalSpacing * LevelManaer.instance.Levels.Count)) - (VerticalSpacing));

	}

	void Scroll ()
	{

	}

	public void DisableMap ()
	{
		foreach(GameObject G in ListOfLevelNodes)
		{
			iTween.ScaleTo(G, Vector3.zero, 0.25f);
		}

		object[] parms = new object[2]{"MessageLaunchPreScree", .3f};
		StartCoroutine( "DelayFunctionCall",parms);
	}

	void MessageLaunchPreScree ()
	{
		foreach(GameObject G in ListOfLevelNodes)
		{
			Destroy(G);
		}

		UIManager.instance.LoadPreScreen ();
		Destroy(gameObject);
	}

	public void LaunchMap ()
	{
		float CurrentNodePosZ = StartZpos;
		int LevelNumber = 1;

		for(int x = 0; x < LevelManaer.instance.Levels.Count; x ++)
		{
			//Get position for level node
			//Debug.Log(CurrentNodePosZ);
			Vector3 levelnodePos = new Vector3 (0,1, CurrentNodePosZ);

			//Create a levelnode
			GameObject NewLevelNode = Instantiate(MapNodes, levelnodePos, Quaternion.Euler(90,0,0))as GameObject;

			//SET IS MAP NODE
			NewLevelNode.GetComponent<NodeManager>().isMapNode = true;

			//Name the level node
			NewLevelNode.transform.name = LevelNumber.ToString();

			//parent it under this
			NewLevelNode.transform.parent = transform;

			//AddLevelnodes to the master list
			ListOfLevelNodes.Add(NewLevelNode);

			//Set Color based on level type
			switch(LevelManaer.instance.Levels[x].MyLevelType)
			{
			case LevelType.Collect:
				NewLevelNode.GetComponent<NodeManager>().SetColor(NodeColors.Red);
				break;
			case LevelType.Survive:
				NewLevelNode.GetComponent<NodeManager>().SetColor(NodeColors.Orange);
				break;
			}

			//Special Exception for the first Level
			if(PlayerPrefs.GetInt("Level_1") < 1 ||!PlayerPrefs.HasKey("Level_"))
			{
				PlayerPrefs.SetInt("Level_1",1);
			}

			//Look for player prefs and set up locked/unloced/or beat //If beat put score
			if(!PlayerPrefs.HasKey("Level_" + name.ToString()))
			{
				PlayerPrefs.SetInt("Level_" + name.ToString(),0);
			}
			switch(PlayerPrefs.GetInt("Level_" + name.ToString()))
			{
			case 0: //Locked
				NewLevelNode.GetComponent<NodeManager>().MyLevelStatus = LevelStatus.Locked;
				break;
			case 1: //Unlocked
				NewLevelNode.GetComponent<NodeManager>().MyLevelStatus = LevelStatus.Unlocked;
				NewLevelNode.GetComponent<NodeManager>().NodeFill.transform.localScale = Vector3.one;
				break;
			case 2: //Beat Good
				NewLevelNode.GetComponent<NodeManager>().MyLevelStatus = LevelStatus.Beat1;
				NewLevelNode.GetComponent<NodeManager>().NodeFill.transform.localScale = Vector3.one;
				NewLevelNode.GetComponent<NodeManager>().SetComboTextTo("1");
				break;
			case 3: //Beat Well
				NewLevelNode.GetComponent<NodeManager>().MyLevelStatus = LevelStatus.Beat2;
				NewLevelNode.GetComponent<NodeManager>().NodeFill.transform.localScale = Vector3.one;
				NewLevelNode.GetComponent<NodeManager>().SetComboTextTo("2");
				break;
			case 4: //Beat Great
				NewLevelNode.GetComponent<NodeManager>().MyLevelStatus = LevelStatus.Beat3;
				NewLevelNode.GetComponent<NodeManager>().NodeFill.transform.localScale = Vector3.one;
				NewLevelNode.GetComponent<NodeManager>().SetComboTextTo("3");
				break;

			}



			//Scale down and bounce in
			NewLevelNode.transform.localScale = Vector3.zero;
			iTween.ScaleTo(NewLevelNode, Vector3.one, 0.25f);


			//Set Up level Number

			//Make Connection if beat

			//Increase Zpos
			CurrentNodePosZ += VerticalSpacing;
			LevelNumber ++;

		}

		//SetUpCollider
		BoxCollider B = gameObject.AddComponent<BoxCollider>();
		Vector3 ColliderPos = new Vector3 (0,0, (StartZpos + ((LevelManaer.instance.Levels.Count * VerticalSpacing)/2) - 2.5f));
		Vector3 ColliderScale = new Vector3 (20f,0,((LevelManaer.instance.Levels.Count) * VerticalSpacing)+ 20f);
		B.center = ColliderPos;
		B.size = ColliderScale;

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
