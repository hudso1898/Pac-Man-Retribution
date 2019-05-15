using UnityEngine;
using System.Collections;
using System;
using TMPro;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System.Collections.Generic;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBall : MonoBehaviour
{

    private static float MOVESPEED = (float)0.5;

    public GameObject ViewCamera = null;
    public AudioClip JumpSound = null;
    public AudioClip HitSound = null;
    public AudioClip CoinSound = null;
    public AudioClip FireSound = null;
    public AudioClip StartSound = null;
    public AudioClip PlayerHitSound = null;
    public AudioClip PlayerDeathSound = null;
    public AudioClip deathSound = null;
    public AudioClip gameOverSound = null;
    public AudioClip levelUpSound = null;
    public AudioClip levelUpPlusSound = null;
    //public AudioClip GameMusic = null;
    public Rigidbody Projectile = null;
    public Rigidbody BallFrag = null;
    public Rigidbody glassTop = null;
    public Enemy enemyControl = null;
    public Music musicControl = null;
    public MazeSpawner maze = null;
    public PowerupControl pControl;

    private float projSpeed = 25.0f;

    public enum Upgrade
    {
        damage = 0, shield = 1, piercing = 2, rapidFire = 3, powerUp = 4, phaseShift = 5, speed = 6
    }


    private LinkedList<GameObject> pellets;
    private Rigidbody Ball = null;
    private AudioSource mAudioSource = null;
    private GameObject scoreText = null;
    private TextMeshProUGUI scoreDisp = null;
    public TextMeshProUGUI levelDisp = null;
    public TextMeshProUGUI timeDisp = null;
    public TextMeshProUGUI healthDisp = null;
    public TextMeshProUGUI maxHealthDisp = null;
    public TextMeshProUGUI damageDisp = null;
    public TextMeshProUGUI dUpDisp, frUpDisp, pDisp, sDisp, puDisp, psDisp, spDisp = null;
    private int score;
    private int targetScore;
    private int numZeros;
    private System.Random rand = null;
    public float reloadTime = 0.4f;
    private float reloadCounter;
    private float gameTime;
    private bool gameStarted;
    public int level;
    private float timeLeft;
    public float levelTime;
    private Vector3 currentDirection;
    private int timeTemp;
    private int maxHealth;
    private int maxHealthd;
    private int uc2, uc3 = 0;
    private int health;
    private int targetHealth;
    private int damage;
    private float collideDeltaT;
    public bool isAlive;
    private float deltaT;
    private bool isDamageUpgrade;
    private bool isPhase, isPierce, isShielded, isRapid = false;
    private float dCount, sCount, xCount, hCount, iCount, fCount, uCount = 0.0f;
    private bool isDUp, isSUp, isX, isInv, isFa = false;
    private float pTime = 10.0f;
    private Vector3[] directions;
    private int pelletsCollected;
    private int maxPellets = 15 * 15; // 15 rows x 15 columns



    private bool[] upgrades;

    private TextMeshProUGUI[] upgradeDisps;
    void Randomize(Color color)
    {
        color.r = (float)rand.NextDouble();
        color.g = (float)rand.NextDouble();
        color.b = (float)rand.NextDouble();
    }

    void Start()
    {

        Ball = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();
        scoreText = GameObject.FindWithTag("ScoreDisplay");
        scoreDisp = scoreText.GetComponent<TextMeshProUGUI>();
        score = 0;
        rand = new System.Random(DateTime.Now.Millisecond);
        reloadCounter = reloadTime;
        currentDirection = transform.forward;
        mAudioSource.PlayOneShot(StartSound, 0.15f);

        level = 1;
        timeLeft = levelTime;
        maxHealth = 100;
        maxHealthd = 100;
        health = 100;
        targetHealth = 100;
        healthDisp.faceColor = Color.green;
        damage = 20;
        collideDeltaT = 0.0f;
        isAlive = true;
        deltaT = 0;
        pellets = maze.pellets;

        upgradeDisps = new TextMeshProUGUI[7];
        upgradeDisps[0] = dUpDisp; upgradeDisps[1] = sDisp;
        upgradeDisps[2] = pDisp; upgradeDisps[3] = frUpDisp;
        upgradeDisps[4] = puDisp; upgradeDisps[5] = psDisp;
        upgradeDisps[6] = spDisp;

        upgrades = new bool[7];
        int i;
        for (i = 0; i < 7; i++) upgrades[i] = false;

        UpdateUpgrades();

        isDamageUpgrade = false;

        directions = new Vector3[4];
        directions[0] = transform.forward.normalized;
        directions[1] = transform.forward.normalized * -1;
        directions[2] = transform.right.normalized;
        directions[3] = transform.right.normalized * -1;



    }

    void FixedUpdate()
    {

        if (targetScore > score)
        {
            numZeros = FindLength(score);
            scoreDisp.text = targetScore.ToString().PadLeft(numZeros, '0');
        }

        if (Ball != null && isAlive)
        {
            if (Input.GetButton("Horizontal"))
            {
                Ball.MovePosition(transform.position + (transform.right * Input.GetAxis("Horizontal")) * MOVESPEED);
                currentDirection = (transform.right * Input.GetAxis("Horizontal")).normalized;

            }
            if (Input.GetButton("Vertical"))
            {
                Ball.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical")) * MOVESPEED);
                currentDirection = (transform.forward * Input.GetAxis("Vertical")).normalized;

            }
            if (Input.GetKey("space"))
            {
                if (reloadTime.CompareTo(reloadCounter) <= 0)
                {
                    if (!isX) FireProjectile();
                    else FireXProjectile();
                    reloadCounter = 0.0f;
                }
            }
            reloadCounter += Time.deltaTime;
        }

    }

    void Update()
    {

        if (pellets == null) pellets = maze.pellets;
        if (!isAlive)
        {
            if (deltaT >= 1.0f)
            {
                scoreDisp.alpha = 0;
                deltaT = 0.0f;

            }
            else if (deltaT >= 0.5f) scoreDisp.alpha = 1;
            deltaT += Time.deltaTime;
            return;
        }
        if (health <= 0)
        {
            isAlive = false;
            musicControl.StopMusic();
            Fragment(Ball.transform, maxHealth);

            Destroy(this.gameObject.GetComponent<MeshRenderer>());
            gameObject.GetComponent<SphereCollider>().enabled = false;
            healthDisp.text = "0";
            targetHealth = 0;
            mAudioSource.PlayOneShot(PlayerDeathSound);

            //save highscore in file
            string[] s = System.IO.File.ReadAllLines(@"highscores.txt");
            int temp = int.Parse(scoreDisp.text);
            int[] n = new int[5];
            n[0] = int.Parse(s[0]);
            n[1] = int.Parse(s[1]);
            n[2] = int.Parse(s[2]);
            n[3] = int.Parse(s[3]);
            n[4] = int.Parse(s[4]);

            if (n[4] < temp)
            {
                n[4] = temp;
                Array.Sort(n);
                Array.Reverse(n);
                s[0] = n[0].ToString();
                s[1] = n[1].ToString();
                s[2] = n[2].ToString();
                s[3] = n[3].ToString();
                s[4] = n[4].ToString();
                System.IO.File.WriteAllLines(@"highscores.txt", s);
            }
        }

        if (maxHealthd < maxHealth)
        {
            if (uc2 >= 1)
            {
                maxHealthd++;
                maxHealthDisp.SetText(maxHealthd.ToString());
                uc2 = 0;
            }
            else uc2++;
        }
        if (targetHealth < health)
        {
            if (uc3 >= 1)
            {
                healthDisp.text = (--health).ToString();
                uc3 = 0;
            }
            else uc3++;
        }


        else if (targetHealth > health)
        {
            if (uc3 >= 1)
            {
                healthDisp.text = (++health).ToString();
                uc3 = 0;
            }
            else uc3++;

        }
        if ((maxHealth - health) <= maxHealth / 2)
        {
            healthDisp.faceColor = Color.green;
            maxHealthDisp.faceColor = Color.green;
        }
        else if ((maxHealth - health) >= maxHealth - (maxHealth / 5))
        {
            healthDisp.faceColor = Color.red;
            maxHealthDisp.faceColor = Color.red;
        }
        else
        {
            healthDisp.faceColor = Color.yellow;
            maxHealthDisp.faceColor = Color.yellow;
        }

        // <powerups>

        if (dCount <= 0.0f)
        {
            isDUp = false;
            pControl.UnsetPowerup(0);
        }
        else dCount -= Time.deltaTime;

        if (sCount <= 0.0f)
        {
            isSUp = false;
            pControl.UnsetPowerup(1);
        }
        else sCount -= Time.deltaTime;

        if (xCount <= 0.0f)
        {
            isX = false;
            pControl.UnsetPowerup(2);
        }
        else xCount -= Time.deltaTime;

        if (hCount <= 0.0f)
        {
            pControl.UnsetPowerup(3);
        }
        else hCount -= Time.deltaTime;

        if (iCount <= 0.0f)
        {
            isInv = false;
            pControl.UnsetPowerup(4);
        }
        else iCount -= Time.deltaTime;

        if (fCount <= 0.0f)
        {
            isFa = false;
            if (isRapid) reloadTime = 0.3f;
            else reloadTime = 0.4f;
            pControl.UnsetPowerup(5);
        }
        else fCount -= Time.deltaTime;

        if (uCount <= 0.0f)
        {
            pControl.UnsetPowerup(6);
        }
        else uCount -= Time.deltaTime;

        // </powerups>

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0.0f) NewLevel();
        else
        {
            timeTemp = (int)timeLeft;
            timeDisp.SetText(timeTemp.ToString());
        }


    }
    private void Fragment(Transform pos, int numFrags)
    {
        int i = 0;
        for (i = 0; i < numFrags; i++)
        {

            Rigidbody fragment = (Rigidbody)Instantiate(BallFrag, pos.position, pos.rotation);
            fragment.AddExplosionForce(100.0f, fragment.transform.position, 10.0f, 0);
            Physics.IgnoreCollision(fragment.GetComponent<Collider>(), glassTop.GetComponent<Collider>(), true);



        }
    }

    private void NewUpgrade()
    {
        if (NoNewUpgrades()) return;

        int upgradeToAdd;

        upgradeToAdd = rand.Next() % 7;
        if (upgrades[upgradeToAdd])
        {
            NewUpgrade();
            return;
        }

        upgrades[upgradeToAdd] = true;
        UpdateUpgrades();
        ApplyUpgrade(upgradeToAdd);
    }

    private void UpdateUpgrades()
    {
        int i;
        for(i=0; i<7; i++)
        {
            if (upgrades[i]) upgradeDisps[i].alpha = 1;
            else upgradeDisps[i].alpha = 0.3f;
        }
    }
    private bool NoNewUpgrades()
    {
        int i = 0;
        for(i=0; i<7; i++)
        {
            if (!upgrades[i]) return false;
        }
        return true;
    }

    private void ApplyUpgrade(int upgradeToAdd)
    {
        Debug.Log("Applying upgrade " + upgradeToAdd.ToString());
        switch(upgradeToAdd)
        {
            case 0: // damage+
                damage = (int)(damage * 1.5);
                isDamageUpgrade = true;
                break;

            case 1: // shield+
                isShielded = true;
                Behaviour halo = (Behaviour)GetComponent("Halo");
                halo.enabled = true;

                break;

            case 2: // piercing
                isPierce = true;
                break;

            case 3: // rapid fire
                reloadTime *= 0.75f;
                reloadCounter = reloadTime;
                isRapid = true;
                break;

            case 4: // powerup+
                pTime *= 1.5f;
                break;

            case 5: // phase shift
                isPhase = true;
                break;

            default: // speed+
                MOVESPEED *= 2;
                break;
        }
    }
    private void NewLevel()
    {
        level++;
        levelDisp.SetText(level.ToString());
        timeLeft = levelTime;
        timeTemp = (int)timeLeft;
        timeDisp.SetText(timeTemp.ToString());
        int healthToAdd = 5 + (rand.Next() % 20);
        if(maxHealth >= 500)
        {
            maxHealth = 500; // maximum health
            healthToAdd = 0;
        }
        maxHealth += healthToAdd;
        targetHealth += healthToAdd;
        if (targetHealth > maxHealth) targetHealth = maxHealth;
        if (level % 5 == 0)
        {
            NewUpgrade();
            targetHealth += (maxHealth / 4); // heal 25% every 5 levels
            if (targetHealth > maxHealth) targetHealth = maxHealth;
        }
        damage += 1 + (rand.Next() % 4);
        if (!isDamageUpgrade && damage > 100) damage = 100;
        else if (damage > 150) damage = 150;
        damageDisp.SetText("Damage  " + damage.ToString());

        enemyControl.SetProps(rand.Next() % 6, 1 + rand.Next() % 3, 0.25f, "red");
        if (level > 5) enemyControl.SetProps(rand.Next() % 5, rand.Next() % 3, 0.1f, "orange");
        if (level > 10) enemyControl.SetProps(rand.Next() % 20, rand.Next() % 10, 0.4f, "cyan");
        if (level > 15) enemyControl.SetProps(0, rand.Next() % 25, 0, "pink");
        AddScore(50 * (level - 1));
        if (level % 5 == 0) mAudioSource.PlayOneShot(levelUpPlusSound, 0.25f);
        else mAudioSource.PlayOneShot(levelUpSound, 0.25f);

        if (level % 10 == 0)
        {
            // every 5 levels
            // Add selection algorithm to select upgrade
            // enum type to represent different upgrades
            // then, have another function to modify properties based on what upgrade was chosen
            // 
            foreach (GameObject pel in pellets)
            {
                pel.GetComponent<Pellet>().SetUncollected();
            }
            pelletsCollected = 0;
        }




    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Pellet"))
        {
            if (mAudioSource != null && CoinSound != null)
            {
                mAudioSource.PlayOneShot(CoinSound, 0.15f);
            }
            if (scoreDisp != null)
            {
                this.AddScore(10 * ((level / 5) + 1));
            }
            if (level < 15) targetHealth++;
            else targetHealth += 2;
            if (targetHealth > maxHealth) targetHealth = maxHealth;

            other.gameObject.GetComponent<Pellet>().SetCollected();
            pelletsCollected++;
            if(pelletsCollected == maxPellets)
            {
                AddScore(1500);
            }
        }
        else if (other.gameObject.tag.Equals("Enemy") && !isInv)
        {
            if (!isShielded) targetHealth -= other.gameObject.GetComponent<EnemyProps>().damage;
            else targetHealth -= (int)(other.gameObject.GetComponent<EnemyProps>().damage * 0.7);
            int n = other.gameObject.GetComponent<EnemyProps>().damage;
            Fragment(Ball.transform, Math.Max(n, 50));
            mAudioSource.PlayOneShot(PlayerHitSound, 0.4f);
        }
        else if (other.gameObject.tag.Equals("Enemy") && isInv)
        {
            mAudioSource.PlayOneShot(deathSound, 0.6f);
            if (other.GetComponent<EnemyProps>().enemyType.Equals("red")) AddScore(75);
            else if (other.GetComponent<EnemyProps>().enemyType.Equals("orange")) AddScore(150);
            else if (other.GetComponent<EnemyProps>().enemyType.Equals("cyan")) AddScore(250);
            else AddScore(1000);
            enemyControl.DestroyEnemy(other.gameObject);
        }
        else if (other.gameObject.tag.Equals("Powerup"))
        {
            int id = other.GetComponent<Powerup>().pId;
            pControl.ApplyPowerup(id);
            Destroy(other.gameObject);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy") && !isInv)
        {
            if (collideDeltaT >= 1.2f)
            {
                if (!isShielded) targetHealth -= other.gameObject.GetComponent<EnemyProps>().damage;
                else targetHealth -= (int)(other.gameObject.GetComponent<EnemyProps>().damage * 0.7);
                int n = other.gameObject.GetComponent<EnemyProps>().damage;
                Fragment(Ball.transform, Math.Max(n, 50));
                mAudioSource.PlayOneShot(PlayerHitSound, 0.4f);
                collideDeltaT = 0.0f;
            }
            collideDeltaT += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy")) collideDeltaT = 0.0f;
    }
    private int FindLength(int input)
    {
        int num = 0;
        while (input <= 100000)
        {
            input = (10 * (int)Math.Pow((double)10, (double)num));
            num++;
        }
        return num;
    }

    public void SetPos(float x, float y, float z)
    {
        Ball.position.Set(x, y, z);
    }

    public void FireProjectile()
    {
        Rigidbody proj = (Rigidbody)Instantiate(Projectile, transform.position, transform.rotation);
        proj.velocity = GetComponent<Rigidbody>().velocity + (currentDirection * 25.0f);
        proj.velocity = Vector3.Normalize(proj.velocity) * projSpeed;


        if (isDUp) proj.GetComponent<Projectile>().damage = this.damage * 2;
        else proj.GetComponent<Projectile>().damage = this.damage;
        proj.GetComponent<Projectile>().isPhase = isPhase;
        proj.GetComponent<Projectile>().isPierce = isPierce;
        mAudioSource.PlayOneShot(FireSound, 0.15f);
        Physics.IgnoreCollision(proj.GetComponent<Collider>(), GetComponent<Collider>(), true);

    }
    public void FireXProjectile()
    {
        int i;
        for (i = 0; i <= 3; i++)
        {
            Rigidbody proj = (Rigidbody)Instantiate(Projectile, transform.position, transform.rotation);
            proj.velocity = GetComponent<Rigidbody>().velocity + (directions[i] * 25.0f);
            proj.velocity = Vector3.Normalize(proj.velocity) * projSpeed;
            if (isDUp) proj.GetComponent<Projectile>().damage = this.damage * 2;
            else proj.GetComponent<Projectile>().damage = this.damage;
            proj.GetComponent<Projectile>().isPhase = isPhase;
            proj.GetComponent<Projectile>().isPierce = isPierce;

            Physics.IgnoreCollision(proj.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        mAudioSource.PlayOneShot(FireSound, 0.15f);

    }
    public void AddScore(int addScore)
    {
        if (isSUp) targetScore += addScore * 2;
        else targetScore += addScore;


    }
    public void PlayDeathSound()
    {

        mAudioSource.PlayOneShot(deathSound);
    }
    public void PlayGOSound()
    {

        mAudioSource.PlayOneShot(gameOverSound);
    }

    public void SetDamageUp()
    {
        isDUp = true;
        dCount = pTime;
    }
    public void SetScoreUp()
    {
        isSUp = true;
        sCount = pTime * 1.5f;
    }
    public void SetXShot()
    {
        isX = true;
        xCount = pTime;
    }
    public void Heal()
    {
        targetHealth += (int)(maxHealth * 0.5f);
        if (targetHealth > maxHealth) targetHealth = maxHealth;
        hCount = 3.0f;
    }
    public void SetInvulnerable()
    {
        isInv = true;
        iCount = pTime;
    }
    public void SetFullAuto()
    {
        reloadTime = 0.075f;
        fCount = pTime;
    }
    public void SetUlt()
    {
        uCount = 5.0f;
    }
}
