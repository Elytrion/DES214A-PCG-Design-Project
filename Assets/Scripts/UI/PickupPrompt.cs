using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupPrompt : MonoBehaviour
{
    public TextMeshProUGUI PromptText;
    public Image PromptImage;

    void Update()
    {
        //float width = PromptText.preferredWidth;
        float height = PromptText.preferredHeight;

        PromptImage.rectTransform.sizeDelta = new Vector2(PromptImage.rectTransform.sizeDelta.x, height);
    }
}
