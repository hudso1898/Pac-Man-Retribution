using UnityEngine;
using System.Collections;
using System;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class AI : MonoBehaviour {

    private static Vector3 move = new Vector3(0,0,5);

	public GameObject ViewCamera = null;

	private Rigidbody ai = null;
	private bool mFloorTouched = false;

	void Start () {
		ai = GetComponent<Rigidbody> ();
    }

	void Update () {
		if (ai != null) {     
            ai.velocity = move;
        }
    }

    void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag.Equals ("Wall")) {
			if(move.x == 5){
                move.x = -5;
            }else if(move.x == -5){
                move.z = 5;
            }else if(move.z == 5){
                move.z = -5;
            }else if(move.z == -5){
                move.x = 5;
            }else{

            }
            
		}
	}
		




}
