using UnityEngine;

public sealed class MVBoss : MonoBehaviour
{
    public MVGameController game;
    public int health = 3;
    private float nextShot;
    private float nextHit;
    private SpriteRenderer image;
    private Vector3 start;
    private bool defeated;

    private void Start() { image = GetComponent<SpriteRenderer>(); start = transform.position; nextShot = Time.time + 2f; }
    private void Update()
    {
        if (defeated || game == null || game.paused) return;
        transform.position = start + Vector3.up * .22f * Mathf.Sin(Time.time * 2f);
        if (image != null) image.color = game.seals < 3 ? new Color(.65f,.68f,1f) : Color.white;
        if (Time.time >= nextShot && game.hero != null && Mathf.Abs(game.hero.transform.position.x - transform.position.x) < 12f)
        {
            nextShot = Time.time + (game.seals < 3 ? 3.2f : 2.6f);
            game.SpawnFire(transform.position + Vector3.left * 1.2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        MVHero hero = other.GetComponent<MVHero>();
        if (hero == null || defeated || Time.time < nextHit) return;
        if (hero.body.linearVelocity.y < -.1f && hero.transform.position.y > transform.position.y + .65f)
        {
            hero.Stomp();
            nextHit = Time.time + 1.3f;
            if (game.seals < 3) { game.ShowHint("¡Activa los tres sellos para romper el escudo!"); return; }
            health--;
            game.AddScore(400);
            MVAudio.OneShot("mario-bros-vida-1", .6f);
            if (health <= 0)
            {
                defeated = true;
                game.BossDefeated();
                GetComponent<Collider2D>().enabled = false;
                game.castle.DefeatCampaignBoss(transform);
            }
            else game.ShowHint("¡Impacto! Quedan " + health + " golpes.");
        }
        else hero.Hit();
    }
}

