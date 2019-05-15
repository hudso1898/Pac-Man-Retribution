using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProps : MonoBehaviour
{
    private int health;
    public int maxHealth;
    public Rigidbody glassTop = null;
    public Rigidbody EnemyPiece = null;
    public Rigidbody testProbe = null;
    private Rigidbody test;
    private Rigidbody enemy;
    public int damage;
    private bool isMoving;
    public RollerBall charControl = null;
    private NavMeshPath pathToChar;
    private NavMeshAgent agent;
    private enum Direction
    {
        up = 0, left = 2, down = 1, right = 3, unknown = 4

    }
    private Enemy enemyControl = null;
    public string enemyType = "";
    private Direction currentDir;
    private Vector3 movementVec;
    public float MOVESPEED;
    private Vector3 targetPos;
    private System.Random rand;
    private int moveCounter;
    private Vector3 origPos;
    private Vector3 pathPos;
    private Vector3 charPos;
    private Vector3[] directions = { new Vector3(0, 0, 0), new Vector3(0, -90, 0), new Vector3(180, 0, 0), new Vector3(0, 90, 0) };
    // Start is called before the first frame updater


        
    void Start()
    {
        enemyControl = FindObjectOfType<Enemy>();
        health = maxHealth;
        isMoving = false;
        enemy = GetComponent<Rigidbody>();
        rand = new System.Random();
        currentDir = Direction.up;
        moveCounter = 0;
        Physics.IgnoreCollision(GetComponent<Collider>(), glassTop.GetComponent<Collider>(), true);
        Physics.IgnoreLayerCollision(8, 9, true);
        Physics.IgnoreLayerCollision(9, 10, true);
        Physics.IgnoreLayerCollision(9, 11, false);
        Physics.IgnoreLayerCollision(9, 9, true);
        origPos = transform.position;
        enemy.freezeRotation = false;
        agent = GetComponent<NavMeshAgent>();
        pathToChar = agent.path;
        test = Instantiate<Rigidbody>(testProbe);
        test.transform.position = this.transform.position;




    }
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag.Equals("Wall"))
        {
            moveCounter = 0;
            transform.position = origPos;
            isMoving = false;
        }
    }
    private void FixedUpdate()
    {

        if(!isMoving)
        {
            isMoving = true;

            enemy.freezeRotation = false;
            charPos = charControl.transform.position;

            currentDir = DecideDirection();




            transform.rotation = Quaternion.identity;
            switch (currentDir)
            {
                case (Direction.up):
                    transform.Rotate(directions[0], Space.World);
                    

                    break;
                case (Direction.left):
                    transform.Rotate(directions[1], Space.World);


                    break;
                case (Direction.down):
                    transform.Rotate(directions[2], Space.World);


                    break;
                case (Direction.right):
                        transform.Rotate(directions[3], Space.World);
                    break;
                default:
                   

                    break;

            }
            test.GetComponent<NavTest>().isColliding = false;
            updateNavPos();
            movementVec = transform.forward * MOVESPEED;


            targetPos = transform.position + (movementVec * (float)(6 / MOVESPEED));

            //origPos = transform.position;


            enemy.freezeRotation = false;

        }

    }

    private Direction DecideDirection() {
        if (!test.GetComponent<NavTest>().isColliding)
        {

            return currentDir;
        }
        else
        {

            int i = rand.Next() % 2;
            if (i == 0)
            {
                switch (currentDir)
                {
                    case (Direction.up):
                        return Direction.right;
                    case (Direction.right):
                        return Direction.down;
                    case (Direction.down):
                        return Direction.left;
                    case (Direction.left):
                        return Direction.up;
                }
            }
            else
            {
                switch (currentDir)
                {
                    case (Direction.up):
                        return Direction.left;
                    case (Direction.right):
                        return Direction.up;
                    case (Direction.down):
                        return Direction.right;
                    case (Direction.left):
                        return Direction.down;
               }
            }
            return Direction.unknown;
        }
    }

    // Wall returns true if there's a wall in the current direction relative to the enemy
    private bool Wall()
    {
        
        if (test.GetComponent<NavTest>().isColliding)
        {


            return true;

        }
        else
        {
            //testPos = transform.position + movementVec * (6 / MOVESPEED);

           // test.GetComponent<NavTest>().isColliding = false;
            return false;
        }

    }

    void updateNavPos()
    {
        Vector3 testPos = transform.position + movementVec * (2 / MOVESPEED); // half of movement
        test.transform.position = testPos;

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < -5 || transform.position.x > 90 ||
        transform.position.z < -5 || transform.position.z > 90)
        {
            GetComponent<MeshRenderer>().enabled = false;
            enemyControl.DestroyEnemyNoFrag(this.gameObject);
        }

        updateNavPos();
        if(Wall())
        {
            moveCounter = 0;
            //transform.position = origPos;
            isMoving = false;

        }

        enemy.MovePosition(transform.position + movementVec);
            moveCounter++;
            //if ((6 / MOVESPEED) <= moveCounter)
            //{

            //    isMoving = false;
            //    moveCounter = 0;
            //}

        
    }
    public void LoseHealth(int h) { health -= h; }

    public bool isDead() { if (health <= 0) return true; else return false; }

    public void AddFrags(int numFrags)
    {
        int i = 0;
        for (i = 0; i < numFrags; i++)
        {

            Rigidbody fragment = (Rigidbody)Instantiate(EnemyPiece, transform.position, transform.rotation);
            fragment.AddExplosionForce(115.0f, fragment.transform.position, 10.0f, 0);
            Physics.IgnoreCollision(fragment.GetComponent<Collider>(),glassTop.GetComponent<Collider>(), true);




        }
    }

    public void SetProps(int health, int damage)
    {
        this.health += (health - maxHealth);
        this.maxHealth = health;

        this.damage = damage;
    }

    public void SetEnemyType(string s)
    {
        this.enemyType = s;
    }
}
