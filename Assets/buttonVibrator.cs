using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class buttonVibrator : MonoBehaviour
{



    [DllImport("__Internal")]
    public static extern void Vibrate2();

    public void OnVibrate2()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        Vibrate2();
#endif
    }

}