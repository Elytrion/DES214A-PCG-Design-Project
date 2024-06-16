using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public PowerUp EffectToApply;
    public bool IsTemporaryTrap = false;
    public bool AffectPlayerOnly = true;
    public float TrapDuration = 5f;
    public float TrapCooldown = 5f;
    public bool Oscillates = true;
    private float _timer = 0f;

    public Sprite ActiveSprite;
    public Sprite InactiveSprite;
    private SpriteRenderer _sr;

    private bool isTrapActive = false;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = ActiveSprite;
        _timer = 0f;
        isTrapActive = true;
    }

    private void Update()
    {
        if (!Oscillates)
            return;

        _timer += Time.deltaTime;
        if (_timer >= TrapDuration)
        {
            if (IsTemporaryTrap)
                Destroy(gameObject);

            _sr.sprite = InactiveSprite;
            isTrapActive = false;
        }
        else if (_timer >= (TrapDuration + TrapCooldown))
        {
            _sr.sprite = ActiveSprite;
            isTrapActive = true;
            _timer = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTrapActive)
            return;

        if (!other.CompareTag("Player") && AffectPlayerOnly)
            return;

        EntityEffectManager eem = other.GetComponent<EntityEffectManager>();
        if (eem != null)
        {
            eem.ApplyPowerUp(EffectToApply);
        }
    }

}
