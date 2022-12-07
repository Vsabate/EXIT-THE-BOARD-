using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class GridManager : MonoBehaviour
{
    #region States
    public enum State { INITIAL, PLAYING, GAMEOVER, GO_TO_BOARD };
    public State currentState = State.INITIAL;
    #endregion

    [Header("GRID")]
    public GameObject m_Grid8x8;
    public GameObject m_Grid10x10;
    public GameObject m_Grid12x12;
    private GameObject m_Grid_Current;
    private Transform m_GridCurrent_Transform;

    [Space]
    [Header("BALDOSAS")]
    public GameObject m_8x8_1;
    public GameObject m_8x8_2;
    public GameObject m_10x10_1;
    public GameObject m_10x10_2;
    public GameObject m_10x10_3;
    public GameObject m_12x12_1;
    public GameObject m_12x12_2;
    public GameObject m_12x12_3;
    public GameObject m_12x12_4;

    [Space]
    [Header("PLAYER VARS")]
    public List<Player> m_playersPlaying = new List<Player>();
    public List<Player> m_playerRanking = new List<Player>();

    [Space]
    [Header("Timer")]
    public bool m_Timer_RankOn = false;
    public float m_Timer_Ranking;
    private float m_Timer_Ranking_Original = 4f;

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
                SpawnPlayers(GameManager.instance.m_BoardManager.m_PlayerOrderList);
                m_playersPlaying.AddRange(GameManager.instance.m_BoardManager.GetOrderedPlayerList().ToArray());
                break;

            case State.PLAYING:
                foreach (Player p in m_playersPlaying)
                {
                    p.gameObject.GetComponentInChildren<BoxCollider>().enabled = true;
                    p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_FallGame = true;
                }
                break;

            case State.GAMEOVER:
                foreach (Player p in m_playerRanking)
                {
                    // SET PLAYING_FALLGAME TO FALSE
                    if (p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_FallGame != false)
                    {
                        p.gameObject.GetComponent<Player_OldSystem>().minigame_Playing_FallGame = false;
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
                    // RETURN TO BOARD POSITION
                    p.gameObject.GetComponent<PlayerMovement>().SetLastNodePos();
                }
                // REMOVE ALL PLAYERS FROM m_playerRanking (and also from m_playersPlaying just in case)
                m_playerRanking.Clear();
                m_playersPlaying.Clear();
                // RETURN ALL BLOCKS TO NORMAL
                foreach (Transform block in m_GridCurrent_Transform)
                {
                    block.gameObject.GetComponent<MeshRenderer>().material = block.gameObject.GetComponent<Casilla_Breakable>().l_Mat_Normal;
                    block.gameObject.GetComponent<Casilla_Breakable>().l_DeathZone = false;
                    block.gameObject.GetComponent<Casilla_Breakable>().l_Stepped = false;
                }
                // CHANGE BOARD STATE TO PLAYER TURN
                Return_To_Board();
                break;
        }
        currentState = newState;
    }

    #region GRID_Functions
    public void GenerateGrid()
    {
        switch (PlayerManager.instance.GetNumberOfPlayers())
        {
            case 2:
                m_Grid8x8.SetActive(true);
                m_Grid_Current = m_Grid8x8;
                break;
            case 3:
                m_Grid10x10.SetActive(true);
                m_Grid_Current = m_Grid10x10;
                break;
            case 4:
                m_Grid12x12.SetActive(true);
                m_Grid_Current = m_Grid12x12;
                break;
        }
        m_GridCurrent_Transform = m_Grid_Current.transform;
    }
    public void SpawnPlayers(List<Player> _PlayerList)
    {
        switch (_PlayerList.Count)
        {
            case 2:
                _PlayerList[0].transform.position = m_8x8_1.transform.position;
                _PlayerList[1].transform.position = m_8x8_2.transform.position;
                break;
            case 3:
                _PlayerList[0].transform.position = m_10x10_1.transform.position;
                _PlayerList[1].transform.position = m_10x10_2.transform.position;
                _PlayerList[2].transform.position = m_10x10_3.transform.position;
                break;
            case 4:
                _PlayerList[0].transform.position = m_12x12_1.transform.position;
                _PlayerList[1].transform.position = m_12x12_2.transform.position;
                _PlayerList[2].transform.position = m_12x12_3.transform.position;
                _PlayerList[3].transform.position = m_12x12_4.transform.position;
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
    #endregion

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

    private void OnEnable()
    {
        ChangeState(State.INITIAL);
    }
    private void OnDisable()
    {
    }

}
