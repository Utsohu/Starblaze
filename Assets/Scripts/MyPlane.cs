using System;
using UnityEngine;

public class MyPlane : MonoBehaviour
{
    public Sprite[] LeftSprites; // 将所有要切换的Sprite拖到这个数组中
    public Sprite[] RightSprites; // 将所有要切换的Sprite拖到这个数组中
    public float Speed = 7f;
    public float Switchfreq = 0.1f;//精灵切换频率
    public GameObject Bullet1;
    public GameObject Bullet2;
    public GameObject Missle;
    public GameObject Bomb;     //底下显示的bomb
    public GameObject ShowBomb;//扔出去的bomb
    public GameObject NoHarm;  //不受伤寒时显示的动画对象，保护圈
    public float FireSpeed1 = 0.05f;
    public float FireSpeed2 = 0.3f;
    public int FireNums = 3;        //连续发射次数

    int m_fireNum = 0;

    public float MissleSpeed = 0.6f;
    float m_oldMissleSpeed;//初始导弹速度，用于复位。
    float m_fMaxMissleSpeed = 0.02f;//最大导弹速度，吃补给后导弹速度加快。但不能超过这个速度
    public bool Ready = false;//是否准备好，用于表示是否可以移动射击了，用于在飞机库飞出来到位后，置为true。

    /*********用于移动变化*********/
    private SpriteRenderer m_spriteRenderer;
    int m_leftindex = 0;
    int m_rightindex = 0;
    float m_calTime = 0f;

    /**********初始信息**************/
    Transform m_startPos;   
    public int m_level = 4;
    public int m_bullettype = 0;
    float m_firetimer = 10f;
    float m_missletimer = 10f;
    /*************用于bomb的生成************/
    int m_bombnum = 3;
    float m_bombPosX = 8.5f;
    float m_bombWidth = 0.4f;
    float m_bombPosY = -4.6f;
    float m_bombtimer = 1.0f;//用于防止一个按键，连续放bomb

    bool m_bNoHarm = false;//不受伤寒状态，避免刚出现就被灭了
    GameObject m_goNoHarm = null;
    int m_blood;        //用来保存原始血量
    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_oldMissleSpeed = MissleSpeed;
        m_fireNum = FireNums;
        this.gameObject.SetActive(Ready);
        m_startPos = transform;
        for(int i = 0; i < m_bombnum; i++)
        {
            GameObject bm = Instantiate(Bomb);
            bm.name = "bomb" + (i + 1);
            bm.transform.position = new Vector3(m_bombPosX, m_bombPosY, 0);
            m_bombPosX = m_bombPosX - (float) (m_bombWidth * 1.5);
        }
        Collision cl = GetComponent<Collision>();
        m_blood = cl.Blood;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Ready) return;
        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        int direct = 0;
        float step = Speed * Time.deltaTime;
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && sp.x > 0)
        {
            transform.Translate(-step, 0, 0, Space.Self);
            direct = 1;
        }
        else if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && sp.x < Screen.width)
        {
            transform.Translate(step, 0, 0, Space.Self);
            direct = 2;
        }
        else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && sp.y < Screen.height)
        {
            transform.Translate(0, step, 0, Space.Self);
        }
        else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && sp.y > 0)
        {
            transform.Translate(0, -step, 0, Space.Self);
        }
        else if (Input.GetKey(KeyCode.H))
        {
            AddBomb();
        }
        else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            FireBomb();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            FireBullet();
        }
        if(m_goNoHarm)      //有保护圈时，带着保护圈一起移动
            m_goNoHarm.transform.position = this.transform.position;

        FireMissle();
        SwitchSprite(direct);
    }

    void FireBullet()
    {
        if (m_fireNum > 0)
        {
            m_firetimer += Time.deltaTime;
            if (m_firetimer <= FireSpeed1) return;
            if (m_bullettype == 0)
            {
                for (int i = 0; i <= m_level; i++)
                {
                    float delta = 15 * i;
                    GameObject bt = Instantiate(Bullet1);
                    bt.name = "MyBullet";
                    bt.transform.position = this.transform.position + new Vector3(-0.3f, 0.5f, 0);
                    if (i != 0)
                    {
                        bt.transform.rotation = Quaternion.Euler(0f, 0.0f, delta);
                        Mybullet1 b = bt.GetComponent<Mybullet1>();
                        b.SetDirect(new Vector3(-Mathf.Tan(delta * Mathf.Deg2Rad), 1, 0));
                    }
                    bt = Instantiate(Bullet1);
                    bt.name = "MyBullet";
                    bt.transform.position = this.transform.position + new Vector3(0.3f, 0.5f, 0);
                    if (i != 0)
                    {
                        bt.transform.rotation = Quaternion.Euler(0f, 0.0f, -delta);
                        Mybullet1 b = bt.GetComponent<Mybullet1>();
                        b.SetDirect(new Vector3(Mathf.Tan(delta * Mathf.Deg2Rad), 1, 0));
                    }
                }
            }
            else if (m_bullettype == 1)
            {

            }
            m_firetimer = 0;
            m_fireNum--;
        }
        else
        {
            m_firetimer += Time.deltaTime;
            if (m_firetimer <= FireSpeed2) return;
            m_fireNum = FireNums;
            m_firetimer -= FireSpeed2;
        }
    }

    void FireMissle()
    {
        m_missletimer += Time.deltaTime;
        if (m_missletimer <= MissleSpeed) return;

        GameObject ms = Instantiate(Missle);
        ms.transform.position = this.transform.position + new Vector3(-0.5f, 0.9f, 0);
        ms.name = "MyMissle";
        ms = Instantiate(Missle);
        ms.transform.position = this.transform.position + new Vector3(0.5f, 0.9f, 0);
        ms.name = "MyMissle";
        m_missletimer = 0f;
//        m_missletimer -= MissleSpeed;
    }
    void FireBomb()
    {
        if (m_bombnum<=0) return;
        m_bombtimer += Time.deltaTime;
        if (m_bombtimer <= 0.3f) return;
        String name = "bomb" + m_bombnum;
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            GameObject ms = Instantiate(ShowBomb);
            ms.transform.position = this.transform.position + new Vector3(0f,3.0f, 0f);
            ms.name = "MyBomb";
            Destroy(obj);
            m_bombPosX = m_bombPosX + (float)(m_bombWidth * 1.5);
        }
        m_bombnum--;
        m_bombtimer = 0f;
    }

    void SwitchSprite(int direct)
    {
        if (Time.time - m_calTime > Switchfreq) return;
        if (direct == 1)
        {
            m_spriteRenderer.sprite = LeftSprites[m_leftindex];
            m_leftindex++;
            if (m_leftindex >= LeftSprites.Length)
                m_leftindex = 0;
        }
        else if (direct == 2)
        {
            m_spriteRenderer.sprite = RightSprites[m_rightindex];
            m_rightindex++;
            if (m_rightindex >= RightSprites.Length)
                m_rightindex = 0;
        }
        m_calTime = Time.time;
    }
    void AddBomb()
    {
        m_bombnum++;
        GameObject bm = Instantiate(Bomb);
        bm.name = "bomb" + m_bombnum;
        bm.transform.position = new Vector3(m_bombPosX, m_bombPosY, 0);
        m_bombPosX = m_bombPosX - (float)(m_bombWidth * 1.5);
    }
    public void GetSuply(string name)
    {
        if (name == "SuplyM")
        {
            if(MissleSpeed < m_fMaxMissleSpeed)
            MissleSpeed /= 1.2f;
        }
        else if (name == "SuplyB")
        {
            AddBomb();
        }
        else
        {
            if (m_level < 4) 
                m_level += 1;
        }

    }
    public bool IsNoHarm()
    {
        return m_bNoHarm;
    }
    public void SetReady(bool bready)
    {
        if (bready)         //刚准备好就设置成不受伤寒状态，并生成保护圈，过定时后才恢复成正常状态
        {
            Ready = true;
            this.gameObject.SetActive(bready);
            m_bNoHarm = true;
            m_goNoHarm = Instantiate(NoHarm);
            m_goNoHarm.transform.position = this.transform.position;
            Invoke("RestoreNoHarm", 3);
        }
        else Reset();            //飞机复位
    }
    void RestoreNoHarm()
    {
        m_bNoHarm = false;              //恢复正常状态，删除保护圈
        if (m_goNoHarm)
        {
            Destroy(m_goNoHarm);
            m_goNoHarm = null;
        }
    }
    void ResetBomb()
    {
        if (m_bombnum > 3)
        {
            for (int i = 4; i <= m_bombnum; i++)
            {
                GameObject ob = GameObject.Find("bomb" + i);
                if (ob) Destroy(ob);
                m_bombPosX = m_bombPosX + (float)(m_bombWidth * 1.5);
            }
            m_bombnum = 3;
        }
        else if (m_bombnum < 3)
        {
            int n = 3 - m_bombnum;
            for (int i = 1; i <= n; i++)
            {
                AddBomb();
            }
        }
    }
    void Reset()
    {
        Collision cl = GetComponent<Collision>();
        cl.Blood = m_blood;
        Ready = false;
        this.gameObject.SetActive(false);
        m_bNoHarm = false;
        if (m_goNoHarm)
        {
            Destroy(m_goNoHarm);
            m_goNoHarm = null;
        }
        ResetBomb();
    }
}
