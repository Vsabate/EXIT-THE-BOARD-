using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region Singleton
    public static CameraManager instance;

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

    [Header("WORLD CAM")]
    public CinemachineVirtualCamera m_WorldCam;

    [Space]
    [Header("PLAYER CAMS")]
    public GameObject PlayerCamera;

    [Space]
    [Header("DICE CAM")]
    public Camera m_DiceCamera;
    bool m_SmallCam;

    [Space]
    [Header("MINIGAMES CAMs")]
    public CinemachineVirtualCamera m_BrujaCamera;
    public CinemachineVirtualCamera m_BaldosasCamera1;
    public CinemachineVirtualCamera m_BaldosasCamera2;
    public CinemachineVirtualCamera m_BaldosasCamera3;
    public CinemachineVirtualCamera m_BallCamera;

    private void Start()
    {
        m_WorldCam.Priority = 100;
    }

    private void Update()
    {
        // QUE LA CÁMARA DE MUNDO MIRE EL TABLERO
        if (GameManager.instance.GetCurrentScene() == GameManager.Scene.BoardScene && GameManager.instance.m_BoardManager != null)
        {
            if (m_WorldCam.LookAt == null)
            {
                m_WorldCam.LookAt = GameObject.FindGameObjectWithTag("Board").transform;
            }

            // DICE CÁMARA
            #region Dice Cam
            if (GameManager.instance.m_BoardManager.GetDiceManager() != null)
            {
                if (GameManager.instance.m_BoardManager.GetDiceManager().DiceThrown())
                {
                    m_DiceCamera.GetComponent<CinemachineVirtualCamera>().Follow = GameManager.instance.m_BoardManager.m_CurrentDice.transform;
                    m_DiceCamera.GetComponent<CinemachineVirtualCamera>().LookAt = GameManager.instance.m_BoardManager.m_CurrentDice.transform;
                    if (!m_SmallCam)
                        UI_Manager.instance.m_DiceUI.SetActive(true);

                    if (GameManager.instance.m_BoardManager.GetDiceManager().m_HasLanded && !m_SmallCam)
                    {
                        if (GameManager.instance.m_BoardManager.GetCurrentState() != "PLAYER_ORDER")
                        {
                            UI_Manager.instance.m_DiceUI.SetActive(false);
                            UI_Manager.instance.m_DiceUI2.SetActive(true);
                        }
                        m_SmallCam = true;
                    }
                }
                else if (m_SmallCam)
                {
                    if (GameManager.instance.m_BoardManager.GetCurrentState() != "PLAYER_ORDER")
                    {
                        UI_Manager.instance.m_DiceUI2.SetActive(false);
                    }
                    m_SmallCam = false;
                    UI_Manager.instance.m_DiceUI.SetActive(false);
                }
            }
            m_DiceCamera.transform.position = new Vector3(
    GameManager.instance.m_BoardManager.m_CurrentDice.transform.position.x,
    GameManager.instance.m_BoardManager.m_CurrentDice.transform.position.y + 1,
    GameManager.instance.m_BoardManager.m_CurrentDice.transform.position.z);
            #endregion
        }

    }
}
