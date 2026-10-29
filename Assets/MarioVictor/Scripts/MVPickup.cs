using UnityEngine;

public sealed class MVPickup : MonoBehaviour
{
    public MVGameController game;
    public int kind; // 0 moneda, 1 vida extra, 2 sello estelar
    private float bornY;
    private bool collected;
    private void Start() { bornY = transform.position.y; }
    private void Update()
    {
        if (game == null || game.paused) return;
        transform.position = new Vector3(transform.position.x, bornY + .12f * Mathf.Sin(Time.time * 4f + transform.position.x), 0);
        transform.Rotate(0, 65f * Time.deltaTime, 0);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var hero = other.GetComponent<MVHero>();
        if (collected || hero == null || !hero.controlEnabled) return;
        collected = true;
        if (kind == 0) game.Coin();
        else if (kind == 1) game.ExtraLife();
        else game.Seal();
        Destroy(gameObject);
    }
}

