using UnityEngine;

public sealed class MVHero : MonoBehaviour
{
    public MVGameController game;
    public Rigidbody2D body;
    public SpriteRenderer graphics;
    public bool inWater;
    public bool controlEnabled = true;
    public float speed = 6f;
    private bool grounded;
    private float nextSwim;
    private float invincibleUntil;
    private Sprite idle, run1, run2, jump, water;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        graphics = GetComponent<SpriteRenderer>();
        idle = MVGameController.Sprite("Mario_Small_Idle");
        run1 = MVGameController.Sprite("Mario_Small_Run1");
        run2 = MVGameController.Sprite("Mario_Small_Run2");
        jump = MVGameController.Sprite("Mario_Small_Jump");
        water = MVGameController.Sprite("Mario_Small_Jump");
    }

    private void Update()
    {
        if (!controlEnabled || game == null || game.paused) return;
        float x = 0;
        if (MVKeys.Held(KeyCode.RightArrow) || MVKeys.Held(KeyCode.D)) x += 1;
        if (MVKeys.Held(KeyCode.LeftArrow) || MVKeys.Held(KeyCode.A)) x -= 1;
        body.linearVelocity = new Vector2(x * (inWater ? 4.25f : speed), body.linearVelocity.y);
        if (x > 0) graphics.flipX = false;
        if (x < 0) graphics.flipX = true;
        if (inWater)
        {
            if ((MVKeys.Held(KeyCode.Space) || MVKeys.Held(KeyCode.W) || MVKeys.Held(KeyCode.UpArrow)) && Time.time >= nextSwim)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, 4.75f);
                nextSwim = Time.time + 0.22f;
                MVAudio.OneShot("mario-bros-salto", 0.25f);
            }
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Clamp(body.linearVelocity.y, -3f, 5f));
            if (transform.position.y > 3.1f) { transform.position = new Vector3(transform.position.x, 3.1f, 0); body.linearVelocity = new Vector2(body.linearVelocity.x, 0); }
        }
        else
        {
            // Suelo bajo los pies. La máscara de contacto evita detectar triggers.
            var hits = Physics2D.OverlapCircleAll((Vector2)transform.position + Vector2.down * 0.52f, 0.12f);
            grounded = false;
            foreach (var hit in hits)
                if (hit != null && hit.gameObject != gameObject && !hit.isTrigger) { grounded = true; break; }
            if (grounded && (MVKeys.Pressed(KeyCode.Space) || MVKeys.Pressed(KeyCode.W) || MVKeys.Pressed(KeyCode.UpArrow)))
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, 12.5f);
                MVAudio.OneShot("mario-bros-salto", 0.45f);
            }
        }
        if (graphics != null)
        {
            if (inWater) graphics.sprite = Time.time % .4f < .2f ? water : idle;
            else if (!grounded) graphics.sprite = jump;
            else if (Mathf.Abs(x) > .01f) graphics.sprite = Mathf.FloorToInt(Time.time * 10f) % 2 == 0 ? run1 : run2;
            else graphics.sprite = idle;
            graphics.color = (Time.time < invincibleUntil && Mathf.FloorToInt(Time.time * 12) % 2 == 0) ? new Color(1,1,1,0.35f) : Color.white;
        }
        if (transform.position.y < -8.5f) Hit();
    }

    public void Hit()
    {
        if (!controlEnabled || Time.time < invincibleUntil) return;
        controlEnabled = false;
        game.LoseLife();
    }

    public void Stomp() { body.linearVelocity = new Vector2(body.linearVelocity.x, inWater ? 4.5f : 8.1f); }
    public void Invincibility(float seconds) { invincibleUntil = Time.time + seconds; }
}

