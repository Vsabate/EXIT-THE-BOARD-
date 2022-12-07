using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using ActionEnums;


public class PlayerSetupPanel_Controller : MonoBehaviour
{
    #region VARIABLES
    [Header("PANEL INFO")]
    [SerializeField]
    private GameObject charSelect_Panel;
    public GameObject panel_CharacterName;
    public GameObject panel_ReadyPanel;

    [Space]
    [Header("STRINGS")]
    public GameObject playerName;
    public string characterName_Selected;

    [Space]
    [Header("ICONS")]
    public Sprite[] iconArray;
    public Image icon_Current;
    [SerializeField] private int icon_counter;
    private int plr_index;

    [Space]
    [Header("BACKGROUND CARDS")]
    public Image card_background;
    public Sprite[] cardArray;

    [Space]
    [Header("INPUT VARS")]
    private Player_OldSystem myInputs;
    [SerializeField] private bool verticalInput_Enabled = false;
    [HideInInspector] public bool selecting_Character = false;
    #endregion

    private void Awake()
    {
        panel_ReadyPanel.SetActive(false);

        iconArray = new Sprite[4];

        verticalInput_Enabled = true;
        selecting_Character = true;

        icon_counter = 0;
    }

    private void Update()
    {
        Icon_CheckInput();
    }

    #region SetupPanel_Functions
    public void SetPlayerName(int Index, Player_OldSystem Input)
    {
        playerName.GetComponent<TextMeshProUGUI>().SetText("PLAYER  " + Index.ToString());
        SetColorTextPlayer(playerName.GetComponent<TextMeshProUGUI>(), Index);
        myInputs = Input;    
    }
    public void Icon_Add(int Index)
    {
        #region ADD_COLOR_ICONS
        switch (Index) // Add respective icons and set current
        {
            case 1:
                int a = 0;
                for (int i = 0; i <= 3; i++)
                {
                    iconArray[a] = UI_Manager.instance.m_IconArray[i];
                    a++;
                }
                icon_Current.sprite = iconArray[0];
                break;

            case 2:
                int b = 0;
                for (int i = 4; i <= 7; i++)
                {
                    iconArray[b] = UI_Manager.instance.m_IconArray[i];
                    b++;
                }
                icon_Current.sprite = iconArray[0];
                break;

            case 3:
                int c = 0;
                for (int i = 8; i <= 11; i++)
                {
                    iconArray[c] = UI_Manager.instance.m_IconArray[i];
                    c++;
                }
                icon_Current.sprite = iconArray[0];
                break;

            case 4:
                int d = 0;
                for (int i = 12; i <= 15; i++)
                {
                    iconArray[d] = UI_Manager.instance.m_IconArray[i];
                    d++;
                }
                icon_Current.sprite = iconArray[0];
                break;
        }
        #endregion
        plr_index = Index;

        Card_Add();
    }
    public void Card_Add()
    {
        switch (plr_index)
        {
            case 1:
                card_background.sprite = cardArray[0];
                break;
            case 2:
                card_background.sprite = cardArray[1];
                break;
            case 3:
                card_background.sprite = cardArray[2];
                break;
            case 4:
                card_background.sprite = cardArray[3];
                break;
        }
    }
    public void Icon_CheckInput()
    {
        if (gameObject.activeSelf)
        {
            if (selecting_Character)
            {
                if (verticalInput_Enabled)
                {
                    if (myInputs.player_Vertical > 0)
                    {
                        Icon_MoveUp();
                    }
                    if (myInputs.player_Vertical < 0)
                    {
                        Icon_MoveDown();
                    }
                }
                else
                {
                    if (myInputs.player_Vertical == 0)
                    {
                        verticalInput_Enabled = true;
                    }
                }

                if (Input.GetKeyDown(myInputs.controlManager.GetKey(myInputs.playerID, ControlKeys.SelectKey)))
                {
                    // if char available, select it -> Black screen + READY!
                    // if not available, nothing happens
                    SetPlayerChar();
                }
            }
            if (Input.GetKeyDown(myInputs.controlManager.GetKey(myInputs.playerID, ControlKeys.CancelKey)))
            {
                // if has char selected, deselect
                DeselectPlayerChar();
            }
        }
    }
    public void Icon_MoveUp()
    {
        // PLAY SOUND: MOVE THROUGH CHARS
        UI_Manager.instance.Sound_ChangeButton();

        if (icon_counter >= 3)
        {
            icon_counter = 0;
        }
        else
        {
            icon_counter++;
        }
        Icon_SetCurrent(icon_counter);
        verticalInput_Enabled = false;
    }
    public void Icon_MoveDown()
    {
        // PLAY SOUND: MOVE THROUGH CHARS
        UI_Manager.instance.Sound_ChangeButton();

        if (icon_counter <= 0)
        {
            icon_counter = 3;
        }
        else
        {
            icon_counter--;
        }
        Icon_SetCurrent(icon_counter);
        verticalInput_Enabled = false;
    }
    public void Icon_SetCurrent(int Counter)
    {
        switch (Counter)
        {
            case 0:
                icon_Current.sprite = iconArray[0];
                panel_CharacterName.GetComponentInChildren<TextMeshProUGUI>().SetText("GOBLIN");
                break;
            case 1:
                icon_Current.sprite = iconArray[1];
                panel_CharacterName.GetComponentInChildren<TextMeshProUGUI>().SetText("WIZARD");
                break;
            case 2:
                icon_Current.sprite = iconArray[2];
                panel_CharacterName.GetComponentInChildren<TextMeshProUGUI>().SetText("KNIGHT");
                break;
            case 3:
                icon_Current.sprite = iconArray[3];
                panel_CharacterName.GetComponentInChildren<TextMeshProUGUI>().SetText("ORC");
                break;
        }
    }
    public void SetPlayerChar()
    {
        switch (icon_counter)
        {
            case 0:
                if (UI_Manager.instance.m_CharName_List.Contains("GOBLIN"))
                {
                    // PLAY SOUND: "CHAR SELECTED"
                    UI_Manager.instance.Sound_PressButton();

                    if (!UI_Manager.instance.m_CharName_List.Contains(characterName_Selected) && characterName_Selected != null)
                    {
                        UI_Manager.instance.m_CharName_List.Add(characterName_Selected);
                    }
                    characterName_Selected = "GOBLIN";
                    UI_Manager.instance.m_CharName_List.Remove("GOBLIN");

                    panel_ReadyPanel.SetActive(true);
                    selecting_Character = false;
                    // if every player has picked a character, set "go to board" menu to true
                    UI_Manager.instance.icon_counter++;
                    UI_Manager.instance.icon_counter = Mathf.Clamp(UI_Manager.instance.icon_counter, 0, GameManager.instance.m_PlayerManager.m_PlayerList.Count);
                }
                else
                {
                    // PLAY SOUND: "Can't select this character :("
                    SoundManager.instance.PlaySound("event:/Menu/Denegar");
                }
                break;

            case 1:
                if (UI_Manager.instance.m_CharName_List.Contains("WIZARD"))
                {
                    // PLAY SOUND: "CHAR SELECTED"
                    UI_Manager.instance.Sound_PressButton();

                    if (!UI_Manager.instance.m_CharName_List.Contains(characterName_Selected) && characterName_Selected != null)
                    {
                        UI_Manager.instance.m_CharName_List.Add(characterName_Selected);
                    }
                    characterName_Selected = "WIZARD";
                    UI_Manager.instance.m_CharName_List.Remove("WIZARD");

                    panel_ReadyPanel.SetActive(true);
                    selecting_Character = false;
                    // if every player has picked a character, set "go to board" menu to true
                    UI_Manager.instance.icon_counter++;
                    UI_Manager.instance.icon_counter = Mathf.Clamp(UI_Manager.instance.icon_counter, 0, GameManager.instance.m_PlayerManager.m_PlayerList.Count);
                }
                else
                {
                    // PLAY SOUND: "Can't select this character :("
                    SoundManager.instance.PlaySound("event:/Menu/Denegar");
                }
                break;

            case 2:
                if (UI_Manager.instance.m_CharName_List.Contains("KNIGHT"))
                {
                    // PLAY SOUND: "CHAR SELECTED"
                    UI_Manager.instance.Sound_PressButton();

                    if (!UI_Manager.instance.m_CharName_List.Contains(characterName_Selected) && characterName_Selected != null)
                    {
                        UI_Manager.instance.m_CharName_List.Add(characterName_Selected);
                    }
                    characterName_Selected = "KNIGHT";
                    UI_Manager.instance.m_CharName_List.Remove("KNIGHT");

                    panel_ReadyPanel.SetActive(true);
                    selecting_Character = false;
                    // if every player has picked a character, set "go to board" menu to true
                    UI_Manager.instance.icon_counter++;
                    UI_Manager.instance.icon_counter = Mathf.Clamp(UI_Manager.instance.icon_counter, 0, GameManager.instance.m_PlayerManager.m_PlayerList.Count);
                }
                else
                {
                    // PLAY SOUND: "Can't select this character :("
                    SoundManager.instance.PlaySound("event:/Menu/Denegar");
                }
                break;

            case 3:
                if (UI_Manager.instance.m_CharName_List.Contains("ORC"))
                {
                    // PLAY SOUND: "CHAR SELECTED"
                    UI_Manager.instance.Sound_PressButton();

                    if (!UI_Manager.instance.m_CharName_List.Contains(characterName_Selected) && characterName_Selected != null)
                    {
                        UI_Manager.instance.m_CharName_List.Add(characterName_Selected);
                    }
                    characterName_Selected = "ORC";
                    UI_Manager.instance.m_CharName_List.Remove("ORC");

                    panel_ReadyPanel.SetActive(true);
                    selecting_Character = false;
                    // if every player has picked a character, set "go to board" menu to true
                    UI_Manager.instance.icon_counter++;
                    UI_Manager.instance.icon_counter = Mathf.Clamp(UI_Manager.instance.icon_counter, 0, GameManager.instance.m_PlayerManager.m_PlayerList.Count);
                }
                else
                {
                    // PLAY SOUND: "Can't select this character :("
                    SoundManager.instance.PlaySound("event:/Menu/Denegar");
                }
                break;
        }
        UI_Manager.instance.CheckIfShow_GoToBoardMenu();

    }
    public void DeselectPlayerChar()
    {
        // deselect
        if (!UI_Manager.instance.m_CharName_List.Contains(characterName_Selected) &&
            (characterName_Selected == "GOBLIN" || characterName_Selected == "WIZARD" || characterName_Selected == "KNIGHT" || characterName_Selected == "ORC"))
        {
            UI_Manager.instance.m_CharName_List.Add(characterName_Selected);
            characterName_Selected = null;
            panel_ReadyPanel.SetActive(false);
            selecting_Character = true;
        }        
        else if (characterName_Selected != "GOBLIN" || characterName_Selected != "WIZARD" || characterName_Selected != "KNIGHT" || characterName_Selected != "ORC")
        {
            selecting_Character = false;
            panel_ReadyPanel.SetActive(false);
            // SET "RETURN TO MAIN MANU PANEL ACTIVE"
            EventSyst.instance.gameObject.SetActive(true);
            UI_Manager.instance.panel_ReturnToMainMenu.SetActive(true);
            UI_Manager.instance.SetSelectedButton_WhenMenusEnabled();
        }
        UI_Manager.instance.icon_counter--;
        UI_Manager.instance.icon_counter = Mathf.Clamp(UI_Manager.instance.icon_counter, 0, GameManager.instance.m_PlayerManager.m_PlayerList.Count);
    }
    public void ResetAfterReturn()
    {
        panel_ReadyPanel.SetActive(false);
        characterName_Selected = null;
    }
    public void SetColorTextPlayer(TextMeshProUGUI Txt, int Index)
    {
        Color aux_txtColor = Color.white;
        Txt.color = aux_txtColor;
    }
    public void ResetBool_Function()
    {
        StartCoroutine(ResetBool());
    }
    public void SetIcon_Final()
    {
        foreach (GameObject player in GameManager.instance.m_PlayerManager.m_PlayerList)
        {
            if (plr_index == player.GetComponent<Player_OldSystem>().plr_index)
            {
                player.GetComponent<Player>().m_PlayerIcon = icon_Current.sprite;
            }

            if (plr_index == 0)
            {
                player.GetComponent<Player_OldSystem>().myColor = "Purple";
            }
            else if (plr_index == 1)
            {
                player.GetComponent<Player_OldSystem>().myColor = "Blue";
            }
            else if (plr_index == 2)
            {
                player.GetComponent<Player_OldSystem>().myColor = "Green";
            }
            else if (plr_index == 3)
            {
                player.GetComponent<Player_OldSystem>().myColor = "Yellow";
            }
        }
    }
    #endregion

    #region COROUTINES
    public IEnumerator ResetBool()
    {
        yield return new WaitForSeconds(0.1f);
        selecting_Character = true;
    }
    #endregion
}
