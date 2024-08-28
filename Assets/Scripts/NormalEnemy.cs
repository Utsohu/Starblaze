using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NormalEnemy : MonoBehaviour
{
    public GameObject Bullet1;       //�����ӵ�
    public GameObject Bullet2;       //�����ӵ�
    public float FireSpeed1 = 0.3f;     //����������
    public float FireSpeed2 = 2.0f;     //����������ͣ�ټ��
    public int FireNums = 3;        //�����������
    int m_fireNum = 0;              //�������������    
    public int FireType = 0;//������ͣ�0����1��bullet��1����2��bullet1��2����2��bullet1��2��bullet2
    public int BulletAngle = 0;//������ӵ��Ƕȣ����0���ӵ������趨������Ϊ����Ƕȣ���Y����ת��
    public Vector3 FirePos = new Vector3(0, -0.5f, 0);//����ĳ�ʼλ�ã������
    float m_firetimer = 0f;
    int OUTEDGE_Y = 6;//����Yλ��

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
