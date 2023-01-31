using UnityEngine;
using System;
using System.Collections.Generic; 		
using Random = UnityEngine.Random; 		


	
public class BoardManager : MonoBehaviour
{

	[Serializable]
	public class Count
	{
		public int minimum; 			
		public int maximum; 			
			
			
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
		
		
	public int columns = 8; 									
	public int rows = 8;										
	public Count wallCount = new Count (5, 9);					
	public Count HPCount = new Count (1, 5);					
	public GameObject exit;										
	public GameObject[] floorTiles;								
	public GameObject[] wallTiles;								
	public GameObject[] HPTiles;								
	public GameObject[] enemyTiles;								
	public GameObject[] outerWallTiles;							
		
	private Transform boardHolder;								
	private List <Vector3> gridPositions = new List <Vector3> (); // 2차원 벡터
		
		
	
	void InitialiseList ()
	{	
		gridPositions.Clear ();			
	
		for(int x = 1; x < columns-1; x++)
		{	
			for(int y = 1; y < rows-1; y++)
			{	
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}
		
		
	
	void BoardSetup ()
	{ // 보드에 대한 내용을 저장할 transform을 설정
		boardHolder = new GameObject ("Board").transform;			
	
		for(int x = -1; x < columns + 1; x++)
		{ // 보드의 끝을 설정하기 위해 양옆으로 한칸씩 더 만든다.
			for(int y = -1; y < rows + 1; y++)
			{	
				// floorTiles 개수 내의 범위에서 랜덤하게 인스턴트화 한다.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
						
				// 보드의 끝부분을 처리한다. 
				if(x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
						
				GameObject instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
						
				// 모두 transform에 넣어준다.
				instance.transform.SetParent (boardHolder);
			}
		}
	}
		
		
	
	Vector3 RandomPosition ()
	{	// 2차원 벡터를 이용하여 랜덤하게 배치
		int randomIndex = Random.Range (0, gridPositions.Count);			
	
		Vector3 randomPosition = gridPositions[randomIndex];			
	
		gridPositions.RemoveAt(randomIndex);			
	
		return randomPosition;
	}
		
		
	
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{	
		int objectCount = Random.Range (minimum, maximum+1);
				
		for(int i = 0; i < objectCount; i++)
		{	
			Vector3 randomPosition = RandomPosition();					
			// 랜덤 타일을 하나 뽑는다. 
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];					
			// 랜덤 타일을 랜덤한 위치에 넣는다. 
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
		
		
	
	public void SetupScene (int level)
	{ // 이 과정을 거쳐 하나의 scene을 만든다. 
		BoardSetup(); // 보드 만들기
			
		InitialiseList(); // 리스트 초기화
		
		// 벽과 아이템을 각 위치에 하나씩 넣는다.
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);			
		LayoutObjectAtRandom (HPTiles, HPCount.minimum, HPCount.maximum);
			
		int enemyCount = (int)Mathf.Log(level, 2f); // 0, 1, 1, 2, 2, 2, 2, 3, 3식으로 적 증가
			
		// 적 랜덤 배치
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
		// 출구 배치
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}

