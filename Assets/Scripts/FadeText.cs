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

	public void DoFadeImage (float startVal, float endVal,float t,float delaytime, GameObject G)
	{
		if(!G.GetComponent<Text>())
		{
			Debug.LogError("FADE SCRIPT PUT ON NONE TEXT ELEMTN");
			return;
		}
		
		TextObj = G;
		TextColor = TextObj.GetComponent<SpriteRenderer>().color;
		
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

	

	public void DoFade (float startVal, float endVal,float t,float delaytime, GameObject G)
	{
		TextObj = G;
		if(TextObj.GetComponent<Text>())
		{
			TextColor = TextObj.GetComponent<Text>().color;
		}
		else if (TextObj.GetComponent<SpriteRenderer>())
		{
			TextColor = TextObj.GetComponent<SpriteRenderer>().color;
		}
		else if (TextObj.GetComponent<Image>())
		{
			TextColor = TextObj.GetComponent<Image>().color;
		}
		else
		{
			Debug.LogError("FADE SCRIPT PUT ON NONE TEXT ELEMTN");
			return;
		}

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

	void OnDestroy ()
	{
		if(gameObject.GetComponent<iTween>())
		Destroy(gameObject.GetComponent<iTween>());
	}

	void tweenOnUpdateCallBack( float newValue )
	{
		if(TextObj)
		{
			//Debug.Log("in here");
			StartAlpha = newValue;
				TextColor.a = StartAlpha;

			if(TextObj.GetComponent<Text>())
			{
				TextColor = TextObj.GetComponent<Text>().color = TextColor;
			}
			else if (TextObj.GetComponent<SpriteRenderer>())
			{
				TextColor = TextObj.GetComponent<SpriteRenderer>().color = TextColor;
			}
			else if (TextObj.GetComponent<Image>())
			{
				TextColor = TextObj.GetComponent<Image>().color = TextColor;
			}

			//TextObj.GetComponent<Text>().color = TextColor;
		//Debug.Log( exampleInt );
	
		}
	}

	void finishtweened ()
	{
		Destroy(this);
	}


}
