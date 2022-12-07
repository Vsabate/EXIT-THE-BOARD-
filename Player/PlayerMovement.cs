using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    RouteManager m_BoardRoute;
    GridManager m_GridRoute;

    [Header("BOOLS")]
    bool m_IsMoving;

    [Header("VARIABLES")]
    int m_BoardPos;
    Vector3 m_TransformPos;
    public int m_LastBoardPos;
    public Vector3 m_LastTransformBoardPos;
    public int m_DiceNumber;
    int m_ExtraDiceNumber = 0;
    public float m_MovementSpeed = 5f;

    GameObject m_WalkingParticle;

    private void Start()
    {
        m_BoardRoute = FindObjectOfType<RouteManager>();
        m_GridRoute = FindObjectOfType<GridManager>();
        m_WalkingParticle = Instantiate(Particles_Manager.instance.m_WalkingParticle, gameObject.transform);
        m_WalkingParticle.SetActive(false);
    }
    private void Update()
    {
        if (GameManager.instance.GetCurrentScene() == GameManager.Scene.BoardScene)
        {
            if (m_BoardRoute == null)
            {
                m_BoardRoute = FindObjectOfType<RouteManager>();
            }
        }
        if (GameManager.instance.GetCurrentScene() == GameManager.Scene.BoardScene)
        {
            if (m_GridRoute == null)
            {
                m_GridRoute = FindObjectOfType<GridManager>();
            }
        }

        m_TransformPos = transform.localPosition;
    }

    public IEnumerator BoardMove()
    {
        if (m_IsMoving) { yield break; }
        m_IsMoving = true;

        gameObject.GetComponentInChildren<Animator>().SetBool("IsWalking", true);
        m_WalkingParticle.transform.position = transform.position;
        m_WalkingParticle.SetActive(true);

        while (m_DiceNumber > 0)
        {
            Vector3 l_NextPos = m_BoardRoute.GetNodeList()[m_BoardPos + 1].position;
            transform.LookAt(l_NextPos);

            while (MoveToNextNode(l_NextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            m_DiceNumber--;
            m_BoardPos++;

        }

        while (m_DiceNumber < 0)
        {
            Vector3 l_NextPos = m_BoardRoute.GetNodeList()[m_BoardPos - 1].position;
            transform.LookAt(l_NextPos);

            while (MoveToNextNode(l_NextPos)) { yield return null; }

            yield return new WaitForSeconds(0.1f);
            m_DiceNumber++;
            m_BoardPos--;

        }
        gameObject.GetComponentInChildren<Animator>().SetBool("IsWalking", false);
        m_WalkingParticle.SetActive(false);
        m_IsMoving = false;
    }

    public IEnumerator ChangePlayerPosition(int NodePos)
    {
        // PLAY TELEPORT SOUND
        SoundManager.instance.Sound_Teleport();

        if (m_IsMoving) { yield break; }
        m_IsMoving = true;
        GetComponent<Player>().m_PlayerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 101;
        GameObject l_Teleport = Instantiate(Particles_Manager.instance.m_TeleportParticle, gameObject.transform);
        l_Teleport.transform.parent = null;
        yield return new WaitForSeconds(1f);
        GameObject l_Doll = GetComponentInChildren<PlayerColor>().gameObject;
        FadeController.instance.FadeIn_Black(1);
        l_Doll.SetActive(false);
        while (MoveToNextNode(m_BoardRoute.GetNodeList()[NodePos].position)) { yield return null; }
        l_Doll.SetActive(true);
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        FadeController.instance.FadeOut_Black(1);
        yield return new WaitForSeconds(1f);
        Destroy(l_Teleport);
        m_BoardPos = NodePos;
        GetComponent<Player>().m_PlayerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 0;
        m_IsMoving = false;
    }

    public bool MoveToNextNode(Vector3 goal)
    {
        // If goal has not been reach, return false
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, m_MovementSpeed * Time.deltaTime));
    }

    public int GetRoutePos()
    {
        return m_BoardPos;
    }
    public void SetRoutePos(int pos)
    {
        m_BoardPos = pos;
    }
    public Vector3 GetTransformPos()
    {
        return m_TransformPos;
    }
    public bool GetIsMoving()
    {
        return m_IsMoving;
    }
    public void SetIsMoving(bool _Bool)
    {
        m_IsMoving = _Bool;
    }

    public int GetDiceNumber()
    {
        return m_DiceNumber;
    }
    public void SetDiceNumber(int _Num)
    {
        m_DiceNumber = _Num;
    }

    public int GetExtraDiceNumber()
    {
        return m_ExtraDiceNumber;
    }
    public void SetExtraDiceNumber(int _Num)
    {
        m_ExtraDiceNumber = _Num;
    }

    public RouteManager GetRouteManager()
    {
        return m_BoardRoute;
    }

    public void SetLastNodePos()
    {
        m_BoardPos = m_LastBoardPos;
        transform.localPosition = m_LastTransformBoardPos;
    }
}
