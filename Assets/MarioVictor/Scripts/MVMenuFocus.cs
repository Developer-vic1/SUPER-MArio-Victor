using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public sealed class MVMenuFocus : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnSelect(BaseEventData data)
    {
        var ring = GetComponent<Outline>();
        ring.effectColor = new Color32(255, 225, 125, 255);
        ring.effectDistance = new Vector2(3, -3);
    }
    public void OnDeselect(BaseEventData data)
    {
        var ring = GetComponent<Outline>();
        ring.effectColor = new Color32(159, 199, 222, 100);
        ring.effectDistance = new Vector2(1, -1);
    }
}
