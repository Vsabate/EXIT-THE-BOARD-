using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla_Breakable : MonoBehaviour
{
    [Header("VARIABLES")]
    [HideInInspector] public bool l_Stepped = false;
    [HideInInspector] public bool l_DeathZone = false;

    private float l_Timer_Original = 4f;
    [SerializeField] private float l_Timer_Current;

    private Rigidbody l_Rigidbody;

    [Header("MATERIAL STUFF")]
    private MeshRenderer l_MeshRend;
    public Material l_Mat_Normal;
    public Material l_Mat_Breaking;
    public Material l_Mat_DeathZone;

    private void Start()
    {
        l_Rigidbody = GetComponent<Rigidbody>();
        l_MeshRend = GetComponent<MeshRenderer>();

        l_MeshRend.material = l_Mat_Normal;

        if (l_Rigidbody.IsSleeping())
        {
            l_Rigidbody.WakeUp();
        }
    }

    private void Update()
    {
        if (l_Rigidbody.IsSleeping())
        {
            l_Rigidbody.WakeUp();
        }

        if (l_Stepped)
        {
            if ((l_Timer_Current <= l_Timer_Original / 2f) && l_Timer_Current > 0f)
            {
                ChangeMaterial_Breaking();
            }
            else
            {
                l_Timer_Current -= Time.deltaTime;
            }

            if (l_Timer_Current <= 0f)
            {
                ChangeMaterial_DeathZone();
            }
            else
            {
                l_Timer_Current -= Time.deltaTime;
            }
        }

    }

    #region ON_COLLISION
    private void OnCollisionEnter(Collision collision)
    {
        Player pl = collision.gameObject.GetComponentInParent<Player>();
        // TIMER STARTS!
        if (collision.transform.tag == "Player_Coll")
        {
            if (!l_Stepped && !l_DeathZone)
            {
                l_Stepped = true;
                l_Timer_Current = l_Timer_Original;
            }
            if (l_DeathZone)
            {
                if (!GameManager.instance.m_GridManager.m_playerRanking.Contains(pl))
                {
                    GameManager.instance.m_GridManager.m_playerRanking.Add(pl);
                    GameManager.instance.m_GridManager.m_playersPlaying.Remove(pl);

                    Instantiate(Particles_Manager.instance.m_ExplosionParticle, pl.transform.position, Quaternion.identity);

                    collision.gameObject.GetComponentInParent<Player_OldSystem>().minigame_Playing_FallGame = false;
                    collision.transform.parent.gameObject.SetActive(false);
                    collision.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        Player pl = collision.gameObject.GetComponentInParent<Player>();
        if (collision.transform.tag == "Player_Coll")
        {
            if (l_DeathZone)
            {
                if (!GameManager.instance.m_GridManager.m_playerRanking.Contains(pl))
                {
                    GameManager.instance.m_GridManager.m_playerRanking.Add(pl);
                    GameManager.instance.m_GridManager.m_playersPlaying.Remove(pl);

                    Instantiate(Particles_Manager.instance.m_ExplosionParticle, pl.transform.position, Quaternion.identity);

                    collision.gameObject.GetComponentInParent<Player_OldSystem>().minigame_Playing_FallGame = false;
                    collision.transform.parent.gameObject.SetActive(false);
                    collision.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }
    #endregion

    #region CHANGE_MATERIAL
    public void ChangeMaterial_DeathZone()
    {
        l_MeshRend.material = l_Mat_DeathZone;
        l_DeathZone = true;
        l_Stepped = false;
        l_Timer_Current = l_Timer_Original;
    }
    public void ChangeMaterial_Breaking()
    {
        if (l_MeshRend.material != l_Mat_Breaking)
        {
            l_MeshRend.material = l_Mat_Breaking;
        }
    }
    #endregion
}
