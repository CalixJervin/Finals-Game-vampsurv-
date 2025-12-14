using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject playerData;

    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentDamage;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;
    [HideInInspector]
    public float currentPoints;
    
    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;

    public List<GameObject> spawnedWeapons;

    [Header("Experience/Level")]
    public int level = 1;
    public int experience = 0;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int expCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;



    public List<LevelRange> levelRanges;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        experienceCap = levelRanges[0].expCapIncrease;
    }

    void Update()
    {
        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        } else if(isInvincible)
        {
            isInvincible = false;
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth > playerData.MaxHealth)
        {
            currentHealth = playerData.MaxHealth;
        }
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int expCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    expCapIncrease = range.expCapIncrease;
                    break;
                }
            }
            experienceCap += expCapIncrease;
        }
    }

    void Awake()
    {
        playerData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        currentHealth = playerData.MaxHealth;
        currentMoveSpeed = playerData.MoveSpeed;
        currentDamage = playerData.Damage;
        currentRecovery = playerData.Recovery;
        currentProjectileSpeed = playerData.ProjectileSpeed;
        currentMagnet = playerData.Magnet;
        currentPoints = playerData.Points;

        SpawnWeapon(playerData.StartingWeapon);
    }

    public void TakeDamage(float dmg)
    {
        if(!isInvincible)
        {
            currentHealth -= dmg;
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            
            StartCoroutine(DamageFlash());

            if(currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        Debug.Log("Player Died");
    }

    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        spawnedWeapons.Add(spawnedWeapon);
    }

    public void IncreaseScore(float amount)
    {
        currentPoints += amount;
        Debug.Log("Current Score: " + currentPoints); // Useful to check if it works
        
    }
}
