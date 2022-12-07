using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [Header("VARIABLES")]
    Rigidbody m_Rb;
    DiceSide[] m_DiceSides;
    Vector3 m_InitPos;
    int m_DiceNumber;
    float m_MinDiceTorque = 1400f;
    float m_MaxDiceTorque = 1800f;

    [Space]
    [Header("BOOLS")]
    public bool m_HasLanded;
    bool m_Thrown;
    bool m_ReadyToThrow;
    public bool m_Die4;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody>();
        m_DiceSides = GetComponentsInChildren<DiceSide>();
        m_InitPos = transform.position;
        m_Rb.useGravity = false;
        m_Rb.isKinematic = false;
        m_ReadyToThrow = true;
    }

    private void Update()
    {
        // Si el dado ha sido tirado y ya esta quieto en el suelo
        if (m_Rb.IsSleeping() && !m_HasLanded && m_Thrown)
        {
            m_HasLanded = true;
            m_Rb.useGravity = false;
            m_Rb.isKinematic = true;
            m_ReadyToThrow = false;
            SideValueCheck();

        }// Si el dado cae mal se hace un re-roll
        else if (m_Rb.IsSleeping() && m_HasLanded && m_DiceNumber == 0)
        {
            RollAgain();
        }
    }

    public void RollDice()
    {
        if (!m_Thrown && !m_HasLanded)
        {
            ThrowDice();
        }
        else if (m_Thrown && m_HasLanded)
        {
            ResetDice();
        }
    }
    public void RollAgain()
    {
        ResetDice();
        ThrowDice();
    }
    void ThrowDice()
    {
        m_Thrown = true;
        m_Rb.useGravity = true;
        m_Rb.AddTorque(
            Random.Range(m_MinDiceTorque, m_MaxDiceTorque),
            Random.Range(m_MinDiceTorque, m_MaxDiceTorque),
            Random.Range(m_MinDiceTorque, m_MaxDiceTorque)
            );
    }
    void SideValueCheck()
    {
        // Se comprueba que cara está tocando el suelo y se asigna el valor SideValue que tiene como DiceNumber
        m_DiceNumber = 0;

        if (m_Die4)
        {
            foreach (DiceSide side in m_DiceSides)
            {
                if (!side.IsOnGround())
                {
                    m_DiceNumber = side.m_SideValue;
                }
            }
        }
        else
        {
            foreach (DiceSide side in m_DiceSides)
            {
                if (side.IsOnGround())
                {
                    m_DiceNumber = side.m_SideValue;
                }
            }
        }

    }
    public void ResetDice()
    {
        transform.position = m_InitPos;
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        m_Thrown = false;
        m_ReadyToThrow = true;
        m_HasLanded = false;
        m_Rb.useGravity = false;
        m_Rb.isKinematic = false;
        foreach (DiceSide side in m_DiceSides)
        {
            side.ResetOnGround();
        }
    }
    public int GetDiceNumber()
    {
        return m_DiceNumber;
    }
    public bool IsReadyToThrow()
    {
        return m_ReadyToThrow;
    }
    public bool DiceThrown()
    {
        return m_Thrown;
    }
}
