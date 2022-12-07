using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionEnums;
using UnityEngine.EventSystems;


public class ControlManager : MonoBehaviour
{
    #region Controls
    /// //////////////////////////////////////////////////////////////////////////

    [Serializable]
    private class PlayerControl
    {
        public List<keyCode> keyCode_list;
        public KeyCode GetKeyCode(ControlKeys ControlKey)
        {
            foreach (keyCode k in keyCode_list)
            {
                if (k.controlKeys == ControlKey)
                {
                    return k.key;
                }
            }
            return KeyCode.None;
        }
    }

    [Serializable]
    public class keyCode
    {
        public ControlKeys controlKeys;
        public KeyCode key;
    }

    [SerializeField]
    List<PlayerControl> playerControls;

    // Player uses this to request KeyCode
    public KeyCode GetKey(int PlayerID, ControlKeys ControlKeys)
    {
        return playerControls[PlayerID].GetKeyCode(ControlKeys);
    }


    /// //////////////////////////////////////////////////////////////////////////
    #endregion

    private string inModule_H_Axis;
    private string inModule_Y_Axis;
    private string inModule_Submit;
    private string inModule_Cancel;

    [HideInInspector] public GameObject[] plr_list;

    private void Update()
    {
        plr_list = GameObject.FindGameObjectsWithTag("Player");
    }

    #region MAPPING_FUNCTIONS
    public void SetMapping_PS4()
    {
        for (int i = 0; i < playerControls.Count; i++)
        {
            if (i != 0)
            {
                for (int j = 0; j < playerControls[i].keyCode_list.Count; j++)
                {
                    if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.SelectKey)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button1;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button1;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button1;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button1;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.CancelKey)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button2;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button2;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button2;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button2;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.JoinGame)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button9;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button9;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button9;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button9;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Triangle)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button3;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button3;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button3;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button3;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Square)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button0;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button0;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button0;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button0;
                        }
                    }
                }
            }
        }

        UI_Manager.instance.m_KeyboardMando_WitchButtons[0] = UI_Manager.instance.m_WitchButtons[0];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[1] = UI_Manager.instance.m_WitchButtons[1];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[2] = UI_Manager.instance.m_WitchButtons[2];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[3] = UI_Manager.instance.m_WitchButtons[3];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[4] = UI_Manager.instance.m_WitchButtons[4];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[5] = UI_Manager.instance.m_WitchButtons[5];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[6] = UI_Manager.instance.m_WitchButtons[6];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[7] = UI_Manager.instance.m_WitchButtons[7];

        // CHANGE UI MAPPING:
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_PS4";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Cancel_PS4";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Horizontal_UI";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Vertical_UI";

        // CHANGE MOVEMENT AXIS MAPPING
        foreach (GameObject p in plr_list)
        {
            p.GetComponent<Player_OldSystem>().PS4_Controls = true;
        }
    }
    public void SetMapping_XBOX()
    {
        for (int i = 0; i < playerControls.Count; i++)
        {
            if (i != 0)
            {
                for (int j = 0; j < playerControls[i].keyCode_list.Count; j++)
                {
                    if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.SelectKey)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button0;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button0;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button0;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button0;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.CancelKey)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button1;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button1;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button1;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button1;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.JoinGame)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button7;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button7;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button7;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button7;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Triangle)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button3;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button3;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button3;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button3;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Square)
                    {
                        if (i == 1)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick1Button2;
                        }
                        else if (i == 2)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick2Button2;
                        }
                        else if (i == 3)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick3Button2;
                        }
                        else
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Joystick4Button2;
                        }
                    }
                }
            }
        }

        UI_Manager.instance.m_KeyboardMando_WitchButtons[0] = UI_Manager.instance.m_WitchButtons[8];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[1] = UI_Manager.instance.m_WitchButtons[9];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[2] = UI_Manager.instance.m_WitchButtons[10];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[3] = UI_Manager.instance.m_WitchButtons[11];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[4] = UI_Manager.instance.m_WitchButtons[4];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[5] = UI_Manager.instance.m_WitchButtons[5];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[6] = UI_Manager.instance.m_WitchButtons[6];
        UI_Manager.instance.m_KeyboardMando_WitchButtons[7] = UI_Manager.instance.m_WitchButtons[7];

        // CHANGE UI MAPPING:
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_XBOX";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Cancel_XBOX";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Horizontal_UI_XBOX";
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Vertical_UI_XBOX";

        // CHANGE MOVEMENT AXIS MAPPING
        foreach (GameObject p in plr_list)
        {
            p.GetComponent<Player_OldSystem>().PS4_Controls = false;
        }
    }

    public void SetMapping_WitchKeyboard()
    {
        for (int i = 0; i < playerControls.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < playerControls[i].keyCode_list.Count; j++)
                {
                    if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.SelectKey)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.S;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.CancelKey)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.D;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Triangle)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.W;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Square)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.A;
                        }
                    }
                }
            }
        }
    }
    public void SetMapping_NormalKeyboard()
    {
        for (int i = 0; i < playerControls.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < playerControls[i].keyCode_list.Count; j++)
                {
                    if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.SelectKey)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Return;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.CancelKey)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.Return;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Triangle)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.None;
                        }
                    }
                    else if (playerControls[i].keyCode_list[j].controlKeys == ControlKeys.Key_Square)
                    {
                        if (i == 0)
                        {
                            playerControls[i].keyCode_list[j].key = KeyCode.None;
                        }
                    }
                }
            }
        }
    }

    public void SetMapping_SwapMenu(Player_OldSystem plr)
    {
        // dar valor a variables inModule:
        inModule_H_Axis = EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis;
        inModule_Y_Axis = EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis;
        inModule_Submit = EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton;
        inModule_Cancel = EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton;

        // checkear qué inputs tiene puestos en el axis
        // Cambiar Inputs del eventsystem por los de este jugador
        if (plr.playerID == 0)
        {
            EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = /*"Submit_Keyboard"*/"Submit_PS4";
            EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Horizontal_Keyboard";
            EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Vertical_Keyboard";
            EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
        }
        else if (plr.playerID == 1)
        {
            // if ps4... else... sbox
            if (plr.PS4_Controls)
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_PS4_J1";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick1_Horizontal";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick1_Vertical";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
            else
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_XBOX_J1";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick1_Horizontal_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick1_Vertical_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
        }
        else if (plr.playerID == 2)
        {
            // if ps4... else... sbox
            if (plr.PS4_Controls)
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_PS4_J2";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick2_Horizontal";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick2_Vertical";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
            else
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_XBOX_J2";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick2_Horizontal_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick2_Vertical_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
        }
        else if (plr.playerID == 3)
        {
            // if ps4... else... sbox
            if (plr.PS4_Controls)
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_PS4_J3";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick3_Horizontal";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick3_Vertical";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
            else
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_XBOX_J3";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick3_Horizontal_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick3_Vertical_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
        }
        else if (plr.playerID == 4)
        {
            // if ps4... else... sbox
            if (plr.PS4_Controls)
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_PS4_J4";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick4_Horizontal";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick4_Vertical";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
            else
            {
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = "Submit_XBOX_J4";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = "Joystick4_Horizontal_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = "Joystick4_Vertical_XBOX";
                EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = "Useless_UI";
            }
        }
    }
    public void ResetInputs_SwapUI()
    {
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().submitButton = inModule_Submit;
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().cancelButton = inModule_Cancel;
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().horizontalAxis = inModule_H_Axis;
        EventSyst.instance.gameObject.GetComponent<StandaloneInputModule>().verticalAxis = inModule_Y_Axis;
    }
    #endregion
}
