using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plr_Sounds : MonoBehaviour
{
    public void Sound_StepSound(string path)
    {
        SoundManager.instance.PlaySound(path);
    }
}
