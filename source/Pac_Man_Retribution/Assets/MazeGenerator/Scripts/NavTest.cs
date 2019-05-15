using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavTest : MonoBehaviour
{
    public bool isColliding;

    // Start is called before the first frame update
    void Start()
    {
        isColliding = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag.Equals("Wall")) isColliding = true;
    }



}
