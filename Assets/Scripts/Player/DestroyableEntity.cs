using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableEntity : MonoBehaviour
{
    [SerializeField]
    private Transform _obj;
    [SerializeField]
    private Entity _entity;
    [SerializeField]
    private Transform _hpBar;

    public float offset = 0.5f;

    public bool isDestroyed = false;
    public bool HasHealthBar = true;

    public bool DropsDebris = true;

    public float SpawnChance = -0.0f;
    public bool DropsCoins = false;
    private PowerUpSpawner _powerUpManager;

    void Start()
    {
        _entity = _obj.GetComponent<Entity>();
        _powerUpManager = FindObjectOfType<PowerUpSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDestroyed)
        {
            if (_obj.gameObject.activeSelf)
                _obj.gameObject.SetActive(false);
            return;
        }
        
        if (_entity.Health <= 0)
        {
            if (_powerUpManager != null)
            {
                bool shouldDrop = false;
                if (DropsDebris)
                {
                    shouldDrop = Random.Range(0.0f, 1.0f) > 0.7f;
                }

                _powerUpManager.TrySpawnDebris(_obj.transform.position, shouldDrop);
            }

            if (SpawnChance > 0.0f && _powerUpManager != null)
            {
                _powerUpManager.TrySpawnPowerup(_obj.transform.position, SpawnChance);
            }

            if (DropsCoins && _powerUpManager != null)
            {
                _powerUpManager.TrySpawnCoin(_obj.transform.position);
            }

            if (HasHealthBar)
                _hpBar.gameObject.SetActive(false);
            isDestroyed = true;

            _obj.gameObject.SetActive(false);
        }
        else
        {
            isDestroyed = false;
            ManageHealthBar();
        }
    }

    private void ManageHealthBar()
    {
        if (_hpBar == null || !HasHealthBar)
            return;

        _hpBar.gameObject.SetActive(_entity.Health < _entity.MaxHealth);
        _hpBar.localScale = new Vector3((float)_entity.Health / (float)_entity.MaxHealth, _hpBar.localScale.y, _hpBar.localScale.z);
        _hpBar.localPosition = new Vector3(_obj.transform.localPosition.x, _obj.transform.localPosition.y + offset, _obj.transform.localPosition.z);
    }
}
