using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallGame : MonoBehaviour
{

    #region States
    public enum State { INITIAL, PLAYING, GAMEOVER, GO_TO_BOARD };
    public State currentState = State.INITIAL;
    #endregion

    #region VARIABLEs
    public GameObject m_EscenarioBall;

    [Space]
    [Header("POSICIONES")]
    public GameObject m_Pos1;
    public GameObject m_Pos2;
    public GameObject m_Pos3;
    public GameObject m_Pos4;

    [Space]
    [Header("PLAYER VARS")]
    public List<Player> m_playersPlaying = new List<Player>();
    public List<Player> m_playerRanking = new List<Player>();

    [Space]
    [Header("Timers")]
    [HideInInspector] public bool m_Timer_RankOn = false;
    [HideInInspector] public float m_Timer_Ranking;
    private float m_Timer_Ranking_Original = 4f;

    [Space]
    [Header("CAMERA")]
    public GameObject m_BallGameCamera;
    #endregion

    private void Update()
    {
        switch (currentState)
        {
            case State.INITIAL:
                if (m_Timer_RankOn)
                {
                    if (m_Timer_Ranking <= 0.0f)
                    {
                        m_Timer_RankOn = false;
                        m_Timer_Ranking = m_Timer_Ranking_Original;
                        ChangeState(State.PLAYING);
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

            case State.PLAYING:
                if (m_playersPlaying.Count < 2)
                {
                    if (m_playersPlaying.Count > 0)
                    {
                        m_playerRanking.Add(m_playersPlaying[0]);
                        m_playersPlaying.Remove(m_playersPlaying[0]);
                    }
                    ChangeState(State.GAMEOVER);
                }
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
                        ChangeState(State.GO_TO_BOARD);
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

    void ChangeState(State newState)
    {
        // EXIT
        switch (currentState)
        {
            case State.INITIAL:
                break;

            case State.PLAYING:
                break;
            case State.GAMEOVER:
                UI_Manager.instance.panel_PlayerRanking.SetActive(false);
                break;
            case State.GO_TO_BOARD:
                break;
        }

        // ENTER
        switch (newState)
        {
            case State.INITIAL:
                m_Timer_Ranking = m_Timer_Ranking_Original;
                //m_Timer_RankOn = true;

                SpawnPlayers(GameManager.instance.m_BoardManager.m_PlayerOrderList);
                m_playersPlaying.AddRange(GameManager.instance.m_BoardManager.GetOrderedPlayerList().ToArray());

                foreach (Player player in m_playersPlaying)
                {
                    player.m_Ball.SetActive(true);
                }
                break;

            case State.PLAYING:
                foreach (Player p in m_playersPlaying)
                {
                    p.gameObject.GetComponentInChildren<BoxCollider>().enabled = true;
                    p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_BallGame = true;
                }
                break;

            case State.GAMEOVER:
                foreach (Player p in m_playerRanking)
                {
                    //SET PLAYING_BALLGAME TO FALSE
                    if (p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_BallGame != false)
                    {
                        p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_BallGame = false;
                        p.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
                    }
                    p.gameObject.SetActive(false);
                }
                // "GAME HAS ENDED": SCREEN POPS UP, SHOWING PLAYERS ORDERED + THEIR NEW DICE
                ShowRankings();
                break;

            case State.GO_TO_BOARD:

                // SET PLAYER STUFF:
                foreach (Player p in GameManager.instance.m_BoardManager.m_PlayerOrderList)
                {
                    p.m_Ball.SetActive(false);
                    // RETURN TO BOARD POSITION
                    p.gameObject.GetComponent<PlayerMovement>().SetLastNodePos();
                }
                // REMOVE ALL PLAYERS FROM m_playerRanking (and also from m_playersPlaying just in case)
                m_playerRanking.Clear();
                m_playersPlaying.Clear();

                // CHANGE BOARD STATE TO PLAYER TURN
                Return_To_Board();
                break;
        }
        currentState = newState;
    }

    public void SpawnPlayers(List<Player> _PlayerList)
    {
        switch (_PlayerList.Count)
        {
            case 2:
                _PlayerList[0].transform.position = m_Pos1.transform.position;
                _PlayerList[1].transform.position = m_Pos2.transform.position;
                break;
            case 3:
                _PlayerList[0].transform.position = m_Pos1.transform.position;
                _PlayerList[1].transform.position = m_Pos2.transform.position;
                _PlayerList[2].transform.position = m_Pos3.transform.position;
                break;
            case 4:
                _PlayerList[0].transform.position = m_Pos1.transform.position;
                _PlayerList[1].transform.position = m_Pos2.transform.position;
                _PlayerList[2].transform.position = m_Pos3.transform.position;
                _PlayerList[3].transform.position = m_Pos4.transform.position;
                break;
        }
    }

    private void ShowRankings()
    {
        SoundManager.instance.Sound_MinigameEnds();
        m_Timer_Ranking = 10;
        UI_Manager.instance.panel_PlayerRanking.SetActive(true);
        UI_Manager.instance.UpdateRanking(m_playerRanking);
        m_Timer_RankOn = true;
    }

    #region RETURN_TO_BOARD FUNCTIONS
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

    private void OnEnable()
    {
        m_EscenarioBall.SetActive(true);
        m_BallGameCamera.SetActive(true);
        ChangeState(State.INITIAL);
    }
    private void OnDisable()
    {
        m_EscenarioBall.SetActive(false);
    }

}
