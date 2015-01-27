using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum NodeColors{First,Red,Orange,Yellow,Blue,Green,Purple,Last};

public class NodeManager : MonoBehaviour {

	new public List<GameObject> ConnectedNodes = new List<GameObject>();

	//new public List<Color> PossibleColors = new List<Color>(); //Should Match Order Of Color Enum

	bool BottomRowFlag;
	public bool IsMoving;
	public bool IsMatched = false;
	bool CascadeFlag = false;
	float RayDistance;

	public NodeColors MyColor = NodeColors.First;

	void Awake () {
		BottomRowCheck();
	}

	// Use this for initialization
	void Start () {
		RayDistance = GameManager.instance.SpacingDistance;
	}
	
	// Update is called once per frame
	void Update () {
		if(!BottomRowFlag && !CascadeFlag && !IsMatched)
		{
			CheckForCascade();
		}
	}

	/*
	void OnGUI() {
		GUI.contentColor = Color.black;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Label (new Rect (screenPos.x, screenPos.y * -1, 100, 30), transform.name);
		//MyName.transform.position = transform.position;

	}
	*/

	public void PickColor ()
	{
		MyColor = (NodeColors)Random.Range(((int)NodeColors.First + 1), GameManager.instance.NumberOfColors );
		//Debug.Log((int)MyColor);
		switch(MyColor)
		{
		case NodeColors.Red:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			break;
		case NodeColors.Orange:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			break;
		case NodeColors.Yellow:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			break;
		case NodeColors.Blue:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			break;
		case NodeColors.Green:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			break;
		case NodeColors.Purple:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			break;
		}
	}

	public void SetColor (NodeColors c)
	{

		switch(c)
		{
		case NodeColors.Red:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			MyColor = NodeColors.Red;
			break;
		case NodeColors.Orange:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			MyColor = NodeColors.Orange;
			break;
		case NodeColors.Yellow:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			MyColor = NodeColors.Yellow;
			break;
		case NodeColors.Blue:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			MyColor = NodeColors.Blue;
			break;
		case NodeColors.Green:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			MyColor = NodeColors.Green;
			break;
		case NodeColors.Purple:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			MyColor = NodeColors.Purple;
			break;
		}
	}

	public void ClearMyConnectedNodes ()
	{
		ConnectedNodes.Clear();
	}

	public void AddConnectedNodes (GameObject G)
	{
		if(!ConnectedNodes.Contains(G))
		{
			ConnectedNodes.Add(G);
		}
	}

	void CheckForCascade ()
	{
		//Debug.DrawRay(transform.position, Vector3.back * RayDistance, Color.blue);

		if (Physics.Raycast(transform.position, Vector3.back, RayDistance))
		{

		}
		else{
			CascadeFlag = true;
			Cascade();
		}
	}

	void Cascade ()
	{
		//We are moving now
		IsMoving = true;

		//Set up parameters for update and complete functions

		float zpos = transform.position.z - GameManager.instance.SpacingDistance;
		Hashtable DrawLineRaram = new Hashtable();

		
		Hashtable RemoveLineAndNodeParam = new Hashtable();

		
		Hashtable Param = new Hashtable();
		Param.Add("time", GameManager.instance.CascadeSpeed); 
		Param.Add("z", zpos);

		Param.Add("oncomplete","FinishCascade");
		Param.Add("oncompletetarget",this.gameObject);
		Param.Add("easetype","spring");
		
		//Move nodes and shrink line
		iTween.MoveTo(this.gameObject,Param);
	}

	void FinishCascade ()
	{
		BottomRowCheck();
		CascadeFlag = false;
		IsMoving = false;
	}

	public void BottomRowCheck ()
	{
		if(transform.position.z <= -7)
		{
			BottomRowFlag = true;
		}
	}
	
	public void HandOverVars (GameObject G)
	{
		if (G.GetComponent<NodeManager>())
		{
			G.GetComponent<NodeManager>().MyColor = this.MyColor;
			G.GetComponent<NodeManager>().BottomRowFlag = this.BottomRowFlag;
			G.GetComponent<NodeManager>().IsMatched = this.IsMatched;
			G.GetComponent<NodeManager>().IsMoving = this.IsMoving;
			G.GetComponent<NodeManager>().CascadeFlag = this.CascadeFlag;
			G.GetComponent<NodeManager>().ConnectedNodes = this.ConnectedNodes;
			G.GetComponent<NodeManager>().SetColor(this.MyColor);
		}
	}
}
