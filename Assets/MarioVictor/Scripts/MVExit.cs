using UnityEngine;

public sealed class MVExit : MonoBehaviour
{
    public MVGameController game;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MVHero>() != null) game.FinishLevel();
    }
}

