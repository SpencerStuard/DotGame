using UnityEngine;
using System.Collections;

public class KillParticleSystem : MonoBehaviour 
{
	
	void FixedUpdate () 
	{
		if(particleSystem != null && particleSystem.particleCount <= 0)
			Destroy (gameObject);
	}
}
