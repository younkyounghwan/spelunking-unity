using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public int playerDamage;
	public AudioClip attackSound1;
	public AudioClip attackSound2;		
		
	private Animator animator;		
	private Transform target;		
	private bool skipMove;				
	
	protected override void Start ()
	{
		GameManager.instance.AddEnemyToList (this);		
		animator = GetComponent<Animator> ();		
		target = GameObject.FindGameObjectWithTag ("Player").transform;		
		base.Start ();
	}
	
	protected override void AttemptMove <T> (int xDir, int yDir)
	{	
		if(skipMove)
		{ // blocking이 걸려있을때는 그냥 return 
			skipMove = false;
			return;				
		}			
	
		base.AttemptMove <T> (xDir, yDir);	
		skipMove = true; // move 시도 후에는 blocking 활성화
	}
			
	public void MoveEnemy ()
	{ // enemy가 player 방향으로 움직인다.
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon
			&& Mathf.Abs(target.position.y - transform.position.y) < float.Epsilon)
			xDir = -1;

		// 만약 player와 enemy의 x좌표가 같다면 
		else if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)	
			yDir = target.position.y > transform.position.y ? 1 : -1; // y좌표 이동
					
		else 
			xDir = target.position.x > transform.position.x ? 1 : -1;

		

			AttemptMove <Player> (xDir, yDir); // 이동
	}
		
	protected override void OnCantMove <T> (T component)
	{		
		Player hitPlayer = component as Player;
		hitPlayer.LoseHP (playerDamage);			
	}
}

