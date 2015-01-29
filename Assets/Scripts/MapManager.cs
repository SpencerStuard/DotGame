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

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		// if swipped move
	}
	public void SetUpVars ()
	{
		VerticalSpacing = (GameManager.instance.TR.z - GameManager.instance.Origin.z) / 2; // Get a quarter of the screen height
		StartZpos = GameManager.instance.BL.z + VerticalSpacing;
	}

	public void LaunchMap ()
	{
		float CurrentNodePosZ = StartZpos;
		int LevelNumber = 1;

		for(int x = 0; x < LevelManaer.instance.Levels.Count; x ++)
		{
			//Get position for level node
			Debug.Log(CurrentNodePosZ);
			Vector3 levelnodePos = new Vector3 (0,1, CurrentNodePosZ);

			//Create a levelnode
			GameObject NewLevelNode = Instantiate(MapNodes, levelnodePos, Quaternion.Euler(90,0,0))as GameObject;

			//SET IS MAP NODE
			NewLevelNode.GetComponent<NodeManager>().isMapNode = true;

			//Name the level node
			NewLevelNode.transform.name = LevelNumber.ToString();

			//parent it under this
			NewLevelNode.transform.parent = transform;

			//Set Color based on level type
			switch(LevelManaer.instance.Levels[x].MyLevelType)
			{
			case LevelType.Colections:
				NewLevelNode.GetComponent<NodeManager>().SetColor(NodeColors.Red);
				break;
			case LevelType.Survive:
				NewLevelNode.GetComponent<NodeManager>().SetColor(NodeColors.Orange);
				break;
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
}
