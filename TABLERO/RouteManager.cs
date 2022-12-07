using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [Header("MATERIALES CASILLAS")]
    public Material CASILLA_NULL;
    public Material CASILLA_AVANZA;
    public Material CASILLA_RETROCEDE;
    public Material CASILLA_REROLL;
    public Material CASILLA_DEATH;
    public Material CASILLA_RANDOMSWAP;
    public Material CASILLA_PARESNONES;

    List<Transform> m_NodeList = new List<Transform>();
    Transform[] m_ChildObjects;

    private void Start()
    {
        FillNodes();
    }

#if (UNITY_EDITOR)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        FillNodes();

        for (int i = 0; i < m_NodeList.Count; i++)
        {
            Vector3 l_CurrentNode = m_NodeList[i].position;
            if (i > 0)
            {
                Vector3 l_PrevNode = m_NodeList[i - 1].position;
                Gizmos.DrawLine(l_PrevNode, l_CurrentNode);
            }
        }
    }
#endif
    void FillNodes()
    {
        m_NodeList.Clear();
        m_ChildObjects = GetComponentsInChildren<Transform>();

        foreach (Transform child in m_ChildObjects)
        {
            if (child != this.transform)
            {
                m_NodeList.Add(child);
            }
        }
    }

    public List<Transform> GetNodeList()
    {
        return m_NodeList;
    }
}
