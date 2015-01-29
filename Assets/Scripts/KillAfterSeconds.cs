using UnityEngine;
using System.Collections;

public class KillAfterSeconds : MonoBehaviour {

	float TimeToKill = Mathf.Infinity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > TimeToKill)
		{
			Destroy(gameObject);
		}
	
	}

	public void KillMeAfterSeconds (float f)
	{
		TimeToKill = Time.time + f;
	}
}
