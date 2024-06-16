using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAttack : ScriptableObject
{
    public Sprite WeaponSprite;

    public abstract void SetUpInstanceData(InstancedData inIData);

    public abstract void UseAttack(Player inPlayer, Transform inPlayerTransform, Transform inPlayerArmTransform, GameObject inAttackProjectilePrefab, int PlayerDamage, InstancedData inIData);

    public abstract bool CanUseAttack(Player inPlayer, Transform inPlayerTransform, Transform inPlayerArmTransform, InstancedData inIData);
}
