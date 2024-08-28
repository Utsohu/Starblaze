using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class ReadyPlane : MonoBehaviour
{
    public GameObject Myplane;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void PlaneReady()
    {
        Myplane.transform.position = this.transform.position;
        MyPlane mp = Myplane.GetComponent<MyPlane>();
        mp.SetReady(true);
        Destroy(gameObject);
    }
}
