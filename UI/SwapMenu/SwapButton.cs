using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwapButton : MonoBehaviour
{
    public Image icon;

    public void SumbitFunction()
    {
        UI_Manager.instance.SwapPlayerButton(gameObject);
    }

    public void SetPlayerInfo(TextMeshProUGUI Txt, int Index) // SET NAME, COLOR TEXT AND ICON
    {
        Color aux_txtColor = Color.white;
        switch (Index)
        {
            case 1:
                aux_txtColor = new Color(97f / 255f, 65f / 255f, 137f/255f); //D439D2 PURPLE
                icon.sprite = GameManager.instance.gameObject.GetComponent<PlayerManager>().m_PlayerList[0].GetComponent<Player>().m_PlayerIcon;
                break;
            case 2:
                aux_txtColor = new Color(0f, 170f/255f, 233f/255f);
                icon.sprite = GameManager.instance.gameObject.GetComponent<PlayerManager>().m_PlayerList[1].GetComponent<Player>().m_PlayerIcon;
                break;
            case 3:
                aux_txtColor = new Color(50f/255f, 135f / 255f, 60f / 255f);
                icon.sprite = GameManager.instance.gameObject.GetComponent<PlayerManager>().m_PlayerList[2].GetComponent<Player>().m_PlayerIcon;
                break;
            case 4:
                aux_txtColor = new Color(232f/255f,  221f/255f, 70f / 255f);
                icon.sprite = GameManager.instance.gameObject.GetComponent<PlayerManager>().m_PlayerList[3].GetComponent<Player>().m_PlayerIcon;
                break;
        }
        Txt.color = aux_txtColor;
    }

    public void Sound_Pressed()
    {
        SoundManager.instance.PlaySound("event:/SFX/Menú/Aceptar");
    }
    public void Sound_ChangeSelected()
    {
        SoundManager.instance.PlaySound("event:/SFX/Taulell/Moure cursor selecció");
    }
}
