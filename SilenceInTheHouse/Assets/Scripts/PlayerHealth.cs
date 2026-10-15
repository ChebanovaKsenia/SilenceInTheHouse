using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public ScreamerTrigger screamerManager;

    public void Die()
    {
        // Отключаем управление
        GetComponent<PlayerControllerFPS>().enabled = false;

        // Запускаем скример
        if (screamerManager != null)
        {
            screamerManager.TriggerScreamer();
        }

        // Через 5 секунд — Game Over
        Invoke("GameOver", 5f);
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        // Здесь можно загрузить меню или рестарт
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}