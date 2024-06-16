using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicMovement : Entity
{
    [SerializeField]
    private GameObject _enemyObj;
    [SerializeField]
    private Transform _hpBar;

    [SerializeField]
    private float _aggroRange = 8f;
    [SerializeField]
    private float _minDeaggroRange = 0.0f;
    public float MovementSpeed = 8f;
    [SerializeField]
    private float _approachRadius = 2f;

    public bool Aggroed = false;

    private Transform _playerTransform;
    private Rigidbody2D _erb;
    private SpriteRenderer _esr;
    private Vector2 _targetMovePos;

    public bool IsColliding = false;

    private float _readjustTimer = 0.0f;
    public Vector2 _readjustVec;
    private Vector2 _hitPos;

    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        _minDeaggroRange = (_minDeaggroRange == 0.0f) ? (_aggroRange * 1.5f) : _minDeaggroRange;
        _erb = _enemyObj.GetComponent<Rigidbody2D>();
        _esr = _enemyObj.GetComponent<SpriteRenderer>();
        _readjustVec = Vector2.zero;
        _hitPos = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            if (_enemyObj.activeSelf)
                _enemyObj.SetActive(false);
            return;
        }
        else
        {
            if (!_enemyObj.activeSelf)
                _enemyObj.SetActive(true);
        }

        if (Vector2.Distance(_playerTransform.position, _enemyObj.transform.position) < _aggroRange)
        {
            Aggroed = true;
        }

        if (Aggroed)
        {
            if (Vector2.Distance(_playerTransform.position, _enemyObj.transform.position) > _minDeaggroRange)
            {
                Aggroed = false;
                _erb.velocity = Vector2.zero;
                return;
            }

            ChasePlayer();
        }

        _hpBar.gameObject.SetActive(Health < MaxHealth);
        _hpBar.localScale = new Vector3((float)Health / (float)MaxHealth, _hpBar.localScale.y, _hpBar.localScale.z);
        if (Vector3.Distance(_enemyObj.transform.position, _playerTransform.position) > _approachRadius)
            ReadjustPosition();
    }

    private void ChasePlayer()
    {
        _esr.flipX = (_playerTransform.position.x > _enemyObj.transform.position.x);


        // set TargetPoint to closest point from the enemy to the player at the approach radius
        // Calculate the direction from the enemy to the player
        Vector2 direction = _playerTransform.position - _enemyObj.transform.position;
        direction.Normalize();

        // Calculate the target position
        Vector2 pTransVec2 = new Vector2(_playerTransform.position.x, _playerTransform.position.y);

        _targetMovePos = pTransVec2 - direction * _approachRadius;

        // Move towards the target position
        if (Vector2.Distance(_enemyObj.transform.position, _targetMovePos) > 0.1f)
        {
            if (Vector3.Distance(_enemyObj.transform.position, _playerTransform.position) < _approachRadius)
                direction = -direction;

            _erb.velocity = direction * MovementSpeed;
        }
        else if (Vector2.Distance(_enemyObj.transform.position, _targetMovePos) < 0.1f)
        {
            _erb.velocity = Vector2.zero;
        }
    }

    public void AvoidWallCollision(Vector2 inWallPos)
    {
        Vector2 selfPos = new Vector2(_enemyObj.transform.position.x, _enemyObj.transform.position.y);
        Vector2 targetPos = new Vector2(_playerTransform.position.x, _playerTransform.position.y);
        _hitPos = inWallPos;
        Vector2 selfToWall = inWallPos - selfPos;
        float xdist = Mathf.Abs(selfToWall.x);
        float ydist = Mathf.Abs(selfToWall.y);

        if (!IsInGeneralDirection(selfPos, _hitPos, targetPos, 125.0f))
            return; 

        bool isCollisionVertical = (xdist > ydist);

        Vector2 dirVec = (targetPos - selfPos).normalized;

        if (isCollisionVertical)
        {
            Vector2 yMoveVec = new Vector2(0, MovementSpeed * Mathf.Sign(dirVec.y));
            if (Mathf.Approximately(_readjustVec.y,0.0f))
                _readjustVec = yMoveVec;
        }
        else
        {
            Vector2 xMoveVec = new Vector2(MovementSpeed * Mathf.Sign(dirVec.x), 0);
            if (Mathf.Approximately(_readjustVec.x, 0.0f))
                _readjustVec = xMoveVec;
        }
    }

    private void ReadjustPosition()
    {
        if (IsColliding)
        {
            Vector2 selfPos = new Vector2(_enemyObj.transform.position.x, _enemyObj.transform.position.y);
            Vector2 targetPos = new Vector2(_playerTransform.position.x, _playerTransform.position.y);
            if (!IsInGeneralDirection(selfPos, _hitPos, targetPos, 100.0f))
                return;
            
            _erb.velocity = _readjustVec;
            _readjustTimer = 0.25f;
        }
        else
        {
            _readjustTimer -= Time.deltaTime;
            if (_readjustTimer <= 0.0f)
            {
                _readjustVec = Vector2.zero;
                _hitPos = Vector2.zero;
                _readjustTimer = 0.0f;
            }
        }
    }

    private bool IsInGeneralDirection(Vector2 inSelfPosition, Vector2 inTargetAPosition, Vector2 inTargetBPosition, float inAngleBuffer)
    {
        // Calculate direction to the wall and to the chase point
        Vector2 vec_a = (inTargetAPosition - inSelfPosition).normalized;
        Vector2 vec_b = (inTargetBPosition - inSelfPosition).normalized;

        // Calculate the angle between these two directions
        float angle = Vector2.Angle(vec_a, vec_b);

        // if the wall is not in the same general direction as the chase point, ignore this
        return (angle < inAngleBuffer);
    }
}
