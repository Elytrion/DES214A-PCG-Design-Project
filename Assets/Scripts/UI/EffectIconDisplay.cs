using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EffectIconDisplay : MonoBehaviour
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextMeshProUGUI _text;

    public void SetIcon(Sprite inIcon)
    {
        _icon.sprite = inIcon;
    }

    public void SetText(string inText)
    {
        _text.text = inText;
    }
}
