using UnityEngine;

public class DiceSide : MonoBehaviour
{
    bool m_OnGround;
    public int m_SideValue;

    public bool IsOnGround()
    {
        return m_OnGround;
    }
    public void ResetOnGround()
    {
        m_OnGround = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "DiceGround")
        {
            m_OnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "DiceGround")
        {
            m_OnGround = false;
        }
    }

}
