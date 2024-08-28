using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float Speed = 0.5f;
    public int MoveType = 1;            //移动类型。0表示朝我的飞机，1表示绕y轴旋转一个角度，2贝塞尔曲线，3:边移动边旋转
    public float FaceChangeSpeed = 5.0f;//朝向转动速度，用于0
    public int MoveAngle = 40;         //旋转角度，用于1
    public float ControlMoveX = 3.0f;         //贝赛尔曲线控制点的X值，用于2，如果小于0.01则控制点为我的飞机
    public float ControlMoveY = -2.0f;         //贝赛尔曲线控制点的Y值，用于2，如果小于0.01则控制点为我的飞机
    public int EnePos = 1;      //用于贝塞尔的目的地，0，左底；1，中间底；2，右底
    BezierMove m_bezierControl;     //贝塞尔控制对象，用于2
    int OUTEDGE_Y = -6;//出界Y位置
    int OUTEDGE_X = 10;//出界X位置
    Vector3 m_direction;
    // Start is called before the first frame update
    void Start()
    {
        if (MoveType == 1 || MoveType == 3)
        {
            transform.rotation = Quaternion.Euler(0f, 0.0f, MoveAngle);
            if(MoveAngle == -90 || MoveAngle == -270)
                m_direction = new Vector3(1, 0, 0);
            else if (MoveAngle == 90 || MoveAngle == 270)
                m_direction = new Vector3(-1, 0, 0);
            else m_direction = new Vector3(Mathf.Tan(MoveAngle * Mathf.Deg2Rad), -1, 0);
        }
        else if (MoveType == 2)
        {
            GameObject target = GameObject.Find("/MyPlane");
            Vector3 controlPos;
            if (Mathf.Abs(ControlMoveX) <= 0.01 && Mathf.Abs(ControlMoveY) <= 0.01 && target !=null)
            {
                controlPos = target.transform.position;
            }
            else
            {
                controlPos = transform.position;
                controlPos += new Vector3(ControlMoveX, ControlMoveY, 0);
            }

            Vector3 endPos;
            if (EnePos == 0)
            {
                endPos = new Vector3(-OUTEDGE_X, OUTEDGE_Y, 0);
            }
            else if (EnePos == 1)
            {
                endPos = new Vector3(0, OUTEDGE_Y, 0);
            }
            else
            {
                endPos = new Vector3(OUTEDGE_X, OUTEDGE_Y, 0); ;
            }
            m_bezierControl = new BezierMove();
            m_bezierControl.Init(transform.position, controlPos, endPos, this.transform, Speed, 100);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        if (sp.y < 0 || sp.x < -10 || sp.x >Screen.width + 100)
        {
            Destroy(this.gameObject);
        }

        if (MoveType == 0)
        {
            GameObject target = GameObject.Find("MyPlane");

            Vector3 face = this.transform.up;
            Vector3 pst = this.transform.position;
            if(target) pst = target.transform.position;
            Vector3 pst_e = this.transform.position;
            Vector3 director = pst - pst_e;
            float angle = Vector3.SignedAngle(face, director, Vector3.forward);
            float rotateAngle = Time.deltaTime * FaceChangeSpeed;
            if (angle < 0)
            {
                rotateAngle = -rotateAngle;
            }
            transform.Rotate(0, 0, -rotateAngle, Space.Self);
            float dis = Speed * Time.deltaTime;
            transform.Translate(0, -dis, 0, Space.Self);
        }
        else if (MoveType == 1 || MoveType == 3)
        {
            if (MoveAngle != 90 && MoveAngle != -90
                && MoveAngle != 270 && MoveAngle != -270)
            {
                if ((sp.x <= 0 || sp.x >= Screen.width) && this.name != "EnemyBullet")
                {
                    m_direction = new Vector3(-m_direction.x, m_direction.y, 0);
                    transform.rotation = Quaternion.Euler(0f, 0.0f, -MoveAngle);
                }
            }
            transform.position += m_direction * Speed * Time.deltaTime;
            if(MoveType == 3)
            {
                float rotateAngle = Time.deltaTime * FaceChangeSpeed;
                transform.Rotate(0, 0, -rotateAngle, Space.Self);

            }
        }
        else if (MoveType == 2)
        {
            m_bezierControl.Move(ONComplete);

        }

    }
    private void ONComplete()
    {
        Destroy(this.gameObject);
    }

}
