using UnityEngine;

public class CutsceneStarter : MonoBehaviour
{
    [Header("Ссылка на кат-сцену")]
    public CutscenePlayer cutscenePlayer;

    [Header("Задержка перед запуском (секунды)")]
    public float delay = 1f; // Ждем 1 секунду после загрузки сцены

    void Start()
    {
        if (cutscenePlayer != null)
        {
            // Ждем указанное время, затем запускаем кат-сцену
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