using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Collision : MonoBehaviour
{
    public string Target;
    public string SuplyTarget;
    public int Harm=1;        //�˺�
    public int Blood=50;       //Ѫ���������ӵ�Ѫ����û����
    public int Score=100;
    public GameObject Effect;//�������Ч��
    GameControl m_control;
    // Start is called before the first frame update
    void Start()
    {
        GameObject game = GameObject.Find("Control");
        m_control = game.GetComponent<GameControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// ��ײ��������
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string name = this.gameObject.name;
        if (name != "MyPlane" && name != "MyBullet" && name != "MyMissle" && name != "MyBomb") return;//ֻ�����ҷ��ķɻ����ӵ������������
        if (collision.tag.Equals(Target))
        {
            string cname = collision.gameObject.name;
            if (name == "EnemyBullet" && (cname == "MyBullet" || cname == "MyMissle")) return;
            if ((name == "MyBullet" || name == "MyMissle") && cname == "EnemyBullet") return;

            Collision cl = collision.gameObject.GetComponent<Collision>();
            if(cname == "MyPlane")
            {
                MyPlane mp = collision.gameObject.GetComponent<MyPlane>();
                if(mp && !mp.IsNoHarm())
                {
                    cl.Blood -= this.Harm;
                }
            }
            else cl.Blood -= this.Harm;
            if(name == "MyPlane")
            {
                MyPlane mp = gameObject.GetComponent<MyPlane>();
                if (mp && !mp.IsNoHarm())
                {
                    this.Blood -= this.Harm;
                }
            }
            else this.Blood -= cl.Harm;

            if (( ((name == "Boss" || name == "Enemy") && Blood <= 0) || ((cname == "Enemy" || cname == "Boss")) && cl.Blood <= 0))
            {                                                                   //���������ǵ��˷ɻ���ӷ�

                int score;
                if (name == "Enemy" || name == "Boss") score = this.Score;
                else score = cl.Score;
                m_control.AddScore(score);
            }

            if (cl.Blood <= 0 || (cname == "MyBullet" || cname == "MyMissle" || cname == "EnemyBullet"))
            {
                DestroyObject(collision.gameObject, cl.Effect);
            }

            if (this.Blood <= 0 || (name == "MyBullet" || name == "MyMissle" || name == "EnemyBullet"))
            {
                DestroyObject(this.gameObject, this.Effect);
            }
        }
        else if(collision.tag.Equals(SuplyTarget))
        {
            if (name != "MyPlane") return;
            MyPlane my = this.gameObject.GetComponent<MyPlane>();
            my.GetSuply(collision.gameObject.name);
            DestroyObject(collision.gameObject, null);
        }

    }
    /// <summary>
    /// ���ٶ��󣬰����ҷ��ĺͶԷ��ģ�����ʾ��Ч
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="effect"></param>
    void DestroyObject(GameObject obj, GameObject effect)
    {
        if (obj.name == "MyPlane")
        {
            MyPlane plane = obj.GetComponent<MyPlane>();
            if (!plane.Ready) return;
            plane.SetReady(false);
            m_control.StartNewPlane();
        }
        else if(obj.name == "Boss")
        {
            m_control.BossEnd();
            Destroy(obj);
        }
        else Destroy(obj);

        if (effect != null)
        {
            GameObject bt = Instantiate(effect);
            bt.name = "effect";
            bt.transform.position = this.transform.position;
        }
    }
}
