using UnityEngine;
using UnityEngine.UI;

public class InteractionHint : MonoBehaviour
{
    [Header("Настройки")]
    public float interactionDistance = 2f;
    public string hintText = "Нажми E";

    [Header("UI")]
    public Text hintUI; // Текст подсказки на экране

    private GameObject player;
    private bool isPlayerNearby = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (hintUI != null) hintUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < interactionDistance)
        {
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
                if (hintUI != null)
                {
                    hintUI.gameObject.SetActive(true);
                    hintUI.text = hintText;
                }
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                if (hintUI != null)
                {
                    hintUI.gameObject.SetActive(false);
                }
            }
        }
    }
}