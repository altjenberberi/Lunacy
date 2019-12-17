using UnityEngine;
using Essentials;

public class Door : MonoBehaviour, IActionable
{
    [SerializeField]
    private bool m_Open = false;

    [SerializeField]
    private float m_Speed = 10;

    [Header("Closed")]
    [SerializeField]
    private Vector3 m_ClosedPos;

    [SerializeField]
    private Vector3 m_ClosedRot;

    [Header("Opened")]
    [SerializeField]
    private Vector3 m_OpenedPos;

    [SerializeField]
    private Vector3 m_OpenedRot;

    public void Interact ()
    {
        m_Open = !m_Open;
    }

    public string Message ()
    {
        return m_Open ? "CLOSE" : "OPEN";
    }

    private void Update ()
    {
        if (m_Open)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_OpenedPos, Time.deltaTime * m_Speed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(m_OpenedRot), Time.deltaTime * m_Speed);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_ClosedPos, Time.deltaTime * m_Speed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(m_ClosedRot), Time.deltaTime * m_Speed);
        }
    }
}
