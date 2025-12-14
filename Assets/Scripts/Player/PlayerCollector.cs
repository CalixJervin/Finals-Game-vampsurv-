using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{   
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    public AudioClip collectSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        player = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDirection = (transform.position - col.transform.position).normalized;
                rb.AddForce(forceDirection * pullSpeed);
            }

            collectible.Collect();

            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }
}
