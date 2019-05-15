using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Enemy : MonoBehaviour
{

    private float deltaT;
    public Rigidbody DefaultEnemy = null;
    public Rigidbody FastEnemy = null;
    public Rigidbody SlowEnemy = null;
    public Rigidbody DeathEnemy = null;
    public Rigidbody EnemyPiece = null;
    public Rigidbody FastEPiece = null;
    public Rigidbody SlowEPiece = null;
    public Rigidbody DeathEPiece = null;
    public Rigidbody glassTop = null;

    public float minRespawnTime;

    public TextMeshProUGUI redDisp = null;
    public TextMeshProUGUI redProps = null;
    public TextMeshProUGUI orangeDisp = null;
    public TextMeshProUGUI orangeProps = null;
    public TextMeshProUGUI cyanDisp = null;
    public TextMeshProUGUI cyanProps = null;
    public TextMeshProUGUI pinkDisp = null;
    public TextMeshProUGUI pinkProps = null;

    public RollerBall gameControl = null;
    private System.Random rand;
    private LinkedList<GameObject> enemies;
    private float rSpawn, oSpawn, cSpawn, pSpawn;
    private int rHealth, rDamage;
    private int oHealth, oDamage;
    private int cHealth, cDamage;
    private int pHealth, pDamage;
    private int numRedEnemies, numOEnemies, numCEnemies, numPEnemies;
    private float deltaT2, dTo, dTc, dTp;
    
    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random();
        deltaT = 5.0f;
        enemies = new LinkedList<GameObject>();
        rHealth = 50;
        rDamage = 10;
        oHealth = 25;
        oDamage = 10;
        cHealth = 200;
        cDamage = 50;
        pHealth = 999999;
        pDamage = 100;
        rSpawn = 10.0f;
        oSpawn = 8.0f;
        cSpawn = 15.0f;
        pSpawn = 30.0f;
        numRedEnemies = 0;
        numOEnemies = 0;
        numCEnemies = 0;
        numPEnemies = 0;
        deltaT2 = 0.0f; dTo = 0.0f; dTc = 0.0f; dTp = 0.0f;
        UpdateDisplay();
        UpdateStats();


    }

    // Update is called once per frame
    void Update()
    {
        if(!gameControl.isAlive)
        {
            if (deltaT2 >= 2.0f)
            {
                foreach (GameObject ene in enemies)
                {
                    if (ene != null) {
                        DestroyEnemy(ene);
                        gameControl.PlayDeathSound();

                    }
                }
                GetComponent<AudioSource>().Play();
                deltaT2 = float.MinValue;
                redDisp.SetText("0");
            }
            else deltaT2 += Time.deltaTime;
        }
        else if (deltaT >= rSpawn && numRedEnemies < 100)
        {
            Rigidbody enemy = Instantiate(DefaultEnemy);
            enemy.GetComponent<EnemyProps>().SetProps(rHealth, rDamage);
            enemy.GetComponent<EnemyProps>().SetEnemyType("red");
            SetPosition(enemy);
            deltaT = 0.0f;
            numRedEnemies++;
            UpdateDisplay();
            enemies.AddLast(enemy.gameObject);
            
        }

        else if (dTo >= oSpawn && gameControl.GetComponent<RollerBall>().level >= 5 && numOEnemies < 50)
        {
            Rigidbody enemy = Instantiate(FastEnemy);
            enemy.GetComponent<EnemyProps>().SetProps(oHealth, oDamage);
            enemy.GetComponent<EnemyProps>().SetEnemyType("orange");
            SetPosition(enemy);
            numOEnemies++;
            UpdateDisplay();
            enemies.AddLast(enemy.gameObject);
            dTo = 0.0f;
        }
        else if (dTc >= cSpawn && gameControl.GetComponent<RollerBall>().level >= 10 && numCEnemies < 25)
        {
            Rigidbody enemy = Instantiate(SlowEnemy);
            enemy.GetComponent<EnemyProps>().SetProps(cHealth, cDamage);
            enemy.GetComponent<EnemyProps>().SetEnemyType("cyan");
            SetPosition(enemy);
            numCEnemies++;
            UpdateDisplay();
            enemies.AddLast(enemy.gameObject);
            dTc = 0.0f;
        }
        else if (dTp >= pSpawn && gameControl.GetComponent<RollerBall>().level >= 15 && numPEnemies < 2)
        {
            Rigidbody enemy = Instantiate(DeathEnemy);
            enemy.GetComponent<EnemyProps>().SetProps(pHealth, pDamage);
            enemy.GetComponent<EnemyProps>().SetEnemyType("pink");
            SetPosition(enemy);
            numPEnemies++;
            UpdateDisplay();
            enemies.AddLast(enemy.gameObject);
            dTp = 0.0f;
        }

        deltaT += Time.deltaTime;
        dTo += Time.deltaTime;
        dTc += Time.deltaTime;
        dTp += Time.deltaTime;
    }

    private void UpdateDisplay()
    {
        if (numRedEnemies >= 0) redDisp.SetText(numRedEnemies.ToString());
        else redDisp.SetText("0");
        if (numOEnemies >= 0) orangeDisp.SetText(numOEnemies.ToString());
        else redDisp.SetText("0");
        if (numCEnemies >= 0) cyanDisp.SetText(numCEnemies.ToString());
        else cyanDisp.SetText("0");
        if (numPEnemies >= 0) pinkDisp.SetText(numPEnemies.ToString());
        else pinkDisp.SetText("0");
    }
    private void SetPosition(Rigidbody enem)
    {
        int cellMax = 15;
        int xCell = rand.Next() % cellMax;
        int yCell = rand.Next() % cellMax;
        enem.transform.SetPositionAndRotation(new Vector3(xCell * 6, 1.0f, yCell * 6), Quaternion.identity);


    }

    public void DestroyEnemy(GameObject enemy)
    {
        string enemT = enemy.gameObject.GetComponent<EnemyProps>().enemyType;
        Transform pos = enemy.transform;
        Destroy(enemy.gameObject);
        if (!enemT.Equals("pink")) Fragment(pos, Math.Max(enemy.gameObject.GetComponent<EnemyProps>().maxHealth / 2, 50), enemT);
        else Fragment(pos, 100, enemT); 

        if (enemT.Equals("red")) numRedEnemies--;
        else if (enemT.Equals("orange")) numOEnemies--;
        else if (enemT.Equals("cyan")) numCEnemies--;
        else numPEnemies--;
        UpdateDisplay();
    }

    public void DestroyEnemyNoFrag(GameObject enemy)
    {
        string enemT = enemy.gameObject.GetComponent<EnemyProps>().enemyType;
        Destroy(enemy.gameObject);
        if (enemT.Equals("red")) numRedEnemies--;
        else if (enemT.Equals("orange")) numOEnemies--;
        else if (enemT.Equals("cyan")) numCEnemies--;
        else numPEnemies--;
        UpdateDisplay();
    }

    private void Fragment(Transform pos, int numFrags, string type)
    {
        int i = 0;
        Rigidbody fragment;
        for (i = 0; i < numFrags; i++)
       
        {
            if (type.Equals("red"))
            {
                fragment = (Rigidbody)Instantiate(EnemyPiece, pos.position, pos.rotation);
            }
            else if (type.Equals("orange"))
            {

                fragment = (Rigidbody)Instantiate(FastEPiece, pos.position, pos.rotation);
            }
            else if (type.Equals("cyan"))
            {
                fragment = (Rigidbody)Instantiate(SlowEPiece, pos.position, pos.rotation);
            }
            else fragment = (Rigidbody)Instantiate(DeathEPiece, pos.position, pos.rotation);
            fragment.AddExplosionForce(100.0f, fragment.transform.position, 10.0f, 0);
            Physics.IgnoreCollision(fragment.GetComponent<Collider>(), glassTop.GetComponent<Collider>(), true);



        }
    }

    public void UpdateStats()
    {
        redProps.SetText("h " + rHealth.ToString() + "\nd " + rDamage.ToString());
        orangeProps.SetText("h " + oHealth.ToString() + "\nd " + oDamage.ToString());
        cyanProps.SetText("h " + cHealth.ToString() + "\nd " + cDamage.ToString());
        pinkProps.SetText("h nan\nd " + pDamage.ToString());
    }

    public void SetProps(int health, int damage, float spawnTime, string enemyName)
    {
        if (enemyName.Equals("red"))
        {
            rHealth += health;
            rDamage += damage;
            if(rSpawn > minRespawnTime) rSpawn -= spawnTime;
            UpdateStats();
            foreach (GameObject ene in enemies)
            {
                if (ene != null && ene.GetComponent<EnemyProps>().enemyType.Equals("red")) ene.GetComponent<EnemyProps>().SetProps(rHealth, rDamage);
            }
        }
        else if (enemyName.Equals("orange"))
        {
            oHealth += health;
            oDamage += damage;
            if(oSpawn > minRespawnTime) oSpawn -= spawnTime;
            UpdateStats();
            foreach (GameObject ene in enemies)
            {
                if (ene != null && ene.GetComponent<EnemyProps>().enemyType.Equals("orange")) ene.GetComponent<EnemyProps>().SetProps(oHealth, oDamage);
            }
        }
        else if (enemyName.Equals("cyan"))
        {
            cHealth += health;
            cDamage += damage;
            if(cSpawn > minRespawnTime) cSpawn -= spawnTime;
            UpdateStats();
            foreach (GameObject ene in enemies)
            {
                if (ene != null && ene.GetComponent<EnemyProps>().enemyType.Equals("cyan")) ene.GetComponent<EnemyProps>().SetProps(cHealth, cDamage);
            }
        }
        else
        {
            pDamage += damage;
            UpdateStats();
            foreach (GameObject ene in enemies)
            {
                if (ene != null && ene.GetComponent<EnemyProps>().enemyType.Equals("pink")) ene.GetComponent<EnemyProps>().SetProps(pHealth, pDamage);
            }

        }
    }

    public void KillAll()
    {
        foreach (GameObject ene in enemies)
        {
            if (ene != null)
            {
                string type = ene.GetComponent<EnemyProps>().enemyType;
                if (type.Equals("red")) gameControl.AddScore(75);
                else if (type.Equals("orange")) gameControl.AddScore(150);
                else if (type.Equals("cyan")) gameControl.AddScore(250);
                else gameControl.AddScore(500);
                DestroyEnemy(ene);
                gameControl.PlayDeathSound();

            }
        }
    }


}
