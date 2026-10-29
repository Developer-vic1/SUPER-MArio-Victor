using UnityEngine;

public sealed class MVProjectile : MonoBehaviour
{
    public MVGameController game;
    private float expiry;
    private void Start() { expiry = Time.time + 5f; }
    private void Update()
    {
        if (game == null || game.paused) return;
        transform.Translate(Vector3.left * 5.1f * Time.deltaTime);
        if (Time.time > expiry) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var hero = other.GetComponent<MVHero>();
        if (hero != null) { hero.Hit(); Destroy(gameObject); }
    }
}

