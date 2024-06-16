using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDunGen : MonoBehaviour
{
    [SerializeField]
    protected TileMapVisualizer _tileMapVis = null;

    [SerializeField]
    protected Vector2Int _startPos = Vector2Int.zero;
    
    [SerializeField]
    protected bool UseSeed = false;
    
    [SerializeField]
    protected int Seed = 0;

    [HideInInspector]
    public bool CompleteGenerationFailure = false;

    public bool GenerationFailed = false;
    protected int MaxGenerationAttemps = 32;
    protected int CurrGenerationAttempts = 0;
    public void GenerateMap()
    {
        if (!GenerationFailed)
            CurrGenerationAttempts = 0;
        
        _tileMapVis.Clear();
        Seed = ((UseSeed) ? Seed : (int)System.DateTime.Now.Ticks);
        
        if (GenerationFailed)
        {
            CurrGenerationAttempts++;
            Seed = (int)System.DateTime.Now.Ticks;
            if (CurrGenerationAttempts > MaxGenerationAttemps)
            {
                Debug.LogError("--------Max generation attempts reached!---------");
                CompleteGenerationFailure = true;
                return;
            }
            GenerationFailed = false;
        }

        Random.InitState(Seed);
        RunGeneration();
    }

    protected abstract void RunGeneration();
}
