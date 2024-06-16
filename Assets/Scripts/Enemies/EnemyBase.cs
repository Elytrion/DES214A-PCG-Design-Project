using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBase : MonoBehaviour
{
    public EnemyGenerator.EnemyDifficulty DifficultyRating;

    [SerializeField]
    private GameObject _enemyObj;
    public Entity _entity;
    [SerializeField]
    private Transform _hpBar;

    public Transform _enemyArm;
    public SpriteRenderer _enemyArmsr;
    
    public SpriteRenderer _bodyOverlay;
    public SpriteRenderer _bodyFullOverlay;
    public SpriteRenderer _headOverlay;
    public SpriteRenderer _fullOverlay;

    [SerializeField]
    private float _aggroRange = 8f;
    [SerializeField]
    private float _minDeaggroRange = 0.0f;
    public float MovementSpeed = 3f;

    public bool Aggroed = false;

    private Transform _playerTransform;
    [HideInInspector]
    public Rigidbody2D _erb;
    [HideInInspector]
    public bool IsColliding = false;

    private float _readjustTimer = 0.0f;
    private Vector2 _readjustVec;
    private Vector2 _hitPos;

    public EnemyMovement MovementLogic;
    public EnemyAttack AttackLogic;
    public EnemySpecial SpecialLogic;

    [HideInInspector]
    public bool ContactedPlayer = false;

    [SerializeField]
    private Vector2 cellOffset;

    private PowerUpSpawner _powerUpManager;

    public InstancedData _idata;

    public GameObject CoinPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _powerUpManager = FindObjectOfType<PowerUpSpawner>();
        _playerTransform = GameObject.Find("Player").transform;
        _minDeaggroRange = (_minDeaggroRange == 0.0f) ? (_aggroRange * 1.5f) : _minDeaggroRange;
        _erb = _enemyObj.GetComponent<Rigidbody2D>();
        _readjustVec = Vector2.zero;
        _hitPos = Vector2.zero;
        if (AttackLogic != null)
            AttackLogic.SetUpInstanceData(_idata);
        _entity = _enemyObj.GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_entity.Health <= 0)
        {
            if (_enemyObj.activeSelf)
                OnDeath();
            return;
        }
        else
        {
            if (!_enemyObj.activeSelf)
                _enemyObj.SetActive(true);

            ManageHealthBar();
        }

        
        if (Vector2.Distance(_playerTransform.position, _enemyObj.transform.position) < _aggroRange)
        {
            Aggroed = true;
        }
        if (Aggroed)
        {
            ManageSprites(_playerTransform.position.x > _enemyObj.transform.position.x);
            if (Vector2.Distance(_playerTransform.position, _enemyObj.transform.position) > _minDeaggroRange)
            {
                Aggroed = false;
                _erb.velocity = Vector2.zero;
                return;
            }

            if (AttackLogic != null && AttackLogic.CanAttack(this, _enemyObj.transform, _erb, _playerTransform, _idata) && _entity.CanAttack)
            {
                if (SpecialLogic != null && SpecialLogic.CanUseSpecial(this, _enemyObj.transform, _erb, _playerTransform, _idata))
                    SpecialLogic.OnAttack(this, _enemyObj.transform, _erb, _playerTransform, _idata);

                AttackLogic.Attack(this, _enemyObj.transform, _erb, _playerTransform, _enemyArm, _idata);
            }

            //Run chase logic
            if (_entity.CanMove && MovementLogic != null)
            {
                if (SpecialLogic != null && SpecialLogic.CanUseSpecial(this, _enemyObj.transform, _erb, _playerTransform, _idata))
                    SpecialLogic.OnMove(this, _enemyObj.transform, _erb, _playerTransform, _idata);
                
                MovementLogic.Chase(_enemyObj.transform, _erb, _playerTransform, MovementSpeed, IsColliding);
            }
        }

        if (AttackLogic != null && AttackLogic.CanAttackOnDeaggro(this, _enemyObj.transform, _erb, _playerTransform, _idata) && _entity.CanAttack)
        {
            if (SpecialLogic != null && SpecialLogic.CanUseSpecial(this, _enemyObj.transform, _erb, _playerTransform, _idata))
                SpecialLogic.OnAttack(this, _enemyObj.transform, _erb, _playerTransform, _idata);
            
            AttackLogic.Attack(this, _enemyObj.transform, _erb, _playerTransform, _enemyArm, _idata);
        }

        // Avoid collisions
        if (_entity.CanMove && MovementLogic != null && MovementLogic.CanReadjust(_enemyObj.transform, _erb, _playerTransform, MovementSpeed))
            ReadjustPosition();          

        Vector3 baseLookPos = (Aggroed) ? _playerTransform.position : _enemyObj.transform.position;
        if (_entity.CanAttack)
            HandleArmRotation(baseLookPos, _enemyObj.transform.position, _enemyArm, _enemyArmsr);
    }

    private void ManageSprites(bool flip)
    {
        if (!_entity.CanMove)
            return;

        if (_bodyOverlay != null)
            _bodyOverlay.flipX = flip;
        if (_bodyFullOverlay != null)
            _bodyFullOverlay.flipX = flip;
        if (_headOverlay != null)
            _headOverlay.flipX = flip;
        if (_fullOverlay != null)
            _fullOverlay.flipX = flip;
    }

    public void TintSprites(Color inTint)
    {
        if (_bodyOverlay != null)
            _bodyOverlay.color = inTint;
        if (_bodyFullOverlay != null)
            _bodyFullOverlay.color = inTint;
        if (_headOverlay != null)
            _headOverlay.color = inTint;
        if (_fullOverlay != null)
            _fullOverlay.color = inTint;
    }

    private void ManageHealthBar()
    {
        if (_hpBar == null)
            return;
        
        _hpBar.gameObject.SetActive(_entity.Health < _entity.MaxHealth);
        _hpBar.localScale = new Vector3((float)_entity.Health / (float)_entity.MaxHealth, _hpBar.localScale.y, _hpBar.localScale.z);
        _hpBar.localPosition = new Vector3(_enemyObj.transform.localPosition.x, _enemyObj.transform.localPosition.y + 0.5f, _enemyObj.transform.localPosition.z);
    }

    public void HandleArmRotation(Vector3 inTargetPos, Vector3 inSelfPos, Transform inArm, SpriteRenderer inArmSr = null)
    {
        if (inArm == null)
            return;

        Vector2 offset = new Vector2(inTargetPos.x - inSelfPos.x, inTargetPos.y - inSelfPos.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        inArm.rotation = Quaternion.Euler(0, 0, angle - 90.0f);

        if (inArmSr != null)
            inArmSr.flipY = (inArm.rotation.eulerAngles.z > 0 && inArm.rotation.eulerAngles.z < 180);
    }

    private void OnDeath()
    {
        if (AttackLogic != null)
        {
            AttackLogic.EndAttack(_idata);
        }

        _hpBar.gameObject.SetActive(false);
        _erb.velocity = Vector2.zero;
        if (SpecialLogic != null && SpecialLogic.CanUseSpecial(this, _enemyObj.transform, _erb, _playerTransform, _idata))
            SpecialLogic.OnDeath(this, _enemyObj.transform, _erb, _playerTransform, _idata);
        if (_powerUpManager != null)
        {
            _powerUpManager.TrySpawnPowerup(_enemyObj.transform.position);
        }
        
        if (CoinPrefab != null)
        {
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            GameObject coin = Instantiate(CoinPrefab, _enemyObj.transform.position + offset, Quaternion.identity);
            CoinPickup coinup = coin.GetComponent<CoinPickup>();
            coinup.CoinValue = (int)DifficultyRating;
        }

        _enemyObj.transform.position = transform.position;       
        _enemyObj.SetActive(false);


    }

    #region Basic Obstacle Avoidance
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
            if (Mathf.Approximately(_readjustVec.y, 0.0f))
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
    #endregion

    #region OnCol/OnTrig functions
    public void OnEnemyCollisionEnter(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ContactedPlayer = true;
        }
    }

    public void OnEnemyCollisionStay(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            ContactedPlayer = true;
        }
        
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Prop")
        {
            Tilemap tilemap = col.gameObject.GetComponent<Tilemap>();

            if (tilemap != null)
            {
                Vector3 hitPosition = Vector3.zero;
                Vector2 cellWorldPos = Vector2.zero;

                // go through each contact point
                foreach (ContactPoint2D hit in col.contacts)
                {
                    hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                    hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

                    Vector3Int cellPos = tilemap.WorldToCell(hitPosition);
                    cellWorldPos = new Vector2(cellPos.x + cellOffset.x, cellPos.y + cellOffset.y);
                }

                IsColliding = true;
                AvoidWallCollision(cellWorldPos);
            }
            else
            {
                Vector2 pos = new Vector2(col.transform.position.x, col.transform.position.y);
                IsColliding = true;
                AvoidWallCollision(pos);
            }
        }
    }

    public void OnEnemyCollisionExit(Collision2D col)
    {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Tag")
        {
            IsColliding = false;
        }

        if (col.gameObject.tag == "Player")
        {
            ContactedPlayer = false;
        }
    }

    public void OnEnemyTriggerEnter(Collider2D col)
    {
        
    }

    public void OnEnemyTriggerStay(Collider2D col)
    {

    }

    public void OnEnemyTriggerExit(Collider2D col)
    {

    }
    #endregion
}
