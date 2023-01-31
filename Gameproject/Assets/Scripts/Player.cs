using UnityEngine;
using System.Collections;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
	public float restartLevelDelay = 1f;
	public int pointsPerHP1 = 10;		
	public int pointsPerHP2 = 20;		
	public int wallDamage = 1;			
	public Text foodText;				
	public AudioClip moveSound1;		
	public AudioClip moveSound2;		
	public AudioClip hp1Sound1;				
	public AudioClip hp1Sound2;				
	public AudioClip hp2Sound1;			
	public AudioClip hp2Sound2;			
	public AudioClip gameOverSound;			
		
	private Animator animator;				
	private int hp;                        
		
		
	protected override void Start ()
	{ // hp 정보를 표시한다.
		animator = GetComponent<Animator>();			
		hp = GameManager.instance.playerHPPoints;			
		foodText.text = "HP: " + hp;			
		base.Start ();
	}
		
		
	private void OnDisable ()
	{ // gamemanager에 hp정보를 반환
		GameManager.instance.playerHPPoints = hp;
	}
		
		
	private void Update ()
	{ // 상하좌우로 이동
		if(!GameManager.instance.playersTurn) return;
			
		int horizontal = 0;  
		int vertical = 0;	
				
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
		if(horizontal != 0)
		{
			vertical = 0;
		}

			
		if(horizontal != 0 || vertical != 0)
		{
			AttemptMove<Wall> (horizontal, vertical);
		}
	}
		
	protected override void AttemptMove <T> (int xDir, int yDir)
	{ // move를 시도하면 hp가 감소한다. 
		hp--;	
		foodText.text = "HP: " + hp;
		base.AttemptMove <T> (xDir, yDir);
		RaycastHit2D hit;

		if (Move (xDir, yDir, out hit)) 
		{
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}
			
		CheckIfGameOver ();
			
		GameManager.instance.playersTurn = false;
	}
		
		
	protected override void OnCantMove <T> (T component)
	{ // 벽이랑 만나면 부신다.
		Wall hitWall = component as Wall;			
		hitWall.DamageWall (wallDamage);			
		animator.SetTrigger ("charChop");
	}
		
		
	private void OnTriggerEnter2D (Collider2D other)
	{ // 각 테그별로 행동양식을 정해둔다.
		if(other.tag == "Exit")
		{
			Invoke ("Restart", restartLevelDelay);				
			enabled = false;
		}
			
		else if(other.tag == "Food")
		{
			hp += pointsPerHP1;				
			foodText.text = "+" + pointsPerHP1 + " HP: " + hp;				
			SoundManager.instance.RandomizeSfx (hp1Sound1, hp1Sound2);				
			other.gameObject.SetActive (false);
		}
			
		else if(other.tag == "Soda")
		{
			hp += pointsPerHP2;				
			foodText.text = "+" + pointsPerHP2 + " HP: " + hp;				
			SoundManager.instance.RandomizeSfx (hp2Sound1, hp2Sound2);				
			other.gameObject.SetActive (false);
		}
	}
		
		
	private void Restart ()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
		
		
	public void LoseHP (int loss)
	{ // player가 공격당하면 체력을 잃는다. 
		animator.SetTrigger ("charHit");			
		hp -= loss;
		if (hp < 0)
			hp = 0;
		foodText.text = "-"+ loss + " HP: " + hp;			
		CheckIfGameOver ();
	}
		
		
	private void CheckIfGameOver ()
	{ // 체력이 바닥나면 게임오버
		if (hp <= 0) 
		{
			SoundManager.instance.PlaySingle (gameOverSound);				
			SoundManager.instance.musicSource.Stop();				
			GameManager.instance.GameOver ();
		}
	}
}


