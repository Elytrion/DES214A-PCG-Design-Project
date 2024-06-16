using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    public string SceneToChangeTo;
    public FadeInFadeOut CamFade;
    private bool _switchScene = false;
    public float FadeSpeed = 0.1f;

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "Player"))
        {
            _switchScene = true;
            CamFade.StartFade = true;
            CamFade.speedScale = FadeSpeed;
        }
    }

    private void Update()
    {
        if (_switchScene)
        {
            if (CamFade.alpha < 1f)
                return;
            CamFade.StartFade = false;
            _switchScene = false;
            // Change scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneToChangeTo);
        }
    }
}
