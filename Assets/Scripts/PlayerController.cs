using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed = 0.4f;
    Vector2 _destination = Vector2.zero;
    public Vector2 direction = Vector2.zero;
    Vector2 _nextDirection = Vector2.zero;

    [Serializable]
    public class PointSprites
    {
        public GameObject[] pointSprites;
    }

    public PointSprites points;

    public static int killstreak = 0;

    private bool _deadPlaying = false;
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    public GameManager gameManager;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    public LayerMask validLayer;
    public Vector3 startPos;

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _destination = transform.position;
        startPos = Vector3Int.RoundToInt(transform.position);
        direction = Vector2.zero;
        _nextDirection = Vector2.zero;
        _rigidbody.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (gameManager.gameState)
        {
            case GameState.Running:
                ReadInputAndMove();
                Animate();
                break;

            case GameState.Dead:
                if (!_deadPlaying)
                    StartCoroutine(nameof(PlayDeadAnimation));
                break;
        }
    }

    public void ResetPacmanPos()
    {
        _animator.SetBool(Die, false);
        _deadPlaying = false;
        transform.position = startPos;

    }

    public IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        _animator.SetBool(Die, true);
        yield return new WaitForSeconds(1);
        _animator.SetBool(Die, false);
        _deadPlaying = false;
    }

    void Animate()
    {
        Vector2 dir = _destination - (Vector2)transform.position;
        _animator.SetFloat(DirX, dir.x);
        _animator.SetFloat(DirY, dir.y);
    }

    bool Valid(Vector2 direction)
    {
        Vector2 pos = transform.position;
        direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
        RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos, validLayer);
        return hit.collider == null;
    }

    public void ResetDestination()
    {
        _destination = new Vector2(15f, 11f);
        _animator.SetFloat(DirX, 1);
        _animator.SetFloat(DirY, 0);
    }

    void ReadInputAndMove()
    {
        // move closer to destination
        Vector2 p = Vector2.MoveTowards(transform.position, _destination, speed);
        _rigidbody.MovePosition(p);

        // get the next direction from keyboard
        if (Input.GetAxis("Horizontal") > 0) _nextDirection = Vector2.right;
        if (Input.GetAxis("Horizontal") < 0) _nextDirection = -Vector2.right;
        if (Input.GetAxis("Vertical") > 0) _nextDirection = Vector2.up;
        if (Input.GetAxis("Vertical") < 0) _nextDirection = -Vector2.up;

        if (!(Vector2.Distance(_destination, transform.position) < 0.00001f)) return;
        if (Valid(_nextDirection))
        {
            _destination = (Vector2)transform.position + _nextDirection;
            direction = _nextDirection;
        }
        else 
        {
            if (Valid(direction))
                _destination = (Vector2)transform.position + direction;
        }
    }

    public Vector2 GetDirection => direction;

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);

    }
}
