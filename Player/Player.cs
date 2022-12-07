using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]

public class Player : MonoBehaviour
{
    public string m_PlayerName = ""; // ONLY FOR DEBUG
    public GameObject m_Avatar;
    public PlayerMovement m_PlayerMovement;
    public Sprite m_PlayerIcon;
    public GameObject m_PlayerCard;
    public GameObject m_PlayerCamera;
    public GameObject m_Ball;

    [Space]
    [Header("PushForce")]
    float m_PushForce = 20f;
    float m_PlayerMass = 3.0f;
    float m_BreakForce = 5.0f;
    Vector3 m_ImpactDir = Vector3.zero;
    CharacterController m_Character;

    [Space]
    [Header("Dice")]
    public int m_CurrentDice = 6;

    private void Start()
    {
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_Character = gameObject.GetComponent<CharacterController>();
        m_Ball.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BoardScene" &&
            GameManager.instance.m_BoardManager.GetCurrentState() == "MINIGAME")
        {
            if (m_ImpactDir.magnitude > 0.2f)
            {
                m_Character.Move(m_ImpactDir * Time.deltaTime);
            }

            m_ImpactDir = Vector3.Lerp(m_ImpactDir, Vector3.zero, m_BreakForce * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (GameManager.instance.m_BoardManager.GetCurrentState() == "MINIGAME" && GameManager.instance.m_BoardManager.m_RandomMinigame == 2)
        {
            switch (gameObject.tag)
            {
                case "PlayerRed":
                    if (hit.transform.parent.tag.Equals("PlayerBlue") ||
                        hit.transform.parent.tag.Equals("PlayerGreen") ||
                        hit.transform.parent.tag.Equals("PlayerOrange"))
                    {
                        Vector3 dir = hit.transform.parent.position - transform.position;
                        dir = -dir.normalized;
                        AddImpact(dir, m_PushForce, hit.transform.parent.gameObject);
                    }
                    break;

                case "PlayerBlue":
                    if (hit.transform.parent.tag.Equals("PlayerRed") ||
                        hit.transform.parent.tag.Equals("PlayerGreen") ||
                        hit.transform.parent.tag.Equals("PlayerOrange"))
                    {

                        Vector3 dir = hit.transform.parent.position - transform.position;
                        dir = -dir.normalized;
                        AddImpact(dir, m_PushForce, hit.transform.parent.gameObject);
                    }
                    break;

                case "PlayerGreen":
                    if (hit.transform.parent.tag.Equals("PlayerBlue") ||
                        hit.transform.parent.tag.Equals("PlayerRed") ||
                        hit.transform.parent.tag.Equals("PlayerOrange"))
                    {
                        Vector3 dir = hit.transform.parent.position - transform.position;
                        dir = -dir.normalized;
                        AddImpact(dir, m_PushForce, hit.transform.parent.gameObject);
                    }
                    break;

                case "PlayerOrange":
                    if (hit.transform.parent.tag.Equals("PlayerBlue") ||
                        hit.transform.parent.tag.Equals("PlayerGreen") ||
                        hit.transform.parent.tag.Equals("PlayerRed"))
                    {
                        Vector3 dir = hit.transform.parent.position - transform.position;
                        dir = -dir.normalized;
                        AddImpact(dir, m_PushForce, hit.transform.parent.gameObject);
                    }
                    break;
            }
        }
    }

    void AddImpact(Vector3 dir, float force, GameObject OtherPlayer)
    {
        dir.Normalize();
        dir = new Vector3(dir.x, 0f, dir.z);
        m_ImpactDir += dir.normalized * force / m_PlayerMass;
        OtherPlayer.GetComponent<Player>().m_ImpactDir = -(this.m_ImpactDir);
        SoundManager.instance.Sound_Boing();
    }

}
