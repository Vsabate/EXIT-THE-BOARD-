using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using ActionEnums;

public class UI_Manager : MonoBehaviour
{
    #region Singleton
    public static UI_Manager instance;
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
    [Header("PANELS")]
    public GameObject panel_MainMenu;
    public GameObject panel_CharSelect;
    public GameObject panel_ReturnToMainMenu;
    public GameObject panel_GoToBoard;
    public GameObject panel_BoardToMenu;

    public GameObject panel_Opts_ALL;
    public GameObject panel_Opts_General;
    public GameObject panel_Opts_Controls;
    public GameObject panel_Opts_Video;
    public GameObject panel_Opts_Audio;

    public GameObject panel_Board;
    public GameObject panel_PlayerRanking;
    public GameObject panel_WitchGame;
    public GameObject panel_SwapMenu;
    public GameObject panel_SwapPlayer;
    public GameObject panel_ShowMinigame;

    public GameObject panel_Bienvenido;
    public GameObject panel_ExtraDice;

    public GameObject panel_VictoryScreen;
    public GameObject image_Congrats;
    public TextMeshProUGUI text_Congrats;

    [Space]
    [Header("SETTINGS: VIDEO")]
    public GameObject text_Resolution;
    public GameObject text_Quality;
    public GameObject text_ScreenMode;

    [Space]
    [Header("SETTINGS: AUDIO")]
    public GameObject text_GeneralVol;
    public GameObject text_MusicVol;
    public GameObject text_SfxVol;

    [Space]
    [Header("FALLGAME")]
    public GameObject text_Timer;

    [Space]
    [Header("BOARD STUFF")]
    public GameObject m_FSM_panel;
    public GameObject m_DiceUI;
    public Sprite[] m_diceBackground;
    public Sprite[] m_ThrowDiceBackground;
    public GameObject m_DiceUI2;
    public GameObject m_CanvasMain;
    public TextMeshProUGUI m_FSMStateText;
    public TextMeshProUGUI m_CasillaText;
    public GameObject m_CasillaBackground;
    public TextMeshProUGUI m_DadoText;
    public TextMeshProUGUI m_ExtraDiceText;

    [Space]
    [Header("WITCH IMGS")]
    public GameObject m_BrujaText_Background;
    public Image m_CurrentButton;
    public Sprite[] m_WitchButtons;
    // m_WitchButtons[0] = PS4_LEFT
    // m_WitchButtons[1] = PS4_UP
    // m_WitchButtons[2] = PS4_RIGHT
    // m_WitchButtons[3] = PS4_DOWN
    // m_WitchButtons[4] = KEYBOARD_LEFT
    // m_WitchButtons[5] = KEYBOARD_UP
    // m_WitchButtons[6] = KEYBOARD_RIGHT
    // m_WitchButtons[7] = KEYBOARD_DOWN
    // m_WitchButtons[8] = XBOX_LEFT
    // m_WitchButtons[9] = XBOX_UP
    // m_WitchButtons[10] = XBOX_RIGHT
    // m_WitchButtons[11] = XBOX_DOWN

    public Sprite[] m_KeyboardMando_WitchButtons; // O to 3 is controller, 4 to 7 is keyboard
    public Sprite[] m_ActualWitchButtons;

    public TextMeshProUGUI m_WitchText;

    [Space]
    [Header("EVENTSYSTEM STUFF")]
    private GameObject m_LastSelected;

    [Space]
    [Header("BUTTONS")]
    public GameObject m_Button_PLAY;
    public GameObject m_Button_CONTROLS;
    public GameObject m_Button_PS4;
    public GameObject m_Button_VIDEO_RESOLUTION_Minus;
    public GameObject m_Button_AUDIO_GENERAL_VOL_Minus;
    public GameObject m_Button_RETURN_TO_MAIN;
    public GameObject m_Button_GO_TO_BOARD;
    public GameObject m_Button_BOARD_TO_MAIN;

    [Space]
    [Header("MANUAL SWAP STUFF")]
    [HideInInspector] public List<GameObject> m_SwapList = new List<GameObject>();
    [HideInInspector] public List<GameObject> m_PlayersToBeSwapped = new List<GameObject>();

    [Space]
    [Header("PLAYERS CARDS")]
    public Sprite[] m_HorizontalCards;
    public GameObject m_PlayerCard;
    public Sprite m_Crown;

    [Space]
    [Header("PLAYERS RANKING")]
    public GameObject m_Player1_RankingPanel;
    public GameObject m_Player2_RankingPanel;
    public GameObject m_Player3_RankingPanel;
    public GameObject m_Player4_RankingPanel;

    [Space]
    public Sprite m_Dice6Icon;
    public Sprite m_Dice4Icon;
    public Sprite m_Dice8Icon;

    [Space]
    public Sprite m_Blue_Background;
    public Sprite m_Purple_Background;
    public Sprite m_Green_Background;
    public Sprite m_Yellow_Background;

    [Space]
    public TextMeshProUGUI m_NumPlayer1_Ranking;
    public TextMeshProUGUI m_Player1_Name;
    public Image m_Player1_Img;
    public Image m_Player1_DiceImg;
    public Image m_Player1_BackgroundImg;
    [Space]
    public TextMeshProUGUI m_NumPlayer2_Ranking;
    public TextMeshProUGUI m_Player2_Name;
    public Image m_Player2_Img;
    public Image m_Player2_DiceImg;
    public Image m_Player2_BackgroundImg;
    [Space]
    public TextMeshProUGUI m_NumPlayer3_Ranking;
    public TextMeshProUGUI m_Player3_Name;
    public Image m_Player3_Img;
    public Image m_Player3_DiceImg;
    public Image m_Player3_BackgroundImg;
    [Space]
    public TextMeshProUGUI m_NumPlayer4_Ranking;
    public TextMeshProUGUI m_Player4_Name;
    public Image m_Player4_Img;
    public Image m_Player4_DiceImg;
    public Image m_Player4_BackgroundImg;

    [Space]
    [Header("PLAYER ICONS")]
    public Sprite[] m_IconArray;
    public int icon_counter;

    // SELECTABLE CHARACTER: ICON STUFF
    public List<string> m_CharName_List = new List<string>();
    [HideInInspector] public GameObject Aux_SelectPanel;

    [Space]
    [Header("MINIGAME CARDS")]
    public Image card_GameName;
    public Image card_GameInstructions;
    public Sprite[] GameNames;
    public Sprite[] GameInstructions;

    [HideInInspector] public bool canDiscount;
    [HideInInspector] public bool canShowBegin;
    private float beginText_timer = 1f;
    public TextMeshProUGUI text_timer;
    public TextMeshProUGUI text_BEGIN;
    public TextMeshProUGUI text_RankingTimer;

    [HideInInspector] public ControlManager controlManager;
    private bool aux_axis;
    #endregion

    void Start()
    {
        controlManager = FindObjectOfType<ControlManager>();

        panel_Board.SetActive(false);
        m_DiceUI.SetActive(false);
        m_DiceUI2.SetActive(false);

        m_CanvasMain.SetActive(true);

        panel_MainMenu.SetActive(true);
        panel_CharSelect.SetActive(false);
        panel_Opts_ALL.SetActive(false);
        panel_ReturnToMainMenu.SetActive(false);
        panel_GoToBoard.SetActive(false);
        panel_PlayerRanking.SetActive(false);
        panel_WitchGame.SetActive(false);
        panel_SwapMenu.SetActive(false);
        panel_ShowMinigame.SetActive(false);
        panel_ExtraDice.SetActive(false);
        m_CasillaBackground.SetActive(false);
        m_BrujaText_Background.SetActive(false);
        panel_VictoryScreen.SetActive(false);
        image_Congrats.SetActive(false);
        panel_BoardToMenu.SetActive(false);

        card_GameName.color = new Color(1, 1, 1, 0);
        card_GameInstructions.color = new Color(1, 1, 1, 0);

        // SET CURRENT WITCH BUTTONS
        m_ActualWitchButtons[0] = m_WitchButtons[0];
        m_ActualWitchButtons[1] = m_WitchButtons[1];
        m_ActualWitchButtons[2] = m_WitchButtons[2];
        m_ActualWitchButtons[3] = m_WitchButtons[3];

        m_PlayersToBeSwapped.Clear();
        icon_counter = 0;

        canDiscount = false;
        aux_axis = true;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "BoardScene" && panel_CharSelect.activeSelf)
        {
            panel_CharSelect.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == "MultiplayerTest")
        {
            Aux_SelectPanel = GameObject.Find("Panel_CharSelect");
            if (Input.GetAxis("Cancel_PS4") > 0 && panel_CharSelect.activeSelf && aux_axis && JoinPlayers.instance.playerList.Count <= 0)
            {
                aux_axis = false;
                EventSyst.instance.gameObject.SetActive(true);
                panel_ReturnToMainMenu.SetActive(true);
                SetSelectedButton_WhenMenusEnabled();
            }
        }
        SelectedButton_Update();
        CheckTimer();
    }

    public void UpdateRanking(List<Player> m_PlayerRanking)
    {
        m_Player1_RankingPanel.SetActive(false);
        m_Player2_RankingPanel.SetActive(false);
        m_Player3_RankingPanel.SetActive(false);
        m_Player4_RankingPanel.SetActive(false);

        switch (m_PlayerRanking.Count)
        {
            case 2:
                //ULTIMO LUGAR
                m_Player1_RankingPanel.SetActive(true);
                m_NumPlayer1_Ranking.text = "2";
                m_Player1_Name.text = m_PlayerRanking[0].m_PlayerName;
                m_PlayerRanking[0].m_CurrentDice = 6;
                m_Player1_DiceImg.sprite = m_Dice6Icon;
                m_Player1_Img.sprite = m_PlayerRanking[0].m_PlayerIcon;
                switch (m_PlayerRanking[0].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player1_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player1_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player1_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player1_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //PRIMER LUGAR
                m_Player2_RankingPanel.SetActive(true);
                m_NumPlayer2_Ranking.text = "1";
                m_Player2_Name.text = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerName;
                m_PlayerRanking[m_PlayerRanking.Count - 1].m_CurrentDice = 8;
                m_Player2_DiceImg.sprite = m_Dice8Icon;
                m_Player2_Img.sprite = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerIcon;

                switch (m_PlayerRanking[m_PlayerRanking.Count - 1].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player2_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player2_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player2_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player2_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                break;

            case 3:
                //ULTIMO LUGAR
                m_Player1_RankingPanel.SetActive(true);
                m_NumPlayer1_Ranking.text = "3";
                m_Player1_Name.text = m_PlayerRanking[0].m_PlayerName;
                m_PlayerRanking[0].m_CurrentDice = 4;
                m_Player1_DiceImg.sprite = m_Dice4Icon;
                m_Player1_Img.sprite = m_PlayerRanking[0].m_PlayerIcon;

                switch (m_PlayerRanking[0].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player1_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player1_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player1_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player1_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //PRIMER LUGAR
                m_Player2_RankingPanel.SetActive(true);
                m_NumPlayer2_Ranking.text = "1";
                m_Player2_Name.text = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerName;
                m_PlayerRanking[m_PlayerRanking.Count - 1].m_CurrentDice = 8;
                m_Player2_DiceImg.sprite = m_Dice8Icon;
                m_Player2_Img.sprite = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerIcon;

                switch (m_PlayerRanking[m_PlayerRanking.Count - 1].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player2_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player2_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player2_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player2_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //MEDIO LUGAR
                m_Player3_RankingPanel.SetActive(true);
                m_NumPlayer3_Ranking.text = "2";
                m_Player3_Name.text = m_PlayerRanking[1].m_PlayerName;
                m_PlayerRanking[1].m_CurrentDice = 6;
                m_Player3_DiceImg.sprite = m_Dice6Icon;
                m_Player3_Img.sprite = m_PlayerRanking[1].m_PlayerIcon;

                switch (m_PlayerRanking[1].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player3_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player3_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player3_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player3_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                break;

            case 4:
                //ULTIMO LUGAR
                m_Player1_RankingPanel.SetActive(true);
                m_NumPlayer1_Ranking.text = "4";
                m_Player1_Name.text = m_PlayerRanking[0].m_PlayerName;
                m_PlayerRanking[0].m_CurrentDice = 4;
                m_Player1_DiceImg.sprite = m_Dice4Icon;
                m_Player1_Img.sprite = m_PlayerRanking[0].m_PlayerIcon;

                switch (m_PlayerRanking[0].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player1_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player1_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player1_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player1_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //PRIMER LUGAR
                m_Player2_RankingPanel.SetActive(true);
                m_NumPlayer2_Ranking.text = "1";
                m_Player2_Name.text = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerName;
                m_PlayerRanking[m_PlayerRanking.Count - 1].m_CurrentDice = 8;
                m_Player2_DiceImg.sprite = m_Dice8Icon;
                m_Player2_Img.sprite = m_PlayerRanking[m_PlayerRanking.Count - 1].m_PlayerIcon;

                switch (m_PlayerRanking[m_PlayerRanking.Count - 1].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player2_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player2_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player2_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player2_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //MEDIO LUGAR
                m_Player3_RankingPanel.SetActive(true);
                m_NumPlayer3_Ranking.text = "2";
                m_Player3_Name.text = m_PlayerRanking[2].m_PlayerName;
                m_PlayerRanking[2].m_CurrentDice = 6;
                m_Player3_DiceImg.sprite = m_Dice6Icon;
                m_Player3_Img.sprite = m_PlayerRanking[2].m_PlayerIcon;

                switch (m_PlayerRanking[2].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player3_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player3_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player3_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player3_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }

                //MEDIO LUGAR 2
                m_Player4_RankingPanel.SetActive(true);
                m_NumPlayer4_Ranking.text = "3";
                m_Player4_Name.text = m_PlayerRanking[1].m_PlayerName;
                m_PlayerRanking[1].m_CurrentDice = 6;
                m_Player4_DiceImg.sprite = m_Dice6Icon;
                m_Player4_Img.sprite = m_PlayerRanking[1].m_PlayerIcon;

                switch (m_PlayerRanking[1].GetComponentInChildren<PlayerColor>().m_ModelColor)
                {
                    case "Blue":
                        m_Player4_BackgroundImg.sprite = m_Blue_Background;
                        break;

                    case "Purple":
                        m_Player4_BackgroundImg.sprite = m_Purple_Background;
                        break;

                    case "Green":
                        m_Player4_BackgroundImg.sprite = m_Green_Background;
                        break;

                    case "Yellow":
                        m_Player4_BackgroundImg.sprite = m_Yellow_Background;
                        break;
                }
                break;
        }
    }
    public void CheckIfShow_GoToBoardMenu()
    {
        if (icon_counter >= GameManager.instance.m_PlayerManager.m_PlayerList.Count && GameManager.instance.m_PlayerManager.m_PlayerList.Count >= 2)
        {
            EventSyst.instance.gameObject.SetActive(true);
            panel_GoToBoard.SetActive(true);
            SetSelectedButton_WhenMenusEnabled();
        }
    }

    #region Button_Functions
    public void PlayButton()
    {
        m_CharName_List.Clear();
        m_CharName_List.Add("GOBLIN");
        m_CharName_List.Add("WIZARD");
        m_CharName_List.Add("KNIGHT");
        m_CharName_List.Add("ORC");
    }
    public void Open_Options_ALL()
    {
        // DESACTIVAR
        panel_MainMenu.SetActive(false);

        // ACTIVAR
        panel_Opts_ALL.SetActive(true);
        panel_Opts_General.SetActive(true);
        panel_Opts_Controls.SetActive(false);
        panel_Opts_Video.SetActive(false);
        panel_Opts_Video.SetActive(false);
        panel_Opts_Audio.SetActive(false);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // CONTROL BUTTONS
    public void PS4_Mapping()
    {
        ControlManager aux_control_manag = JoinPlayers.instance.GetComponent<ControlManager>();
        aux_control_manag.SetMapping_PS4();
    }
    public void XBOX_Mapping()
    {
        ControlManager aux_control_manag = JoinPlayers.instance.GetComponent<ControlManager>();
        aux_control_manag.SetMapping_XBOX();
    }

    // VIDEO BUTTONS
    public void Resolution_SetInitial()
    {
        GameManager.instance.m_resolutionIndex = GameManager.instance.m_resol_list.Length - 1;

        Resolution aux_res = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex];
        Screen.SetResolution(aux_res.width, aux_res.height, Screen.fullScreen);

        string aux_res_text = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].width +
    " x " + GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].height;
        text_Resolution.GetComponent<TextMeshProUGUI>().SetText(aux_res_text);
    }
    public void Resolution_Plus()
    {
        // INCREASE
        GameManager.instance.m_resolutionIndex++;
        GameManager.instance.m_resolutionIndex = Mathf.Clamp(GameManager.instance.m_resolutionIndex,
            GameManager.instance.m_resol_Min, GameManager.instance.m_resol_Max);

        Resolution aux_res = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex];
        Screen.SetResolution(aux_res.width, aux_res.height, Screen.fullScreen);

        string aux_res_text = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].width +
            " x " + GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].height;
        text_Resolution.GetComponent<TextMeshProUGUI>().SetText(aux_res_text);
    }
    public void Resolution_Minus()
    {
        // DECREASE
        GameManager.instance.m_resolutionIndex--;
        GameManager.instance.m_resolutionIndex = Mathf.Clamp(GameManager.instance.m_resolutionIndex,
            GameManager.instance.m_resol_Min, GameManager.instance.m_resol_Max);

        Resolution aux_res = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex];
        Screen.SetResolution(aux_res.width, aux_res.height, Screen.fullScreen);

        string aux_res_text = GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].width +
            " x " + GameManager.instance.m_resol_list[GameManager.instance.m_resolutionIndex].height;
        text_Resolution.GetComponent<TextMeshProUGUI>().SetText(aux_res_text);
    }
    public void Quality_SetInitial()
    {
        GameManager.instance.m_qualityIndex = 2;
        QualitySettings.SetQualityLevel(GameManager.instance.m_qualityIndex);
        text_Quality.GetComponent<TextMeshProUGUI>().SetText(QualitySettings.names[GameManager.instance.m_qualityIndex]);
    }
    public void Quality_Plus()
    {
        GameManager.instance.m_qualityIndex++;
        GameManager.instance.m_qualityIndex = Mathf.Clamp(GameManager.instance.m_qualityIndex, GameManager.instance.m_qualit_Min, GameManager.instance.m_qualit_Max);
        QualitySettings.SetQualityLevel(GameManager.instance.m_qualityIndex);
        text_Quality.GetComponent<TextMeshProUGUI>().SetText(QualitySettings.names[GameManager.instance.m_qualityIndex]);
    }
    public void Quality_Minus()
    {
        GameManager.instance.m_qualityIndex--;
        GameManager.instance.m_qualityIndex = Mathf.Clamp(GameManager.instance.m_qualityIndex, GameManager.instance.m_qualit_Min, GameManager.instance.m_qualit_Max);
        QualitySettings.SetQualityLevel(GameManager.instance.m_qualityIndex);
        text_Quality.GetComponent<TextMeshProUGUI>().SetText(QualitySettings.names[GameManager.instance.m_qualityIndex]);
    }
    public void Screen_SetInitial()
    {
        Screen.fullScreen = true;
        text_ScreenMode.GetComponent<TextMeshProUGUI>().SetText("Fullscreen");
    }
    public void ScreenMode()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            text_ScreenMode.GetComponent<TextMeshProUGUI>().SetText("Windowed");
        }
        else if (!Screen.fullScreen)
        {
            Screen.fullScreen = true;
            text_ScreenMode.GetComponent<TextMeshProUGUI>().SetText("Fullscreen");
        }
    }

    // AUDIO BUTTONS
    public void Volume_SetInitialText()
    {
        // set general volume vol text
        float aux_GeneralVol = (int)(SoundManager.instance.OverallVolume_Master * 100);
        text_GeneralVol.GetComponent<TextMeshProUGUI>().SetText(aux_GeneralVol.ToString());
        // set music volume vol text
        float aux_MusicVol = (int)(SoundManager.instance.OverallVolume_Music * 100);
        text_MusicVol.GetComponent<TextMeshProUGUI>().SetText(aux_MusicVol.ToString());
        // set sfx volume vol text
        float aux_SfxVol = (int)(SoundManager.instance.OverallVolume_SFX * 100);
        text_SfxVol.GetComponent<TextMeshProUGUI>().SetText(aux_SfxVol.ToString());
    }
    public void SetVolText_General_Up()
    {
        SoundManager.instance.Up_GeneralVol(0.1f);
        float aux_GeneralVol = SoundManager.instance.OverallVolume_Master * 100.0f;
        text_GeneralVol.GetComponent<TextMeshProUGUI>().SetText(aux_GeneralVol.ToString());

        float aux_MusicVol = SoundManager.instance.OverallVolume_Music * 100.0f;
        text_MusicVol.GetComponent<TextMeshProUGUI>().SetText(aux_MusicVol.ToString());
        float aux_SfxVol = SoundManager.instance.OverallVolume_SFX * 100.0f;
        text_SfxVol.GetComponent<TextMeshProUGUI>().SetText(aux_SfxVol.ToString());
    }
    public void SetVolText_General_Down()
    {
        SoundManager.instance.Down_GeneralVol(0.1f);
        float aux_GeneralVol = SoundManager.instance.OverallVolume_Master * 100.0f;
        text_GeneralVol.GetComponent<TextMeshProUGUI>().SetText(aux_GeneralVol.ToString());

        float aux_MusicVol = SoundManager.instance.OverallVolume_Music * 100.0f;
        text_MusicVol.GetComponent<TextMeshProUGUI>().SetText(aux_MusicVol.ToString());
        float aux_SfxVol = SoundManager.instance.OverallVolume_SFX * 100.0f;
        text_SfxVol.GetComponent<TextMeshProUGUI>().SetText(aux_SfxVol.ToString());
    }
    public void SetVolText_Music_Up()
    {
        SoundManager.instance.Up_MusicVol(0.1f);
        float aux_MusicVol = SoundManager.instance.OverallVolume_Music * 100.0f;
        text_MusicVol.GetComponent<TextMeshProUGUI>().SetText(aux_MusicVol.ToString());
    }
    public void SetVolText_Music_Down()
    {
        SoundManager.instance.Down_MusicVol(0.1f);
        float aux_MusicVol = SoundManager.instance.OverallVolume_Music * 100.0f;
        text_MusicVol.GetComponent<TextMeshProUGUI>().SetText(aux_MusicVol.ToString());
    }
    public void SetVolText_SFX_Up()
    {
        SoundManager.instance.Up_SFXVol(0.1f);
        float aux_SfxVol = SoundManager.instance.OverallVolume_SFX * 100.0f;
        text_SfxVol.GetComponent<TextMeshProUGUI>().SetText(aux_SfxVol.ToString());
    }
    public void SetVolText_SFX_Down()
    {
        SoundManager.instance.Down_SFXVol(0.1f);
        float aux_SfxVol = SoundManager.instance.OverallVolume_SFX * 100.0f;
        text_SfxVol.GetComponent<TextMeshProUGUI>().SetText(aux_SfxVol.ToString());
    }

    // BUTTON SELECTED FUNCTIONS
    public void SetSelectedButton_WhenMenusEnabled()
    {
        // MAIN MENU UI
        if (panel_MainMenu.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_PLAY);
        }
        // OPTIONS MENU UI
        if (panel_Opts_General.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_CONTROLS);
        }
        // CONTROLS MENU UI
        if (panel_Opts_Controls.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_PS4);
        }
        // VIDEO MENU UI
        if (panel_Opts_Video.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_VIDEO_RESOLUTION_Minus);
        }
        // AUDIO MENU UI
        if (panel_Opts_Audio.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_AUDIO_GENERAL_VOL_Minus);
        }
        if (panel_ReturnToMainMenu.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_RETURN_TO_MAIN);
        }
        if (panel_GoToBoard.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_GO_TO_BOARD);
        }
        if (panel_BoardToMenu.activeSelf)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_Button_BOARD_TO_MAIN);
        }
    }
    public void SelectedButton_Update()
    {
        bool aux_selected = EventSyst.instance.GetComponent<EventSystem>().currentSelectedGameObject != null;
        if (!aux_selected)
        {
            EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(m_LastSelected);
        }
        else
        {
            m_LastSelected = EventSyst.instance.GetComponent<EventSystem>().currentSelectedGameObject;
        }
    }

    // PLAYER SWAP MENU FUNCTIONS
    public void OpenSwapMenu()
    {
        JoinPlayers.instance.gameObject.GetComponent<ControlManager>().SetMapping_SwapMenu
            (GameManager.instance.m_BoardManager.m_PlayerOrderList[GameManager.instance.m_BoardManager.m_CurrentTurn].
            gameObject.GetComponent<Player_OldSystem>()/*.playerID*/);

        panel_SwapMenu.SetActive(true);

        foreach (Player p in GameManager.instance.m_BoardManager.m_PlayerOrderList) // CREATE AS MUCH PANELS AS PLAYERS
        {
            GameObject p_Swap = Instantiate(panel_SwapPlayer, new Vector3(0, 0, 0), Quaternion.identity);
            p_Swap.transform.SetParent(GameObject.Find("Panel_PlayersPlaying").transform, false);
            m_SwapList.Add(p_Swap);
        }

        for (int i = 0; i < m_SwapList.Count; i++)
        {
            m_SwapList[i].GetComponentInChildren<TextMeshProUGUI>().SetText("PLAYER  " + (i + 1).ToString()); // SET NAME
            // SET COLOR AND IMAGE
            m_SwapList[i].GetComponentInParent<SwapButton>().SetPlayerInfo(m_SwapList[i].GetComponentInChildren<TextMeshProUGUI>(), i + 1);
        }

        //SET 1st BUTTON OF LIST TO BE CURRENT SELECTED
        GameObject aux_FirstButton = m_SwapList[0].GetComponentInChildren<Button>().gameObject;
        EventSyst.instance.GetComponent<EventSystem>().SetSelectedGameObject(aux_FirstButton);
    }
    public void SwapPlayerButton(GameObject Btn_Pnl)
    {
        #region ALL_BUTTONS
        if (Btn_Pnl.GetComponentInChildren<TextMeshProUGUI>().text == "PLAYER  1")
        {
            // Si no estaba seleccionado, seleccionarlo: Moverlo a la derecha + Añadir su correspondiente Player a la lista.
            // Si estaba seleccionado, deseleccionarlo.
            if (m_PlayersToBeSwapped.Contains(JoinPlayers.instance.playerList[0].gameObject))
            {
                m_PlayersToBeSwapped.Remove(JoinPlayers.instance.playerList[0].gameObject); // remove player from list
                Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(-10, 0, 0); // move player panel to left
            }
            else
            {
                if (m_PlayersToBeSwapped.Count < 2)
                {
                    m_PlayersToBeSwapped.Add(JoinPlayers.instance.playerList[0].gameObject); // add player to list
                    Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(10, 0, 0); // move player panel to right
                }
            }
        }
        else if (Btn_Pnl.GetComponentInChildren<TextMeshProUGUI>().text == "PLAYER  2")
        {
            if (m_PlayersToBeSwapped.Contains(JoinPlayers.instance.playerList[1].gameObject))
            {
                m_PlayersToBeSwapped.Remove(JoinPlayers.instance.playerList[1].gameObject); // remove player from list
                Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(-10, 0, 0); // move player panel to left
            }
            else
            {
                if (m_PlayersToBeSwapped.Count < 2)
                {
                    m_PlayersToBeSwapped.Add(JoinPlayers.instance.playerList[1].gameObject); // add player to list
                    Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(10, 0, 0); // move player panel to right
                }
            }
        }
        else if (Btn_Pnl.GetComponentInChildren<TextMeshProUGUI>().text == "PLAYER  3")
        {
            if (m_PlayersToBeSwapped.Contains(JoinPlayers.instance.playerList[2].gameObject))
            {
                m_PlayersToBeSwapped.Remove(JoinPlayers.instance.playerList[2].gameObject); // remove player from list
                Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(-10, 0, 0); // move player panel to left
            }
            else
            {
                if (m_PlayersToBeSwapped.Count < 2)
                {
                    m_PlayersToBeSwapped.Add(JoinPlayers.instance.playerList[2].gameObject); // add player to list
                    Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(10, 0, 0); // move player panel to right
                }
            }
        }
        else if (Btn_Pnl.GetComponentInChildren<TextMeshProUGUI>().text == "PLAYER  4")
        {
            if (m_PlayersToBeSwapped.Contains(JoinPlayers.instance.playerList[3].gameObject))
            {
                m_PlayersToBeSwapped.Remove(JoinPlayers.instance.playerList[3].gameObject); // remove player from list
                Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(-10, 0, 0); // move player panel to left
            }
            else
            {
                if (m_PlayersToBeSwapped.Count < 2)
                {
                    m_PlayersToBeSwapped.Add(JoinPlayers.instance.playerList[3].gameObject); // add player to list
                    Btn_Pnl.transform.localPosition = Btn_Pnl.transform.localPosition + new Vector3(10, 0, 0); // move player panel to right
                }
            }
        }
        #endregion
    }
    public void SwapButton()
    {
        // Send some kind of signal to player turn (casilla) in BoardManager
        if (m_PlayersToBeSwapped.Count >= 2)
        {
            panel_SwapMenu.SetActive(false);
            GameManager.instance.m_BoardManager.m_SwapMenu = false;
            JoinPlayers.instance.gameObject.GetComponent<ControlManager>().ResetInputs_SwapUI();
        }
    }

    // RETURN TO MAIN MENU BUTTONS
    public void Button_YES()
    {
        aux_axis = true;
        if (panel_CharSelect.activeSelf)
        {
            // First, disable everything from CharSelect. Then, destroy every instance.
            foreach (Transform item in Aux_SelectPanel.transform)
            {
                Destroy(item.gameObject);
            }

            // Second, make sure to clean the player list and set all players' parent to null
            // Also unload player models
            GameManager.instance.m_PlayerManager.m_PlayerList.Clear();
            var c_Count = JoinPlayers.instance.transform.childCount;

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


            // Third, disable and enable the rest
            EventSyst.instance.gameObject.SetActive(true);
            panel_CharSelect.SetActive(false);
            panel_MainMenu.SetActive(true);
        }
        if (GameManager.instance.GetCurrentScene() == GameManager.Scene.BoardScene)
        {
            // set panel to false
            panel_ReturnToMainMenu.SetActive(false);

            // COROUTINE TO GO BACK TO MAIN MENU
            StartCoroutine(ReturnMainMenu());
        }
        SetSelectedButton_WhenMenusEnabled();
    }
    public void Button_NO()
    {
        // Return to main menu, from char select menu
        if (panel_CharSelect.activeSelf)
        {
            // set return panel to false, as well as event system
            panel_ReturnToMainMenu.SetActive(false);
            EventSyst.instance.gameObject.SetActive(false);

            foreach (Transform item in Aux_SelectPanel.transform)
            {
                item.gameObject.GetComponent<PlayerSetupPanel_Controller>().ResetBool_Function();
            }
        }
    }

    // "GO TO BOARD GAME" BUTTONS
    public void Button_GO_TO_GAME()
    {
        // MENU THEME STOPS
        SoundManager.instance.Music_MainMenu.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        // SOUND PLAYS
        SoundManager.instance.Sound_StartGame();

        // change scene
        StartCoroutine(StartGame());
    }
    public void Button_NOT_GO_TO_GAME()
    {
        if (panel_GoToBoard.activeSelf)
        {
            ResetCharNameList();
            panel_GoToBoard.SetActive(false);
            EventSyst.instance.gameObject.SetActive(false);

            foreach (Transform plr in Aux_SelectPanel.transform)
            {
                PlayerSetupPanel_Controller aux_pnl = plr.GetComponent<PlayerSetupPanel_Controller>();
                aux_pnl.ResetAfterReturn();
                aux_pnl.ResetBool_Function();
            }
        }
    }
    #endregion

    private IEnumerator StartGame()
    {
        // assign icon to Player Script
        foreach (Transform plr in Aux_SelectPanel.transform)
        {
            PlayerSetupPanel_Controller aux_pnl = plr.GetComponent<PlayerSetupPanel_Controller>();
            aux_pnl.SetIcon_Final();
        }

        // set "go to board hud" to false
        panel_GoToBoard.SetActive(false);

        // set respective model to active(true)
        GameManager.instance.LoadModels();

        FadeController.instance.GetComponent<Canvas>().sortingOrder = 10;
        yield return StartCoroutine(FadeController.instance.Fade_In_Black(1.0f));
        FadeController.instance.Enable_TextLoading();
        yield return new WaitForSeconds(3f);
        GameManager.instance.ChangeScene(GameManager.Scene.BoardScene);
    }
    private IEnumerator ReturnMainMenu()
    {
        FadeController.instance.GetComponent<Canvas>().sortingOrder = 10;
        yield return StartCoroutine(FadeController.instance.Fade_In_Black(1.0f));
        FadeController.instance.Enable_TextLoading();
        yield return new WaitForSeconds(0.1f);
        panel_BoardToMenu.SetActive(false);
        panel_VictoryScreen.SetActive(false);
        GameManager.instance.UnloadModels();
        SoundManager.instance.Music_MainMenu.start();
        GameManager.instance.ChangeScene(GameManager.Scene.MultiplayerTest);
    }
    public void ResetCharNameList()
    {
        icon_counter = 0;
        m_CharName_List.Clear();
        m_CharName_List.Add("GOBLIN");
        m_CharName_List.Add("WIZARD");
        m_CharName_List.Add("KNIGHT");
        m_CharName_List.Add("ORC");
    }

    #region MINIGAME_FUNCTIONS
    public void ShowMinigame(int Game)
    {
        panel_ShowMinigame.SetActive(true);
        switch (Game)
        {
            case 0:
                card_GameName.sprite = GameNames[0];
                card_GameInstructions.sprite = GameInstructions[0];
                break;
            case 1:
                card_GameName.sprite = GameNames[1];
                card_GameInstructions.sprite = GameInstructions[1];
                break;
            case 2:
                card_GameName.sprite = GameNames[2];
                card_GameInstructions.sprite = GameInstructions[2];
                break;
        }
        FadeController.instance.ScaleUp_Image(card_GameName, 2.0f); // change to scale up
    }
    public void SetCardImages_EnterGame()
    {
        card_GameName.color = new Color(1, 1, 1, 0);
        card_GameInstructions.color = new Color(1, 1, 1, 0);
    }
    public void SetCardImages_EnterGame_FirstTime()
    {
        card_GameName.color = new Color(1, 1, 1, 0);
        card_GameInstructions.color = new Color(1, 1, 1, 0);
        FadeController.instance.FadeIn_FromTransparent(card_GameInstructions, 3f);
    }
    public void SetCard_2()
    {
        FadeController.instance.FadeOut_FromTransparent(card_GameInstructions, 3f);
    }
    public void ShowTimer_Begin(int Game)
    {
        int timerToPrint;
        switch (Game)
        {
            case 0:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_GridManager.GetComponent<GridManager>().m_Timer_Ranking);
                text_timer.SetText(timerToPrint.ToString());
                text_timer.color = Color.white;
                text_BEGIN.color = Color.white;
                if (!text_timer.gameObject.activeSelf && timerToPrint <= 3)
                {
                    text_timer.gameObject.SetActive(true);
                }
                else if (timerToPrint <= 0)
                {
                    canDiscount = false;
                    canShowBegin = true;
                }
                break;
            case 1:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_BrujaManager.GetComponent<BrujaManager>().m_Timer_Ranking);
                text_timer.SetText(timerToPrint.ToString());
                text_timer.color = Color.white;
                text_BEGIN.color = Color.white;
                if (!text_timer.gameObject.activeSelf && timerToPrint <= 3)
                {
                    text_timer.gameObject.SetActive(true);
                }
                else if (timerToPrint <= 0)
                {
                    canDiscount = false;
                    canShowBegin = true;
                }
                break;
            case 2:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_BallManager.GetComponent<BallGame>().m_Timer_Ranking);
                text_timer.SetText(timerToPrint.ToString());
                text_timer.color = Color.white;
                text_BEGIN.color = Color.white;
                if (!text_timer.gameObject.activeSelf && timerToPrint <= 3)
                {
                    text_timer.gameObject.SetActive(true);
                }
                else if (timerToPrint <= 0)
                {
                    canDiscount = false;
                    canShowBegin = true;
                }
                break;
        }
    }
    public void CheckTimer()
    {
        if (canDiscount)
        {
            ShowTimer_Begin(GameManager.instance.m_BoardManager.GetRandomMinigame());
        }
        else
        {
            if (text_timer.gameObject.activeSelf)
            {
                text_timer.gameObject.SetActive(false);
            }
            if (canShowBegin)
            {
                if (!text_BEGIN.gameObject.activeSelf)
                {
                    text_BEGIN.gameObject.SetActive(true);
                    Sound_BeginMinigame(); // play horn sound
                }
                if (beginText_timer <= 0f)
                {
                    canShowBegin = false;
                    beginText_timer = 1f;
                }
                else
                {
                    beginText_timer -= Time.deltaTime;
                }
            }
            else
            {
                if (text_BEGIN.gameObject.activeSelf)
                {
                    text_BEGIN.gameObject.SetActive(false);
                }
            }
        }
        if (panel_PlayerRanking.activeSelf)
        {
            ShowTimer_End(GameManager.instance.m_BoardManager.GetRandomMinigame());
        }
    }
    public void ShowTimer_End(int Game)
    {
        int timerToPrint;
        switch (Game)
        {
            case 0:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_GridManager.GetComponent<GridManager>().m_Timer_Ranking);
                text_RankingTimer.SetText(timerToPrint.ToString());
                break;
            case 1:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_BrujaManager.GetComponent<BrujaManager>().m_Timer_Ranking);
                text_RankingTimer.SetText(timerToPrint.ToString());
                break;
            case 2:
                timerToPrint = Convert.ToInt32(GameManager.instance.m_BallManager.GetComponent<BallGame>().m_Timer_Ranking);
                text_RankingTimer.SetText(timerToPrint.ToString());
                break;
        }
    }
    #endregion

    #region SOUND_FUNCTIONS
    public void Sound_PressButton() // OnSubmit
    {
        SoundManager.instance.PlaySound("event:/Menu/Seleccionar");
    }
    public void Sound_ChangeButton() // OnChangeSelected
    {
        SoundManager.instance.PlaySound("event:/Menu/Canviar seleccio");
    }
    public void Sound_BeginMinigame()
    {
        SoundManager.instance.Sound_BeginMinigame();
    }
    #endregion
}
