using UnityEngine;
using System.Collections;

public enum LevelType {Collect, Survive};


[System.Serializable]
 public class LevelData 
{

	public  int Rows;
	public  int Columns;
	public  int NumberOfColors;
	public  LevelType MyLevelType;


}
