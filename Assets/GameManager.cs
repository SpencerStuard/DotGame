using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance { get; private set; }

	public float CascadeSpeed;
	public float LineResolveSpeed;
	public float LineResolveIncreaseSpeed;
	public float MinimumResolveSpeed;
	public float MaxStrechDistance;
	public float SpacingDistance = 2f;

	int LineScore;
	public int TotalScore;


	float TempLineResolveSpeed;

	public GameObject SpawnerObj;
	public GameObject DrawCubeObj;
	public GameObject NodeObj;
	public GameObject TempDoubleTouchedObj;
	public int TempDoubleIndexNumber;

	public GameObject LastSelectedDot;
	public GameObject SecondToLastSelectedDot;
	public GameObject CurrentDrawLine;

	public GameObject ConnectionLineHolder;
	public GameObject CatchNodeHolder;
	public GameObject MeshHolder;
	public GameObject NodeHolder;

	public GameObject CurrentLevel;
	public GameObject CatchNodeRef;


	public bool DrawingLine = false;

	public List<GameObject> Nodes = new List<GameObject>();
	public List<GameObject> Lines = new List<GameObject>();
	public List<GameObject> TotalNodes = new List<GameObject>();

	public Vector3 TouchPosition;

	void Awake ()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
	
		Application.targetFrameRate = 60;
		BuildBoard (8,5);
		ResetScore ();
		//PickLevel ();

	}
	
	// Update is called once per frame
	void Update () {
		 
		if (Input.GetMouseButton (0)) 
		{
			CheckHit ();
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			EndRound ();
		}

		if(CurrentDrawLine)
		{
			DrawConnectLine ();
		}
	
	}

	void ResetScore ()
	{
		LineScore = 0;
		TotalScore = 0;
	}


	void BuildBoard (int Rows, int Columns)
	{
		float xpos = 4;
		float ypos = 7;
		float StartingYpos = ypos;
		float StartingXpos = xpos;



		for(int x = 0; x < Columns; x++)
		{
			for(int y= 0; y < Rows; y++)
			{
				GameObject NewNode = Instantiate(NodeObj,new Vector3(xpos, 1, ypos),Quaternion.Euler(90,0,0))as GameObject;
				NewNode.transform.parent = NodeHolder.transform;
				NewNode.GetComponent<NodeManager>().PickColor();

				ypos -= SpacingDistance;
			}
			ypos = StartingYpos;
			xpos -= SpacingDistance;
		}

		PlaceSpawners(StartingXpos,StartingYpos,Columns);
	}

	void PlaceSpawners(float xpos, float ypos, int Columns)
	{
		ypos += SpacingDistance;

		for(int y = 0; y < Columns; y++)
		{
				GameObject NewNode = Instantiate(SpawnerObj,new Vector3(xpos, 1, ypos),Quaternion.Euler(90,0,0))as GameObject;
				//NewNode.transform.parent = NodeHolder.transform;				
				xpos -= SpacingDistance;
		}
	}

	void PickLevel ()
	{
		if(!CurrentLevel)
		{
			GameObject TempCurrentLevel = LevelData.instance.Levels[Random.Range(0, LevelData.instance.Levels.Count)];
			//Debug.Log(TempCurrentLevel);
			CurrentLevel = Instantiate(TempCurrentLevel, Vector3.zero, Quaternion.identity) as GameObject;
		}

		SpwanCatchNodes ();
	}

	void SpwanCatchNodes ()
	{
		foreach(Transform T in CatchNodeHolder.transform)
		{
			Destroy(T.gameObject);
		}

		for(int x = 0; x < 5; x++)
		{
			GameObject NewCatchNode = Instantiate(CatchNodeRef, new Vector3(Random.Range(-3,3),1,Random.Range(-6,6)),Quaternion.identity) as GameObject;
			NewCatchNode.transform.GetComponent<CatchNode>().SetUpCatchNode(0);
			NewCatchNode.transform.parent = CatchNodeHolder.transform;
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
		//Make sure new dot isn't the one of the last 2 you have touched
		if(LastSelectedDot == H || SecondToLastSelectedDot == H)
		{
			return;
		}

		//Start flag that we are drawing a line
		if(Nodes.Count == 0)
		{
			DrawingLine = true;
		}

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

		H.GetComponent<NodeManager>().IsMoving = true;

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
		DrawingLine = false;

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

	void StartLineResolve ()
	{
		TempLineResolveSpeed = LineResolveSpeed;
		if(Nodes.Count > 1)
		{
			DoResolveLine ();
			CurrentDrawLine = null;
		}
		else
		{
			Debug.Log("Didn't Connect");
			Destroy(CurrentDrawLine);
			CurrentDrawLine = null;
			Nodes.Clear();
		}
	}


	void DoResolveLine ()
	{

		int z = 0;

		//Check if this node has been hit twice
		TempDoubleIndexNumber = 0;
		for(int x = 1; x < Nodes.Count; x ++)
		{
			if(Nodes[x] == Nodes[z])
			{
				TempDoubleIndexNumber = x;
			}
		}


		if(TempDoubleIndexNumber != 0)
		{
			GameObject newdot = Instantiate(NodeObj,Nodes[z].transform.position, Nodes[z].transform.rotation) as GameObject;
			newdot.GetComponent<NodeManager>().SetColor(Nodes[z].GetComponent<NodeManager>().MyColor);
			newdot.GetComponent<NodeManager>().IsMoving = true;
			TempDoubleTouchedObj = newdot;
		}

		
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
			Nodes[TempDoubleIndexNumber] = TempDoubleTouchedObj;
			TempDoubleIndexNumber = 0;

			Nodes.Remove(TempLastNode);
			Destroy(TempLastNode);
		}
		Destroy(TempLastNode);

		//Remove Line From List of chained lines
		GameObject TempLastLine = refLine;
		Lines.Remove(TempLastLine);
		Destroy(TempLastLine);
		
		//TODO ADD TO THE CHAIN SCORE

		DoResolveLine();
	}

	void FinishLineResolve ()
	{
		//Set is moving to false
		GameObject TempLastNode = Nodes[0];
		TempLastNode.GetComponent<NodeManager>().BottomRowCheck();
		TempLastNode.GetComponent<NodeManager>().IsMoving = false;
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
		DrawingLine = false;
	}

	void TryToDrawSteak ()
	{

	}
}
