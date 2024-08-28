using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadScene : MonoBehaviour
{
    public Slider o_sliderLoading;
    public Text o_sLoading;
    AsyncOperation m_async;

    static string g_TargeScene;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "LoadingScene")
        {
            m_async = SceneManager.LoadSceneAsync(g_TargeScene);
            m_async.allowSceneActivation = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (o_sLoading && o_sliderLoading)
        {
            o_sLoading.text = "Loading " + m_async.progress / 0.9f * 100 + "%";
            o_sliderLoading.value = m_async.progress / 0.9f;
            if (m_async.progress >= 0.9f)
            {
                o_sLoading.text = "Press any key to start";
            }
            if (Input.anyKeyDown)
            {
                m_async.allowSceneActivation = true;
            }

        }
    }
    public void JumpScene(string target)
    {
        g_TargeScene = target;
        SceneManager.LoadScene("LoadingScene");
    }
}


