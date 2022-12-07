using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles_Manager : MonoBehaviour
{
    #region Singleton
    public static Particles_Manager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    public GameObject m_WalkingParticle;
    public GameObject m_TeleportParticle;
    public GameObject m_ExplosionParticle;
}
