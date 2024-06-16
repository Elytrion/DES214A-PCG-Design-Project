using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    [SerializeField]
    private SpriteRenderer _sr;
    [SerializeField]
    private Transform _playerArm;
    [SerializeField]
    private SpriteRenderer _playerArmsr;

    public GameObject _bulletPrefab;

    public int playerDamage = 1;

    public float Speed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 movementInputs;

    public PlayerAttack AttackLogic;
    public InstancedData IData;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MaxHealth = Health;
    }

    void Update()
    {
        _playerArm.gameObject.SetActive(CanAttack);

        if (CanMove)
        {
            movementInputs.x = Input.GetAxis("Horizontal");
            movementInputs.y = Input.GetAxis("Vertical");
            _sr.flipX = (movementInputs.x > 0);
            Vector2 movement = new Vector2(movementInputs.x, movementInputs.y);
            rb.velocity = movement * Speed;

            HandleArmRotation(Input.mousePosition, Camera.main.WorldToScreenPoint(transform.localPosition), _playerArm, _playerArmsr);
        }


        if (Input.GetMouseButtonDown(0) && CanAttack)
        {
            //FireBullet();
            Attack();
        }
    }
    
    public void HandleArmRotation(Vector3 inTargetPos, Vector3 inSelfPos, Transform inArm, SpriteRenderer inArmSr = null)
    {
        Vector2 offset = new Vector2(inTargetPos.x - inSelfPos.x, inTargetPos.y - inSelfPos.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        inArm.rotation = Quaternion.Euler(0, 0, angle - 90.0f);

        if (inArmSr != null)
        {
            inArmSr.flipY = (inArm.rotation.eulerAngles.z > 0 && inArm.rotation.eulerAngles.z < 180);
            inArmSr.sprite = AttackLogic.WeaponSprite;
        }
    }
    
    public void FreezePlayer()
    {
        CanMove = false;
        CanAttack = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        _playerArm.rotation = Quaternion.identity;
    }
    
    private void Attack()
    {
        if (AttackLogic == null)
            return;

        if (AttackLogic.CanUseAttack(this, transform, _playerArm, IData))
        {
            AttackLogic.UseAttack(this, transform, _playerArm, _bulletPrefab, playerDamage, IData);
        }
    }
}
