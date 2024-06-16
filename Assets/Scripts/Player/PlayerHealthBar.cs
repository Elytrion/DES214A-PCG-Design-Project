using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public RectTransform healthBarContainer;
    public Image healthBarFill;
    public float healthBarWidthPerHealthPoint = 10f; // determines how wide the health bar should be per point of health

    public Player PlayerScript;

    private void Update()
    {
        float CurrHP = PlayerScript.Health;
        float MaxHP = PlayerScript.MaxHealth;

        // Modify the width of the health bar based on max health
        healthBarContainer.sizeDelta = new Vector2(MaxHP * healthBarWidthPerHealthPoint, healthBarContainer.sizeDelta.y);
        healthBarFill.rectTransform.sizeDelta = new Vector2(MaxHP * healthBarWidthPerHealthPoint, healthBarFill.rectTransform.sizeDelta.y);

        // Modify the fill amount based on current health
        healthBarFill.fillAmount = Mathf.Min(1.0f, CurrHP / MaxHP);
    }
}
