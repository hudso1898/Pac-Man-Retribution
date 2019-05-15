using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerupControl : MonoBehaviour
{
    private float spawnC = 0.0f;
    private float uSpawnC = 0.0f;
    public float spawnTime = 15.0f;
    public float ultSpawnTime = 60.0f;
    private bool[] powerups;
    private TextMeshProUGUI[] pupDisps;
    public TextMeshProUGUI dUpDisp, scoreDisp, xDisp, healDisp, invDisp, fireDisp, uDisp = null;
    private System.Random rand;
    public RollerBall playerControl = null;
    private Enemy enemyControl;

    public Rigidbody damagePowerup, scorePowerup, xPowerup, healPowerup, invPowerup, firePowerup, ultPowerup = null;
    // Start is called before the first frame update
    void Start()
    {
        rand = new System.Random(DateTime.Now.Millisecond);
        powerups = new bool[7];
        int i;
        for (i = 0; i < 7; i++) powerups[i] = false;

        pupDisps = new TextMeshProUGUI[7];
        pupDisps[0] = dUpDisp; pupDisps[1] = scoreDisp;
        pupDisps[2] = xDisp; pupDisps[3] = healDisp;
        pupDisps[4] = invDisp; pupDisps[5] = fireDisp;
        pupDisps[6] = uDisp;

        enemyControl = FindObjectOfType<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerControl.isAlive) return;
        if(spawnC >= spawnTime)
        {
            int toSpawn = rand.Next() % 6;
            SpawnPowerup(toSpawn);
            spawnC = 0.0f - (float)(rand.NextDouble() * 0.5f);
        }
        else if (uSpawnC >= ultSpawnTime && playerControl.level >= 10)
        {
            SpawnPowerup(6);
            uSpawnC = 0.0f - (float)(rand.NextDouble() * 5.0f);
        }
        spawnC += Time.deltaTime;
        uSpawnC += Time.deltaTime;
        UpdatePowerups();
    }

    private void SpawnPowerup(int toSpawn)
    {
        Rigidbody newUp;
        switch(toSpawn)
        {
            case 0:
                newUp = Instantiate(damagePowerup);
          
                break;

            case 1:
                newUp = Instantiate(scorePowerup);

                break;

            case 2:
                newUp = Instantiate(xPowerup);
               
                break;

            case 3:
                newUp = Instantiate(healPowerup);

                break;

            case 4:
                newUp = Instantiate(invPowerup);
               
                break;

            case 5:
                newUp = Instantiate(firePowerup);

                break;

            default:
                newUp = Instantiate(ultPowerup);
               
                break;
        }
        SetPosition(newUp);
    }

    private void SetPosition(Rigidbody pUp)
    {
        int cellMax = 15;
        int xCell = rand.Next() % cellMax;
        int yCell = rand.Next() % cellMax;
        pUp.transform.SetPositionAndRotation(new Vector3(xCell * 6, 1.0f, yCell * 6), Quaternion.identity);


    }

    private void UpdatePowerups()
    {
        int i;
        for (i = 0; i < 7; i++)
        {
            if (powerups[i]) pupDisps[i].alpha = 1;
            else pupDisps[i].alpha = 0.3f;
        }
    }

    public void SetPowerup(int toAdd) {
        powerups[toAdd] = true;
    }
    public void UnsetPowerup(int toLose)
    {
        powerups[toLose] = false;
    }
    public void ApplyPowerup(int pToAdd)
    {
        Debug.Log("Applying powerup " + pToAdd.ToString());
        SetPowerup(pToAdd);
        switch (pToAdd)
        {
            case 0: // damage x2
                playerControl.SetDamageUp();
                break;

            case 1: // score x2
                playerControl.SetScoreUp();

                break;

            case 2: // xShot
                playerControl.SetXShot();
                break;

            case 3: // heal
                playerControl.Heal();
                break;

            case 4: // invulnerability
                playerControl.SetInvulnerable();
                break;

            case 5: // full auto
                playerControl.SetFullAuto();
                break;

            default: // ultima
                playerControl.SetUlt();
                enemyControl.KillAll();
                break;
        }
    }
}
