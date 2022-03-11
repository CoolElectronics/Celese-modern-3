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
    Sprite springsprite1;
    [SerializeField]
    Sprite springsprite2;

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
    GameObject deathParticles2;
    [SerializeField]
    GameObject sprite;
    [SerializeField]
    TileBase crumblingTile;
    [SerializeField]
    Vector3 springVel;
    [SerializeField]
    Vector3 jspringVel;
    [SerializeField]
    float springTime;

    playerMovement movement;

    bool crouched = false;
    bool ended = false;

    List<Vector3Int> crumbledTiles = new List<Vector3Int>();
    [SerializeField]
    AudioSource dashCrystal;

    public Transform berry;
    [SerializeField]
    float goldenSmoothTime;
    [SerializeField]
    float goldenMaxSpeed;
    [SerializeField]
    float maxBerryDist;
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            NewCameraController.i.blockStateUpdateEvent(0);
        }

    }
    void FixedUpdate()
    {
        if (berry)
        {
            if ((berry.transform.position - transform.position).magnitude > maxBerryDist)
            {
                Vector3 v = Vector3.zero;
                berry.position = Vector3.SmoothDamp(berry.position, transform.position, ref v, goldenSmoothTime, goldenMaxSpeed);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch (col.tag)
        {
            case "GoldBerry":
                Destroy(col.gameObject.GetComponent<BoxCollider2D>());
                berry = col.transform;
                break;
            case "Battery":

                if (movement.dashes == 0)
                {
                    col.gameObject.SetActive(false);
                    this.Invoke(() => col.gameObject.SetActive(true), 1);
                    movement.dashes = movement.maxDashCount;
                    dashCrystal.Play();
                }
                break;
            case "DoubleBattery":

                if (movement.dashes != movement.maxDashCount + 1)
                {
                    dashCrystal.Play();
                    col.gameObject.SetActive(false);
                    this.Invoke(() => col.gameObject.SetActive(true), 2);
                    movement.dashes = movement.maxDashCount + 1;
                }
                break;
            case "BlinkBattery":
                dashCrystal.Play();

                movement.dashes = 1;
                movement.blink = true;
                col.gameObject.SetActive(false);
                this.Invoke(() => col.gameObject.SetActive(true), 4);
                break;
            case "Spring":
                Vector2 newvel = col.gameObject.transform.rotation * jspringVel;
                movement.dashes = movement.maxDashCount;
                col.gameObject.GetComponent<SpriteRenderer>().sprite = springsprite2;
                this.Invoke(() => col.gameObject.GetComponent<SpriteRenderer>().sprite = springsprite1, 0.2f);
                if (col.gameObject.transform.rotation.eulerAngles.z == 90 || col.gameObject.transform.rotation.eulerAngles.z == 270)
                {
                    newvel = col.gameObject.transform.rotation * springVel;

                    movement.overrideMove = true;
                    this.Invoke(() => movement.overrideMove = false, springTime);
                    newvel.y = springVel.z;
                }
                rb.velocity = newvel;
                break;
            case "Flag":
                if (!ended)
                {
                    ended = true;
                    NewCameraController.i.Finish();
                }
                break;
        }
        Tilemap hmap = NewCameraController.i.triggerMap;

        Vector3Int tilepos = hmap.WorldToCell(col.ClosestPoint(transform.position));
        (Tilemap, TileBase) tile = NewCameraController.i.TileGet(tilepos);
        if (tile.Item2 != null)
        {
            if (!dead)
            {
                if (tile.Item2.name.ToLower().Contains("spike"))
                {
                    float rotation = tile.Item1.GetTransformMatrix(tilepos).rotation.eulerAngles.z;
                    Vector2 direction = new Vector2(-Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad));
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
    private void OnCollisionStay2D(Collision2D col)
    {
        Tilemap hmap = NewCameraController.i.terrainMap;
        foreach (ContactPoint2D contact in col.contacts)
        {
            Vector3Int tilepos = hmap.WorldToCell(contact.point - contact.normal / 2);
            TileBase tile = NewCameraController.i.TileGet(tilepos).Item2;
            if (tile != null)
            {
                if (tile.name == "crumbling0")
                {
                    this.Invoke(() => { crumbledTiles.Add(tilepos); hmap.SetTile(tilepos, null); }, 0.2f);

                }
            }
            if (!dead)
            {
                (Tilemap, TileBase) ntile = NewCameraController.i.TileGet(hmap.WorldToCell(contact.point));
                if (ntile.Item2 != null && col.gameObject == ntile.Item1.gameObject)
                {
                    if (ntile.Item2.name.Contains("spikes"))
                    {
                        float rotation = ntile.Item1.GetTransformMatrix(tilepos).rotation.eulerAngles.z;

                        Vector2 direction = new Vector2(-Mathf.Sin(rotation * Mathf.Deg2Rad), Mathf.Cos(rotation * Mathf.Deg2Rad));

                        if (Vector2.Dot(rb.velocity, direction) < dotThreshold)
                        {
                            Vector2 normal = col.contacts[0].normal;
                            Kill(normal);

                        }
                    }
                }
            }
        }
    }

    void DeathAnimStage2()
    {
        //GetComponent<SpriteRenderer>().enabled = false;
        rb.velocity = Vector2.zero;
        Invoke("Respawn", respawnTime);
        Destroy(Instantiate(deathParticles2, transform.position, Quaternion.identity), 1.5f);
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
        GetComponent<Animator>().SetTrigger("respawned");
        dead = false;
    }
    public void Kill(Vector3 normal)
    {
        NewCameraController.i.Shake(0.3f, 2, 10);
        GetComponent<Animator>().SetTrigger("killed");

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

        // GameObject deathParticles = Instantiate(dedParticles, transform.position, Quaternion.identity);
        // deathParticles.transform.rotation =
        // Quaternion.Euler(0, 0, deathAnimVel);
        movement.dashes = movement.maxDashCount;
        movement.blink = false;
        movement.blinking = false;
        NewCameraController.i.deaths++;
        if (berry)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        // Destroy(deathParticles, 3f);
    }
}
