using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newBoxController : MonoBehaviour
{


    public playerBoxController playerBoxController;

    public Shaker shaker;

    public bool PlayerHasBox = false;
    public void GivePlayerBox()
    {
        PlayerHasBox = true;
        playerBoxController.playerBoxImage.enabled = true;
        playerBoxController.ImageFrameByFrameAnimation();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}