using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomPrefabTileSet = MapGenerator.RoomPrefabTileSet;

public class BossFightManager : MonoBehaviour
{
    public enum BossFightState
    {
        IDLE = 0,
        PHASE1,
        PHASE2,
        PHASE3,
        END,
        NONE
    };

    [SerializeField]
    private MapGenerator _mapGenerator;
    [SerializeField]
    private EnemyDistributer _enemySpawner;
    [SerializeField]
    private DialogueManager _dialogueManager;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private BossBase _boss;

    public string StartRoomID;
    public string CheckpointRoomID;
    public string BossRoomID;
    public string BatteryRoomID1;
    public string BatteryRoomID2;
    private bool _setupLevel = false;

    public Vector3 DifficultyThresholdRatio;

    public DestroyableEntity Battery1;
    public DestroyableEntity Battery2;

    public GameObject PortalPrefab;

    public BossFightState CurrentBossFightState = BossFightState.NONE;

    public string[] CutsceneIDs;

    public GameObject BossFightTrigger;

    [SerializeField]
    private List<MapGraph> _mapGraphPool;
    [SerializeField]
    private List<RoomPrefabTileSet> _roomPrefabPools;
    [SerializeField]
    private string[] IgnoredIDs;
    public int NumOfIgnoredStarts;
    public int NumOfAllIgnored;
    public FinaleBossRoom FinaleRoomObjects;

    private bool fightStarted = false;

    private bool oneBatteryDestroyed = false;
    private bool twoBatteriesDestroyed = false;

    public ObjectiveArrow _objArrow;

    // Start is called before the first frame update
    void Start()
    {
        Battery1.gameObject.SetActive(true);
        Battery2.gameObject.SetActive(true);
        _boss.gameObject.SetActive(true);
        BossFightTrigger.SetActive(true);
        PortalPrefab.SetActive(false);
        _enemySpawner.IgnoredIDs = IgnoredIDs;
        _enemySpawner.NumOfIgnoredStarts = NumOfIgnoredStarts;
        _enemySpawner.NumOfAllIgnored = NumOfAllIgnored;
        _mapGenerator._mapGraphPool = _mapGraphPool;
        _mapGenerator._roomPrefabPools = _roomPrefabPools;
        fightStarted = false;
        _boss._eb._entity.NoDamage = true;
        _objArrow.objectivePosition = _boss._eb._entity.transform.position;
        SetupLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentBossFightState == BossFightState.IDLE && !_dialogueManager.InCutscene)
        {
            FinaleRoomObjects.OpenAll();
            FinaleRoomObjects.ShutEntrances();
            CurrentBossFightState = BossFightState.PHASE1;
            _objArrow.objectivePosition = Battery1.transform.position;
        }

        if (Battery1.isDestroyed || Battery2.isDestroyed)
        {
            if (!oneBatteryDestroyed)
            {
                if (Battery1.isDestroyed)
                {
                    _objArrow.objectivePosition = Battery2.transform.position;
                }
                else
                {
                    _objArrow.objectivePosition = Battery1.transform.position;
                }

                oneBatteryDestroyed = true;
                _dialogueManager.RunCutscene(CutsceneIDs[1]);
            }
        }

        if (Battery1.isDestroyed && Battery2.isDestroyed)
        {
            if (!twoBatteriesDestroyed)
            {
                _objArrow.objectivePosition = _boss._eb._entity.transform.position;
                twoBatteriesDestroyed = true;
                _dialogueManager.RunCutscene(CutsceneIDs[2]);
            }
        }

        if (Battery1.isDestroyed && Battery2.isDestroyed && CurrentBossFightState == BossFightState.PHASE1)
        {
            if (Vector3.Distance(_player.transform.position, _boss.transform.position) < 12.0f)
            {
                _boss._eb._entity.NoDamage = false;
                FinaleRoomObjects.ShutAll();
                FinaleRoomObjects._bossShield.SetActive(false);
                CurrentBossFightState = BossFightState.PHASE2;
                _dialogueManager.RunCutscene(CutsceneIDs[3]);
            }
        }

        if (CurrentBossFightState == BossFightState.PHASE2 && ((float)_boss._eb._entity.Health <= (float)_boss._eb._entity.MaxHealth / 2.0f))
        {
            _dialogueManager.RunCutscene(CutsceneIDs[4]);
            CurrentBossFightState = BossFightState.PHASE3;
        }

        if (_player.Health <= 0)
        {
            _player.transform.position = _mapGenerator.GetStartingPosition(CheckpointRoomID);
            var wallet = _player.GetComponent<Wallet>();
            wallet.DeductCoins(10);
            _player.Health = _player.MaxHealth;
            FinaleRoomObjects.OpenEntrances();
            BossFightTrigger.SetActive(true);
            var eem = _player.GetComponent<EntityEffectManager>();
            eem.RemoveAllEffects();
        }

        if (_boss._eb._entity.Health <= 0 && CurrentBossFightState == BossFightState.PHASE3)
        {
            CurrentBossFightState = BossFightState.END;
            _boss.gameObject.SetActive(false);
            PortalPrefab.SetActive(true);
            PortalPrefab.transform.position = _mapGenerator.GetStartingPosition(BossRoomID);
            _dialogueManager.RunCutscene(CutsceneIDs[5]);
            _objArrow.objectivePosition = PortalPrefab.transform.position;
        }

        if (_boss._eb._entity.Health > 0)
            _boss.CurrentBossState = CurrentBossFightState;
    }

    private void SetupLevel()
    {
        if (!_setupLevel)
            _setupLevel = true;

        _mapGenerator.GenerateMap();

        if (!_mapGenerator.GenerationFailed)
        {
            _player.transform.position = _mapGenerator.GetStartingPosition(StartRoomID);
            _boss.transform.position = _mapGenerator.GetStartingPosition(BossRoomID);
            Battery1.gameObject.transform.position = _mapGenerator.GetStartingPosition(BatteryRoomID1);
            Battery2.gameObject.transform.position = _mapGenerator.GetStartingPosition(BatteryRoomID2);
            BossFightTrigger.transform.position = _mapGenerator.GetStartingPosition(BossRoomID);
            _dialogueManager.RunCutscene("BossStartingArea");
        }
    }

    public void StartFight()
    {
        if (!fightStarted)
        {
            fightStarted = true;
            _dialogueManager.RunCutscene(CutsceneIDs[0]);
            FinaleRoomObjects.ShutAllButEntrance();
            CurrentBossFightState = BossFightState.IDLE;
        }
        else
        {
            FinaleRoomObjects.ShutEntrances();
        }

    }
}
