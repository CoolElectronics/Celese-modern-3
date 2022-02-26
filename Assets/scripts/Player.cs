using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    float dotThreshold;

    [SerializeField]
    GameObject dedParticles;

    [SerializeField]
    GameObject sprite;

    [SerializeField]
    GameObject sprite_1;
    [SerializeField]
    TileBase crumblingTile;

    playerMovement movement;

    bool crouched = false;

    List<Vector3Int> crumbledTiles = new List<Vector3Int>();
    void Awake()
    {

        i = this;
    }

    void Start()
    {
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

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Battery")
        {
            if (movement.dashes != movement.maxDashCount)
            {
                col.gameObject.SetActive(false);
                this.Invoke(() => col.gameObject.SetActive(true), 1);
                movement.dashes = movement.maxDashCount;
            }
        }
        Tilemap hmap = NewCameraController.i.triggerMap;

        Vector3Int tilepos = hmap.WorldToCell(col.ClosestPoint(transform.position));
        TileBase tile = NewCameraController.i.TileGet(tilepos);
        if (tile != null)
        {
            if (!dead)
            {
                if (tile.name.Contains("spikes"))
                {
                    Vector2 direction = Vector2.up;
                    switch (tile.name)
                    {
                        case "spikesU":
                            direction = Vector2.up;
                            break;
                        case "spikesD":
                            direction = Vector2.down;
                            break;
                        case "spikesL":
                            direction = Vector2.left;
                            break;
                        case "spikesR":
                            direction = Vector2.right;
                            break;
                    }

                    Debug.Log(Vector2.Dot(rb.velocity.normalized, direction));
                    if (Vector2.Dot(rb.velocity, direction) < dotThreshold)
                    {
                        Kill(direction);
                    }
                }
            }
        }
        else
        {
            // Debug.Log(tile);
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!dead)
        {
            Tilemap hmap = NewCameraController.i.terrainMap;


            TileBase tile = NewCameraController.i.TileGet(hmap.WorldToCell(col.contacts[0].point));
            Vector2 direction = Vector2.zero;
            if (tile != null)
            {
                switch (tile.name)
                {
                    case "spikesU":
                        direction = Vector2.up;
                        break;
                    case "spikesD":
                        direction = Vector2.down;
                        break;
                    case "spikesL":
                        direction = Vector2.left;
                        break;
                    case "spikesR":
                        direction = Vector2.right;
                        break;
                }
                if (Vector2.Dot(rb.velocity, direction) < dotThreshold)
                {
                    Vector2 normal = col.contacts[0].normal;
                    Kill(normal);

                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        Tilemap hmap = NewCameraController.i.terrainMap;
        foreach (ContactPoint2D contact in col.contacts)
        {
            Vector3Int tilepos = hmap.WorldToCell(contact.point - contact.normal / 2);
            TileBase tile = NewCameraController.i.TileGet(tilepos);
            if (tile != null)
            {
                if (tile.name == "crumbling0")
                {
                    this.Invoke(() => { crumbledTiles.Add(tilepos); hmap.SetTile(tilepos, null); }, 0.2f);

                }
            }
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
        Tilemap hmap = NewCameraController.i.terrainMap;
        foreach (Vector3Int tile in crumbledTiles)
        {
            hmap.SetTile(tile, crumblingTile);
        }
        NewCameraController.i.RespawnPlayer();
        GetComponent<playerMovement>().enabled = true;
        NewCameraController.i.blockStateUpdateEvent(0);

        //GetComponent<SpriteRenderer>().enabled = true;
        dead = false;
    }
    public void Kill(Vector3 normal)
    {
        NewCameraController.i.Shake(0.3f, 2, 10);

        GetComponent<playerMovement>().enabled = false;
        rb.gravityScale = 0;
        dead = true;
        float variation =
            Random
                .Range(minDeathAnimPushVariation,
                maxDeathAnimPushVariation);
        float dir = Mathf.Atan2(normal.y, normal.x) + variation;

        rb.velocity =
            new Vector2(Mathf.Cos(dir), Mathf.Sin(dir)) * deathAnimVel;
        Invoke("DeathAnimStage2", deathAnimPushTime);

        GameObject deathParticles = Instantiate(dedParticles, transform.position, Quaternion.identity);
        deathParticles.transform.rotation =
            Quaternion.Euler(0, 0, variation);
        Destroy(deathParticles, 3f);
    }
}
