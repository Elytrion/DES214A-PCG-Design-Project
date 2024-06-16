using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunHook : MonoBehaviour
{
    [SerializeField]
    private string StartRoomID;
    [SerializeField]
    private string DialogueID;
    [SerializeField]
    private string RoomID;
    
    private DialogueManager _dialogueManager;
    private MapGenerator _mapGenerator;
    [SerializeField]
    private MoveThenDespawn _otherPlayerMover;
    private bool _hasntRun = true;
    private Vector2Int _roomPtToRunTo;

    private
    // Start is called before the first frame update
    void Start()
    {
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _mapGenerator = FindObjectOfType<MapGenerator>();
        var MapGraphNode = _mapGenerator.MapGraphChosen.GetNode(RoomID);
        // get an id from MapGraphNode.Neighbors that isnt the start room ID
        foreach (var neighbor in MapGraphNode.Neighbors)
        {
            if (neighbor != StartRoomID)
            {
                _roomPtToRunTo = _mapGenerator._instantiatedRooms[neighbor].SpawnPosition;
                break;
            }
        }
        _otherPlayerMover.gameObject.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_hasntRun)
        {
            if (!_dialogueManager.InCutscene)
            {
                _otherPlayerMover.MoveTo(_roomPtToRunTo);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "Player") && _hasntRun)
        {
            _dialogueManager.RunCutscene(DialogueID);
            _dialogueManager.InCutscene = true;
            _hasntRun = false;
        }
    }
}
