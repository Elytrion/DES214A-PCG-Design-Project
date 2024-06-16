using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HardCodedCutscene : MonoBehaviour
{
    [SerializeField]
    private DialoguePanel _dialoguePanel;

    [System.Serializable]
    public struct Dialogue
    {
        public int ActorIndex;
        public string Text;
    };
    [System.Serializable]
    public struct Actor
    {
        public string Name;
        public Sprite Image;
        public Transform ActorPosition;
    };
    [System.Serializable]
    public struct DialogueScene
    {
        public Dialogue[] Dialogues;
        public Actor[] Actors;
    }

    public DialogueScene[] AllScenes;

    [HideInInspector]
    public bool InCutscene = false;

    private int _currDialogueIndex = 0;

    public Player PlayerScript;

    public Entity Enemy;

    public GameObject MeetingCutsceneTrigger;

    public GameObject OtherPlayer;

    public GameObject BlockingDoor;

    public GameObject Portal;

    public GameObject FinalCutsceneTrigger;

    public GameObject EnemyDoor;

    private DialogueScene _currRunningScene;

    private bool _stopCheckingEnemyHealth = false;

    private int _currSceneIndex = 0;

    public TMP_Text MovementTutorialText;
    public TMP_Text CombatTutorialText;
    public TMP_Text PropTutorialText;
    public TMP_Text ObjectiveTrackerText;

    public ObjectiveArrow _objectiveArrow;
    public CameraFollowCursor _camFollow;

    // Start is called before the first frame update
    void Start()
    {
        _dialoguePanel.MoveOut();
        RunDialogueScene(0);    
        MeetingCutsceneTrigger.SetActive(false);
        OtherPlayer.SetActive(false);
        Portal.SetActive(false);
        BlockingDoor.SetActive(true);
        EnemyDoor.SetActive(false);
        MovementTutorialText.gameObject.SetActive(false);
        CombatTutorialText.gameObject.SetActive(false);
        PropTutorialText.gameObject.SetActive(false);
        ObjectiveTrackerText.gameObject.SetActive(false);
        PlayerScript.CanAttack = false;
        Enemy.CanAttack = false;
        _objectiveArrow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InCutscene)
        {
            PlayerScript.FreezePlayer();
            _dialoguePanel.MoveIn();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currDialogueIndex++;
                if (_currDialogueIndex >= _currRunningScene.Dialogues.Length)
                {
                    InCutscene = false;
                    _currDialogueIndex = 0;

                    if (_currSceneIndex == 0)
                    {
                        MovementTutorialText.gameObject.SetActive(true);
                    }
                    
                    if (_currSceneIndex == 1)
                    {
                        EnemyDoor.SetActive(true);
                        CombatTutorialText.gameObject.SetActive(true);
                        MovementTutorialText.gameObject.SetActive(false);
                        Enemy.gameObject.GetComponent<Entity>().CanAttack = true;
                    }

                    if (_currSceneIndex == 3)
                    {
                        Portal.SetActive(true);
                        PropTutorialText.gameObject.SetActive(true);
                        OtherPlayer.GetComponent<MoveThenDespawn>().StartMoving = true;
                    }
                }
                else
                {
                    PlayDialogue();
                }
            }
        }
        else
        {
            PlayerScript.CanMove = true;
            if (_currSceneIndex != 0)
            {
                PlayerScript.CanAttack = true;
            }
            _dialoguePanel.MoveOut();
        }
        
        if (!_stopCheckingEnemyHealth && Enemy.Health <= 0)
        {
            _stopCheckingEnemyHealth = true;
            RunDialogueScene(2);
            MeetingCutsceneTrigger.SetActive(true);
            OtherPlayer.SetActive(true);
            EnemyDoor.SetActive(false);
            CombatTutorialText.gameObject.SetActive(false);
        }
    }

    public void RunDialogueScene(int sceneIndex)
    {
        if (sceneIndex > AllScenes.Length)
        {
            Debug.LogWarning("Scene index out of range");
            return;
        }
        _currSceneIndex = sceneIndex;
        _currDialogueIndex = 0;
        InCutscene = true;
        _currRunningScene = AllScenes[sceneIndex];
        PlayDialogue();
    }

    void PlayDialogue()
    {
        Dialogue currDialogue = _currRunningScene.Dialogues[_currDialogueIndex];
        Actor currActor = _currRunningScene.Actors[currDialogue.ActorIndex];
        if (currActor.ActorPosition != null)
        {
            _camFollow.TargetTransform = currActor.ActorPosition;
        }
        _dialoguePanel.UpdateCharacterName(currActor.Name);
        _dialoguePanel.UpdateCharacterSprite(currActor.Image);
        _dialoguePanel.UpdateDialogueText(currDialogue.Text);
    }

    public void RunObjectiveCutscene()
    {
        RunDialogueScene(4);
        if (_objectiveArrow != null)
        {
            _objectiveArrow.gameObject.SetActive(true);
            _objectiveArrow.objectivePosition = Portal.transform.position;
        }
        BlockingDoor.SetActive(false);
        PropTutorialText.gameObject.SetActive(false);
        ObjectiveTrackerText.gameObject.SetActive(true);
    }
}
