using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ActionEnums;
using Cinemachine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    #region STATES
    public enum State { INITIAL, PLAYER_ORDER, PLAYER1_TURN, PLAYER2_TURN, PLAYER3_TURN, PLAYER4_TURN, MINIGAME, GAMEOVER };
    public State currentState = State.INITIAL;
    #endregion

    #region MANAGERS
    RouteManager m_CurrentRoute;
    public DiceManager m_Dice6;
    public DiceManager m_Dice8;
    public DiceManager m_Dice4;
    public DiceManager m_CurrentDice;
    PlayerManager m_PlayerManager;
    PlayerMovement m_RandomPlayerPos;
    #endregion

    #region BOOLS
    bool m_DiceDone;
    bool m_CheckingEvent;
    bool m_DiceIsRolling;
    bool m_TurnOver;
    bool m_PrimerTurno;
    [HideInInspector] public bool m_SwapMenu;
    public bool boolean;
    [HideInInspector] public bool canSkipInstructions;
    #endregion

    #region LISTS
    public List<Player> m_PlayerOrderList = new List<Player>();
    List<PlayerMovement> m_PlayersLocation = new List<PlayerMovement>();
    public List<GameObject> m_MinigamesList = new List<GameObject>();
    public List<Player> m_PlayersRanking = new List<Player>();
    string[] m_NodeTagList = {
        "CASILLA - AVANZA", "CASILLA - RETROCEDE", "CASILLA - REROLL",
        "CASILLA - SWITCH", "CASILLA - RANDOMSWITCH"
    };
    #endregion

    int m_ActualSwitchPos_1;
    int m_ActualSwitchPos_2;
    [HideInInspector] public int m_RandomMinigame;
    [HideInInspector] public int m_CurrentTurn;
    int m_TurnoPartida = 0;
    float m_IdleTime = 6f;
    float m_Timer = 0f;

    private void Start()
    {
        UI_Manager.instance.panel_Board.SetActive(true);
        m_PlayerManager = GameManager.instance.m_PlayerManager;
        m_CurrentRoute = FindObjectOfType<RouteManager>();
        m_CurrentDice = m_Dice6;
        m_Dice8.gameObject.SetActive(false);
        m_Dice4.gameObject.SetActive(false);
        m_PrimerTurno = true;
        canSkipInstructions = false;
    }
    private void Update()
    {
        switch (currentState)
        {
            #region INITIAL
            case State.INITIAL:
                if (m_PrimerTurno)
                {
                    FadeController.instance.Disable_TextLoading();
                }

                if (m_Timer >= m_IdleTime)
                {
                    ChangeState(State.PLAYER_ORDER);
                }
                else
                {
                    m_Timer += Time.deltaTime;
                    for (int i = 0; i < m_PlayerManager.GetNumberOfPlayers(); i++)
                    {
                        m_PlayerManager.m_PlayerList[i].transform.position = m_CurrentRoute.GetNodeList()[0].position;
                    }
                }

                break;
            #endregion

            #region PLAYER ORDER
            case State.PLAYER_ORDER:
                if (!m_DiceIsRolling)
                {
                    if (m_PlayerOrderList.Count < m_PlayerManager.GetNumberOfPlayers())
                    {
                        StartCoroutine(DiceOrder());
                    }
                    else
                    {
                        StartCoroutine(BeginningFirstTurn());
                    }
                }
                break;
            #endregion

            #region PLAYER TURNS
            case State.PLAYER1_TURN:

                if (!m_TurnOver)
                {
                    // LANZAR DADO
                    if (m_PlayerOrderList[0].gameObject.GetComponent<Player_OldSystem>().canThrowDice &&
                    !m_PlayerOrderList[0].m_PlayerMovement.GetIsMoving() &&
                    m_CurrentDice.IsReadyToThrow() && !m_DiceIsRolling)
                    {
                        SetCameraActive(m_PlayerOrderList[0].m_PlayerCamera.GetComponent<CinemachineVirtualCamera>());
                        StartCoroutine(RollDiceAndMove(m_PlayerOrderList[0].m_PlayerMovement));

                        // SET "SHOW THROW DICE" HUD TO FALSE
                        if (UI_Manager.instance.panel_ExtraDice.activeSelf) UI_Manager.instance.panel_ExtraDice.SetActive(false);
                    }

                    // MIRAR QUE HACE LA CASILLA
                    if (!m_PlayerOrderList[0].m_PlayerMovement.GetIsMoving() && m_DiceDone && !m_CheckingEvent &&
                        m_PlayerOrderList[0].m_PlayerMovement.m_DiceNumber == 0)
                    {
                        StartCoroutine(CheckNodeEvent(m_PlayerOrderList[0].m_PlayerMovement));
                    }

                    UpdatePlayerCardPos(m_PlayerOrderList[0]);
                }
                else
                {
                    ChangeState(State.PLAYER2_TURN);
                }
                break;

            case State.PLAYER2_TURN:

                if (!m_TurnOver)
                {
                    // LANZAR DADO
                    if (m_PlayerOrderList[1].gameObject.GetComponent<Player_OldSystem>().canThrowDice &&
                    !m_PlayerOrderList[1].m_PlayerMovement.GetIsMoving() &&
                    m_CurrentDice.IsReadyToThrow() && !m_DiceIsRolling)
                    {
                        SetCameraActive(m_PlayerOrderList[1].m_PlayerCamera.GetComponent<CinemachineVirtualCamera>());
                        StartCoroutine(RollDiceAndMove(m_PlayerOrderList[1].m_PlayerMovement));

                        // SET "SHOW THROW DICE" HUD TO FALSE
                        if (UI_Manager.instance.panel_ExtraDice.activeSelf) UI_Manager.instance.panel_ExtraDice.SetActive(false);
                    }

                    // MIRAR QUE HACE LA CASILLA
                    if (!m_PlayerOrderList[1].m_PlayerMovement.GetIsMoving() && m_DiceDone && !m_CheckingEvent &&
                        m_PlayerOrderList[1].m_PlayerMovement.m_DiceNumber == 0)
                    {
                        StartCoroutine(CheckNodeEvent(m_PlayerOrderList[1].m_PlayerMovement));
                    }

                    UpdatePlayerCardPos(m_PlayerOrderList[1]);
                }
                else
                {
                    if (m_PlayerOrderList.Count >= 3)
                    {
                        ChangeState(State.PLAYER3_TURN);
                    }
                    else
                    {
                        ChangeState(State.MINIGAME);
                    }
                }

                break;

            case State.PLAYER3_TURN:

                if (!m_TurnOver)
                {
                    // LANZAR DADO
                    if (m_PlayerOrderList[2].gameObject.GetComponent<Player_OldSystem>().canThrowDice &&
                        !m_PlayerOrderList[2].m_PlayerMovement.GetIsMoving() &&
                        m_CurrentDice.IsReadyToThrow() && !m_DiceIsRolling)
                    {
                        SetCameraActive(m_PlayerOrderList[2].m_PlayerCamera.GetComponent<CinemachineVirtualCamera>());
                        StartCoroutine(RollDiceAndMove(m_PlayerOrderList[2].m_PlayerMovement));

                        // SET "SHOW THROW DICE" HUD TO FALSE
                        if (UI_Manager.instance.panel_ExtraDice.activeSelf) UI_Manager.instance.panel_ExtraDice.SetActive(false);
                    }

                    // MIRAR QUE HACE LA CASILLA
                    if (!m_PlayerOrderList[2].m_PlayerMovement.GetIsMoving() && m_DiceDone && !m_CheckingEvent &&
                        m_PlayerOrderList[2].m_PlayerMovement.m_DiceNumber == 0)
                    {
                        StartCoroutine(CheckNodeEvent(m_PlayerOrderList[2].m_PlayerMovement));
                    }

                    UpdatePlayerCardPos(m_PlayerOrderList[2]);
                }
                else
                {
                    if (m_PlayerOrderList.Count == 4)
                    {
                        ChangeState(State.PLAYER4_TURN);
                    }
                    else
                    {
                        ChangeState(State.MINIGAME);
                    }
                }

                break;

            case State.PLAYER4_TURN:

                if (!m_TurnOver)
                {
                    // LANZAR DADO
                    if (m_PlayerOrderList[3].gameObject.GetComponent<Player_OldSystem>().canThrowDice &&
                        !m_PlayerOrderList[3].m_PlayerMovement.GetIsMoving() &&
                        m_CurrentDice.IsReadyToThrow() && !m_DiceIsRolling)
                    {
                        SetCameraActive(m_PlayerOrderList[3].m_PlayerCamera.GetComponent<CinemachineVirtualCamera>());
                        StartCoroutine(RollDiceAndMove(m_PlayerOrderList[3].m_PlayerMovement));

                        // SET "SHOW THROW DICE" HUD TO FALSE
                        if (UI_Manager.instance.panel_ExtraDice.activeSelf) UI_Manager.instance.panel_ExtraDice.SetActive(false);
                    }

                    // MIRAR QUE HACE LA CASILLA
                    if (!m_PlayerOrderList[3].m_PlayerMovement.GetIsMoving() && m_DiceDone && !m_CheckingEvent &&
                        m_PlayerOrderList[3].m_PlayerMovement.m_DiceNumber == 0)
                    {
                        StartCoroutine(CheckNodeEvent(m_PlayerOrderList[3].m_PlayerMovement));
                    }

                    UpdatePlayerCardPos(m_PlayerOrderList[3]);
                }
                else
                {
                    ChangeState(State.MINIGAME);
                }

                break;
            #endregion

            #region MINIGAME
            case State.MINIGAME:

                if (canSkipInstructions && Input.GetAxisRaw(EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton) > 0)
                {
                    canSkipInstructions = false;
                    // skip minigame instructions
                    UI_Manager.instance.SetCard_2();
                    if (m_RandomMinigame == 0)
                    {
                        m_MinigamesList[0].GetComponent<GridManager>().m_Timer_RankOn = true;
                    }
                    else if (m_RandomMinigame == 1)
                    {
                        m_MinigamesList[1].GetComponent<BrujaManager>().m_Timer_RankOn = true;
                    }
                    else if (m_RandomMinigame == 2)
                    {
                        m_MinigamesList[2].GetComponent<BallGame>().m_Timer_RankOn = true;
                    }
                    if (!UI_Manager.instance.canDiscount)
                    {
                        UI_Manager.instance.canDiscount = true;
                    }
                }

                break;
                #endregion
        }
    }

    public void ChangeState(State newState)
    {
        // EXIT STATE
        switch (currentState)
        {
            case State.INITIAL:
                m_TurnoPartida++;
                m_Timer = 0f;
                break;

            case State.PLAYER_ORDER:
                GameObject PlayerCardParent = GameObject.Find("Panel_CardPlayer");
                foreach (Player player in m_PlayerOrderList)
                {
                    GameObject l_PCard = Instantiate(UI_Manager.instance.m_PlayerCard);
                    l_PCard.transform.SetParent(PlayerCardParent.transform);
                    l_PCard.GetComponent<PlayerCard>().m_PlayerName.text = player.m_PlayerName;
                    l_PCard.GetComponent<PlayerCard>().m_NumCasilla.text = player.m_PlayerMovement.GetRoutePos().ToString();
                    l_PCard.GetComponent<PlayerCard>().m_PlayerIcon.sprite = player.m_PlayerIcon;

                    if (player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin1") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago1") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier1") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco1"))
                    {
                        l_PCard.GetComponent<PlayerCard>().m_Background.sprite =
                        UI_Manager.instance.m_HorizontalCards[0];
                    }
                    else if (player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin2") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago2") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier2") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco2"))
                    {
                        l_PCard.GetComponent<PlayerCard>().m_Background.sprite =
                        UI_Manager.instance.m_HorizontalCards[1];
                    }
                    else if (player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin3") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago3") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier3") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco3"))
                    {
                        l_PCard.GetComponent<PlayerCard>().m_Background.sprite =
                        UI_Manager.instance.m_HorizontalCards[2];
                    }
                    else if (player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin4") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago4") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier4") ||
                        player.gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco4"))
                    {
                        l_PCard.GetComponent<PlayerCard>().m_Background.sprite =
                        UI_Manager.instance.m_HorizontalCards[3];
                    }

                    l_PCard.GetComponent<PlayerCard>().m_CrownImg.color = Color.clear;
                    player.m_PlayerCard = l_PCard;
                    UI_Manager.instance.m_CasillaText.gameObject.SetActive(true);
                    UI_Manager.instance.m_CasillaText.text = "";
                    player.m_PlayerCamera = GameObject.Instantiate(CameraManager.instance.PlayerCamera);
                    player.m_PlayerCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.m_Avatar.transform;
                    player.m_PlayerCamera.GetComponent<CinemachineVirtualCamera>().LookAt = player.m_Avatar.transform;
                }

                for (int i = 0; i < m_PlayerOrderList.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos1;

                            break;

                        case 1:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos2;
                            break;

                        case 2:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos3;
                            break;

                        case 3:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos4;
                            break;
                    }
                }

                break;

            case State.PLAYER1_TURN:

                m_PlayerOrderList[0].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = false;
                m_PlayerOrderList[0].gameObject.GetComponent<Player_OldSystem>().canThrowDice = false;
                m_PlayerOrderList[0].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[0].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos1;

                CheckPlayerCrown(m_PlayerOrderList[0]);
                m_Timer = 0f;
                break;

            case State.PLAYER2_TURN:

                m_PlayerOrderList[1].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = false;
                m_PlayerOrderList[1].gameObject.GetComponent<Player_OldSystem>().canThrowDice = false;
                m_PlayerOrderList[1].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[1].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos2;

                CheckPlayerCrown(m_PlayerOrderList[1]);
                m_Timer = 0f;
                break;

            case State.PLAYER3_TURN:

                m_PlayerOrderList[2].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = false;
                m_PlayerOrderList[2].gameObject.GetComponent<Player_OldSystem>().canThrowDice = false;
                m_PlayerOrderList[2].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[2].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos3;

                CheckPlayerCrown(m_PlayerOrderList[2]);
                m_Timer = 0f;
                break;

            case State.PLAYER4_TURN:

                m_PlayerOrderList[3].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = false;
                m_PlayerOrderList[3].gameObject.GetComponent<Player_OldSystem>().canThrowDice = false;
                m_PlayerOrderList[3].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[3].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos4;

                CheckPlayerCrown(m_PlayerOrderList[3]);
                m_Timer = 0f;
                break;

            case State.MINIGAME:
                SoundManager.instance.s_CanPlayDrums = false;
                // SET THE MINIGAME OBJECT TO FALSE
                m_MinigamesList[m_RandomMinigame].SetActive(false);
                // SET ALL PLAYER OBJECTS TO TRUE
                foreach (Player p in m_PlayerOrderList)
                {
                    p.gameObject.SetActive(true);
                }

                // ENABLE BOARD UI
                UI_Manager.instance.panel_Board.SetActive(true);

                m_TurnoPartida++;
                m_Timer = 0f;

                for (int i = 0; i < m_PlayerOrderList.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos1;

                            break;

                        case 1:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos2;
                            break;

                        case 2:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos3;
                            break;

                        case 3:
                            m_PlayerOrderList[i].gameObject.transform.position +=
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()
                    [m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos()].GetComponent<Casilla_Manager>().m_pos4;
                            break;
                    }
                }

                break;

        }

        // ENTER STATE
        switch (newState)
        {
            case State.INITIAL:
                m_PlayersRanking.Clear();
                SetCameraActive(CameraManager.instance.m_WorldCam);
                break;

            case State.PLAYER_ORDER:
                UI_Manager.instance.m_FSM_panel.SetActive(true);
                if (m_PlayerOrderList.Count < m_PlayerManager.GetNumberOfPlayers())
                {
                    for (int i = 0; i < m_PlayerManager.GetNumberOfPlayers(); i++)
                    {
                        m_PlayerManager.m_PlayerList[i].transform.position = m_CurrentRoute.GetNodeList()[0].position;
                    }
                }
                UI_Manager.instance.m_FSMStateText.gameObject.SetActive(true);
                UpdateTextUI("PLAYER ORDER", 0f, 0f, 0f);
                break;

            case State.PLAYER1_TURN:
                UI_Manager.instance.m_CasillaText.text = "";
                if (!UI_Manager.instance.m_CasillaBackground.activeSelf)
                {
                    UI_Manager.instance.m_CasillaBackground.SetActive(true);
                }
                // RESUME BOARD MUSIC IF PAUSED
                bool aux_BoardMusic;
                SoundManager.instance.Music_Board.getPaused(out aux_BoardMusic);
                if (aux_BoardMusic)
                {
                    SoundManager.instance.Music_Board.setPaused(false);
                }

                m_CurrentTurn = 0;
                m_PlayerOrderList[0].GetComponent<PlayerMovement>().SetIsMoving(false);
                m_PlayerOrderList[0].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = true;
                m_PlayerOrderList[0].gameObject.transform.position =
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[0].m_PlayerMovement.GetRoutePos()].position;

                CheckTextColor(m_PlayerOrderList[0]);
                CheckCurrentDie(m_PlayerOrderList[0]);
                ResetVariablesTurn();
                SetCameraActive(m_PlayerOrderList[0].GetComponent<PlayerCameraIdle>().m_PlayerCamFront);

                CheckDiceUI_P1();
                break;

            case State.PLAYER2_TURN:
                UI_Manager.instance.m_CasillaText.text = "";
                m_CurrentTurn = 1;
                m_PlayerOrderList[1].GetComponent<PlayerMovement>().SetIsMoving(false);
                m_PlayerOrderList[1].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = true;
                m_PlayerOrderList[1].gameObject.transform.position =
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[1].m_PlayerMovement.GetRoutePos()].position;

                CheckCurrentDie(m_PlayerOrderList[1]);
                CheckTextColor(m_PlayerOrderList[1]);
                ResetVariablesTurn();
                SetCameraActive(m_PlayerOrderList[1].GetComponent<PlayerCameraIdle>().m_PlayerCamFront);

                CheckDiceUI_P2();
                break;

            case State.PLAYER3_TURN:
                UI_Manager.instance.m_CasillaText.text = "";
                m_CurrentTurn = 2;
                m_PlayerOrderList[2].GetComponent<PlayerMovement>().SetIsMoving(false);
                m_PlayerOrderList[2].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = true;
                m_PlayerOrderList[2].gameObject.transform.position =
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[2].m_PlayerMovement.GetRoutePos()].position;

                CheckTextColor(m_PlayerOrderList[2]);
                CheckCurrentDie(m_PlayerOrderList[2]);
                ResetVariablesTurn();
                SetCameraActive(m_PlayerOrderList[2].GetComponent<PlayerCameraIdle>().m_PlayerCamFront);

                CheckDiceUI_P3();
                break;

            case State.PLAYER4_TURN:
                UI_Manager.instance.m_CasillaText.text = "";
                m_CurrentTurn = 3;
                m_PlayerOrderList[3].GetComponent<PlayerMovement>().SetIsMoving(false);
                m_PlayerOrderList[3].gameObject.GetComponent<Player_OldSystem>().myDiceTurn = true;
                m_PlayerOrderList[3].gameObject.transform.position =
                    GameManager.instance.m_BoardManager.m_CurrentRoute.GetNodeList()[m_PlayerOrderList[3].m_PlayerMovement.GetRoutePos()].position;

                CheckCurrentDie(m_PlayerOrderList[3]);
                CheckTextColor(m_PlayerOrderList[3]);
                ResetVariablesTurn();
                SetCameraActive(m_PlayerOrderList[3].GetComponent<PlayerCameraIdle>().m_PlayerCamFront);

                CheckDiceUI_P4();
                break;

            case State.MINIGAME:
                SetCameraActive(CameraManager.instance.m_WorldCam);
                UI_Manager.instance.m_DiceUI2.SetActive(false);
                StartCoroutine(BeginningMinigame());
                break;

            case State.GAMEOVER:
                foreach (Transform child in GameObject.Find("Panel_CardPlayer").transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                foreach (Player player in m_PlayerOrderList)
                {
                    player.GetComponent<PlayerMovement>().SetRoutePos(0);
                    player.GetComponent<PlayerMovement>().SetDiceNumber(0);

                }
                StartCoroutine(Victory());
                break;
        }

        currentState = newState;
    }

    PlayerMovement RandomPlayerLocation(PlayerMovement PlayerMov)
    {
        m_PlayersLocation.Clear();
        for (int i = 0; i < m_PlayerManager.GetNumberOfPlayers(); i++)
        {
            if (!PlayerMov.gameObject.CompareTag(m_PlayerOrderList[i].tag))
            {
                m_PlayersLocation.Add(m_PlayerOrderList[i].GetComponent<PlayerMovement>());
            }
        }
        return m_PlayersLocation[Random.Range(0, m_PlayersLocation.Count)];
    }

    void ResetVariablesTurn()
    {
        m_TurnOver = false;
        m_CurrentDice.ResetDice();
        m_DiceDone = false;
    }

    void CheckCurrentDie(Player m_Player)
    {
        m_Dice6.gameObject.SetActive(true);
        m_Dice8.gameObject.SetActive(true);
        m_Dice4.gameObject.SetActive(true);

        switch (m_Player.m_CurrentDice)
        {
            case 6:
                m_CurrentDice = m_Dice6;
                m_Dice8.gameObject.SetActive(false);
                m_Dice4.gameObject.SetActive(false);
                break;

            case 8:
                m_CurrentDice = m_Dice8;
                m_Dice6.gameObject.SetActive(false);
                m_Dice4.gameObject.SetActive(false);
                break;

            case 4:
                m_CurrentDice = m_Dice4;
                m_Dice8.gameObject.SetActive(false);
                m_Dice6.gameObject.SetActive(false);
                break;
        }
    }

    #region COROUTINES
    IEnumerator Victory()
    {
        // BOARD THEME STOPS
        SoundManager.instance.Music_Board.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        UI_Manager.instance.panel_Board.SetActive(false);

        yield return new WaitForSeconds(1f);

        SetCameraActive(CameraManager.instance.m_WorldCam);
        CameraManager.instance.m_WorldCam.GetComponent<CinemachineDollyCart>().m_Position = 0;

        yield return new WaitForSeconds(1f);

        SoundManager.instance.Sound_EndGame();
        UI_Manager.instance.panel_VictoryScreen.SetActive(true);
        UI_Manager.instance.text_Congrats.transform.localScale = new Vector2(0, 0);
        UI_Manager.instance.image_Congrats.gameObject.SetActive(true);

        yield return StartCoroutine(FadeController.instance.Fade_In_FromTransparent
            (UI_Manager.instance.image_Congrats.GetComponent<Image>(), 1f));

        UI_Manager.instance.text_Congrats.transform.localScale = new Vector2(1, 1);

        yield return new WaitForSeconds(3f);

        UI_Manager.instance.panel_BoardToMenu.SetActive(true);
        UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();

        yield return null;
    }
    IEnumerator DiceOrder()
    {
        m_DiceIsRolling = true;
        int l_DiceNumber;
        bool l_DiceRepeated = false;
        List<int> l_DicePlayer = new List<int>();
        int l_Player = 0;

        // Se tira el dado para decidir el ordern de los jugadores
        while (l_DicePlayer.Count < m_PlayerManager.GetNumberOfPlayers())
        {
            switch (l_Player)
            {
                case 0:
                    UpdateTextUI("ORDER:  PLAYER  1", 97f / 255f, 65f / 255f, 137f / 255f); // purple
                    UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
                    break;
                case 1:
                    UpdateTextUI("ORDER:  PLAYER  2", 0f, 170f / 255f, 233f / 255f); // blue
                    UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
                    break;
                case 2:
                    UpdateTextUI("ORDER:  PLAYER  3", 50f / 255f, 135f / 255f, 60f / 255f); // green
                    UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
                    break;
                case 3:
                    UpdateTextUI("ORDER:  PLAYER  4", 232f / 255f, 221f / 255f, 70f / 255f); // yellow
                    UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
                    break;
            }

            m_CurrentDice.ResetDice();
            m_CurrentDice.RollDice();
            while (m_CurrentDice.IsReadyToThrow()) { yield return null; }
            yield return new WaitForSeconds(1f);
            l_DiceNumber = m_CurrentDice.GetDiceNumber();

            if (l_DicePlayer.Count > 0)
            {
                // Se comprueba si hay algun dado repetido, si es así se vuelve a lanzar
                for (int j = 0; j < l_DicePlayer.Count; j++)
                {
                    if (l_DiceNumber == l_DicePlayer[j])
                    {
                        l_DiceRepeated = true; ;
                    }
                }
            }

            if (l_DiceRepeated)
            {
                l_DiceRepeated = false;
                continue;
            }
            else
            {
                l_DicePlayer.Add(l_DiceNumber);
                l_Player++;
            }
        }

        var l_DicePlayerSorted = new List<int>(l_DicePlayer);
        Player[] pList = new Player[m_PlayerManager.GetNumberOfPlayers()];
        l_DicePlayerSorted.Sort();

        //Se comparan los resultados con el orden de tirada
        for (int z = 0; z < l_DicePlayer.Count; z++)
        {
            for (int x = 0; x < l_DicePlayerSorted.Count; x++)
            {
                if (l_DicePlayer[z] == l_DicePlayerSorted[x])
                {
                    switch (z)
                    {
                        case 0:
                            pList[x] = GameObject.FindGameObjectWithTag("PlayerRed").GetComponent<Player>();
                            break;

                        case 1:
                            pList[x] = GameObject.FindGameObjectWithTag("PlayerBlue").GetComponent<Player>();
                            break;

                        case 2:
                            pList[x] = GameObject.FindGameObjectWithTag("PlayerGreen").GetComponent<Player>();
                            break;

                        case 3:
                            pList[x] = GameObject.FindGameObjectWithTag("PlayerOrange").GetComponent<Player>();
                            break;
                    }
                }
            }
        }

        // Se añaden a la lista de jugadores ordenados
        for (int q = pList.Length - 1; q >= 0; q--)
        {
            m_PlayerOrderList.Add(pList[q]);
        }

        m_CurrentDice.ResetDice();
        m_DiceIsRolling = false;
    }

    IEnumerator RollDiceAndMove(PlayerMovement m_Player)
    {
        m_DiceIsRolling = true;

        m_CurrentDice.ResetDice();
        m_CurrentDice.RollDice();

        while (m_CurrentDice.IsReadyToThrow()) { yield return null; }
        yield return new WaitForSeconds(1f);

        m_Player.SetDiceNumber(m_CurrentDice.GetDiceNumber() + m_Player.GetExtraDiceNumber());

        // CONTROL! Per si la tirada es más grande que el nº de Casillas restantes.
        if (m_Player.GetRoutePos() + m_Player.GetDiceNumber() < m_Player.GetRouteManager().GetNodeList().Count)
        {
            StartCoroutine(m_Player.BoardMove());
            if (m_Player.GetExtraDiceNumber() != 0)
                m_Player.SetExtraDiceNumber(0);
        }
        else
        {
            m_Player.SetDiceNumber(m_Player.GetRouteManager().GetNodeList().Count - m_Player.GetRoutePos() - 1);
            StartCoroutine(m_Player.BoardMove());
        }

        UI_Manager.instance.m_DadoText.text = m_CurrentDice.GetDiceNumber().ToString();
        m_Player.GetComponent<Player>().m_CurrentDice = 6;

        m_DiceDone = true;
        m_DiceIsRolling = false;
    }

    IEnumerator DiceParesNones(PlayerMovement m_Player)
    {
        m_DiceIsRolling = true;
        m_Player.GetComponent<Player>().m_CurrentDice = 6;
        m_CurrentDice.ResetDice();
        m_CurrentDice.RollDice();

        while (m_CurrentDice.IsReadyToThrow()) { yield return null; }
        yield return new WaitForSeconds(1f);

        if (m_CurrentDice.GetDiceNumber() % 2 == 0)
        {
            m_Player.SetExtraDiceNumber(2);
        }
        else
        {
            m_Player.SetExtraDiceNumber(-2);
        }

        #region UI ExtraDice
        if (m_Player.GetExtraDiceNumber() > 0)
        {
            UI_Manager.instance.m_ExtraDiceText.text = "Die number +" + m_Player.GetExtraDiceNumber().ToString() + " next turn!";
        }
        else
        {
            UI_Manager.instance.m_ExtraDiceText.text = "Die number " + m_Player.GetExtraDiceNumber().ToString() + " next turn!";
        }

        UI_Manager.instance.panel_ExtraDice.SetActive(true);
        yield return new WaitForSeconds(1f);
        UI_Manager.instance.panel_ExtraDice.SetActive(false);
        #endregion

        m_DiceDone = true;
        m_DiceIsRolling = false;
    }

    IEnumerator CheckNodeEvent(PlayerMovement m_Player, string NodeTag = "")
    {
        m_CheckingEvent = true;
        #region NodeTag
        string l_NodeTag;
        if (NodeTag != "")
        {
            l_NodeTag = NodeTag;
        }
        else
        {
            l_NodeTag = m_Player.GetRouteManager().GetNodeList()[m_Player.GetRoutePos()].tag;
        }
        #endregion

        yield return new WaitForSeconds(1f);

        switch (l_NodeTag)
        {
            case "CASILLA - NULL":
                m_TurnOver = true;
                break;

            case "CASILLA - AVANZA":
                SoundManager.instance.Sound_PositiveEffect();
                UI_Manager.instance.m_CasillaText.text = "Move Forward";
                yield return new WaitForSeconds(1f);
                if (m_CurrentRoute.GetNodeList()[m_Player.GetRoutePos()].GetComponent<Casilla_Avanzar>() != null)
                {
                    m_Player.SetDiceNumber(m_CurrentRoute.GetNodeList()[m_Player.GetRoutePos()].GetComponent<Casilla_Avanzar>().m_NumeroCasillasAvanzar);
                    StartCoroutine(m_Player.BoardMove());
                    while (m_Player.GetIsMoving()) { yield return null; }

                }
                else
                {
                    Debug.LogError("FALTA EL COMPONENTE CASILLA_AVANZAR!");
                }

                break;

            case "CASILLA - RETROCEDE":
                SoundManager.instance.Sound_NegativeEffect();
                UI_Manager.instance.m_CasillaText.text = "Move Backwards";
                yield return new WaitForSeconds(1f);
                if (m_CurrentRoute.GetNodeList()[m_Player.GetRoutePos()].GetComponent<Casilla_Retroceder>() != null)
                {
                    m_Player.SetDiceNumber(-(m_CurrentRoute.GetNodeList()[m_Player.GetRoutePos()].GetComponent<Casilla_Retroceder>().m_NumeroCasillasRetroceder));
                    StartCoroutine(m_Player.BoardMove());
                    while (m_Player.GetIsMoving()) { yield return null; }
                }
                else
                {
                    Debug.LogError("FALTA EL COMPONENTE CASILLA_RETROCEDER!");
                }
                break;

            case "CASILLA - REROLL":
                SoundManager.instance.Sound_PositiveEffect();
                UI_Manager.instance.m_CasillaText.text = "Reroll Square";
                yield return new WaitForSeconds(1f);
                ChangeState(currentState);
                break;

            case "CASILLA - DEATH":
                SoundManager.instance.Sound_NegativeEffect();
                UI_Manager.instance.m_CasillaText.text = "Death Square";
                yield return new WaitForSeconds(1f);
                StartCoroutine(m_Player.ChangePlayerPosition(0));
                while (m_Player.GetIsMoving()) { yield return null; }
                yield return new WaitForSeconds(0.1f);
                if (!m_Player.GetIsMoving())
                    m_TurnOver = true;
                break;

            case "CASILLA - SWITCH":
                UI_Manager.instance.m_CasillaText.text = "Swap Square";
                if (!m_SwapMenu) // esta variable a lo mejor borrarla
                {
                    UI_Manager.instance.OpenSwapMenu();
                    m_SwapMenu = true;
                }
                yield return new WaitUntil(() => m_SwapMenu == false);

                m_ActualSwitchPos_1 = UI_Manager.instance.m_PlayersToBeSwapped[0].GetComponent<PlayerMovement>().GetRoutePos();
                m_ActualSwitchPos_2 = UI_Manager.instance.m_PlayersToBeSwapped[1].GetComponent<PlayerMovement>().GetRoutePos();

                yield return StartCoroutine(UI_Manager.instance.m_PlayersToBeSwapped[0].GetComponent<PlayerMovement>().ChangePlayerPosition(m_ActualSwitchPos_2));
                yield return StartCoroutine(UI_Manager.instance.m_PlayersToBeSwapped[1].GetComponent<PlayerMovement>().ChangePlayerPosition(m_ActualSwitchPos_1));
                yield return new WaitForSeconds(1f);

                while (UI_Manager.instance.m_PlayersToBeSwapped[0].GetComponent<PlayerMovement>().GetIsMoving()) { yield return null; }
                if (!UI_Manager.instance.m_PlayersToBeSwapped[0].GetComponent<PlayerMovement>().GetIsMoving())
                    m_TurnOver = true;

                // clean everything from this node event
                foreach (GameObject sw in UI_Manager.instance.m_SwapList)
                {
                    Destroy(sw);
                }
                UI_Manager.instance.m_SwapList.Clear();
                UI_Manager.instance.m_PlayersToBeSwapped.Clear();
                break;

            case "CASILLA - RANDOMSWITCH":
                UI_Manager.instance.m_CasillaText.text = "Random Swap Square";
                m_ActualSwitchPos_1 = m_Player.GetRoutePos();
                m_RandomPlayerPos = RandomPlayerLocation(m_Player);
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(m_Player.ChangePlayerPosition(m_RandomPlayerPos.GetRoutePos()));
                yield return StartCoroutine(m_RandomPlayerPos.ChangePlayerPosition(m_ActualSwitchPos_1));
                yield return new WaitForSeconds(1f);
                while (m_Player.GetIsMoving()) { yield return null; }
                if (!m_Player.GetIsMoving())
                    m_TurnOver = true;
                break;

            case "CASILLA - GAMEOVER":
                UI_Manager.instance.m_CasillaText.text = "Finish Square";
                m_Player.GetComponentInChildren<Animator>().SetBool("IsWinning", true);
                yield return new WaitForSeconds(2f);
                m_Player.GetComponentInChildren<Animator>().SetBool("IsWinning", false);
                ChangeState(State.GAMEOVER);
                break;

            case "CASILLA - ?":
                UI_Manager.instance.m_CasillaText.text = "Random effect";
                string l_RTag = m_NodeTagList[Random.Range(0, m_NodeTagList.Length)];
                yield return StartCoroutine(CheckNodeEvent(m_Player, l_RTag));
                break;

            case "CASILLA - PARESNONES":
                UI_Manager.instance.m_CasillaText.text = "Odds and Evens Square";
                StartCoroutine(DiceParesNones(m_Player));
                while (m_DiceIsRolling) { yield return null; }
                m_TurnOver = true;
                break;
        }

        m_CheckingEvent = false;
    }

    private IEnumerator BeginningFirstTurn()
    {
        FadeController.instance.gameObject.GetComponent<Canvas>().sortingOrder = 10;
        yield return StartCoroutine(FadeController.instance.Fade_In_Black(1.5f));
        ChangeState(State.PLAYER1_TURN);
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeController.instance.Fade_Out_Black(1.5f));
        FadeController.instance.gameObject.GetComponent<Canvas>().sortingOrder = -10;
    }
    private IEnumerator BeginningMinigame()
    {
        UI_Manager.instance.m_CasillaBackground.SetActive(false);
        // PAUSE BOARD MUSIC & PLAY MINIGAME START SONG
        SoundManager.instance.Music_Board.setPaused(true);
        SoundManager.instance.Sound_ShowMinigameName();

        // DISABLE BOARD UI
        UI_Manager.instance.panel_Board.SetActive(false);

        // SET ALL PLAYER OBJECTS TO TRUE
        foreach (Player p in m_PlayerOrderList)
        {
            if (!p.gameObject.activeSelf)
            {
                p.gameObject.SetActive(true);
            }
        }

        int l_NewRandomGame = 0;
        do
        {
            l_NewRandomGame = Random.Range(0, m_MinigamesList.Count);
        } while (m_RandomMinigame == l_NewRandomGame);

        m_RandomMinigame = l_NewRandomGame;

        //// [m_RandomMinigame = 0] => BALDOSAS
        //// [m_RandomMinigame = 1] => WITCH
        //// [m_RandomMinigame = 2] => BALL
        //m_RandomMinigame = 0;

        for (int i = 0; i < m_PlayerManager.GetNumberOfPlayers(); i++)
        {
            //m_PlayerOrderList[i].gameObject.SetActive(false); -- NO BORRAR
            m_PlayerOrderList[i].m_PlayerMovement.m_LastBoardPos = m_PlayerOrderList[i].m_PlayerMovement.GetRoutePos();
            m_PlayerOrderList[i].m_PlayerMovement.m_LastTransformBoardPos = m_PlayerOrderList[i].m_PlayerMovement.GetTransformPos();
        }

        // 1- APARECEN LAS TARGETAS ESAS HACIENDO SU ANIMACIÓN O LO QUE SEA
        UI_Manager.instance.ShowMinigame(m_RandomMinigame);
        yield return new WaitForSeconds(3.0f);

        // 2- PANTALLA FADE IN NEGRA
        FadeController.instance.FadeIn_Black(1.2f);
        yield return new WaitForSeconds(2.0f);

        // 3- LA CÁMARA PRINCIPAL PASA A SER LA DEL MINIJUEGO ESCOGIDO Y
        // 4- DESACTIVAR PANEL MINIJUEGO + PONER COLOR DE LA ANTERIOR CARTA EN APLHA 0
        switch (m_RandomMinigame)
        {
            // BALDOSAS
            case 0:
                switch (m_PlayerOrderList.Count)
                {
                    case 2:
                        SetCameraActive(CameraManager.instance.m_BaldosasCamera1);
                        break;
                    case 3:
                        SetCameraActive(CameraManager.instance.m_BaldosasCamera2);
                        break;
                    case 4:
                        SetCameraActive(CameraManager.instance.m_BaldosasCamera3);
                        break;
                }
                m_MinigamesList[0].SetActive(true);
                if (m_PrimerTurno)
                {
                    UI_Manager.instance.SetCardImages_EnterGame();
                }
                break;

            // BRUJA
            case 1:
                SetCameraActive(CameraManager.instance.m_BrujaCamera);
                m_MinigamesList[1].SetActive(true);
                foreach (Player p in m_PlayerOrderList)
                {
                    p.gameObject.GetComponent<Player_OldSystem>().playing = true;
                    p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_WitchGame = true;
                }
                if (m_PrimerTurno)
                {
                    UI_Manager.instance.SetCardImages_EnterGame();
                }
                break;

            // BALL
            case 2:
                SetCameraActive(CameraManager.instance.m_BallCamera);
                m_MinigamesList[2].SetActive(true);

                if (m_PrimerTurno)
                {
                    UI_Manager.instance.SetCardImages_EnterGame();
                }
                break;
        }


        // 5- FADE OUT DE PANTALLA NEGRA. SE MUESTRA UNA IMAGEN TUTORIAL DEL MINIJUEGO.
        // SE ACTIVA UN CONTADOR "3, 2, 1... START!" Y EMPIEZA EL MINIJUEGO (HACERLO DESDE CADA MINIJUEGO)
        FadeController.instance.FadeOut_Black(1.2f);

        yield return new WaitForSeconds(2.0f);

        UI_Manager.instance.SetCardImages_EnterGame_FirstTime();
        canSkipInstructions = true;

        yield return null;
    }
    #endregion

    #region UI
    private void CheckDiceUI_P1()
    {
        // SET BACKGROUND CAMERA COLOR
        // SHOW "THROW THE DICE" HUD AND SET COLOR
        if (m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin1") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago1") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier1") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco1"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[0];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin2") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago2") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier2") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco2"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[1];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin3") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago3") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier3") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco3"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[2];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin4") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago4") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier4") ||
            m_PlayerOrderList[0].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco4"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[3];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
    }
    private void CheckDiceUI_P2()
    {
        // SET BACKGROUND CAMERA COLOR
        // SHOW "THROW THE DICE" HUD AND SET COLOR
        if (m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin1") ||
    m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago1") ||
    m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier1") ||
    m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco1"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[0];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin2") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago2") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier2") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco2"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[1];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin3") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago3") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier3") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco3"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[2];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin4") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago4") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier4") ||
            m_PlayerOrderList[1].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco4"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[3];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
    }
    private void CheckDiceUI_P3()
    {
        // SET BACKGROUND CAMERA COLOR
        // SHOW "THROW THE DICE" HUD AND SET COLOR
        if (m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin1") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago1") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier1") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco1"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[0];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin2") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago2") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier2") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco2"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[1];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin3") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago3") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier3") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco3"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[2];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin4") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago4") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier4") ||
            m_PlayerOrderList[2].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco4"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[3];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
    }
    private void CheckDiceUI_P4()
    {
        // SET BACKGROUND CAMERA COLOR
        // SHOW "THROW THE DICE" HUD AND SET COLOR
        if (m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin1") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago1") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier1") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco1"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[0];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[0];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin2") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago2") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier2") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco2"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[1];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[1];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin3") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago3") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier3") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco3"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[2];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[2];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
        else if (m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin4") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago4") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier4") ||
            m_PlayerOrderList[3].gameObject.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco4"))
        {
            UI_Manager.instance.m_DiceUI.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.m_DiceUI2.GetComponent<Image>().sprite = UI_Manager.instance.m_diceBackground[3];
            UI_Manager.instance.panel_ExtraDice.GetComponent<Image>().sprite = UI_Manager.instance.m_ThrowDiceBackground[3];
            UI_Manager.instance.panel_ExtraDice.SetActive(true);
        }
    }

    public void UpdateTextUI(string _Text, float r = 0f, float g = 0f, float b = 0f)
    {
        UI_Manager.instance.m_FSMStateText.color = new Color(r, g, b);
        UI_Manager.instance.m_FSMStateText.text = _Text;
    }
    void CheckTextColor(Player _Player)
    {
        switch (_Player.tag)
        {
            case "PlayerRed":
                UpdateTextUI("TURN: PLAYER 1", 97f / 255f, 65f / 255f, 137f / 255f);
                break;
            case "PlayerBlue":
                UpdateTextUI("TURN: PLAYER 2", 0f, 170f / 255f, 233f / 255f);
                break;
            case "PlayerGreen":
                UpdateTextUI("TURN: PLAYER 3", 50f / 255f, 135f / 255f, 60f / 255f);
                break;
            case "PlayerOrange":
                UpdateTextUI("TURN: PLAYER 4", 232f / 255f, 221f / 255f, 70f / 255f);
                break;
        }
    }
    void UpdatePlayerCardPos(Player player)
    {
        player.m_PlayerCard.GetComponent<PlayerCard>().m_NumCasilla.text = player.m_PlayerMovement.GetRoutePos().ToString();
    }
    void CheckPlayerCrown(Player player)
    {
        int l_PlayerPos = 0;
        foreach (Player p in m_PlayerOrderList)
        {
            if (p.m_PlayerName != player.m_PlayerName)
            {
                if (p.m_PlayerMovement.GetRoutePos() > l_PlayerPos)
                {
                    l_PlayerPos = p.m_PlayerMovement.GetRoutePos();
                }
            }
        }

        if (player.m_PlayerMovement.GetRoutePos() > l_PlayerPos)
        {
            player.m_PlayerCard.GetComponent<PlayerCard>().m_CrownImg.sprite = UI_Manager.instance.m_Crown;
            player.m_PlayerCard.GetComponent<PlayerCard>().m_CrownImg.color = Color.white;

            foreach (Player p in m_PlayerOrderList)
            {
                if (p.m_PlayerName != player.m_PlayerName)
                {
                    p.m_PlayerCard.GetComponent<PlayerCard>().m_CrownImg.sprite = null;
                    p.m_PlayerCard.GetComponent<PlayerCard>().m_CrownImg.color = Color.clear;
                }
            }
        }
    }
    #endregion

    #region GETTERS & SETTERS
    public List<Player> GetOrderedPlayerList()
    {
        return m_PlayerOrderList;
    }
    public DiceManager GetDiceManager()
    {
        return m_CurrentDice;
    }
    public string GetCurrentState()
    {
        return currentState.ToString();
    }
    public bool GetDiceRolling()
    {
        return m_DiceIsRolling;
    }
    public int GetRandomMinigame()
    {
        return m_RandomMinigame;
    }
    public void SetCameraActive(CinemachineVirtualCamera cam)
    {
        foreach (CinemachineVirtualCamera vCam in FindObjectsOfType<CinemachineVirtualCamera>())
        {
            vCam.Priority = 0;
        }
        cam.Priority = 100;
    }
    #endregion

}
