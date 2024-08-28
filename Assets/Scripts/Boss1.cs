using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    public int FireNums = 3;        //连续发射次数
    int m_fireNum = 0;              //连续发射计数器    
    public float FireSpeed1 = 0.3f;     //连续发射间隔
    public float FireSpeed2 = 2.0f;     //连续发射后的停顿间隔
    float m_firetimer = 0f;
    public GameObject Bullet1;       //关联子弹
    public GameObject Bullet2;       //关联子弹
    public GameObject Bullet3;       //关联子弹
    public GameObject Bullet4;       //关联子弹
    Collision m_cl;     //关联的Collision脚本，读取血量
    Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        m_cl = GetComponent<Collision>();
        m_animator = GetComponent<Animator>();
        m_animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        if (sp.y < 0 || sp.y > Screen.height) return;
        if (!m_animator.enabled)
            m_animator.enabled = true;
        if (m_fireNum > 0)
        {
            m_firetimer += Time.deltaTime;
            if (m_firetimer > FireSpeed1)
            {
                Fire();
                m_firetimer = 0;
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
        Vector3 firePos = new Vector3(0, -0.5f, 0);
        if (m_cl.Blood >= 500)
        {
            MakeBullet(Bullet1, new Vector3(0.2f, firePos.y, firePos.z), 0);
            MakeBullet(Bullet1, new Vector3(-0.2f, firePos.y, firePos.z), 0);
        }
        else if (m_cl.Blood >= 300)
        {
            MakeBullet(Bullet1, new Vector3(0.2f, firePos.y, firePos.z), 0);
            MakeBullet(Bullet1, new Vector3(-0.2f, firePos.y, firePos.z), 0);

            MakeBullet(Bullet2, new Vector3(0.5f, firePos.y, firePos.z), 30);
            MakeBullet(Bullet2, new Vector3(-0.5f, firePos.y, firePos.z), -30);
        }
        else if (m_cl.Blood >= 200)
        {
            MakeBullet(Bullet2, new Vector3(0.5f, firePos.y, firePos.z), 0);
            MakeBullet(Bullet2, new Vector3(-0.5f, firePos.y, firePos.z), 0);

            MakeBullet(Bullet3, new Vector3(0.8f, firePos.y, firePos.z), 0);
            MakeBullet(Bullet3, new Vector3(-0.8f, firePos.y, firePos.z), 0);

        }
        else if (m_cl.Blood >= 100)
        {
            MakeBullet(Bullet3, new Vector3(0.8f, firePos.y, firePos.z), 0);
            MakeBullet(Bullet3, new Vector3(-0.8f, firePos.y, firePos.z), 0);

            MakeBullet(Bullet4, new Vector3(0.8f, firePos.y + 1.0f, firePos.z), 50);
            MakeBullet(Bullet4, new Vector3(-0.8f, firePos.y + 1.0f, firePos.z), -50);
        }
        else if (m_cl.Blood >= 0)
        {
            MakeBullet(Bullet4, new Vector3(0.8f, firePos.y + 1.0f, firePos.z), 0);
            MakeBullet(Bullet4, new Vector3(-0.8f, firePos.y + 1.0f, firePos.z), 0);
        }
    }

    void MakeBullet(GameObject bullet, Vector3 pos, int BulletAngle)
    {
        GameObject bt = Instantiate(bullet);
        bt.name = "EnemyBullet";
        bt.transform.position = this.transform.position + pos;
        if (BulletAngle != 0)
        {
            Move mv = bt.GetComponent<Move>();
            mv.MoveAngle = BulletAngle;
            mv.MoveType = 1;
        }
    }

}
