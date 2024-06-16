using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour
{
    [SerializeField]
    private GameObject objectiveArrows;
    public Vector3 objectivePosition;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the pointer to the objective relative to the player
        Vector3 direction = objectivePosition - player.transform.position;
        direction.Normalize();
        this.transform.up = direction;

        if (Input.GetKeyDown(KeyCode.V))
        {
            objectiveArrows.GetComponent<Image>().enabled = !objectiveArrows.GetComponent<Image>().enabled;
        }
    }
}
