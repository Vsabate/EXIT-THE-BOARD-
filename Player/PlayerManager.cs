using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

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

    int m_NumberOfPlayers;
    public List<GameObject> m_PlayerList;

    public int GetNumberOfPlayers()
    {
        return m_PlayerList.Count;
    }
    public void SetNumberOfPlayers(int nPlayers)
    {
        m_NumberOfPlayers = nPlayers;
    }
}
