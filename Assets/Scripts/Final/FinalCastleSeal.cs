using UnityEngine;

/// <summary>
/// Acoplar a un objeto con Collider2D marcado como Is Trigger.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public sealed class FinalCastleSeal : MonoBehaviour
{
    public FinalCastleManager castleManager;
    public Color activatedColor = new Color(0.45f, 1f, 0.68f, 1f);
    private bool activated;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated || castleManager == null || !other.CompareTag("Player")) return;
        castleManager.ActivateSeal(this);
    }

    public bool TryActivate()
    {
        if (activated) return false;
        activated = true;
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null) renderer.color = activatedColor;
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        return true;
    }
}
