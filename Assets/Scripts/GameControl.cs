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
    public float time;          //产生时间，现在是当前y轴的值
    public string type;         //类型是否敌人或石头或补给
    public int no;              //模板序号
    public int sprite;          //精灵图片序号
    public int style;           //队形，0是排1排；1是排直的两排，以后扩充其他阵列
    public int num;             //每排个数
    public float posx;          //开始位置
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

    public GameObject Menu;     //菜单，在暂停、gameover和下一关时要弹出来
    public GameObject MenuWindow;     //菜单，在暂停、gameover和下一关时要弹出来
    public Button btnContinue;     //菜单里的继续按钮
    public Text txtGameOver;       //菜单里的gameover文字
    public Text txtRestart;        //菜单里重启按钮的文字，

    public float BackMoveSpeed = 0.2f;     //背景滚动速度
    public GameObject[] SceneBack;  //每关的背景
    int m_level = 0;//第几关
    GameObject m_curBack;//当前关的背景
    public GameObject MyPlane;      //我的飞机
    public GameObject[] Enemy;      //用于生成敌人的模板对象
    public Sprite[] EnemySprite;    //用于存放敌人的图片
    public GameObject Stone;        //用于生成石头的模板对象
    public Sprite[] StoneSprite;    //用于存放石头的图片
    public GameObject[] Suply;        //用于生成补给的模板对象
    public GameObject[] ReadyPlane;//右下角显示的等待飞机
    public GameObject ShowPlane;    //启动时飞出来的飞机模板
    int m_nLeftPlane;
    int m_score;                //分数
    static float m_fTimeScale;         //时间刻度，运行时间刻度为0时则暂停，所以用该刻度用于保留运行时时间刻度.设为静态值，是因为退出后，再进去需要这个值
//    float m_fGameTimer = 0f;    //游戏进行时间，每次update累计，故事线按游戏时间推进。
    DramaData m_drama;
    /// <summary>
    /// 开始初始化
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        m_nLeftPlane = 3;
        m_score = 0;
        Menu.SetActive(false);//隐藏菜单
        MenuWindow.SetActive(false);//隐藏菜单
        m_level = 0;
        Restart();
    }
    /// <summary>
    /// 每帧调用，判断Esc键是否按下，移动背景，读取每帧剧本
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
        if(m_curBack.transform.position.y > -16)        //背景已经拉到最底了//停止移动
            m_curBack.transform.position -= new Vector3(0, BackMoveSpeed * Time.deltaTime, 0);
//        m_fGameTimer += Time.deltaTime;           //按y轴位置来推进剧情比时间更精确从16-》-16
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
    /// 菜单继续调用
    /// </summary>
    public void GameContinue()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Time.timeScale = m_fTimeScale;
        Menu.SetActive(false);
        MenuWindow.SetActive(false);
    }
    /// <summary>
    /// 菜单返回调用
    /// </summary>
    public void ReturnHome()
    {
        SceneManager.LoadScene("StartScene");
    }
    /// <summary>
    /// 分数累计
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        m_score += score;
        ScoreText.text = m_score.ToString("D6");
    }
    /// <summary>
    /// 剧本加载
    /// </summary>
    void LoadDrama()
    {
        string fname = "scene" + m_level;
        TextAsset info = Resources.Load<TextAsset>(fname);

        m_drama = JsonUtility.FromJson<DramaData>(info.text);
        m_drama.Init();
    }
    /// <summary>
    /// 开始新的飞机
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
    /// 根据脚本生成敌人
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
    /// 重启本关，也用于开始下一关
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
//        m_fGameTimer = 0f;    //游戏进行时间，每次update累计，故事线按游戏时间推进。
        m_drama.Init();
        foreach(GameObject rp in ReadyPlane)
        {
            rp.SetActive(true);
        }

        StartNewPlane();
    }
    /// <summary>
    /// 每关最后大BOSS死后调用，判断进入下关或结束
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
