using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    private MVGameController Campaign => FindFirstObjectByType<MVGameController>();

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        if (Campaign != null) return;
        NewGame();
    }

    public void NewGame()
    {
        if (Campaign != null) { Campaign.StartNewGame(); return; }
        lives = 3;
        coins = 0;

        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        // TODO: show game over screen

        NewGame();
    }

    public void LoadLevel(int world, int stage)
    {
        if (Campaign != null) { Campaign.FinishLevel(); return; }
        this.world = world;
        this.stage = stage;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        if (Campaign != null) { Campaign.LoseLife(); return; }
        lives--;

        if (lives > 0) {
            LoadLevel(world, stage);
        } else {
            GameOver();
        }
    }

    public void AddCoin()
    {
        if (Campaign != null) { Campaign.Coin(); return; }
        coins++;

        if (coins == 100)
        {
            coins = 0;
            AddLife();
        }
    }

    public void AddLife()
    {
        if (Campaign != null) { Campaign.ExtraLife(); return; }
        lives++;
    }

}
