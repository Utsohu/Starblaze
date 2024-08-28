using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSprite : MonoBehaviour
{
    public float Switchfreq = 0.1f;//精灵切换频率
    public Sprite[] Sprites;

    /*********用于移动变化*********/
    SpriteRenderer m_spriteRenderer;
    int m_index = 0;
    float m_calTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Sprites.Length > 0)//精灵切换
        {
            if ((Time.time - m_calTime) > Switchfreq)
            {
                m_spriteRenderer.sprite = Sprites[m_index++];
                if (m_index >= Sprites.Length)
                    m_index = 0;
                m_calTime = Time.time;
            }
        }
    }
}
