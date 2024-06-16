using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCutsceneOnDestroy : MonoBehaviour
{
    public HardCodedCutscene _hcc;
    
    void OnDestroy()
    {
        _hcc.RunObjectiveCutscene();
    }
}
