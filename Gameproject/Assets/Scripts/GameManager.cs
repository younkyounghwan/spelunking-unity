using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using System.Collections.Generic;		
using UnityEngine.UI;					
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;	
	public float turnDelay = 0.1f;	
	public int playerHPPoints = 100;
	public static GameManager instance = null;	
	[HideInInspector] public bool playersTurn = true;
		
		
	private Text levelText;	
	private GameObject levelImage;	
	private BoardManager boardScript;
	private int level = 1; // 단계	
	private List<Enemy> enemies; // enemy를 리스트로 관리한다. 
	private bool enemiesMoving;	// trigger	
	private bool doingSetup = true;	
	
	void Awake()
	{ // 
        if (instance == null)
			instance = this; // instance가 null이면 
		      
        else if (instance != this) // null이 아닌데 this도 아니면
            Destroy(gameObject); // 지운다.
		DontDestroyOnLoad(gameObject);
		
		enemies = new List<Enemy>();
				
		boardScript = GetComponent<BoardManager>();
			
		InitGame();
	}

    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization() // awake와 start 사이에 호출
    {    // scene을 load한다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{   // scene을 load한다.
		instance.level++;
        instance.InitGame();
    }

		
	
	void InitGame()
	{	// 게임 시작
		doingSetup = true;
		
		// 첫 화면에 뜨는 것들 
		levelImage = GameObject.Find("LevelImage");		
		levelText = GameObject.Find("LevelText").GetComponent<Text>();		
		levelText.text = "Day " + level;	
		
		// 화면 활성화
		levelImage.SetActive(true);			

		// delay 후에 tag 호출
		Invoke("HideLevelImage", levelStartDelay);
		enemies.Clear(); // enemy 모두 지우고
		boardScript.SetupScene(level); // board 호출	
	}
		
		
	void HideLevelImage()
	{ // level image 비활성화
		levelImage.SetActive(false);
		doingSetup = false;
	}
		
	void Update()
	{ // trigger check하고 coroutine 활성화
		if(playersTurn || enemiesMoving || doingSetup)				
			return;			
		StartCoroutine (MoveEnemies());
	}
		
	public void AddEnemyToList(Enemy script)
	{ 
		enemies.Add(script);
	}
		
		
	public void GameOver()
	{
		levelText.text = "After " + level + " Days, You Dead.";			
		levelImage.SetActive(true);			
		enabled = false;
	}
		
	IEnumerator MoveEnemies() // 코루틴 함수 선언
	{ //enemy를 움직인다. 
		enemiesMoving = true; // blocking			
		yield return new WaitForSeconds(turnDelay);
		
		if (enemies.Count == 0) 
		{
			yield return new WaitForSeconds(turnDelay);
		}
			
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy ();
				
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playersTurn = true;
			
		enemiesMoving = false;
	}
}


