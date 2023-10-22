using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    // Start is called before the first frame update

    public float AngleToRoatateBy = 25f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, AngleToRoatateBy * Time.deltaTime);
    }
}