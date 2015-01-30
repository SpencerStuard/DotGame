using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance { get; private set; }
	//GAMESTATE
	public enum GameState{Null,MainMenu,Map,Game,PreScreen,PostScreen};
	public GameState CurrentGameState = GameState.Null;

	//EDITOR VARS
	public int NumberOfColors;
	public float CascadeSpeed;
	public float LineResolveSpeed;
	public float LineResolveIncreaseSpeed;
	public float MinimumResolveSpeed;
	public float MaxStrechDistance;
	public float SpacingDistance = 2f;
	//public int Rows;
	//public int Columns;
	public float ScreenEdgePadding;
	public float MapDamening;

	//UI/SCORE VARS
	int LineScore;
	public int TotalScore;
	public GameObject StartDots;

	//POINTER VARS
	public GameObject SpawnerObj;
	public GameObject DrawCubeObj;
	public GameObject NodeObj;
	public GameObject ConnectionLineHolder;
	public GameObject CatchNodeHolder;
	public GameObject MeshHolder;
	public GameObject NodeHolder;
	public GameObject MeshContainer;
	public Material CullingOffMat;
	
	//DYNAMIC VARS
	public Vector3 TouchPosition;
	public int DotName;
	float TempLineResolveSpeed;
	public bool IsResolving = false;
	bool MapDownAndUpFlag = false;
	public GameObject TempDoubleTouchedObj;
	public int TempDoubleIndexNumber;
	
	public GameObject LastSelectedDot;
	public GameObject SecondToLastSelectedDot;
	public GameObject CurrentDrawLine;

	public GameObject CurrentLevel;
	public GameObject CatchNodeRef;

	public float BottomRowValue;
	
	public List<GameObject> Nodes = new List<GameObject>();
	public List<GameObject> Lines = new List<GameObject>();
	public List<GameObject> TotalNodes = new List<GameObject>();

	//EVENT AHNDLER
	public delegate void LassoCheckEvent();
	public event LassoCheckEvent LassoCheck;

	public delegate void PlayGame();
	public event PlayGame KillMainMenu;

	//CONSTANT
	public Vector3 BL;
	public Vector3 TR;
	public Vector3 Origin;

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {


	
		Application.targetFrameRate = 60;
		BL = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f,10f));
		TR = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f,10f));
		Origin = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f,10f));
		ResetScore ();
		LaunchMainMenu ();




	}

	void LaunchMainMenu ()
	{
		CurrentGameState = GameState.MainMenu;
		UIManager.instance.LaunchMainMenuUI ();

		//place dots
		GameObject PlaceStartDots = Instantiate(StartDots,Vector3.up,Quaternion.Euler(0f,0f,0f))as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		 
		if(CurrentGameState == GameState.Game)
		{
			if (Input.GetMouseButtonDown (0) && !IsResolving) 
			{
				ResetTouchVars ();
			}

			if (Input.GetMouseButton (0) && !IsResolving) 
			{
				CheckHit ();
			}

			if (Input.GetMouseButtonUp (0) && !IsResolving) 
			{
				EndRound ();
			}

			if(CurrentDrawLine)
			{
				DrawConnectLine ();
			}
		}

		if(CurrentGameState == GameState.MainMenu)
		{
			if (Input.GetMouseButtonDown (0) && !IsResolving) 
			{
				ResetTouchVars ();
			}
			
			if (Input.GetMouseButton (0) && !IsResolving) 
			{
				CheckHit ();
			}
			
			if (Input.GetMouseButtonUp (0) && !IsResolving) 
			{
				MainMenuEndRound ();
			}
			
			if(CurrentDrawLine)
			{
				DrawConnectLine ();
			}
		}

		if(CurrentGameState == GameState.Map)
		{
			/*
			if (Input.GetMouseButtonDown (0) && !IsResolving) 
			{
				MapDownAndUpFlag = true;
			}

			if (Input.GetMouseButtonUp (0) && !IsResolving) 
			{

			}
			*/

		}
	
	}

	void ResetScore ()
	{
		LineScore = 0;
		TotalScore = 0;
	}

	void SetUpLevelTypeUI (LevelType currentlvltype)
	{

	}


	public void BuildBoard (int LevelNumber)
	{

		int rows = LevelManaer.instance.Levels[LevelNumber].Rows;
		int columns = LevelManaer.instance.Levels[LevelNumber].Columns;
		NumberOfColors = LevelManaer.instance.Levels[LevelNumber].NumberOfColors;
		SetUpLevelTypeUI(LevelManaer.instance.Levels[LevelNumber].MyLevelType);

		//Get boarder units

		//Debug.LogError(BL);
		//Debug.LogError(TR);

		float XScreenDistance = Mathf.Abs(BL.x) + Mathf.Abs(TR.x) - ScreenEdgePadding; //Total usable screen space horizontal
		SpacingDistance = XScreenDistance/columns;

		float xpos = Origin.x + ((columns/2) * SpacingDistance);
		if(columns % 2 == 0)
		{
			xpos -= (SpacingDistance/2);
		}
		float ypos = Origin.z + ((rows/2) * SpacingDistance);
		if(rows % 2 == 0)
		{
			ypos -= (SpacingDistance/2);
		}
		float StartingYpos = ypos;
		float StartingXpos = xpos;

		//Set the bottomchecknumber
		BottomRowValue = Origin.z - ((rows/2) * SpacingDistance);
		if(rows % 2 == 0)
		{
			BottomRowValue += (SpacingDistance/2);
		}


		for(int x = 0; x < columns; x++)
		{
			for(int y= 0; y < rows; y++)
			{
				//Vector3 SpawnPos = new Vector3(xpos, 1, ypos);
				//GameObject NewNode = CreateANewNode(SpawnPos);

				ypos -= SpacingDistance;
			}
			ypos = StartingYpos;
			xpos -= SpacingDistance;
		}

		PlaceSpawners(StartingXpos,StartingYpos,columns);
	}

	void PlaceSpawners(float xpos, float ypos, int columns)
	{
		ypos += SpacingDistance;

		for(int y = 0; y < columns; y++)
		{
				GameObject NewNode = Instantiate(SpawnerObj,new Vector3(xpos, 1, ypos),Quaternion.Euler(90,0,0))as GameObject;
				xpos -= SpacingDistance;
		}
	}

	void ResetTouchVars ()
	{
		if(Lines.Count > 0)
		{
			Lines.Clear();
		}
	}



	void CheckHit ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		TouchPosition = ray.GetPoint(1);

		if (Physics.Raycast(ray, out hit)) 
		{
			if(hit.transform.tag == "Node_01")
			{
				HitNode(hit.transform.gameObject);
			}
		}
	}

	void HitNode (GameObject H)
	{
		//Make sure new dot isn't the one of the last 2 you have touched and is not moving
		if(LastSelectedDot == H || SecondToLastSelectedDot == H || H.GetComponent<NodeManager>().IsMoving )
		{
			return;
		}

		//Drop Connection Line
		//AddNodeToChain
		//CreateNewConnection Node

		//Drop the connection lines as we go
		if(Nodes.Count >= 1)
		{
			//Checkif same color and within the right distance, if not don't match
			if(H.GetComponent<NodeManager>().MyColor != LastSelectedDot.GetComponent<NodeManager>().MyColor 
			   ||
			   Vector3.Distance(H.transform.position, LastSelectedDot.transform.position) > MaxStrechDistance 
			   ||
			   !CheckForPriorConnections(H)
			   )
			{
				return;
			}
			SecondToLastSelectedDot = LastSelectedDot;
			PlaceConnectLine(H);
		}

		H.GetComponent<NodeManager>().IsMatched = true;

		//Tell dots they are connected to each other
		if(H.transform.GetComponent<NodeManager>() && LastSelectedDot != null)
		{
			H.transform.GetComponent<NodeManager>().AddConnectedNodes(LastSelectedDot);
			LastSelectedDot.transform.GetComponent<NodeManager>().AddConnectedNodes(H);
		}

		//Play Sound
		AudioManager.instance.PlayRandomDotSFX();

		//Tween scale when hit
		iTween.PunchScale(H,Vector3.one,2);
		H.GetComponent<NodeManager>().ScaleFillNodeUp();
		H.GetComponent<NodeManager>().PunchScaleComboText();

		LastSelectedDot = H;

		Nodes.Add(H);

		CreateNewConnectCube(H);

	}

	bool CheckForPriorConnections (GameObject H)
	{
		if(LastSelectedDot.GetComponent<NodeManager>().ConnectedNodes.Contains(H))
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	void CreateNewConnectCube (GameObject node)
	{
		CurrentDrawLine = Instantiate(DrawCubeObj, LastSelectedDot.transform.position, Quaternion.identity) as GameObject;
		Lines.Add(CurrentDrawLine);
		CurrentDrawLine.renderer.material.color = ColorManager.instance.PossibleColors[(int)node.transform.GetComponent<NodeManager>().MyColor];
		CurrentDrawLine.transform.parent = ConnectionLineHolder.transform;
	}

	void DrawConnectLine ()
	{
		Vector3 ToPosition = new Vector3(TouchPosition.x, 0.5f, TouchPosition.z);
		Vector3 FromPosition = new Vector3(LastSelectedDot.transform.position.x, 0.5f, LastSelectedDot.transform.position.z);
		//Debug.Log(Vector3.Distance(ToPosition,FromPosition));
		//Position
		CurrentDrawLine.transform.position = (ToPosition + FromPosition)/2;
		//Rotation
		CurrentDrawLine.transform.LookAt(ToPosition);
		//Scale
		float ScaleDist = Vector3.Distance(FromPosition,ToPosition);
		CurrentDrawLine.transform.localScale = new Vector3(.03f,.03f, ScaleDist);
	}

	void PlaceConnectLine(GameObject H)
	{
		Vector3 ToPosition = new Vector3(H.transform.position.x, 0.5f, H.transform.position.z);
		Vector3 FromPosition = new Vector3(LastSelectedDot.transform.position.x, 0.5f, LastSelectedDot.transform.position.z);
		//Position
		CurrentDrawLine.transform.position = (ToPosition + FromPosition)/2;
		//Rotation
		CurrentDrawLine.transform.LookAt(ToPosition);
		//Scale
		float ScaleDist = Vector3.Distance(FromPosition,ToPosition);
		CurrentDrawLine.transform.localScale = new Vector3(.03f,.03f, ScaleDist);
	}
	
	void SetUpShrinkLine(Hashtable hash)
	{
		ShrinkLine((GameObject)hash["GameObject01"],(GameObject)hash["GameObject02"],(GameObject)hash["GameObject03"]);
	}

	void ShrinkLine(GameObject ShrinkingLine ,GameObject MovingDot, GameObject ToDot)
	{
		//Incase it is the last frame bfore it is deleted
		if(ShrinkingLine)
		{
			Vector3 ToPosition = ToDot.transform.position;
			Vector3 FromPosition = MovingDot.transform.position;
			//Position
			ShrinkingLine.transform.position = (ToPosition + FromPosition)/2;
			//Rotation
			ShrinkingLine.transform.LookAt(ToPosition);
			//Scale
			float ScaleDist = Vector3.Distance(FromPosition,ToPosition);
			ShrinkingLine.transform.localScale = new Vector3(.03f,.03f, ScaleDist - .5f);
		}
	}

	void EndRound ()
	{
		IsResolving = true;

		LastSelectedDot = null;
		SecondToLastSelectedDot = null;

		StartLineResolve ();


		//Clear the last none connected line
		if(Lines.Count > 1)
		{
		GameObject TempLastLine = Lines[Lines.Count - 1];
		Lines.Remove(TempLastLine);
		Destroy(TempLastLine);
		}

	}

	void MainMenuEndRound ()
	{
		IsResolving = true;
		
		LastSelectedDot = null;
		SecondToLastSelectedDot = null;

		//Clear the last none connected line
		if(Lines.Count > 2)
		{
			StartLineResolve ();
			GameObject TempLastLine = Lines[Lines.Count - 1];
			Lines.Remove(TempLastLine);
			Destroy(TempLastLine);
			KillMainMenu();
			CurrentGameState = GameState.Map;
		}
		else
		{
			//Debug.Log("Didn't Connect");
			Destroy(CurrentDrawLine);
			CurrentDrawLine = null;
			IsResolving = false;
			
			for(int x = Nodes.Count - 1; x >= 0; x--)
			{
				Nodes[x].GetComponent<NodeManager>().ScaleFillNodeDown();
				Nodes[x].GetComponent<NodeManager>().IsMoving = false;
				Nodes[x].GetComponent<NodeManager>().IsMatched = false;
				Nodes[x].GetComponent<NodeManager>().SetComboTextTo("");
				Nodes[x].GetComponent<NodeManager>().ClearMyConnectedNodes();
			}
			for(int y = Lines.Count - 1; y >= 0; y--)
			{
				GameObject TempLastLine = Lines[y];
				Lines.Remove(TempLastLine);
				Destroy(TempLastLine);
			}
			
			Nodes.Clear();
			Lines.Clear();
		}
	}

	void StartLineResolve ()
	{
		TempLineResolveSpeed = LineResolveSpeed;

		LassoCheck();

		//Debug.LogError("Break");

		foreach(GameObject G in Nodes)
		{
			G.GetComponent<NodeManager>().IsMoving = true;
		}

		if(Nodes.Count > 1)
		{
			DoResolveLine ();
			CurrentDrawLine = null;
		}
		else
		{
			//Debug.Log("Didn't Connect");
			Destroy(CurrentDrawLine);
			CurrentDrawLine = null;
			IsResolving = false;

			if(Nodes.Count > 0)
			{
				Nodes[0].GetComponent<NodeManager>().ScaleFillNodeDown();
				Nodes[0].GetComponent<NodeManager>().IsMoving = false;
				Nodes[0].GetComponent<NodeManager>().IsMatched = false;
				Nodes[0].GetComponent<NodeManager>().SetComboTextTo("");
			}

			Nodes.Clear();
		}
	}

	void CheckForReoccuringNodes ()
	{

		List<int> ListIndexNum = new List<int>();

		for(int x = 1; x < Nodes.Count; x ++)
		{
			if(Nodes[x] == Nodes[0])
			{
				ListIndexNum.Add(x);//Get all the spots the dot is referrenced in the Nodes list
			}
		}

		if(ListIndexNum.Count > 0)
		{
			GameObject NewNode = CreateANewNode(Nodes[ListIndexNum[0]].transform.position);//Position
			//NewNode.transform.position = Nodes[ListIndexNum[0]].transform.position;//Postion
			Nodes[ListIndexNum[0]].GetComponent<NodeManager>().HandOverVars(NewNode);//HandOverVars

			for(int x = 0; x < ListIndexNum.Count; x ++)
			{
				Nodes[ListIndexNum[x]] = NewNode;
			}

		}
	}

	GameObject CreateANewNode (Vector3 pos)
	{
		GameObject NewNode = Instantiate(NodeObj,pos, Quaternion.Euler(90,0,0)) as GameObject;
		NewNode.GetComponent<NodeManager>().PickColor();
		NewNode.transform.parent = NodeHolder.transform;
		NewNode.transform.name = "Node_" + DotName.ToString();
		DotName ++;
		return(NewNode);
	}


	void DoResolveLine ()
	{

		CheckForReoccuringNodes();

		int z = 0;

		//Move nodes
		if(Nodes.Count > 1)
		{
			//Set up parameters for update and complete functions
			Hashtable DrawLineRaram = new Hashtable();
			DrawLineRaram.Add("GameObject01", Lines[z]);
			DrawLineRaram.Add("GameObject02", Nodes[z]);
			DrawLineRaram.Add("GameObject03", Nodes[z + 1]);

			Hashtable RemoveLineAndNodeParam = new Hashtable();
			RemoveLineAndNodeParam.Add("GameObject01", Nodes[z]);
			RemoveLineAndNodeParam.Add("GameObject02", Lines[z]);

			Hashtable Param = new Hashtable();
			Param.Add("time", TempLineResolveSpeed); 
			Param.Add("x", Nodes[z+1].transform.position.x);
			Param.Add("z", Nodes[z+1].transform.position.z);
			Param.Add("y",1f);
			Param.Add("onupdate", "SetUpShrinkLine");
			Param.Add("onupdateparams", DrawLineRaram);
			Param.Add("onupdatetarget", this.gameObject);
			Param.Add("oncomplete","SetUpClearAndKeepResolving");
			Param.Add("oncompleteparams", RemoveLineAndNodeParam);
			Param.Add("oncompletetarget",this.gameObject);
			Param.Add("easetype","spring");

			//Move nodes and shrink line
			iTween.MoveTo(Nodes[z],Param);

			//Add to score
			LineScore += 1;
		}
		else
		{
			FinishLineResolve();
		}
	}

	void SetUpClearAndKeepResolving(Hashtable hash)
	{
		ClearAndKeepResolving((GameObject)hash["GameObject01"],(GameObject)hash["GameObject02"]);
	}

	public void ClearAndKeepResolving (GameObject refNode, GameObject refLine)
	{
		//Increase the collect speed
		if((TempLineResolveSpeed - LineResolveIncreaseSpeed) >= MinimumResolveSpeed)
		{
			TempLineResolveSpeed -= LineResolveIncreaseSpeed;
		}
		
		//Remove Node From List of chained nodes
		GameObject TempLastNode = refNode;
		if(TempDoubleIndexNumber == 0)
		{
			Nodes.Remove(TempLastNode);
			Destroy(TempLastNode);
		}
		else
		{
			//Put the newly created dot in the right spot on the node list
			Nodes[TempDoubleIndexNumber] = TempDoubleTouchedObj;
			TempDoubleIndexNumber = 0;

			Nodes.Remove(TempLastNode);
			Destroy(TempLastNode);
		}
		//Destroy(TempLastNode);

		//Remove Line From List of chained lines
		GameObject TempLastLine = refLine;
		Lines.Remove(TempLastLine);
		Destroy(TempLastLine);
		
		//TODO ADD TO THE CHAIN SCORE

		DoResolveLine();
	}

	void FinishLineResolve ()
	{
		//Set vars on the last one in the line left
		GameObject TempLastNode = Nodes[0];
		TempLastNode.GetComponent<NodeManager>().BottomRowCheck();
		TempLastNode.GetComponent<NodeManager>().IsMatched = false;
		TempLastNode.GetComponent<NodeManager>().IsMoving = false;
		TempLastNode.GetComponent<NodeManager>().ScaleFillNodeDown();
		TempLastNode.GetComponent<NodeManager>().ClearMyConnectedNodes();
		Nodes.Remove(TempLastNode);
		Nodes.Clear();

		//GetScrore
		LineScore += 1; //For last node
		TotalScore += LineScore;

		//Delet the Last Line
		if(Lines.Count >= 1)
		{
			GameObject TempLastLine = Lines[0];
			Lines.Remove(TempLastLine);
			Lines.Clear();
			Destroy(TempLastLine);
		}

		//MakeDraw Line False
		IsResolving = false;
	}

	void TryToDrawSteak ()
	{

	}
}
