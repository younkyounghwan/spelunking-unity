using UnityEngine;
using System.Collections;


public class Wall : MonoBehaviour
{
	public AudioClip chopSound1;				
	public AudioClip chopSound2;				
					
	public int hp = 3;							
		
	private SpriteRenderer spriteRenderer;		
		
		
	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
		
		
	public void DamageWall (int loss)
	{ // 벽을 3번 가격하면 
		SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);			
		hp -= loss;			
		if(hp <= 0) // 벽을 비활성화한다. 
			gameObject.SetActive (false);
	}
}

