using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeText : MonoBehaviour {

	float StartAlpha;
	float EndAlpha;
	GameObject TextObj;
	Color TextColor;

	// Use this for initialization
	void Update () {

	
	}
	

	public void DoFadeText (float startVal, float endVal,float t,float delaytime, GameObject G)
	{
		if(!G.GetComponent<Text>())
		{
			Debug.LogError("FADE SCRIPT PUT ON NONE TEXT ELEMTN");
			return;
		}

		TextObj = G;
		TextColor = TextObj.GetComponent<Text>().color;

		StartAlpha = startVal;

			iTween.ValueTo( TextObj, iTween.Hash(
			"delay", delaytime, 
			"from", startVal,
			"to", endVal,
			"time", t,
			"onupdatetarget", TextObj,
			"onupdate", "tweenOnUpdateCallBack",
			"easetype", iTween.EaseType.easeInOutCubic,
			"oncompletetarget", TextObj,
			"oncomplete", "finishtweened"
			)
		    );
	}

	void tweenOnUpdateCallBack( float newValue )
	{
		//Debug.Log("in here");
		StartAlpha = newValue;
			TextColor.a = StartAlpha;
		TextObj.GetComponent<Text>().color = TextColor;
		//Debug.Log( exampleInt );
	}

	void finishtweened ()
	{
		Destroy(this);
	}


}
