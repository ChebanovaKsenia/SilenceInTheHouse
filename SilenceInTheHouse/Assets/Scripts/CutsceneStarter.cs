using UnityEngine;

public class CutsceneStarter : MonoBehaviour
{
    [Header("Ссылка на кат-сцену")]
    public CutscenePlayer cutscenePlayer;

    [Header("Задержка перед запуском (секунды)")]
    public float delay = 0f; // ИЗМЕНЕНО: 0 секунд, запускаем мгновенно!

    void Start()
    {
        if (cutscenePlayer != null)
        {
            Invoke(nameof(StartCutscene), delay);
        }
        else
        {
            Debug.LogError("CutsceneStarter: Не назначен CutscenePlayer!");
        }
    }

    void StartCutscene()
    {
        Debug.Log("CutsceneStarter запускает кат-сцену...");
        cutscenePlayer.PlayCutscene();
    }
}