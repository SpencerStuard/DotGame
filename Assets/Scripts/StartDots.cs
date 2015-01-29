using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartDots : MonoBehaviour {

	public List<GameObject> StartDotsObj = new List<GameObject>();


	// Use this for initialization
	void Start () {

		GameManager.instance.KillMainMenu += KillMainMenu;


		foreach(Transform T in transform)
		{
			StartDotsObj.Add(T.gameObject);
		}

		float x = 0;

		foreach(GameObject G in StartDotsObj)
		{
			Color col = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
			col.a = 0;
			G.GetComponent<NodeManager>().SetColor(NodeColors.Red);
			G.GetComponent<SpriteRenderer>().color = col;
			G.AddComponent<FadeText>().DoFade(0,1,2,(x + 1f),G);
			x += .25f;
		}
	
	}

	void OnDestroy ()
	{
		GameManager.instance.KillMainMenu -= KillMainMenu;
	}

	void KillMainMenu ()
	{
		float x = 0;
		
		foreach(GameObject G in StartDotsObj)
		{
			if(G)
			{
				Color col = ColorManager.instance.PossibleColors[(int)NodeColors.Red];
				col.a = 0;
				G.GetComponent<NodeManager>().SetColor(NodeColors.Red);
				G.GetComponent<SpriteRenderer>().color = col;
				G.AddComponent<FadeText>().DoFade(1,0,1,0f,G);

			}
		}

		gameObject.AddComponent<KillAfterSeconds>().KillMeAfterSeconds(1f);
	}
}
