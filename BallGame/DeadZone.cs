using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Player pl = collision.gameObject.GetComponentInParent<Player>();
        if (collision.transform.tag == "Player_Coll")
        {
            if (!GameManager.instance.m_BallManager.m_playerRanking.Contains(pl))
            {
                GameManager.instance.m_BallManager.m_playerRanking.Add(pl);
                GameManager.instance.m_BallManager.m_playersPlaying.Remove(pl);

                Instantiate(Particles_Manager.instance.m_ExplosionParticle, pl.transform.position, Quaternion.identity);

                collision.gameObject.GetComponentInParent<Player_OldSystem>().minigame_Playing_FallGame = false;
                collision.transform.parent.gameObject.SetActive(false);
            }
        }

    }
}
