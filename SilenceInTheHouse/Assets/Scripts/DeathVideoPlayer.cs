using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class DeathVideoPlayer : MonoBehaviour
{
    [Header("Звук скримера")]
    public VideoPlayer videoPlayer;
    public RawImage deathScreen; // Raw Image на Canvas
    public GameObject deathCanvas; // Сам Canvas
    public AudioClip screamerSound;
    private AudioSource screamerAudio;

    private bool isPlaying = false;

    void Start()
    {
        // Выключаем всё при старте
        if (deathCanvas != null) deathCanvas.SetActive(false);
        screamerAudio = gameObject.AddComponent<AudioSource>();
        screamerAudio.playOnAwake = false;
        screamerAudio.spatialBlend = 0f; // 2D звук
    }

    // Вызывается из MonsterAI при смерти
    public void PlayDeathVideo()
    {
        if (isPlaying) return;
        isPlaying = true;

        Debug.Log("ПРОИГРЫВАЕМ ВИДЕО СМЕРТИ!");

        // Включаем Canvas и Raw Image
        if (deathCanvas != null) deathCanvas.SetActive(true);
        if (deathScreen != null) deathScreen.gameObject.SetActive(true);

        // Запускаем видео
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
        if (screamerSound != null)
    {
        screamerAudio.PlayOneShot(screamerSound);
    }

        // Ждём окончания видео + 1 секунду, потом перезагружаем сцену
        Invoke("EndDeath", (float)videoPlayer.clip.length + 1f);
    }

    void EndDeath()
    {
        Debug.Log("Видео смерти окончено. Перезагрузка...");

        // Перезагружаем сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}