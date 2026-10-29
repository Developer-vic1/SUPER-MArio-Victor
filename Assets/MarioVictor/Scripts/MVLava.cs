using UnityEngine;

public sealed class MVLava : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        MVHero hero = other.GetComponent<MVHero>();
        if (hero != null) hero.Hit();
    }
}

