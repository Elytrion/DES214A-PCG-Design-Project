using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New EnemyGenSet", menuName = "EnemyLogic/EnemyGenSet")]
public class EnemyGenerator : ScriptableObject
{

    public enum EnemyDifficulty
    {
        EASY = 1,
        MEDIUM,
        HARD
    };

    [System.Serializable]
    public struct BodyTypes
    {
        public EnemyDifficulty Difficulty;
        public Sprite[] BodySprites;
    };

    //[System.Serializable]
    //public struct MovementContainer
    //{
    //    public EnemyMovement MovementLogic;
    //};
    //[System.Serializable]
    //public struct MovementTypes
    //{
    //    public EnemyDifficulty Difficulty;
    //    public MovementContainer[] MovementLogics;
    //};

    //[System.Serializable]
    //public struct AttackContainer
    //{
    //    public EnemyAttack AttackLogic;
    //    public Sprite AttackSprite;
    //    public Sprite BodySprite;
    //    public Sprite FullBodySprite;
    //};
    //[System.Serializable]
    //public struct AttackTypes
    //{
    //    public EnemyDifficulty Difficulty;
    //    public AttackContainer[] AttackLogics;
    //};

    [System.Serializable]
    public struct SpecialContainer
    {
        public EnemySpecial SpecialLogic;
        public Sprite OverlaySprite;
        public float ChanceToHave;
    };
    [System.Serializable]
    public struct SpecialTypes
    {
        public EnemyDifficulty Difficulty;
        public SpecialContainer[] SpecialLogics;
    };
    
    [System.Serializable]
    public struct EnemyGenSet
    {
        public EnemyMovement MovementLogic;
        public EnemyAttack AttackLogic;

        public Sprite BodySprite;
        public Sprite AttackSprite;
        public Sprite FullBodySprite;
    };
    [System.Serializable]
    public struct EnemyTypes
    {
        public EnemyDifficulty Difficulty;
        public EnemyGenSet[] EnemyLogics;
    };


    public List<BodyTypes> AllBodyTypes;
    //public List<MovementTypes> AllMovementTypes;
    //public List<AttackTypes> AllAttackTypes;
    public List<EnemyTypes> AllEnemyTypes;
    public List<SpecialTypes> AllSpecialTypes;

    public GameObject BaseEnemyPrefab;

    public GameObject CreateEnemy(Vector3 inSpawn, EnemyDifficulty inDifficulty, int inHP, List<EnemyAttack> inDoNotSpawnTypes = null)
    {
        GameObject enemy = Instantiate(BaseEnemyPrefab, inSpawn, Quaternion.identity);
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        ModifyEnemyData(enemyBase, inDifficulty, inHP, inDoNotSpawnTypes);
        return enemy;
    }

    public void ModifyEnemyData(EnemyBase inEnemy, EnemyDifficulty inDifficulty, int inHP, List<EnemyAttack> inDoNotSpawnTypes = null)
    {
        // select a random  enemy sprite, set as head sprite
        Sprite headSprite = GetRandomBaseFromDifficulty(inDifficulty);

        //// select a random enemy movement, set leg sprite + movement logic
        //MovementContainer movement = GetRandomMovementFromDifficulty(inDifficulty);
        //// select a random enemy attack, set body sprite + attack logic
        //AttackContainer attack = GetRandomAttackFromDifficulty(inDifficulty);

        // select a random enemy type
        EnemyGenSet enemyType = GetRandomEnemyTypeFromDifficulty(inDifficulty, inDoNotSpawnTypes);

        // select a random enemy special, set special logic
        SpecialContainer special = GetRandomSpecialFromDifficulty(inDifficulty);

        inEnemy.DifficultyRating = inDifficulty;

        inEnemy._headOverlay.sprite = headSprite;
        
        inEnemy._bodyOverlay.sprite = enemyType.BodySprite;
        inEnemy._bodyFullOverlay.sprite = enemyType.FullBodySprite;
        inEnemy._enemyArmsr.sprite = enemyType.AttackSprite;
        inEnemy.AttackLogic = enemyType.AttackLogic;
        inEnemy.MovementLogic = enemyType.MovementLogic;
        
        inEnemy.SpecialLogic = special.SpecialLogic;
        inEnemy._fullOverlay.sprite = special.OverlaySprite;

        inEnemy._entity.Health = inEnemy._entity.MaxHealth = inHP;
    }

    public Sprite GetRandomBaseFromDifficulty(EnemyDifficulty inDifficulty)
    {
        foreach (BodyTypes bodyType in AllBodyTypes)
        {
            if (bodyType.Difficulty == inDifficulty)
            {
                if (bodyType.BodySprites.Length > 0)
                {
                    return bodyType.BodySprites[Random.Range(0, bodyType.BodySprites.Length)];
                }
                break;
            }
        }
        return null;
    }

    public EnemyGenSet GetRandomEnemyTypeFromDifficulty(EnemyDifficulty inDifficulty, List<EnemyAttack> inDoNotSpawnTypes = null)
    {
        foreach (EnemyTypes enemyType in AllEnemyTypes)
        {
            if (enemyType.Difficulty == inDifficulty)
            {
                if (enemyType.EnemyLogics.Length > 0)
                {
                    EnemyGenSet[] allocatedEnemies = enemyType.EnemyLogics;
                    if (inDoNotSpawnTypes != null)
                    {
                        allocatedEnemies = allocatedEnemies.Where(x => !inDoNotSpawnTypes.Contains(x.AttackLogic)).ToArray();
                    }
                    return allocatedEnemies[Random.Range(0, allocatedEnemies.Length)];
                }
            }
        }

        return new EnemyGenSet();
    }

    //public MovementContainer GetRandomMovementFromDifficulty(EnemyDifficulty inDifficulty)
    //{
    //    foreach (MovementTypes movementType in AllMovementTypes)
    //    {
    //        if (movementType.Difficulty == inDifficulty)
    //        {
    //            if (movementType.MovementLogics.Length > 0)
    //            {
    //                return movementType.MovementLogics[Random.Range(0, movementType.MovementLogics.Length)];
    //            }
    //        }
    //    }
    //    return new MovementContainer();
    //}
    //public AttackContainer GetRandomAttackFromDifficulty(EnemyDifficulty inDifficulty)
    //{
    //    foreach (AttackTypes attackType in AllAttackTypes)
    //    {
    //        if (attackType.Difficulty == inDifficulty)
    //        {
    //            if (attackType.AttackLogics.Length > 0)
    //            {
    //                return attackType.AttackLogics[Random.Range(0, attackType.AttackLogics.Length)];
    //            }
    //        }
    //    }

    //    return new AttackContainer();
    //}

    public SpecialContainer GetRandomSpecialFromDifficulty(EnemyDifficulty inDifficulty)
    {
        foreach (SpecialTypes specialType in AllSpecialTypes)
        {
            if (specialType.Difficulty == inDifficulty)
            {
                if (specialType.SpecialLogics.Length > 0)
                {
                    Dictionary<SpecialContainer, float> specialChances = new Dictionary<SpecialContainer, float>();
                    foreach (SpecialContainer special in specialType.SpecialLogics)
                    {
                        specialChances.Add(special, special.ChanceToHave);
                    }
                    SpecialContainer selectedSpecial = PCG.WeightedRandom(specialChances);
                    return selectedSpecial;
                }
            }
        }

        return new SpecialContainer();
    }


}
