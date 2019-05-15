using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float x, y, z;
    public RollerBall gameController = null;
    public Enemy enemyController = null;
    public AudioClip hitSound = null;
    public AudioClip deathSound = null;
    public int damage;
    public bool isPhase, isPierce = false;

    private Rigidbody objTemp;
    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<RollerBall>();
        enemyController = FindObjectOfType<Enemy>();
        
        
    }
    void Update()
    {
        this.x = this.gameObject.transform.position.x;
        this.y = this.gameObject.transform.position.y;
        this.z = this.gameObject.transform.position.z;
        if (OutOfBounds()) Destroy(this.gameObject);
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Wall") && !isPhase)
        {
            Destroy(this.gameObject);
        }
        else if (other.gameObject.tag.Equals("Enemy"))
        {

            other.gameObject.GetComponent<EnemyProps>().LoseHealth(damage);

           
            other.gameObject.GetComponent<EnemyProps>().AddFrags(Mathf.Max(damage, 40) - 10);
            if (other.gameObject.GetComponent<EnemyProps>().isDead())
            {
                gameController.GetComponent<AudioSource>().PlayOneShot(deathSound, 0.6f);
                if (other.GetComponent<EnemyProps>().enemyType.Equals("red")) gameController.AddScore(75);
                else if (other.GetComponent<EnemyProps>().enemyType.Equals("orange")) gameController.AddScore(150);
                else gameController.AddScore(250);
                enemyController.DestroyEnemy(other.gameObject);
            }
            else gameController.GetComponent<AudioSource>().PlayOneShot(hitSound, 0.6f);
            if(!isPierce) Destroy(this.gameObject);
        }

    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Wall") && !isPhase) Destroy(this.gameObject);
    }
    private bool OutOfBounds()
    {
        if (x < -5 || x >= 90 || z < -5 || z >= 90) return true;
        else return false;
    }
}
