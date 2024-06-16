using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AOELogic : MonoBehaviour
{
    public GameObject BG_Obj;
    public GameObject FG_Obj;
    public SpriteRenderer BG_Sr;
    public SpriteRenderer FG_Sr;

    public float GrowTime = 1.0f;
    public float FadeTime = 1.0f;
    public float StayTime = 1.0f;

    private float _growTimer = 0.0f;
    private float _fadeTimer = 0.0f;
    private float _stayTimer = 0.0f;

    [SerializeField]
    private float _radius;

    private float lifeTimeRatio = 0.0f;

    public int Damage = 1;
    public PowerUp EffectToApplyOnHit;

    public List<string> tagsToHit;

    private void Start()
    {
        BG_Obj.transform.localScale = new Vector3(_radius * 2.0f, _radius * 2.0f, 1.0f);
        FG_Obj.transform.localScale = new Vector3(0f, 0f, 1.0f);

        _growTimer = 0.0f;
        _fadeTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_growTimer < GrowTime)
        {
            _growTimer += Time.deltaTime;
            lifeTimeRatio = _growTimer / GrowTime;
            FG_Obj.transform.localScale = Vector3.Lerp(new Vector3(0f, 0f, 1.0f), BG_Obj.transform.localScale, lifeTimeRatio);
            if (_growTimer >= GrowTime)
            {
                FG_Obj.transform.localScale = BG_Obj.transform.localScale;
            }
        }
        else if (_stayTimer < StayTime)
        {
            _stayTimer += Time.deltaTime;
            if (_stayTimer >= StayTime)
            {
                RunAttack();
            }
        }
        else if (_fadeTimer < FadeTime)
        {

            _fadeTimer += Time.deltaTime;
            lifeTimeRatio = _fadeTimer / FadeTime;
            float alpha = 1.0f - lifeTimeRatio;
            FG_Sr.color = new Color(FG_Sr.color.r, FG_Sr.color.g, FG_Sr.color.b, alpha);
            BG_Sr.color = Color.clear;
            if (_fadeTimer >= FadeTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void RunAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _radius);
        foreach (Collider2D hit in hits)
        {
            if (tagsToHit.Contains(hit.gameObject.tag))
            {
                hit.GetComponent<Entity>().Health -= Damage;
                if (EffectToApplyOnHit != null)
                {
                    var eem = hit.GetComponent<EntityEffectManager>();
                    if (eem != null)
                    { 
                        eem.ApplyPowerUp(EffectToApplyOnHit);
                    }
                }
                return;
            }
        }
    }

}
