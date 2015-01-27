using UnityEngine;
using System.Collections;

public class NodeSpawner : MonoBehaviour {

	public GameObject NodeObj;

	float RayDistance;

	// Use this for initialization
	void Start () {
		RayDistance = GameManager.instance.SpacingDistance + 1f;
	}
	
	// Update is called once per frame
	void Update () {
		CheckForCascade ();
	}

	void CheckForCascade ()
	{


		Debug.DrawRay(transform.position, Vector3.back * RayDistance, Color.yellow);
		
		if (Physics.Raycast(transform.position, Vector3.back, RayDistance))
		{
			
		}
		else{
			SpawnNewNode ();
		}
	}

	void SpawnNewNode ()
	{
		GameObject NewNode = Instantiate(NodeObj,new Vector3(transform.position.x, 1, transform.position.z - GameManager.instance.SpacingDistance),Quaternion.Euler(90,0,0))as GameObject;
		NewNode.transform.parent = GameManager.instance.NodeHolder.transform;
		NewNode.GetComponent<NodeManager>().PickColor();
		NewNode.transform.name = "Node_" + GameManager.instance.DotName.ToString();
		GameManager.instance.DotName ++;
	}
}
