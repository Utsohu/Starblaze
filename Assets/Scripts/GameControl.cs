using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[Serializable]
public struct JsonFrame{ 
    public float time;          //����ʱ�䣬�����ǵ�ǰy���ֵ
    public string type;         //�����Ƿ���˻�ʯͷ�򲹸�
    public int no;              //ģ�����
    public int sprite;          //����ͼƬ���
    public int style;           //���Σ�0����1�ţ�1����ֱ�����ţ��Ժ�������������
    public int num;             //ÿ�Ÿ���
    public float posx;          //��ʼλ��
    public float posy;
}

[Serializable]
public class DramaData
{
    public List<JsonFrame> frames;
    int m_curno;
    public void Init() { 
        m_curno = 0; 
    }
    public bool IsFinish()
    {
        return m_curno == frames.Count ? true : false;
    }
    public float CurTime()
    {
        if (m_curno >= frames.Count) return 10000f;
        return frames[m_curno].time;
    }
    public JsonFrame Next()
    {
        if(m_curno >= frames.Count)
        {
            return new JsonFrame();
        }
        m_curno++;
        return frames[m_curno - 1];
    }
}
public class GameControl : MonoBehaviour
{
    public Text ScoreText;

    public GameObject Menu;     //�˵�������ͣ��gameover����һ��ʱҪ������
    public GameObject MenuWindow;     //�˵�������ͣ��gameover����һ��ʱҪ������
    public Button btnContinue;     //�˵���ļ�����ť
    public Text txtGameOver;       //�˵����gameover����
    public Text txtRestart;        //�˵���������ť�����֣�

    public float BackMoveSpeed = 0.2f;     //���������ٶ�
    public GameObject[] SceneBack;  //ÿ�صı���
    int m_level = 0;//�ڼ���
    GameObject m_curBack;//��ǰ�صı���
    public GameObject MyPlane;      //�ҵķɻ�
    public GameObject[] Enemy;      //�������ɵ��˵�ģ�����
    public Sprite[] EnemySprite;    //���ڴ�ŵ��˵�ͼƬ
    public GameObject Stone;        //��������ʯͷ��ģ�����
    public Sprite[] StoneSprite;    //���ڴ��ʯͷ��ͼƬ
    public GameObject[] Suply;        //�������ɲ�����ģ�����
    public GameObject[] ReadyPlane;//���½���ʾ�ĵȴ��ɻ�
    public GameObject ShowPlane;    //����ʱ�ɳ����ķɻ�ģ��
    int m_nLeftPlane;
    int m_score;                //����
    static float m_fTimeScale;         //ʱ��̶ȣ�����ʱ��̶�Ϊ0ʱ����ͣ�������øÿ̶����ڱ�������ʱʱ��̶�.��Ϊ��ֵ̬������Ϊ�˳����ٽ�ȥ��Ҫ���ֵ
//    float m_fGameTimer = 0f;    //��Ϸ����ʱ�䣬ÿ��update�ۼƣ������߰���Ϸʱ���ƽ���
    DramaData m_drama;
    /// <summary>
    /// ��ʼ��ʼ��
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        m_nLeftPlane = 3;
        m_score = 0;
        Menu.SetActive(false);//���ز˵�
        MenuWindow.SetActive(false);//���ز˵�
        m_level = 0;
        Restart();
    }
    /// <summary>
    /// ÿ֡���ã��ж�Esc���Ƿ��£��ƶ���������ȡÿ֡�籾
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && (Time.timeScale!=0))
        {
            m_fTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Menu.SetActive(true);
            MenuWindow.SetActive(true);
            txtGameOver.gameObject.SetActive(false);
            btnContinue.gameObject.SetActive(true);
        }
        if(m_curBack.transform.position.y > -16)        //�����Ѿ����������//ֹͣ�ƶ�
            m_curBack.transform.position -= new Vector3(0, BackMoveSpeed * Time.deltaTime, 0);
//        m_fGameTimer += Time.deltaTime;           //��y��λ�����ƽ������ʱ�����ȷ��16-��-16
        while(!m_drama.IsFinish())
        {
            if (m_drama.CurTime() >= m_curBack.transform.position.y)
            {
                JsonFrame frame = m_drama.Next();
                MakeEenmy(frame);
            }
            else break;
        }
    }
    /// <summary>
    /// �˵���������
    /// </summary>
    public void GameContinue()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Time.timeScale = m_fTimeScale;
        Menu.SetActive(false);
        MenuWindow.SetActive(false);
    }
    /// <summary>
    /// �˵����ص���
    /// </summary>
    public void ReturnHome()
    {
        SceneManager.LoadScene("StartScene");
    }
    /// <summary>
    /// �����ۼ�
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        m_score += score;
        ScoreText.text = m_score.ToString("D6");
    }
    /// <summary>
    /// �籾����
    /// </summary>
    void LoadDrama()
    {
        string fname = "scene" + m_level;
        TextAsset info = Resources.Load<TextAsset>(fname);

        m_drama = JsonUtility.FromJson<DramaData>(info.text);
        m_drama.Init();
    }
    /// <summary>
    /// ��ʼ�µķɻ�
    /// </summary>
    public void StartNewPlane()
    {
        if(m_nLeftPlane <= 0)
        {
            Menu.SetActive(true);
            MenuWindow.SetActive(true);
            txtGameOver.gameObject.SetActive(true);
            btnContinue.gameObject.SetActive(false);
            return;
        }
        m_nLeftPlane--;
        GameObject rp = ReadyPlane[m_nLeftPlane];
        GameObject sp = Instantiate(ShowPlane);
        ReadyPlane ready = sp.GetComponent<ReadyPlane>();
        ready.Myplane = MyPlane;
        sp.transform.position = rp.transform.position;
        rp.SetActive(false);
    }
    /// <summary>
    /// ���ݽű����ɵ���
    /// </summary>
    /// <param name="frame"></param>
    void MakeEenmy(JsonFrame frame)
    {
        GameObject template;
        Sprite pic = null;
        if (frame.type == "enemy")
        {
            template = Enemy[frame.no];
            pic = EnemySprite[frame.sprite];
        }
        else if (frame.type == "stone")
        {
            template = Stone;
            pic = StoneSprite[frame.sprite];
        }
        else if (frame.type == "suply")
        {
            template = Suply[frame.no];
        }
        else return;

        if (frame.type == "enemy")
        {
            if (frame.style == 0)
            {
                float y = frame.posy;
                for (int j = 0; j < frame.num; j++)
                {
                    GameObject gm = Instantiate(template);
                    gm.name = "Enemy";
                    SpriteRenderer sp = gm.GetComponent<SpriteRenderer>();
                    sp.sprite = pic;
                    gm.transform.position = new Vector3(frame.posx, y, 0);
                    y += 1.5f;
                }
            }
            else if(frame.style == 1)
            {
                float x = frame.posx - 0.75f;
                float y = frame.posy;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < frame.num; j++)
                    {
                        GameObject gm = Instantiate(template);
                        gm.name = "Enemy";
                        SpriteRenderer sp = gm.GetComponent<SpriteRenderer>();
                        sp.sprite = pic;
                        gm.transform.position = new Vector3(x, y, 0);
                        y += 1.5f;
                    }
                    y = frame.posy;
                    x += 1.5f;
                }
            }
        }
        else if(frame.type == "suply")
        {
            GameObject gm = Instantiate(template);
            gm.name = name;
            gm.transform.position = new Vector3(frame.posx, frame.posy, 0);
            if (frame.no == 1) gm.name = "SuplyM";
            else if (frame.no == 2) gm.name = "SuplyB";
            else gm.name = "Suply1";
        }
        else
        {
            GameObject gm = Instantiate(template);
            gm.name = "Enemy";
            gm.transform.position = new Vector3(frame.posx, frame.posy, 0);
        }
    }
    /// <summary>
    /// �������أ�Ҳ���ڿ�ʼ��һ��
    /// </summary>
    public void Restart()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (m_fTimeScale > 0.00001 && Time.timeScale < 0.00001)
            Time.timeScale = m_fTimeScale;
        Menu.SetActive(false);
        MenuWindow.SetActive(false);
        GameObject[] list = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in list){
            Vector3 sp = Camera.main.WorldToScreenPoint(obj.transform.position);
            if (sp.y >= 0 && sp.y <= Screen.height)
            {
                Destroy(obj);
            }
        }
        MyPlane mp = MyPlane.GetComponent<MyPlane>();
        mp.SetReady(false);
        if(m_curBack)
        {
            Destroy(m_curBack);
        }
        m_curBack = Instantiate(SceneBack[m_level]);
        m_curBack.transform.position = new Vector3(0f, 16f, 0);
        m_curBack.name = "SceneBack" + m_level;

        LoadDrama();
        m_nLeftPlane = 3;
//        m_fGameTimer = 0f;    //��Ϸ����ʱ�䣬ÿ��update�ۼƣ������߰���Ϸʱ���ƽ���
        m_drama.Init();
        foreach(GameObject rp in ReadyPlane)
        {
            rp.SetActive(true);
        }

        StartNewPlane();
    }
    /// <summary>
    /// ÿ������BOSS������ã��жϽ����¹ػ����
    /// </summary>
    public void BossEnd()
    {
        Invoke("DoBossEnd", 4);
    }

    void DoBossEnd() 
    {
        Menu.SetActive(true);
        MenuWindow.SetActive(true);
        txtGameOver.gameObject.SetActive(true);
        btnContinue.gameObject.SetActive(false);
        m_level++;
        if(m_level < SceneBack.Count())
        {
            txtGameOver.text = "Level " + m_level + " Clear!";
            txtRestart.text = "Next Level";
        }
        else
        {
            txtGameOver.text = "All Level Clear!";
            m_level = 0;
            txtRestart.text = "Level 1";
        }
    }
}
