using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPiece : MonoBehaviour
{
    private float timeToLive = 60.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0.0f) Destroy(this.gameObject);

    }
}
