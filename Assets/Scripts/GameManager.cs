using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MapGenerator _mapGenerator;
    [SerializeField]
    private EnemyDistributer _enemySpawner;
    [SerializeField]
    private DialogueManager _dialogueManager;
    [SerializeField]
    private Player _player;

    public string StartRoomID;
    public string EndRoomID;
    public float DistanceToExit = 1.0f;

    public bool ForceReset = false;

    public int MapsGenerated = 0;
    public bool PerformStressTest = false;
    public int StressTestIterations = 100;
    private float _stressTestTimer = 0.0f;
    [SerializeField]
    private float _stressTestInterval = 0.1f;

    private bool _setupLevel = false;

    [HideInInspector]
    public bool StressTestFailed = false;

    public int NumOfMaps = 1;
    public List<Vector3> DifficultyThresholdRatios;

    private bool _switchScene = false;

    [SerializeField]
    private GameObject _bossFightManager;

    [SerializeField]
    private FadeInFadeOut _camFade;

    private bool endoflevel = false;

    [SerializeField]
    private ObjectiveArrow _objectiveArrow;


    // Start is called before the first frame update
    void Start()
    {
        _camFade.StartFade = true;
        _bossFightManager.SetActive(false);
        _switchScene = false;
        _enemySpawner.NumOfMaps = NumOfMaps;
        _enemySpawner.SumOfRooms = 0;
        _enemySpawner.DifficultyThresholdRatios = DifficultyThresholdRatios[0];
        endoflevel = false;
        if (NumOfMaps > 0)
            SetupLevel();
        else
        {
            _bossFightManager.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ForceReset || IsPlayerAtEnd())
        {
            endoflevel = true;
            _camFade.StartFade = true;
            ForceReset = false;
        }

        if (endoflevel)
        {
            if (_camFade.alpha < 1f)
                return;
            _camFade.StartFade = false;
            if (_switchScene)
            {
                _bossFightManager.SetActive(true);
                gameObject.SetActive(false);
                return;
            }
            endoflevel = false;
            SetupLevel();
        }

        if (_player.Health <= 0)
        {
            ResetPlayer();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!_setupLevel)
                _setupLevel = true;

            _mapGenerator.GenerateMap();

            if (!_mapGenerator.GenerationFailed)
            {
                _player.transform.position = _mapGenerator.GetStartingPosition(StartRoomID);
                if (!PerformStressTest)
                {
                    _dialogueManager.RunCutscene("StartingScene");
                    _dialogueManager.InCutscene = true;
                    int index = (MapsGenerated >= DifficultyThresholdRatios.Count) ? DifficultyThresholdRatios.Count - 1 : MapsGenerated;
                    _enemySpawner.DifficultyThresholdRatios = DifficultyThresholdRatios[index];
                    if (MapsGenerated >= NumOfMaps)
                    {
                        _switchScene = true;
                    }
                }
            }
        }

        if (PerformStressTest)
        {
            _stressTestTimer += Time.deltaTime;
            if (_stressTestTimer >= _stressTestInterval)
            {
                _stressTestTimer = 0.0f;
                SetupLevel();
                MapsGenerated++;
            }

            if (MapsGenerated >= StressTestIterations || _mapGenerator.CompleteGenerationFailure)
            {
                Debug.Log("-------- STRESS TEST COMPLETE --------");
                PerformStressTest = false;            
            }
        }
    }

    private bool IsPlayerAtEnd()
    {
        if (!_setupLevel)
            return false;

        Vector3 playerPos = _player.transform.position;
        Vector3 endPos = _mapGenerator.GetEndPosition(EndRoomID);
        return (Vector3.Distance(playerPos, endPos) < DistanceToExit);
    }

    private void SetupLevel()
    {
        if (!_setupLevel)
            _setupLevel = true;

        _mapGenerator.GenerateMap();

        if (!_mapGenerator.GenerationFailed)
        {
            _player.transform.position = _mapGenerator.GetStartingPosition(StartRoomID);
            if (!PerformStressTest)
            {
                _dialogueManager.RunCutscene("StartingScene");
                _dialogueManager.InCutscene = true;
                MapsGenerated++;
                int index = (MapsGenerated >= DifficultyThresholdRatios.Count) ? DifficultyThresholdRatios.Count - 1 : MapsGenerated;
                _enemySpawner.DifficultyThresholdRatios = DifficultyThresholdRatios[index];
                Vector3 endPos = _mapGenerator.GetEndPosition(EndRoomID);
                _objectiveArrow.objectivePosition = endPos;
                if (MapsGenerated >= NumOfMaps)
                {
                    _switchScene = true;
                }
            }
        }
    }

    private void ResetPlayer()
    {
        _player.transform.position = _mapGenerator.GetStartingPosition(StartRoomID);
        _player.Health = _player.MaxHealth;
        _player.Speed = 5.0f;
        var wallet = _player.GetComponent<Wallet>();
        wallet.DeductCoins(10);
        var eem = _player.GetComponent<EntityEffectManager>();
        eem.RemoveAllEffects();
    }

   

}
