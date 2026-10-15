using UnityEngine;

public class TeddyBear : MonoBehaviour
{
    [Header("Настройки")]
    public float interactionDistance = 2.5f;

    [Header("Ссылки")]
    public MonsterAI monster;

    [Header("Состояние")]
    public bool isPickedUp = false;

    private GameObject player;
    private Collider bearCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bearCollider = GetComponent<Collider>();
    }

    void Update()
    {
        // Если уже подобран, скрипт перестает проверять нажатия
        if (isPickedUp || player == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < interactionDistance)
            {
                Debug.Log(">>> НАЖАТО E У МИШКИ. ЗАПУСК PICKUP()");
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        isPickedUp = true;
        Debug.Log(">>> МИШКА ПОДОБРАНА. ПРИКРЕПЛЕНИЕ К ИГРОКУ.");

        // 1. Прикрепляем к игроку
        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0.4f, -0.5f, 0.6f); // Чуть спереди и внизу (как в руках)
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        // 2. Отключаем коллайдер, чтобы мишка случайно не триггерила кровать
        if (bearCollider != null) bearCollider.enabled = false;

        // 3. Отключаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 4. Злим монстра
        if (monster != null)
        {
            Debug.Log(">>> ВЫЗОВ MONSTER.ENRAGE()");
            monster.Enrage();
        }

        // ВАЖНО: Здесь НЕТ вызова кровати или кат-сцены!
    }

    public void Drop()
    {
        isPickedUp = false;
        Debug.Log(">>> МИШКА ПОЛОЖЕН НА КРОВАТЬ.");

        transform.SetParent(null); // Открепляем от игрока

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        if (bearCollider != null) bearCollider.enabled = true;
    }
}