using UnityEngine;

public sealed class MVEnemy : MonoBehaviour
{
    public MVGameController game;
    public bool aquatic;
    public float originX;
    public float distance = 2.5f;
    public float velocity = 1.25f;
    private float startY;
    private SpriteRenderer image;

    private void Start() { startY = transform.position.y; image = GetComponent<SpriteRenderer>(); }
    private void Update()
    {
        if (game == null || game.paused) return;
        float phase = (Time.time * velocity + originX);
        transform.position = new Vector3(originX + Mathf.Sin(phase) * distance,
            aquatic ? startY + Mathf.Sin(phase * 1.5f) * .48f : startY, 0);
        if (image != null) image.flipX = Mathf.Cos(phase) > 0f;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        MVHero hero = other.GetComponent<MVHero>();
        if (hero == null) return;
        if (!aquatic && hero.body.linearVelocity.y < -.2f && hero.transform.position.y > transform.position.y + .45f)
        {
            MVAudio.OneShot("mario-moneda", .2f);
            game.AddScore(100);
            hero.Stomp();
            Destroy(gameObject);
        }
        else hero.Hit();
    }
}

