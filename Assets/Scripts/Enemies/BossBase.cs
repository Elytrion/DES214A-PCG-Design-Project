using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BossState = BossFightManager.BossFightState;
using System.Linq;

[RequireComponent(typeof(EnemyBase))]
public class BossBase : MonoBehaviour
{
    public EnemyBase _eb;
    public BossState CurrentBossState = BossState.IDLE;
    [System.Serializable]
    public struct AttackPhase
    {
        public BossState Phase;
        public EnemyAttack AttackLogic;
        public Sprite WeaponSprite;
    };

    public List<AttackPhase> AllAttacks;

    [System.Serializable]
    public struct MovementPhase
    {
        public BossState Phase;
        public EnemyMovement MovementLogic;
    };

    public List<MovementPhase> AllMovements;

    [SerializeField]
    private float TimeBtwAttackChanges = 5.0f;
    [SerializeField]
    private float TimeBtwMovementChanges = 5.0f;
    
    private float timer_ac = 0.0f;
    private float timer_mc = 0.0f;
    
    private bool _shouldChangeAttacks;
    private bool _shouldChangeMovements;

    // Start is called before the first frame update
    void Start()
    {
        _eb = GetComponent<EnemyBase>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_shouldChangeAttacks)
        {
            timer_ac += Time.deltaTime;
            if (timer_ac >= TimeBtwAttackChanges)
            {
                _shouldChangeAttacks = true;
                timer_ac = 0.0f;
            }
        }

        if (!_shouldChangeMovements)
        {
            timer_mc += Time.deltaTime;
            if (timer_mc >= TimeBtwMovementChanges)
            {
                _shouldChangeMovements = true;
                timer_mc = 0.0f;
            }
        }

        if (_shouldChangeAttacks)
        {
            if (_eb.AttackLogic == null || !_eb.AttackLogic.IsAttacking(_eb._idata))
            {
                _shouldChangeAttacks = false;
                ChangeAttack();
            }
        }

        if (_shouldChangeMovements)
        {
            _shouldChangeMovements = false;
            ChangeMovement();
        }
        
    }

    private void ChangeAttack()
    {
        var allAttacksForState = AllAttacks.Where(a => a.Phase == CurrentBossState).ToList();

        if (allAttacksForState.Count <= 0)
            return;

        AttackPhase randomAttackPhase = allAttacksForState[Random.Range(0, allAttacksForState.Count)];
        _eb.AttackLogic = randomAttackPhase.AttackLogic;
        _eb.AttackLogic.SetUpInstanceData(_eb._idata);

        _eb._enemyArmsr.sprite = randomAttackPhase.WeaponSprite;
    }

    private void ChangeMovement()
    {
        var allMovementsForState = AllMovements.Where(a => a.Phase == CurrentBossState).ToList();

        if (allMovementsForState.Count <= 0)
            return;

        MovementPhase randomMovePhase = allMovementsForState[Random.Range(0, allMovementsForState.Count)];
        _eb.MovementLogic = randomMovePhase.MovementLogic;

    }
}
