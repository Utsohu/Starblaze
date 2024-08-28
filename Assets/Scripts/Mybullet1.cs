using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mybullet1 : MonoBehaviour
{
    public float Speed = 20f;
    Vector3 m_direction = new Vector3(0, 1, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += m_direction * Speed * Time.deltaTime;
        Vector3 s = Camera.main.WorldToScreenPoint(transform.position);
        if (s.x > Screen.width || s.y > Screen.height || s.x < 0 || s.y < 0)
            Destroy(this.gameObject);
    }

    public void SetDirect(Vector3 v)
    {
        m_direction = v;
    }
}
