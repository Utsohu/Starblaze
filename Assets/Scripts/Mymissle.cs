using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Mymissle : MonoBehaviour
{
    public float ControlMoveX = 15;
    public float ControlMoveY = 15;
    public float Speed = 15;
    int OUTEDGE_Y = 6;
    int OUTEDGE_X = 10;

    int m_nRepeat = 5;
    BezierMove m_bezierControl;
    bool m_destory = false;
    GameObject m_target = null;
    // Start is called before the first frame update
    void Start()
    {
        m_bezierControl = new BezierMove();
        m_target = GameObject.Find("Enemy");
        float y = OUTEDGE_Y;
        if(m_target) y = Mathf.Abs(m_target.transform.position.y);
        if (y >= OUTEDGE_Y) m_target = null;            //如果目标对象在屏幕外，则不考虑
        Vector3 controlPos = transform.position;
        Vector3 endPos;
        if (m_target) 
            endPos = m_target.transform.position;
        else
        {
            float x = OUTEDGE_X;
            if (controlPos.x > 0) x = -OUTEDGE_X;
            endPos = new Vector3(x, OUTEDGE_Y, 0);
            m_nRepeat = 1;
        }
        controlPos += new Vector3(ControlMoveX, ControlMoveY, 0);
        m_bezierControl.Init(transform.position, controlPos, endPos, transform, Speed, 100);
    }

    void Locate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        m_bezierControl.Move(ONComplete);
    }
    private void ONComplete()
    {
        if(m_destory)
        {
            Destroy(this.gameObject);
            return;
        }
        m_nRepeat--;
        if (m_nRepeat == 0)
        {
            if (m_bezierControl) Destroy(m_bezierControl);
            Destroy(this.gameObject);
        }
        else
        {
            Vector3 endPos;
            if (!m_target) { 
               endPos = new Vector3(transform.position.x, -OUTEDGE_Y, 0);
                m_destory = true;
            }
            else
                endPos = m_target.transform.position;
            Vector3 controlPos = transform.position;
            controlPos += new Vector3(ControlMoveX, ControlMoveY, 0);
            if (m_bezierControl) Destroy(m_bezierControl);
            m_bezierControl = new BezierMove();
            m_bezierControl.Init(transform.position, endPos, endPos, transform, Speed, 100);
        }
    }
}