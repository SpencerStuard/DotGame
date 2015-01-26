using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance { get; private set; }

	public List<AudioClip> DotSFX = new List<AudioClip>();
	public AudioClip DotTest;

	public AudioSource Source01;

	// Use this for initialization
	void Awake () {

		instance = this;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayRandomDotSFX ()
	{
		//Debug.Log(DotSFX.Count);
		int SoundToPlay = Random.Range (0, DotSFX.Count - 1);
		AudioClip Toplay = DotSFX[SoundToPlay] ;

		Source01.PlayOneShot(Toplay);
	}
}
