using UnityEngine;

/// <summary>Trampa de demostración: vuelve al jugador al inicio sin tocar el contador oficial.</summary>
[RequireComponent(typeof(BoxCollider2D))]
public sealed class FinalDemoLava : MonoBehaviour
{
    private void Reset() { GetComponent<Collider2D>().isTrigger = true; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var demo = other.GetComponent<FinalDemoPlayer>();
        if (demo != null) demo.Respawn();
        else
        {
            var player = other.GetComponent<Player>();
            if (player != null && !player.dead) player.Death();
        }
    }
}
