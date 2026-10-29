using UnityEngine;

/// <summary>
/// Control SOLO para la escena de prototipo. El castillo final real usará el prefab Mario.
/// Usa el Input Manager clásico, conservado en Unity 6 mediante Active Input Handling = Both.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public sealed class FinalDemoPlayer : MonoBehaviour
{
    public float runSpeed = 7f;
    public float jumpSpeed = 11f;
    public Vector3 spawnPoint = new Vector3(0f, 0f, 0f);
    private Rigidbody2D body;
    private AudioSource sound;
    private float horizontal;
    private bool jumpQueued;
    private bool grounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.freezeRotation = true;
        body.gravityScale = 3f;
        sound = GetComponent<AudioSource>();
        if (sound == null) sound = gameObject.AddComponent<AudioSource>();
        sound.playOnAwake = false;
        sound.spatialBlend = 0f;
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        var hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down * 0.36f,
            Vector2.down, 0.19f, Physics2D.DefaultRaycastLayers);
        grounded = hit.collider != null && hit.collider.gameObject != gameObject && !hit.collider.isTrigger;
        if (grounded && Input.GetButtonDown("Jump")) jumpQueued = true;

        if (horizontal > 0.01f) transform.rotation = Quaternion.identity;
        else if (horizontal < -0.01f) transform.rotation = Quaternion.Euler(0, 180, 0);
        if (transform.position.y < -7f) Respawn();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(horizontal * runSpeed, body.linearVelocity.y);
        if (!jumpQueued) return;
        jumpQueued = false;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
        var clip = Resources.Load<AudioClip>("MarioVictor/Audio/mario-bros-salto");
        if (clip != null) sound.PlayOneShot(clip, 0.6f);
    }

    public void Respawn()
    {
        body.linearVelocity = Vector2.zero;
        transform.position = spawnPoint;
    }
}
