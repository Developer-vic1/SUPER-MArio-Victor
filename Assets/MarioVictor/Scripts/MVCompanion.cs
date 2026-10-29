using UnityEngine;

public sealed class MVCompanion : MonoBehaviour
{
    public MVHero hero;
    private SpriteRenderer image;
    private float t;
    private void Start() { image = GetComponent<SpriteRenderer>(); }
    private void Update()
    {
        if (hero == null) return;
        t += Time.deltaTime;
        Vector3 target = hero.transform.position + new Vector3(-1.45f, .35f + .22f * Mathf.Sin(t * 5f), 0);
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5f);
        if (image != null) image.flipX = hero.graphics != null && hero.graphics.flipX;
    }
}
