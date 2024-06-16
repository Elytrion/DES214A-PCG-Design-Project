using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private DialoguePanel _dialoguePanel;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private CameraFollowCursor _camFollow;

    [System.Serializable]
    public struct Dialogue
    {
        public int ActorIndex;
        public bool Randomize;
        public string[] Text;
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
        public string SceneID;
        public Dialogue[] Dialogues;
        public Actor[] Actors;
    }

    public List<DialogueScene> AllScenes;

    private DialogueScene _currentScene;
    private int _currentDialogueIndex = 0;

    public bool InCutscene = false;

    private void Start()
    {
        InCutscene = false;
    }

    public void RunCutscene(string ID)
    {
        _currentScene = AllScenes.Where(x => x.SceneID == ID).FirstOrDefault();
        if (_currentScene.SceneID == null)
        {
            Debug.LogError("Scene ID not found: " + ID);
            return;
        }
        _currentDialogueIndex = 0;
        InCutscene = true;
        _dialoguePanel.MoveIn();
        _player.FreezePlayer();
        _player.NoDamage = true;
        FreezeEnemies();
        PlayDialogue();
    }

    void PlayDialogue()
    {
        Dialogue currDialogue = _currentScene.Dialogues[_currentDialogueIndex];
        Actor currActor = _currentScene.Actors[currDialogue.ActorIndex];

        if (currActor.ActorPosition != null)
        {
            _camFollow.TargetTransform = currActor.ActorPosition;
        }

        _dialoguePanel.UpdateCharacterName(currActor.Name);
        _dialoguePanel.UpdateCharacterSprite(currActor.Image);
        string text = (currDialogue.Randomize) ? currDialogue.Text[Random.Range(0, currDialogue.Text.Length)] : currDialogue.Text[0];
        _dialoguePanel.UpdateDialogueText(text);     
    }

    void Update()
    {
        if (InCutscene)
        {
            RunCutscene();
        }
        else
        {
            _dialoguePanel.MoveOut();
        }
    }

    private void RunCutscene()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _currentDialogueIndex++;
            if (_currentDialogueIndex >= _currentScene.Dialogues.Length)
            {
                InCutscene = false;
                _currentDialogueIndex = 0;
                _dialoguePanel.MoveOut();
                _player.CanMove = true;
                _player.CanAttack = true;
                _player.NoDamage = false;
                _camFollow.TargetTransform = _player.transform;
                UnFreezeEnemies();
            }
            else
            {
                PlayDialogue();
            }
        }
    }

    private void FreezeEnemies()
    {
        EnemyDistributer ed = FindObjectOfType<EnemyDistributer>();
        foreach (EnemyBase eb in ed.SpawnedEnemiesScripts)
        {
            if (eb == null)
                continue;

            eb._entity.CanMove = false;
            eb._entity.CanAttack = false;
            if (eb._erb != null)
            {
                eb.gameObject.GetComponent<InstancedData>().SetID<Vector2>("prevVelocity", eb._erb.velocity);
                eb._erb.velocity = Vector2.zero;
            }
        }

        var allBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in allBullets)
        {
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void UnFreezeEnemies()
    {
        EnemyDistributer ed = FindObjectOfType<EnemyDistributer>();
        foreach (EnemyBase eb in ed.SpawnedEnemiesScripts)
        {
            if (eb == null)
                continue;

            eb._entity.CanMove = true;
            eb._entity.CanAttack = true;
            if (eb._erb != null)
            {
                eb._erb.velocity = eb.gameObject.GetComponent<InstancedData>().GetID<Vector2>("prevVelocity");
            }
        }

        var allBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in allBullets)
        {
            var projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
                bullet.GetComponent<Rigidbody2D>().velocity = projectile.IntitalProjectileSpeed;
        }
    }
}
