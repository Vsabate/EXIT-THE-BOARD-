using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

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

    #region VARIABLES
    public enum Scene { MultiplayerTest, BoardScene }
    public Scene m_currentScene;
    bool m_GridDone;

    [Space]
    [Header("MANAGERS")]
    public BoardManager m_BoardManager;
    public GridManager m_GridManager;
    public BrujaManager m_BrujaManager;
    public BallGame m_BallManager;
    public PlayerManager m_PlayerManager;

    [Space]
    [Header("SETTINGS: VIDEO")]
    [HideInInspector] public Resolution[] m_resol_list;
    [HideInInspector] public int m_resolutionIndex;
    [HideInInspector] public int m_resol_Max;
    [HideInInspector] public int m_resol_Min = 0;

    public int m_qualityIndex;
    [HideInInspector] public int m_qualit_Max = 2;
    [HideInInspector] public int m_qualit_Min = 0;

    #endregion

    private void Start()
    {
        m_PlayerManager = GetComponent<PlayerManager>();

        m_resol_list = Screen.resolutions;
        m_resol_Max = m_resol_list.Length - 1;

        UI_Manager.instance.Quality_SetInitial();
        UI_Manager.instance.Screen_SetInitial();
        UI_Manager.instance.Resolution_SetInitial();

        // FADE OUT
        FadeController.instance.gameObject.GetComponent<Canvas>().sortingOrder = 10;
        SoundManager.instance.Sound_WELCOME();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "MultiplayerTest" && Input.GetKeyDown(KeyCode.H) &&
        PlayerManager.instance.GetNumberOfPlayers() >= 2)
        {
            UI_Manager.instance.Button_GO_TO_GAME();
        }
#endif

        if (m_BoardManager == null && GetCurrentScene() == Scene.BoardScene)
            m_BoardManager = FindObjectOfType<BoardManager>();

        if (m_GridManager == null && GetCurrentScene() == Scene.BoardScene)
            m_GridManager = FindObjectOfType<GridManager>();

        if (m_GridManager != null && !m_GridDone)
        {
            m_GridManager.GenerateGrid();
            m_GridDone = true;
        }

        if (m_BrujaManager == null && GetCurrentScene() == Scene.BoardScene)
            m_BrujaManager = FindObjectOfType<BrujaManager>();

        if (m_BallManager == null && GetCurrentScene() == Scene.BoardScene)
            m_BallManager = FindObjectOfType<BallGame>();
    }

    public void ChangeScene(Scene newScene)
    {
        switch (newScene)
        {
            case Scene.MultiplayerTest:
                Load(Scene.MultiplayerTest);
                // FADE OUT
                FadeController.instance.gameObject.GetComponent<Canvas>().sortingOrder = 10;
                FadeController.instance.FadeOut_Black(1.0f);
                FadeController.instance.Disable_TextLoading();

                UI_Manager.instance.panel_MainMenu.SetActive(true);
                UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
                break;

            case Scene.BoardScene:
                Load(Scene.BoardScene);
                FadeController.instance.gameObject.GetComponent<Canvas>().sortingOrder = 10;
                SoundManager.instance.Music_Board.start();
                FadeController.instance.FadeOut_Black(1.0f);
                break;
        }
    }
    public void Load(Scene newScene)
    {
        if (SceneManager.GetSceneByBuildIndex((int)newScene).isLoaded)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)newScene));
            SceneManager.UnloadSceneAsync((int)newScene);
        }
        else
        {
            SceneManager.LoadScene((int)newScene, LoadSceneMode.Single);
        }
        m_currentScene = newScene;
    }
    public Scene GetCurrentScene()
    {
        return m_currentScene;
    }

    public void LoadModels()
    {
        foreach (GameObject p in m_PlayerManager.m_PlayerList)
        {
            if (p.GetComponent<Player>().m_PlayerIcon.name.StartsWith("goblin"))
            {
                foreach (Transform child in p.transform)
                {
                    if (child.name.StartsWith("Goblin"))
                    {
                        child.gameObject.SetActive(true);
                        p.GetComponent<Player>().m_Avatar = child.gameObject;
                        p.GetComponent<Player>().m_Avatar.SetActive(true);
                    }
                }
            }
            else if (p.GetComponent<Player>().m_PlayerIcon.name.StartsWith("mago"))
            {
                foreach (Transform child in p.transform)
                {
                    if (child.name.StartsWith("Wizard"))
                    {
                        child.gameObject.SetActive(true);
                        p.GetComponent<Player>().m_Avatar = child.gameObject;
                        p.GetComponent<Player>().m_Avatar.SetActive(true);
                    }
                }
            }
            else if (p.GetComponent<Player>().m_PlayerIcon.name.StartsWith("soldier"))
            {
                foreach (Transform child in p.transform)
                {
                    if (child.name.StartsWith("Knight"))
                    {
                        child.gameObject.SetActive(true);
                        p.GetComponent<Player>().m_Avatar = child.gameObject;
                        p.GetComponent<Player>().m_Avatar.SetActive(true);
                    }
                }
            }
            else if (p.GetComponent<Player>().m_PlayerIcon.name.StartsWith("orco"))
            {
                foreach (Transform child in p.transform)
                {
                    if (child.name.StartsWith("Orc"))
                    {
                        child.gameObject.SetActive(true);
                        p.GetComponent<Player>().m_Avatar = child.gameObject;
                        p.GetComponent<Player>().m_Avatar.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.Log(p.GetComponent<Player>().m_PlayerName + " can't load model");
            }
        }
    }
    public void UnloadModels()
    {
        // 1- delete models from every player
        // 2- delete all player panel instances like in BUTTON_YES() in UI_Manager
        foreach (Transform item in UI_Manager.instance.Aux_SelectPanel.transform)
        {
            Destroy(item.gameObject);
        }

        m_PlayerManager.m_PlayerList.Clear();
        foreach (Transform plr in JoinPlayers.instance.transform)
        {
            plr.gameObject.GetComponent<Player>().m_PlayerName = null;
            plr.gameObject.GetComponent<Player>().m_PlayerIcon = null;
            plr.gameObject.tag = "Player";
            foreach (Transform model in plr.transform)
            {
                if (model.gameObject.name.StartsWith("Gob") || model.gameObject.name.StartsWith("Wiz")
                    || model.gameObject.name.StartsWith("Kni") || model.gameObject.name.StartsWith("Orc"))
                {
                    Destroy(model.gameObject);
                }
            }
        }

        JoinPlayers.instance.transform.DetachChildren();
        JoinPlayers.instance.playerList.Clear();
        JoinPlayers.instance.ResetCurrentTagList();
        JoinPlayers.instance.ResetCurrentMaterialList();
    }
}