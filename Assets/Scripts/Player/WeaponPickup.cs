using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : ItemPickup
{
    public PlayerAttack AttackLogic;
    private SpriteRenderer _sr;
    public GameObject WeaponPickupPrefab;
    public GameObject BulletToBeShotPrefab;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null && AttackLogic != null)
            _sr.sprite = AttackLogic.WeaponSprite;
    }

    public override void OnPickup(GameObject target)
    {
        var player = target.GetComponent<Player>();      
        if (player != null)
        {
            Vector3 offset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
            var weaponPickup = Instantiate(WeaponPickupPrefab, transform.position + offset, Quaternion.identity);
            var weaponPickupLogic = weaponPickup.GetComponent<WeaponPickup>();
            if (weaponPickupLogic != null)
            {
                weaponPickupLogic.AttackLogic = player.AttackLogic;
                weaponPickup.GetComponent<SpriteRenderer>().sprite = player.AttackLogic.WeaponSprite;
                weaponPickupLogic.BulletToBeShotPrefab = player._bulletPrefab;
            }          
            player.AttackLogic = AttackLogic;
            if (BulletToBeShotPrefab != null)
                player._bulletPrefab = BulletToBeShotPrefab;

            var promptLogic = weaponPickup.GetComponent<PickupPrompt>();
            if (promptLogic != null)
                promptLogic.PromptText.text = "Press E to swap weapons";
        }
    }

    public override bool CanPickup(GameObject target)
    {
        return Input.GetKeyDown(KeyCode.E);
    }
}
