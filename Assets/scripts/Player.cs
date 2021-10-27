using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player i;

    public bool dead = false;

    Rigidbody2D rb;

    [SerializeField]
    float deathAnimVel;

    [SerializeField]
    float deathAnimPushTime;

    [SerializeField]
    float respawnTime;

    [SerializeField]
    float minDeathAnimPushVariation;

    [SerializeField]
    float maxDeathAnimPushVariation;

    [SerializeField]
    GameObject dedParticles;

    [SerializeField]
    GameObject sprite;

    [SerializeField]
    GameObject sprite_1;

    playerMovement movement;

    bool crouched = false;

    void Start()
    {
        i = this;
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<playerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (movement.isGrounded && !crouched)
            {
                if (sprite.activeInHierarchy)
                {
                    sprite.GetComponent<Animator>().SetTrigger("crouch");
                }
                if (sprite_1.activeInHierarchy)
                {
                    sprite_1.GetComponent<Animator>().SetTrigger("crouch");
                }
                crouched = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (crouched)
            {
                if (sprite.activeInHierarchy)
                {
                    sprite.GetComponent<Animator>().SetTrigger("uncrouch");
                }
                if (sprite_1.activeInHierarchy)
                {
                    sprite_1.GetComponent<Animator>().SetTrigger("uncrouch");
                }
                crouched = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == 7 && !dead)
        {
            RoomManager.i.Shake(0.3f, 2, 10);

            GetComponent<playerMovement>().enabled = false;
            rb.gravityScale = 0;
            dead = true;
            Vector2 normal = col.contacts[0].normal.normalized;
            float variation =
                Random
                    .Range(minDeathAnimPushVariation,
                    maxDeathAnimPushVariation);
            float dir = Mathf.Atan2(normal.y, normal.x) + variation;

            rb.velocity =
                new Vector2(Mathf.Cos(dir), Mathf.Sin(dir)) * deathAnimVel;
            Invoke("DeathAnimStage2", deathAnimPushTime);

            GameObject deathParticles = Instantiate(dedParticles, transform);
            deathParticles.transform.rotation =
                Quaternion.Euler(0, 0, variation);
            Destroy(deathParticles, 3f);
        }
    }

    void DeathAnimStage2()
    {
        //GetComponent<SpriteRenderer>().enabled = false;
        rb.velocity = Vector2.zero;
        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        Vector2 respawnPoint = RoomManager.i.currentRoom.currentRespawn;
        transform.position = respawnPoint;
        GetComponent<playerMovement>().enabled = true;

        //GetComponent<SpriteRenderer>().enabled = true;
        dead = false;
    }
}
