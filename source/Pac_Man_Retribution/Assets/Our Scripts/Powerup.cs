using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public int pId;
    public float timeToLive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToLive <= 0.0f) Destroy(this.gameObject);
        else timeToLive -= Time.deltaTime;
    }
}
