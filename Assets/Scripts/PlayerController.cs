using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed = 0.4f;
    Vector2 _destination = Vector2.zero;
    Vector2 _direction = Vector2.zero;
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
    public LayerMask validLayer;

    // Use this for initialization
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _destination = transform.position;
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

    IEnumerator PlayDeadAnimation()
    {
        _deadPlaying = true;
        GetComponent<Animator>().SetBool(Die, true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool(Die, false);
        _deadPlaying = false;
    }

    void Animate()
    {
        Vector2 dir = _destination - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat(DirX, dir.x);
        GetComponent<Animator>().SetFloat(DirY, dir.y);
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
        GetComponent<Animator>().SetFloat(DirX, 1);
        GetComponent<Animator>().SetFloat(DirY, 0);
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
            _direction = _nextDirection;
        }
        else 
        {
            if (Valid(_direction))
                _destination = (Vector2)transform.position + _direction;
        }
    }

    public Vector2 GetDirection => _direction;

    public void UpdateScore()
    {
        killstreak++;

        // limit killstreak at 4
        if (killstreak > 4) killstreak = 4;

        Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);

    }
}
