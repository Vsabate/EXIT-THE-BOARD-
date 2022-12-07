using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using Cinemachine;
using System.Linq;
using UnityEngine.EventSystems;

public class BrujaManager : MonoBehaviour
{
    #region States
    public enum State { PRE_INITIAL, INITIAL, FILL_RANDOM_LIST, WITCH_TURN, WAIT_PHASE, PLAYERS_TURN, GAMEOVER, GO_TO_BOARD };
    public State currentState = State.PRE_INITIAL;
    #endregion

    #region RandomList
    bool m_ListReady;
    string[] m_MovimientosBruja = { "<", "^", ">", "v" };
    [SerializeField] List<int> m_RandomList = new List<int>();
    int m_RandomListLength = 100;
    #endregion

    #region Counters
    int m_Counter;
    int m_PlayerCounter;
    public int m_PlayerTurn = 0;
    int m_CurrentLevel;
    public float m_BrujaSpeed = 1f;
    #endregion

    #region Lists
    List<Player> m_playersPlaying = new List<Player>();
    List<Player> m_playerRanking = new List<Player>();
    public List<int> m_PlayerInputs = new List<int>();
    #endregion

    #region Players & Bruja Positions
    [Header("Players & Bruja Positions")]
    public Transform m_Player1Pos;
    public Transform m_Player2Pos;
    public Transform m_Player3Pos;
    public Transform m_Player4Pos;
    public Transform m_BrujaPos;
    Transform m_OriginalPos;
    #endregion

    [Space]
    #region Players & Bruja Characters
    [Header("Players & Bruja Characters")]
    public GameObject m_Bruja;
    #endregion

    [Space]
    #region Players Particles
    [Header("Players Particles")]
    public GameObject m_FireBubbles1;
    public GameObject m_FireBubbles2;
    public GameObject m_FireBubbles3;
    public GameObject m_FireBubbles4;

    public GameObject m_Explosion1;
    public GameObject m_Explosion2;
    public GameObject m_Explosion3;
    public GameObject m_Explosion4;

    public GameObject m_TurnoPlayer1;
    public GameObject m_TurnoPlayer2;
    public GameObject m_TurnoPlayer3;
    public GameObject m_TurnoPlayer4;

    #endregion

    [Space]
    #region Timer Ranking
    [Header("Timer for ranking screen")]
    [HideInInspector] public bool m_Timer_RankOn = false;
    public float m_Timer_Ranking;
    private float m_Timer_Ranking_Original = 4f;
    #endregion

    #region Bools
    bool m_DoingMove;
    bool m_Waiting;
    #endregion


    private void Update()
    {
        switch (currentState)
        {
            case State.PRE_INITIAL:
                if (m_Timer_RankOn)
                {
                    if (m_Timer_Ranking <= 0.0f)
                    {
                        m_Timer_RankOn = false;
                        m_Timer_Ranking = m_Timer_Ranking_Original;
                        StartCoroutine(ChangeState(State.INITIAL));
                    }
                    else
                    {
                        m_Timer_Ranking -= Time.deltaTime;
                        if (m_Timer_Ranking <= 3.0f && !SoundManager.instance.s_CanPlayDrums)
                        {
                            SoundManager.instance.s_CanPlayDrums = true;
                            SoundManager.instance.s_IsPlayingDrums = true;
                        }
                        if (SoundManager.instance.s_IsPlayingDrums)
                        {
                            SoundManager.instance.Sound_Drums();
                        }
                    }
                }
                break;

            case State.INITIAL:
                StartCoroutine(ChangeState(State.FILL_RANDOM_LIST));
                break;

            case State.FILL_RANDOM_LIST:
                if (m_ListReady)
                {
                    StartCoroutine(ChangeState(State.WITCH_TURN));
                }

                break;

            case State.WITCH_TURN:
                if (!m_DoingMove)
                    StartCoroutine(WitchTurn());

                break;

            case State.WAIT_PHASE:
                if (!m_Waiting)
                {
                    StartCoroutine(WaitPhase());
                }
                break;

            case State.PLAYERS_TURN:
                break;

            case State.GAMEOVER:
                if (m_Timer_RankOn)
                {
                    if (Input.GetAxisRaw(EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton) > 0)
                    {
                        m_Timer_Ranking = 0f;
                    }
                    if (m_Timer_Ranking <= 0f)
                    {
                        // GO TO BOARD
                        m_Timer_RankOn = false;
                        StartCoroutine(ChangeState(State.GO_TO_BOARD));
                    }
                    else
                    {
                        m_Timer_Ranking -= Time.deltaTime;
                    }
                }
                break;

            case State.GO_TO_BOARD:
                break;
        }
    }

    #region Coroutines
    IEnumerator ChangeState(State newState)
    {
        // EXIT STATE
        switch (currentState)
        {
            case State.PRE_INITIAL:
                break;

            case State.FILL_RANDOM_LIST:
                break;

            case State.WITCH_TURN:
                StopCoroutine(WitchTurn());
                break;

            case State.PLAYERS_TURN:
                if (m_PlayerTurn == m_playersPlaying.Count - 1)
                {
                    m_PlayerTurn = 0;
                }
                else
                {
                    m_PlayerTurn++;
                }
                m_PlayerInputs.Clear();
                break;

            case State.GAMEOVER:
                UI_Manager.instance.panel_PlayerRanking.SetActive(false);
                UI_Manager.instance.panel_WitchGame.gameObject.SetActive(false);
                break;
        }

        // ENTER STATE
        switch (newState)
        {
            case State.PRE_INITIAL:
                m_Timer_Ranking = m_Timer_Ranking_Original;

                if (m_playersPlaying.Count <= 0)
                {
                    //Se añaden jugadores
                    m_playersPlaying.AddRange(GameManager.instance.m_BoardManager.GetOrderedPlayerList().ToArray());
                    // ORDENAR JUGADORES SEGÚN POSICIÓN EN TABLERO!
                    m_playersPlaying.Sort(SortByRoutePos);
                    SpawnPlayers(m_playersPlaying);
                }

                FireOff();
                FireOn(m_playersPlaying.Count);

                foreach (Player player in m_playersPlaying)
                {
                    player.gameObject.transform.localScale *= 4;
                }

                break;

            case State.INITIAL:
                SoundManager.instance.Sound_WitchLaugh();

                break;

            case State.FILL_RANDOM_LIST:
                m_Counter = 0;
                m_PlayerCounter = 0;
                m_PlayerTurn = 0;
                m_CurrentLevel = 0;
                m_RandomList.Clear();
                FillRandomList();
                break;

            case State.WITCH_TURN:
                UI_Manager.instance.m_WitchText.color = Color.white;
                UI_Manager.instance.m_CurrentButton.color = Color.clear;
                m_Counter = 0;
                m_PlayerCounter = 0;
                TurnOff();
                yield return new WaitForSeconds(1f);
                UI_Manager.instance.m_WitchText.text = "TURN:  WITCH";
                UI_Manager.instance.m_BrujaText_Background.SetActive(true);
                break;

            case State.PLAYERS_TURN:
                UI_Manager.instance.m_CurrentButton.color = Color.clear;
                for (int i = 0; i < m_playersPlaying.Count; i++)
                {
                    if (m_PlayerTurn == i)
                    {
                        m_playersPlaying[i].gameObject.GetComponent<Player_OldSystem>().minigame_Witch_MyTurn = true;
                    }
                    else
                    {
                        m_playersPlaying[i].gameObject.GetComponent<Player_OldSystem>().minigame_Witch_MyTurn = false;
                    }
                }

                CheckTextColor(m_playersPlaying[m_PlayerTurn]);

                TurnIndicator(GameManager.instance.m_BoardManager.GetOrderedPlayerList().Count);
                m_playersPlaying[m_PlayerTurn].gameObject.GetComponentInChildren<Animator>().SetBool("IsCooking", true);
                break;

            case State.GAMEOVER:
                UI_Manager.instance.m_WitchText.color = Color.black;
                UI_Manager.instance.m_WitchText.text = "";

                foreach (Player p in m_playerRanking)
                {
                    if (p == null)
                    {
                        m_playerRanking.Remove(p);
                    }
                    else
                    {
                        //SET PLAYING_WITCHGAME TO FALSE
                        if (p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_WitchGame != false)
                        {
                            p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_WitchGame = false;
                        }
                        p.gameObject.SetActive(false);
                    }
                }
                // "GAME HAS ENDED": SCREEN POPS UP, SHOWING PLAYERS ORDERED + THEIR NEW DICE
                ShowRankings();
                break;

            case State.GO_TO_BOARD:
                // SET PLAYER STUFF
                foreach (Player p in GameManager.instance.m_BoardManager.m_PlayerOrderList)
                {
                    // RETURN TO BOARD POSITION
                    p.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    p.gameObject.GetComponent<PlayerMovement>().SetLastNodePos();
                }
                // REMOVE ALL PLAYERS FROM m_playerRanking (and also from m_playersPlaying just in case)
                m_playerRanking.Clear();
                m_playersPlaying.Clear();

                // KEYBOARD PLAYER MAPPING RESETS
                JoinPlayers.instance.gameObject.GetComponent<ControlManager>().SetMapping_NormalKeyboard();

                Return_To_Board();
                break;
        }

        currentState = newState;

    }

    IEnumerator WitchTurn()
    {
        if (currentState == State.WITCH_TURN)
        {
            // SET ACTUAL SPRITES TO SHOW AFTER, DEPENDING ON CONTROLLER (KEYBOARD OR JOYSTICK)
            if (m_playersPlaying[m_PlayerTurn].gameObject.GetComponent<Player_OldSystem>().playerID == 0)
            {
                UI_Manager.instance.m_ActualWitchButtons[0] = UI_Manager.instance.m_KeyboardMando_WitchButtons[4];
                UI_Manager.instance.m_ActualWitchButtons[1] = UI_Manager.instance.m_KeyboardMando_WitchButtons[5];
                UI_Manager.instance.m_ActualWitchButtons[2] = UI_Manager.instance.m_KeyboardMando_WitchButtons[6];
                UI_Manager.instance.m_ActualWitchButtons[3] = UI_Manager.instance.m_KeyboardMando_WitchButtons[7];
            }
            else
            {
                UI_Manager.instance.m_ActualWitchButtons[0] = UI_Manager.instance.m_KeyboardMando_WitchButtons[0];
                UI_Manager.instance.m_ActualWitchButtons[1] = UI_Manager.instance.m_KeyboardMando_WitchButtons[1];
                UI_Manager.instance.m_ActualWitchButtons[2] = UI_Manager.instance.m_KeyboardMando_WitchButtons[2];
                UI_Manager.instance.m_ActualWitchButtons[3] = UI_Manager.instance.m_KeyboardMando_WitchButtons[3];
            }

            m_DoingMove = true;
            yield return new WaitForSeconds(m_BrujaSpeed);

            #region Text & Color
            switch (m_MovimientosBruja[m_RandomList[m_Counter]])
            {
                case "<":
                    // ienumerator fade out
                    UI_Manager.instance.m_CurrentButton.sprite = UI_Manager.instance.m_ActualWitchButtons[0];
                    UI_Manager.instance.m_CurrentButton.color = Color.white;
                    FadeController.instance.FadeOut_BrujaButtons(UI_Manager.instance.m_CurrentButton, 1f);
                    break;

                case "^":
                    UI_Manager.instance.m_CurrentButton.sprite = UI_Manager.instance.m_ActualWitchButtons[1];
                    UI_Manager.instance.m_CurrentButton.color = Color.white;
                    FadeController.instance.FadeOut_BrujaButtons(UI_Manager.instance.m_CurrentButton, 1f);
                    break;

                case ">":
                    UI_Manager.instance.m_CurrentButton.sprite = UI_Manager.instance.m_ActualWitchButtons[2];
                    UI_Manager.instance.m_CurrentButton.color = Color.white;
                    FadeController.instance.FadeOut_BrujaButtons(UI_Manager.instance.m_CurrentButton, 1f);
                    break;

                case "v":
                    UI_Manager.instance.m_CurrentButton.sprite = UI_Manager.instance.m_ActualWitchButtons[3];
                    UI_Manager.instance.m_CurrentButton.color = Color.white;
                    FadeController.instance.FadeOut_BrujaButtons(UI_Manager.instance.m_CurrentButton, 1f);
                    break;
            }
            SoundManager.instance.Sound_WitchButton();
            #endregion

            if (m_Counter >= m_CurrentLevel)
            {
                m_CurrentLevel++;
                StartCoroutine(ChangeState(State.WAIT_PHASE));
            }
            else
            {
                m_Counter++;
            }

            m_DoingMove = false;
        }
        else
        {
            UI_Manager.instance.m_CurrentButton.color = new Color(1, 1, 1, 0);
        }
    }

    IEnumerator WaitPhase()
    {
        m_Waiting = true;
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ChangeState(State.PLAYERS_TURN));
        m_Waiting = false;
    }
    #endregion

    public void CheckKeyDown()
    {
        //SI EL JUGADOR FALLA
        if (m_PlayerInputs[m_PlayerCounter] != m_RandomList[m_PlayerCounter])
        {
            m_playersPlaying[m_PlayerTurn].gameObject.GetComponentInChildren<Animator>().SetBool("IsCooking", false);
            UI_Manager.instance.m_BrujaText_Background.SetActive(false);
            // TO MAKE SURE THAT PLAYER CAN'T PLAY AGAIN... SET minigame_Witch_MyTurn TO FALSE
            m_playersPlaying[m_PlayerTurn].gameObject.GetComponent<Player_OldSystem>().minigame_Witch_MyTurn = false;
            m_playersPlaying[m_PlayerTurn].gameObject.GetComponent<Player_OldSystem>().minigame_Playing_WitchGame = false;

            if (GameManager.instance.m_BoardManager.GetOrderedPlayerList().Count == 2)
            {
                switch (m_PlayerTurn)
                {
                    case 0:
                        m_Explosion2.SetActive(true);
                        break;
                    case 1:
                        m_Explosion3.SetActive(true);
                        break;
                }

            }
            else if (GameManager.instance.m_BoardManager.GetOrderedPlayerList().Count == 3)
            {
                switch (m_PlayerTurn)
                {
                    case 0:
                        m_Explosion1.SetActive(true);
                        break;
                    case 1:
                        m_Explosion2.SetActive(true);
                        break;
                    case 2:
                        m_Explosion3.SetActive(true);
                        break;
                }
            }
            else if (GameManager.instance.m_BoardManager.GetOrderedPlayerList().Count == 4)
            {
                switch (m_PlayerTurn)
                {

                    case 0:
                        m_Explosion1.SetActive(true);
                        break;
                    case 1:
                        m_Explosion2.SetActive(true);
                        break;
                    case 2:
                        m_Explosion3.SetActive(true);
                        break;
                    case 3:
                        m_Explosion4.SetActive(true);
                        break;
                }
            }

            m_playerRanking.Add(m_playersPlaying[m_PlayerTurn]);
            m_playersPlaying[m_PlayerTurn].gameObject.SetActive(false);
            m_playersPlaying.Remove(m_playersPlaying[m_PlayerTurn]);

            if (m_playersPlaying.Count > 1)
            {
                StartCoroutine(ChangeState(State.INITIAL));
            }
            else
            {
                m_playerRanking.Add(m_playersPlaying[0]);
                StartCoroutine(ChangeState(State.GAMEOVER));
            }
        }

        // SI EL JUGADOR LO HACE BIEN
        if (m_PlayerCounter == m_Counter && m_playersPlaying.Count > 1)
        {
            SoundManager.instance.Sound_WitchButton();
            m_playersPlaying[m_PlayerTurn].gameObject.GetComponentInChildren<Animator>().SetBool("IsCooking", false);
            m_playersPlaying[m_PlayerTurn].gameObject.GetComponent<Player_OldSystem>().minigame_Witch_MyTurn = false;
            StartCoroutine(ChangeState(State.WITCH_TURN));
        }
        else
        {
            SoundManager.instance.Sound_WitchButton();
            m_PlayerCounter++;
        }
    }

    public void SpawnPlayers(List<Player> _PlayerList)
    {
        switch (_PlayerList.Count)
        {
            case 2:
                _PlayerList[0].transform.position = m_Player2Pos.position;
                _PlayerList[0].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[1].transform.position = m_Player3Pos.position;
                _PlayerList[1].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);
                break;
            case 3:
                _PlayerList[0].transform.position = m_Player1Pos.position;
                _PlayerList[0].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[1].transform.position = m_Player2Pos.position;
                _PlayerList[1].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[2].transform.position = m_Player3Pos.position;
                _PlayerList[2].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);
                break;
            case 4:
                _PlayerList[0].transform.position = m_Player1Pos.position;
                _PlayerList[0].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[1].transform.position = m_Player2Pos.position;
                _PlayerList[1].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[2].transform.position = m_Player3Pos.position;
                _PlayerList[2].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);

                _PlayerList[3].transform.position = m_Player4Pos.position;
                _PlayerList[3].transform.LookAt(CameraManager.instance.m_BrujaCamera.transform);
                break;
        }
    }

    void FillRandomList()
    {
        // SE CREA UNA LISTA ALEATORIA PARA LA BRUJA
        for (int i = 0; i <= m_RandomListLength; i++)
        {
            m_RandomList.Add(Random.Range(0, 4));
        }
        m_ListReady = true;
    }

    void ShowRankings()
    {
        SoundManager.instance.Sound_MinigameEnds();
        m_Timer_Ranking = 10;
        UI_Manager.instance.panel_PlayerRanking.SetActive(true);
        UI_Manager.instance.UpdateRanking(m_playerRanking);
        m_Timer_RankOn = true;
    }
    int SortByRoutePos(Player p1, Player p2)
    {
        return p1.m_PlayerMovement.GetRoutePos().CompareTo(p2.m_PlayerMovement.GetRoutePos());
    }

    void TurnIndicator(int TotalPlayers)
    {
        if (TotalPlayers == 2)
        {
            switch (m_PlayerTurn)
            {
                case 0:
                    m_TurnoPlayer2.SetActive(true);
                    break;
                case 1:
                    m_TurnoPlayer3.SetActive(true);
                    break;
            }

        }
        else if (TotalPlayers == 3)
        {
            switch (m_PlayerTurn)
            {
                case 0:
                    m_TurnoPlayer1.SetActive(true);
                    break;
                case 1:
                    m_TurnoPlayer2.SetActive(true);
                    break;
                case 2:
                    m_TurnoPlayer3.SetActive(true);
                    break;
            }
        }
        else if (TotalPlayers == 4)
        {
            switch (m_PlayerTurn)
            {
                case 0:
                    m_TurnoPlayer1.SetActive(true);
                    break;
                case 1:
                    m_TurnoPlayer2.SetActive(true);
                    break;
                case 2:
                    m_TurnoPlayer3.SetActive(true);
                    break;
                case 3:
                    m_TurnoPlayer4.SetActive(true);
                    break;
            }
        }
    }
    void TurnOff()
    {
        m_TurnoPlayer1.SetActive(false);
        m_TurnoPlayer2.SetActive(false);
        m_TurnoPlayer3.SetActive(false);
        m_TurnoPlayer4.SetActive(false);
    }
    void ExplosionOff()
    {
        m_Explosion1.SetActive(false);
        m_Explosion2.SetActive(false);
        m_Explosion3.SetActive(false);
        m_Explosion4.SetActive(false);
    }

    void FireOn(int TotalPlayers)
    {
        if (TotalPlayers == 2)
        {
            m_FireBubbles2.SetActive(true);
            m_FireBubbles3.SetActive(true);
        }
        else if (TotalPlayers == 3)
        {
            m_FireBubbles1.SetActive(true);
            m_FireBubbles2.SetActive(true);
            m_FireBubbles3.SetActive(true);
        }
        else if (TotalPlayers == 4)
        {
            m_FireBubbles1.SetActive(true);
            m_FireBubbles2.SetActive(true);
            m_FireBubbles3.SetActive(true);
            m_FireBubbles4.SetActive(true);
        }
    }
    void FireOff()
    {
        m_FireBubbles1.SetActive(false);
        m_FireBubbles2.SetActive(false);
        m_FireBubbles3.SetActive(false);
        m_FireBubbles4.SetActive(false);
    }


    #region Enable & Disable
    private void OnEnable()
    {
        // KEYBOARD PLAYER MAPPING CHANGES FOR THIS GAME
        JoinPlayers.instance.gameObject.GetComponent<ControlManager>().SetMapping_WitchKeyboard();
        UI_Manager.instance.panel_WitchGame.SetActive(true);
        m_OriginalPos = m_BrujaPos;
        StartCoroutine(ChangeState(State.PRE_INITIAL));
        ExplosionOff();
        TurnOff();
    }
    private void OnDisable()
    {
        UI_Manager.instance.panel_WitchGame.SetActive(false);
        m_playersPlaying.Clear();
        m_PlayerInputs.Clear();
        m_RandomList.Clear();
        m_ListReady = false;
        FireOff();
        m_Counter = 0;
        m_PlayerCounter = 0;
        m_PlayerTurn = 0;
        m_CurrentLevel = 0;
        ExplosionOff();
        TurnOff();
    }
    #endregion

    #region UI
    public void UpdateTextUI(string _Text, Color32 _TextColor = new Color32())
    {
        UI_Manager.instance.m_WitchText.color = _TextColor;
        UI_Manager.instance.m_WitchText.text = _Text;
    }
    void CheckTextColor(Player _Player)
    {
        switch (_Player.tag)
        {
            case "PlayerRed":
                UpdateTextUI("TURN:  PLAYER  1", new Color(97f / 255f, 65f / 255f, 137f / 255f));
                break;
            case "PlayerBlue":
                UpdateTextUI("TURN:  PLAYER  2", new Color(0f, 170f / 255f, 233f / 255f));
                break;
            case "PlayerGreen":
                UpdateTextUI("TURN:  PLAYER  3", new Color(50f / 255f, 135f / 255f, 60f / 255f));
                break;
            case "PlayerOrange":
                UpdateTextUI("TURN:  PLAYER  4", new Color(232f / 255f, 221f / 255f, 70f / 255f));
                break;
        }
    }
    #endregion

    #region RETURN_TO_BOARD
    // RETURN TO BOARD FUNCTIONS
    private void Return_To_Board()
    {
        StartCoroutine(ReturnToBoard());
    }
    private IEnumerator ReturnToBoard()
    {
        yield return StartCoroutine(FadeController.instance.Fade_In_Black(1.2f));
        GameManager.instance.m_BoardManager.ChangeState(BoardManager.State.PLAYER1_TURN);
        FadeController.instance.FadeOut_Black(1.2f);
    }
    #endregion
}
