using UnityEngine;
using System.Collections;

public class PlaceExplosion : MonoBehaviour 
{
	public static PlaceExplosion main;
	public ParticleSystem explosion;

	void Start () 
	{
		main = this;
	}
	
	public void Place(Vector3 pos)
	{
		Instantiate(explosion,pos,Quaternion.identity);	
	}

}
