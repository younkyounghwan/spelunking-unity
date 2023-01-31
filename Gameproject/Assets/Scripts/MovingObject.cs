using UnityEngine;
using System.Collections;


public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.01f;		
	public LayerMask blockingLayer;				
		
	private BoxCollider2D boxCollider; 	
	private Rigidbody2D rb2D;			
	private float inverseMoveTime;		
	private bool isMoving;				
		
		
	protected virtual void Start ()
	{
		boxCollider = GetComponent <BoxCollider2D> ();			
		rb2D = GetComponent <Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}
		
		
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;			
		Vector2 end = start + new Vector2 (xDir, yDir);			
		boxCollider.enabled = false;			
		hit = Physics2D.Linecast (start, end, blockingLayer);			
		boxCollider.enabled = true;			
		if(hit.transform == null && !isMoving)
		{
			StartCoroutine (SmoothMovement (end));
			return true;
		}

		// Raycast vs Linecast
		// Raycast: 방향만 존재
		// Linecast: 방향과 목적지가 같이 존재
			
		return false;
	}
		
		
	protected IEnumerator SmoothMovement (Vector3 end)
	{ // 부드럽게 움직일 수 있게 해줌
		isMoving = true; // trigger 활성화
			
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
		while(sqrRemainingDistance > float.Epsilon)
		{ // 움직일 거리가 0이상일때 
			Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, 
				inverseMoveTime * Time.deltaTime); 
			rb2D.MovePosition (newPostion);				
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;				
			yield return null;
		}
			
		rb2D.MovePosition (end);
			
		isMoving = false; // trigger 비활성화
	}
		
		
	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit; // 충돌전에 미리 충돌을 감지한다. 
			
		bool canMove = Move (xDir, yDir, out hit);
			
		// error_handling
		if(hit.transform == null)
			return;
			
		T hitComponent = hit.transform.GetComponent <T> ();
			
		if(!canMove && hitComponent != null)				
			OnCantMove (hitComponent);
	}
		
		
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}

