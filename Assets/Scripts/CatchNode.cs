using UnityEngine;
using System.Collections;

public class CatchNode : MonoBehaviour {

	public GameObject CatchNodesprite_01;
	public GameObject CatchNodesprite_02;
	public GameObject CatchNodesprite_03;

	// Use this for initialization
	void Start () {
		//CatchNodesprite_03.SetActive(false);
		//CatchNodesprite_02.SetActive(false);
		//CatchNodesprite_01.SetActive(false);
	}
	
	public void SetUpCatchNode (int CatchNodeType)
	{
		CatchNodesprite_03.SetActive(false);
		CatchNodesprite_02.SetActive(false);
		CatchNodesprite_01.SetActive(false);

		switch(CatchNodeType)
		{	
		case 0:
			CatchNodesprite_01.SetActive(true);
			break;
		case 1:
			CatchNodesprite_02.SetActive(true);
			break;
		case 2:
			CatchNodesprite_03.SetActive(true);
			break;
		}
	}
}
