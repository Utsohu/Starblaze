using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NormalEnemy : MonoBehaviour
{
    public GameObject Bullet1;       //关联子弹
    public GameObject Bullet2;       //关联子弹
    public float FireSpeed1 = 0.3f;     //连续发射间隔
    public float FireSpeed2 = 2.0f;     //连续发射后的停顿间隔
    public int FireNums = 3;        //连续发射次数
    int m_fireNum = 0;              //连续发射计数器    
    public int FireType = 0;//射击类型：0发射1颗bullet；1发射2颗bullet1；2发射2颗bullet1和2颗bullet2
    public int BulletAngle = 0;//射出的子弹角度，如果0随子弹自身设定，其他为射击角度（绕Y轴旋转）
    public Vector3 FirePos = new Vector3(0, -0.5f, 0);//射击的初始位置，相对于
    float m_firetimer = 0f;
    int OUTEDGE_Y = 6;//出界Y位置

    // Start is called before the first frame update
    void Start()
    {
        m_fireNum = FireNums;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fireNum > 0) { 
            m_firetimer += Time.deltaTime;
            if (m_firetimer > FireSpeed1)
            {
                Fire();
                m_firetimer -= FireSpeed1;
                m_fireNum--;
            }
        }
        else
        {
            m_firetimer += Time.deltaTime;
            if (m_firetimer > FireSpeed2)
            {
                m_fireNum = FireNums;
                m_firetimer -= FireSpeed2;
            }
        }
    }

    void Fire()
    {
        if (Bullet1 == null) return;
        if (transform.position.y > OUTEDGE_Y || transform.position.y < -OUTEDGE_Y) return;
        if (FireType == 0)
        {
            MakeBullet(Bullet1, FirePos);
        }
        else if (FireType == 1)
        {
            MakeBullet(Bullet1, new Vector3(0.2f, FirePos.y, FirePos.z));
            MakeBullet(Bullet1, new Vector3(-0.2f, FirePos.y, FirePos.z));
        }
        else if (FireType == 2)
        {
            MakeBullet(Bullet1, new Vector3(0.2f, FirePos.y, FirePos.z));
            MakeBullet(Bullet1, new Vector3(-0.2f, FirePos.y, FirePos.z));

            MakeBullet(Bullet2, new Vector3(0.5f, FirePos.y, FirePos.z));
            MakeBullet(Bullet2, new Vector3(-0.5f, FirePos.y, FirePos.z));
        }
    }

    void MakeBullet(GameObject bullet, Vector3 pos)
    {
        GameObject bt = Instantiate(bullet);
        bt.name = "EnemyBullet";
        bt.transform.position = this.transform.position + pos;
        if(BulletAngle != 0)
        {
            Move mv = bt.GetComponent<Move>();
            mv.MoveAngle = BulletAngle;
            mv.MoveType = 1;
        }
    }
    private void ONComplete()
    {
        Destroy(this.gameObject);
    }
}
