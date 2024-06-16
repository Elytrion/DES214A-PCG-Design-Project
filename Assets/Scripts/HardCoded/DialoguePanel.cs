using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialoguePanel : MonoBehaviour
{
    public Image CharacterImage;
    public TMP_Text CharacterName;
    public TMP_Text DialogueText;
    public Vector2 offScreenPos;   // Position where the panel is off the screen
    public Vector2 onScreenPos;    // Position where the panel is on the screen

    public float PanelMoveSpeed = 5.0f;
    
    private RectTransform rectTransform;

    private bool isOnScreen = false;

    [SerializeField]
    private Vector2 _targetPos;
    private bool _shouldMove = false;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void MoveIn()
    {
        if (!isOnScreen)
        {
            _shouldMove = true;
            isOnScreen = true;
            _targetPos = onScreenPos;
        }
    }

    public void MoveOut()
    {
        if (isOnScreen)
        {
            _shouldMove = true;
            isOnScreen = false;
            _targetPos = offScreenPos;
        }
    }

    private void Update()
    {
        if (_shouldMove)
        {
            float moveSpeed = PanelMoveSpeed; // Duration of movement. Change this to control speed.
            if (Vector2.Distance(rectTransform.anchoredPosition, _targetPos) > 0.5f)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, _targetPos, Time.deltaTime * moveSpeed);
            }
            else if (Vector2.Distance(rectTransform.anchoredPosition, _targetPos) < 0.5f)
            {
                rectTransform.anchoredPosition = _targetPos;
                _shouldMove = false;
            }
        }
    }
    
    public void UpdateCharacterName(string name)
    {
        CharacterName.text = name;
    }

    public void UpdateDialogueText(string text)
    {
        DialogueText.text = text;
    }

    public void UpdateCharacterSprite(Sprite newSprite)
    {
        CharacterImage.sprite = newSprite;
    }
}
