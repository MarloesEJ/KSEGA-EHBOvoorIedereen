using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private bool autoFindGameManager = true;

    private GameManager gameManager;

    private void Awake()
    {
        if (autoFindGameManager)
        {
            gameManager = GameManager.Instance;
        }
    }

    public void SignalNextLevel()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager != null)
        {
            gameManager.LoadNextLevel();
        }
    }

    public void SignalWin()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager != null)
        {
            gameManager.SetState(GameState.Win);
        }
    }

    public void SignalLose()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (gameManager != null)
        {
            gameManager.SetState(GameState.Lose);
        }
    }
}
