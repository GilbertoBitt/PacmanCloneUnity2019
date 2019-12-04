using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GhostMove : MonoBehaviour
{
	public Vector3 waypoint;
	
	public float speed = 0.3f;
	
    private static readonly int RunWhite = Animator.StringToHash("Run_White");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    public GameManager gameManager;
    private Rigidbody2D _rigidbody2D;
    
    public enum State { Wait, Init, Scatter, Chase, Run, Returning, Exiting };
    public State state = State.Chase;
    private float _distance;
    private Vector2 _nextDirection;
    private Vector2 _direction;
    public Transform targetTransform;
    public Transform playerTransform;
    public Transform scatterTransform;
    public LayerMask layerMask;
    private bool _timerEnded = false;
    public IEnumerator timerCoroutine;
    private Animator _animator;
    private float _timeToToggleWhite;
    private float _toggleInterval = 1f;
    public bool isWhite;
    public Transform startPos;
    
    public IEnumerator DelayedCallback(float delay, Action callBack) {
	    // Waits 3 seconds
	    yield return new WaitForSeconds(delay);
	    // CALL callBack after 3 seconds
	    callBack();
    }

    public void RunFromPacman()
    {
	    if (timerCoroutine != null)
	    {
		    StopCoroutine(timerCoroutine);
	    }
	    state = State.Run;
	    _timerEnded = false;
	    _animator.SetBool(Run, true);
	    timerCoroutine = DelayedCallback(20f, () =>
	    { 
		    _timerEnded = true;
	    });
	    StartCoroutine(timerCoroutine);
    }

    private void Start()
    {
	    _animator = GetComponent<Animator>();
	    _rigidbody2D = GetComponent<Rigidbody2D>();
	    waypoint = transform.position;
	    _direction = Vector2.zero;
	    state = State.Scatter;
	    _timerEnded = false;
	    timerCoroutine = DelayedCallback(UnityEngine.Random.Range(1f, 5f), () => { _timerEnded = true; });
	    StartCoroutine(timerCoroutine);
    }

    void FixedUpdate ()
	{
		
		_distance = Vector3.Distance(transform.position, waypoint);
		Vector2 p = Vector2.MoveTowards(transform.position,
			waypoint, speed);
		_rigidbody2D.MovePosition(p);

		if (gameManager.gameState == GameState.Running)
		{
			if (!(Vector2.Distance(waypoint, transform.position) < 0.00001f)) return;
			
			switch (state)
			{
				case State.Chase:
					targetTransform = playerTransform;
					_nextDirection = DirectionByDistance(true).First();
					if (_timerEnded)
					{
						state = State.Scatter;
						_timerEnded = false;
						timerCoroutine = DelayedCallback(UnityEngine.Random.Range(1f, 5f), () => { _timerEnded = true; });
						StartCoroutine(timerCoroutine);
					}
					break;
				case State.Scatter:
					targetTransform = scatterTransform;
					_nextDirection = DirectionByDistance(true).First();
					if (_timerEnded)
					{
						state = State.Chase;
						_timerEnded = false;
						timerCoroutine = DelayedCallback(UnityEngine.Random.Range(1f, 5f), () => { _timerEnded = true; });
						StartCoroutine(timerCoroutine);
					}
					break;
				case State.Run:
					targetTransform = playerTransform;
					_nextDirection = DirectionByDistance(false).First();
					RunAway();
					if (_timerEnded)
					{
						_timeToToggleWhite = 0;
						_animator.SetBool(RunWhite, false);
						_animator.SetBool(Run, false);
						state = State.Scatter;
						_timerEnded = false;
						timerCoroutine = DelayedCallback(UnityEngine.Random.Range(1f, 5f), () => { _timerEnded = true; });
						StartCoroutine(timerCoroutine);
					}
					break;
				case State.Returning:
					targetTransform = startPos;
					_nextDirection = DirectionByDistance(true).First();
					_animator.SetBool(RunWhite, false);
					_animator.SetBool(Run, false);
					if (_timerEnded)
					{
						_timeToToggleWhite = 0;
						state = State.Scatter;
						_timerEnded = false;
						timerCoroutine = DelayedCallback(UnityEngine.Random.Range(1f, 5f), () => { _timerEnded = true; });
						StartCoroutine(timerCoroutine);
					}
					break;
			}
			
			
			if (Valid(_nextDirection))
			{
				waypoint = (Vector2)transform.position + _nextDirection;
				_direction = _nextDirection;
			}
			else 
			{
				if (Valid(_direction))
					waypoint = (Vector2)transform.position + _direction;
			}
		}
		
		AnimationGhost ();
			
	}

    private void RunAway()
    {
	    _animator.SetBool(Run, true);
	    if(Time.time >= _timeToToggleWhite)   ToggleBlueWhite();
    }
    
    public void ToggleBlueWhite()
    {
	    isWhite = !isWhite;
	    GetComponent<Animator>().SetBool("Run_White", isWhite);
	    _timeToToggleWhite = Time.time + _toggleInterval;
    }

    List<Vector2> DirectionByDistance(bool chaseTarget)
    {
	    var directionsValid = new List<(Vector2 direction, float distance)>();
	    var playerPos = targetTransform.position;
	    var ghostPos = transform.position;
	    if(Valid(Vector2.right) && _direction != Vector2.left)
	    {
		    directionsValid.Add((Vector2.right, Vector2.Distance((Vector2) ghostPos + Vector2.right, playerPos)));
	    }
	    
	    if(Valid(Vector2.left) && _direction != Vector2.right)
	    {
		    directionsValid.Add((Vector2.left, Vector2.Distance((Vector2) ghostPos + Vector2.left, playerPos)));
	    }
	    
	    if(Valid(Vector2.up) && _direction != Vector2.down)
	    {
		    directionsValid.Add((Vector2.up, Vector2.Distance((Vector2) ghostPos + Vector2.up, playerPos)));
	    }
	    
		if(Valid(Vector2.down) && _direction != Vector2.up) {
			directionsValid.Add((Vector2.down, Vector2.Distance((Vector2) ghostPos + Vector2.down, playerPos)));
	    }

		var list = chaseTarget 
			? directionsValid.OrderBy(x => x.distance).Select(x => x.direction)
			.ToList() 
			: directionsValid.OrderByDescending(x => x.distance).Select(x => x.direction)
			.ToList() ;
	    return list;
    }

    void AnimationGhost()
    {
	    Vector3 dir = waypoint - transform.position;
	    _animator.SetFloat(DirX, dir.x);
	    _animator.SetFloat(DirY, dir.y);
    }
		
    bool Valid(Vector2 direction)
    {
	    Vector2 pos = transform.position;
	    direction += direction * 0.45f;
	    var nextPos = pos + direction;
	    RaycastHit2D hit = Physics2D.Linecast(nextPos, pos, layerMask);
	    Debug.DrawLine(pos, nextPos, Color.red);
	    return hit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
	    if (other.CompareTag("Player"))
	    {
		    if (state == State.Run)
		    {
			    gameManager.AddScore(200);
			    state = State.Returning;
			    timerCoroutine = DelayedCallback(5f, () =>
			    {
				    _timerEnded = true;
			    });
			    StartCoroutine(timerCoroutine);
		    }
		    else if(state != State.Returning) {
			    gameManager.gameState = GameState.Dead;
		    }
	    }
    }
}
