using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum NodeColors{First,Red,Orange,Yellow,Blue,Green,Purple,Last};

public class NodeManager : MonoBehaviour {

	//POINTER VARS
	public Text ComboText;
	public GameObject NodeFill;

	new public List<GameObject> ConnectedNodes = new List<GameObject>();
	
	bool PowerUp = false;
	bool BottomRowFlag;
	float BottomValue;
	public bool IsMoving = false;
	public bool IsMatched = false;
	bool CascadeFlag = false;
	float RayDistance;
	bool isFirstFrame = true;
	public LevelStatus MyLevelStatus;
	public bool isMapNode;
	

	public NodeColors MyColor = NodeColors.First;

	void Awake () {
		if(!isMapNode)
		{
			BottomRowCheck();
		}
	}

	// Use this for initialization
	void Start () {
		if(!IsMatched)
		{
			NodeFill.transform.localScale = Vector3.zero;
		}
		ComboText.text = "";
		RayDistance = GameManager.instance.SpacingDistance;
		GameManager.instance.LassoCheck += CheckIfIAmLassoed;
		BottomValue = GameManager.instance.BottomRowValue;
		//BottomRowCheck();
		//Debug.LogError("Check Flag");
	}

	void OnDestroy ()
	{
		GameManager.instance.LassoCheck -= CheckIfIAmLassoed;
	}
	
	// Update is called once per frame
	void Update () {
		if(!BottomRowFlag && !CascadeFlag && !IsMatched && !isMapNode)
		{
			//Debug.Log("Made it here");
			CheckForCascade();
		}
		if(IsMatched)
		{
			ComboText.text = ConnectedNodes.Count.ToString();
		}
	}

	void OnMouseDown ()
	{
		if(isMapNode)
		{
			UIManager.instance.ClickedOnLevel( int.Parse( transform.name ));
		}
	}

	public void PunchScaleComboText ()
	{
		iTween.PunchScale(ComboText.gameObject, Vector3.one, .5f);
		//iTween.PunchRotation(ComboText.gameObject, Vector3.forward * 360, 1);
	}

	public void SetComboTextTo(string s)
	{
		ComboText.text = s;
	}

	public void ScaleFillNodeUp ()
	{
		iTween.ScaleTo(NodeFill, Vector3.one, 1f);
	}

	public void ScaleFillNodeDown ()
	{
		iTween.ScaleTo(NodeFill, Vector3.zero, 1f);
	}

	public void SetFillToFullScale ()
	{
		NodeFill.transform.localScale = Vector3.one;
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
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			break;
		case NodeColors.Orange:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			break;
		case NodeColors.Yellow:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			break;
		case NodeColors.Blue:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			break;
		case NodeColors.Green:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			break;
		case NodeColors.Purple:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			break;
		}
	}

	public void SetColor (NodeColors c)
	{
		switch(c)
		{
		case NodeColors.Red:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			MyColor = NodeColors.Red;
			break;
		case NodeColors.Orange:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Orange];
			MyColor = NodeColors.Orange;
			break;
		case NodeColors.Yellow:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Yellow];
			MyColor = NodeColors.Yellow;
			break;
		case NodeColors.Blue:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Blue];
			MyColor = NodeColors.Blue;
			break;
		case NodeColors.Green:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Green];
			MyColor = NodeColors.Green;
			break;
		case NodeColors.Purple:
			transform.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			NodeFill.GetComponent<SpriteRenderer>().color = ColorManager.instance.PossibleColors[(int)NodeColors.Purple];
			MyColor = NodeColors.Purple;
			break;
		}
	}

	public void ClearMyConnectedNodes ()
	{
		ConnectedNodes.Clear();
		ComboText.text = "";
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
		Debug.DrawRay(transform.position, Vector3.back * RayDistance, Color.blue);

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
		//Debug.Log(GameManager.instance.BottomRowValue);
		if(transform.position.z <= (GameManager.instance.BottomRowValue + (GameManager.instance.SpacingDistance/3f)))
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
			G.GetComponent<NodeManager>().NodeFill.transform.localScale = Vector3.one;
		}
	}

	void SetUpPowerUp ()
	{
		PowerUp = true;
		//transform.renderer.material.color = Color.black;
	}

	void CheckIfIAmLassoed ()
	{
		if(!PowerUp && AmILassoed())
		{
			SetUpPowerUp();
		}
	}

	bool AmILassoed ()
	{
		bool Left = false;
		bool Right = false;
		bool Up = false;
		bool Down = false;

		//Left
		RaycastHit[] HitsLeft;
		HitsLeft = Physics.RaycastAll(transform.position, -transform.right, 100.0F);
		int i = 0;
		while (i < HitsLeft.Length) 
		{
			RaycastHit hit = HitsLeft[i];
			if (hit.transform.GetComponent<NodeManager>().IsMatched) 
			{
				Left = true;
				break;
			}
			i++;
		}

		//Catch
		if(Left == false)
		{
			return false;
		}

		//Right
		RaycastHit[] HitsRight;
		HitsRight = Physics.RaycastAll(transform.position, transform.right, 100.0F);
		i = 0;
		while (i < HitsRight.Length) 
		{
			RaycastHit hit = HitsRight[i];
			if (hit.transform.GetComponent<NodeManager>().IsMatched) 
			{
				Right = true;
				break;
			}
			i++;
		}

		if(Right == false)
		{
			return false;
		}

		//Up
		RaycastHit[] HitsUp;
		HitsUp = Physics.RaycastAll(transform.position, transform.up, 100.0F);
		i = 0;
		while (i < HitsUp.Length) 
		{
			RaycastHit hit = HitsUp[i];
			if (hit.transform.GetComponent<NodeManager>().IsMatched) 
			{
				Up = true;
				break;
			}
			i++;
		}

		if(Up == false)
		{
			return false;
		}

		//Down
		RaycastHit[] HitsDown;
		HitsDown = Physics.RaycastAll(transform.position, -transform.up, 100.0F);
		i = 0;
		while (i < HitsDown.Length) 
		{
			RaycastHit hit = HitsDown[i];
			if (hit.transform.GetComponent<NodeManager>().IsMatched) 
			{
				Down = true;
				break;
			}
			i++;
		}
		if(Down == false)
		{
			return false;
		}
		else{
			//Debug.Log(transform.name);
			return true;
		}

	}
}
