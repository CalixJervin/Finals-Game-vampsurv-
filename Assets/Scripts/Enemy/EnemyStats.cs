using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentDamage;
    [HideInInspector]
    public float points;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;
    Transform player;

    //Soundfx
    public AudioClip hitSound;
    public AudioClip killSound;
    private AudioSource audioSource;

    void Awake()
    {
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
        currentDamage = enemyData.Damage;
        points = enemyData.Points;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();
        audioSource = GetComponent<AudioSource>();
        
        //Sound
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        if(knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <=0)
        {
            Kill();
        }
    }
    

    void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
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
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.IncreaseScore(points);
        }
        if (killSound != null)
        {
            AudioSource.PlayClipAtPoint(killSound, transform.position);
        }
        DisableHitbox();
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;
        
        while(t < deathFadeTime) 
        {
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        Destroy(gameObject);
        
    }

    void DisableHitbox()
{
    // 1. Get the collider component attached to this object
    Collider2D myCollider = GetComponent<Collider2D>();

    // 2. Turn it off
    if (myCollider != null)
    {
        myCollider.enabled = false;
    }
}
}