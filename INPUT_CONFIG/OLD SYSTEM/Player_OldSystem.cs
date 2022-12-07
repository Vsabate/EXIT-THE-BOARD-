using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionEnums;


public class Player_OldSystem : MonoBehaviour
{
    #region VARIABLES
    [Header("PLAYER")]
    public int playerID;
    [HideInInspector] public int plr_index;
    [HideInInspector]
    public ControlManager controlManager;
    public GameObject playerSetup_Prefab;
    private int maxPlayers = 4;

    [Space]
    [Header("BOARD GAME")]
    [HideInInspector] public bool playing = false;
    [HideInInspector] public bool myDiceTurn = false;
    [HideInInspector] public bool canThrowDice = false;

    [Space]
    [Header("FALL MINIGAME")]
    [HideInInspector] public bool minigame_Playing_FallGame = false;
    private float player_speed = 3.0f;
    private Vector3 player_movement;
    [HideInInspector] public float player_Horizontal;
    [HideInInspector] public float player_Vertical;
    public bool PS4_Controls;
    [HideInInspector] public float X_Axis;
    [HideInInspector] public float Y_Axis;

    [Space]
    [Header("WITCH MINIGAME")]
    [HideInInspector] public bool minigame_Playing_WitchGame = false;
    [HideInInspector] public bool minigame_Witch_MyTurn = false;

    [Space]
    [Header("BATTLE MINIGAME")]
    [HideInInspector] public bool minigame_Playing_BallGame = false;

    public string myColor;
    #endregion

    private void Start()
    {
        controlManager = FindObjectOfType<ControlManager>();
        PS4_Controls = true;
    }

    private void Update()
    {
        // MOVEMENT STUFF
        switch (playerID)
        {
            case 0:
                player_Horizontal = Input.GetAxisRaw("Horizontal");
                player_Vertical = Input.GetAxisRaw("Vertical");
                break;
            case 1:
                if (PS4_Controls)
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick1_Horizontal");
                    player_Vertical = Input.GetAxisRaw("Joystick1_Vertical");
                }
                else
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick1_Horizontal_XBOX");
                    player_Vertical = Input.GetAxisRaw("Joystick1_Vertical_XBOX");
                }
                break;
            case 2:
                if (PS4_Controls)
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick2_Horizontal");
                    player_Vertical = Input.GetAxisRaw("Joystick2_Vertical");
                }
                else
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick2_Horizontal_XBOX");
                    player_Vertical = Input.GetAxisRaw("Joystick2_Vertical_XBOX");
                }
                break;
            case 3:
                if (PS4_Controls)
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick3_Horizontal");
                    player_Vertical = Input.GetAxisRaw("Joystick3_Vertical");
                }
                else
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick3_Horizontal_XBOX");
                    player_Vertical = Input.GetAxisRaw("Joystick3_Vertical_XBOX");
                }
                break;
            case 4:
                if (PS4_Controls)
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick4_Horizontal");
                    player_Vertical = Input.GetAxisRaw("Joystick4_Vertical");
                }
                else
                {
                    player_Horizontal = Input.GetAxisRaw("Joystick4_Horizontal_XBOX");
                    player_Vertical = Input.GetAxisRaw("Joystick4_Vertical_XBOX");
                }
                break;
        }
        player_movement = new Vector3(player_Horizontal, 0f, player_Vertical);
        CheckActions();
    }

    public void CheckActions()
    {
        #region Inputs
        if (playing && minigame_Playing_FallGame)
        {
            // PLAYER MOVEMENT WITH INPUTS
            if (player_movement.magnitude > 0)
            {
                player_movement.Normalize();
                player_movement *= player_speed * Time.deltaTime;
                gameObject.GetComponent<CharacterController>().Move(player_movement);
                transform.forward = player_movement;

                gameObject.GetComponentInChildren<Animator>().SetBool("IsWalking", true);
            }
            else
            {
                gameObject.GetComponentInChildren<Animator>().SetBool("IsWalking", false);
            }
        }
        if (playing && minigame_Playing_BallGame)
        {
            // PLAYER MOVEMENT WITH INPUTS
            if (player_movement.magnitude > 0)
            {
                player_movement.Normalize();
                player_movement *= player_speed * Time.deltaTime;
                gameObject.GetComponent<CharacterController>().Move(player_movement);
                transform.forward = player_movement;

                gameObject.GetComponentInChildren<Animator>().SetBool("IsBall", true);
            }
            else
            {
                gameObject.GetComponentInChildren<Animator>().SetBool("IsBall", false);
            }
        }
        if (Input.GetKeyDown(controlManager.GetKey(playerID, ControlKeys.SelectKey)))
        {
            // BOARD MOVEMENT
            if (playing && myDiceTurn)
            {
                canThrowDice = true;
            }
            else
            {
                canThrowDice = false;
            }

            // WITCH MINIGAME
            if (playing && minigame_Playing_WitchGame && minigame_Witch_MyTurn)
            {
                GameManager.instance.m_BrujaManager.m_PlayerInputs.Add(3);
                GameManager.instance.m_BrujaManager.CheckKeyDown();
            }
        }
        if (Input.GetKeyDown(controlManager.GetKey(playerID, ControlKeys.JoinGame)))
        {
            // THIS SHOULD BE ONLY POSIBLE IN SETUP MENU SCENE 
            // Player already in game joins the PlayerList.
            if (UI_Manager.instance.panel_CharSelect.activeSelf)
            {
                if (JoinPlayers.instance.playerList.Count < maxPlayers)
                {
                    if (!controlManager.gameObject.GetComponent<JoinPlayers>().playerList.Contains(gameObject))
                    {
                        JoinPlayers.instance.id_list.Add(playerID);
                        gameObject.transform.SetParent(JoinPlayers.instance.transform);
                        JoinPlayers.instance.playerList.Add(gameObject);

                        gameObject.transform.tag = JoinPlayers.instance.current_tagList[0];
                        JoinPlayers.instance.current_tagList.Remove(JoinPlayers.instance.current_tagList[0]);
                        gameObject.GetComponentInChildren<MeshRenderer>().material = JoinPlayers.instance.current_MaterialList[0];
                        JoinPlayers.instance.current_MaterialList.Remove(JoinPlayers.instance.current_MaterialList[0]);

                        GameManager.instance.GetComponent<PlayerManager>().SetNumberOfPlayers(JoinPlayers.instance.playerList.Count);
                        int aux_index = JoinPlayers.instance.playerList.Count;
                        PlayerManager.instance.m_PlayerList.Add(gameObject);

                        playing = true;

                        Spawn_PlayerSetup(aux_index);

                        gameObject.GetComponent<Player>().m_PlayerName = "PLAYER " + aux_index;
                        gameObject.GetComponentInChildren<BoxCollider>().enabled = false;

                        // INSTANTIATE ALL PLAYER MODELS and MAKE PLAYER THEIR PARENT:
                        // if index = 0, instantiate all purple models (goblin, knight, wizard, orc)
                        Spawn_PlayerModels(aux_index);
                        plr_index = aux_index;
                    }
                }
            }

        }
        if (Input.GetKeyDown(controlManager.GetKey(playerID, ControlKeys.CancelKey)))
        {
            if (playing && minigame_Playing_WitchGame && minigame_Witch_MyTurn)
            {
                GameManager.instance.m_BrujaManager.m_PlayerInputs.Add(2);
                GameManager.instance.m_BrujaManager.CheckKeyDown();
            }

            // CLOSE - OPEN UI MENUS (FALTA EL DE GAMEPLAY)
            if (GameManager.instance.GetCurrentScene() == GameManager.Scene.MultiplayerTest)
            {
                if (UI_Manager.instance.panel_Opts_General.activeSelf)
                {
                    UI_Manager.instance.panel_Opts_General.SetActive(false);
                    UI_Manager.instance.panel_MainMenu.SetActive(true);
                    UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
                }
                if (UI_Manager.instance.panel_Opts_Controls.activeSelf)
                {
                    UI_Manager.instance.panel_Opts_Controls.SetActive(false);
                    UI_Manager.instance.panel_Opts_General.SetActive(true);
                    UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
                }
                if (UI_Manager.instance.panel_Opts_Video.activeSelf)
                {
                    UI_Manager.instance.panel_Opts_Video.SetActive(false);
                    UI_Manager.instance.panel_Opts_General.SetActive(true);
                    UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
                }
                if (UI_Manager.instance.panel_Opts_Audio.activeSelf)
                {
                    UI_Manager.instance.panel_Opts_Audio.SetActive(false);
                    UI_Manager.instance.panel_Opts_General.SetActive(true);
                    UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
                }
                if (UI_Manager.instance.panel_CharSelect.activeSelf && UI_Manager.instance.panel_GoToBoard.activeSelf)
                {
                    UI_Manager.instance.panel_GoToBoard.SetActive(false);
                }
            }
        }
        if (Input.GetKeyDown(controlManager.GetKey(playerID, ControlKeys.Key_Triangle)))
        {
            if (playing && minigame_Playing_WitchGame && minigame_Witch_MyTurn)
            {
                GameManager.instance.m_BrujaManager.m_PlayerInputs.Add(1);
                GameManager.instance.m_BrujaManager.CheckKeyDown();
            }
        }
        if (Input.GetKeyDown(controlManager.GetKey(playerID, ControlKeys.Key_Square)))
        {
            if (playing && minigame_Playing_WitchGame && minigame_Witch_MyTurn)
            {
                GameManager.instance.m_BrujaManager.m_PlayerInputs.Add(0);
                GameManager.instance.m_BrujaManager.CheckKeyDown();
            }
        }
        #endregion
    }

    public void Spawn_PlayerSetup(int Index)
    {
        var rootMenu = GameObject.Find("Canvas_MAIN");
        if (rootMenu != null)
        {
            // Crear en panel que tenga horizontal layout
            foreach (Transform child in rootMenu.transform)
            {
                if (child.name == "Panel_CharSelection")
                {
                    foreach (Transform c in child.transform)
                    {
                        if (c.name == "Panel_CharSelect")
                        {
                            var menu = Instantiate(playerSetup_Prefab, c.transform);
                            menu.GetComponent<PlayerSetupPanel_Controller>().SetPlayerName(Index, this); // también pasar referencia este script
                            menu.GetComponent<PlayerSetupPanel_Controller>().Icon_Add(Index);
                        }
                    }
                }
            }
        }
    }
    public void Spawn_PlayerModels(int Index)
    {
        switch (Index)
        {
            case 1:
                GameObject m1_1 = Instantiate(JoinPlayers.instance.model_List[0], transform);
                GameObject m1_2 = Instantiate(JoinPlayers.instance.model_List[4], transform);
                GameObject m1_3 = Instantiate(JoinPlayers.instance.model_List[8], transform);
                GameObject m1_4 = Instantiate(JoinPlayers.instance.model_List[12], transform);

                m1_1.transform.localPosition = new Vector3(0, 0, 0);
                m1_2.transform.localPosition = new Vector3(0, 0, 0);
                m1_3.transform.localPosition = new Vector3(0, 0, 0);
                m1_4.transform.localPosition = new Vector3(0, 0, 0);

                m1_1.SetActive(false);
                m1_2.SetActive(false);
                m1_3.SetActive(false);
                m1_4.SetActive(false);
                break;

            case 2:
                GameObject m2_1 = Instantiate(JoinPlayers.instance.model_List[1], transform);
                GameObject m2_2 = Instantiate(JoinPlayers.instance.model_List[5], transform);
                GameObject m2_3 = Instantiate(JoinPlayers.instance.model_List[9], transform);
                GameObject m2_4 = Instantiate(JoinPlayers.instance.model_List[13], transform);

                m2_1.transform.localPosition = new Vector3(0, 0, 0);
                m2_2.transform.localPosition = new Vector3(0, 0, 0);
                m2_3.transform.localPosition = new Vector3(0, 0, 0);
                m2_4.transform.localPosition = new Vector3(0, 0, 0);

                m2_1.SetActive(false);
                m2_2.SetActive(false);
                m2_3.SetActive(false);
                m2_4.SetActive(false);
                break;

            case 3:
                GameObject m3_1 = Instantiate(JoinPlayers.instance.model_List[2], transform);
                GameObject m3_2 = Instantiate(JoinPlayers.instance.model_List[6], transform);
                GameObject m3_3 = Instantiate(JoinPlayers.instance.model_List[10], transform);
                GameObject m3_4 = Instantiate(JoinPlayers.instance.model_List[14], transform);

                m3_1.transform.localPosition = new Vector3(0, 0, 0);
                m3_2.transform.localPosition = new Vector3(0, 0, 0);
                m3_3.transform.localPosition = new Vector3(0, 0, 0);
                m3_4.transform.localPosition = new Vector3(0, 0, 0);

                m3_1.SetActive(false);
                m3_2.SetActive(false);
                m3_3.SetActive(false);
                m3_4.SetActive(false);
                break;

            case 4:
                GameObject m4_1 = Instantiate(JoinPlayers.instance.model_List[3], transform);
                GameObject m4_2 = Instantiate(JoinPlayers.instance.model_List[7], transform);
                GameObject m4_3 = Instantiate(JoinPlayers.instance.model_List[11], transform);
                GameObject m4_4 = Instantiate(JoinPlayers.instance.model_List[15], transform);

                m4_1.transform.localPosition = new Vector3(0, 0, 0);
                m4_2.transform.localPosition = new Vector3(0, 0, 0);
                m4_3.transform.localPosition = new Vector3(0, 0, 0);
                m4_4.transform.localPosition = new Vector3(0, 0, 0);

                m4_1.SetActive(false);
                m4_2.SetActive(false);
                m4_3.SetActive(false);
                m4_4.SetActive(false);
                break;
        }
    }
}
