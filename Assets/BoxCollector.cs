using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxCollector : MonoBehaviour
{
    [SerializeField] private string boxTag = "Box";
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] private Key dropKey = Key.E;
    [SerializeField] private Vector2 dropOffset = new Vector2(1.5f, 0f);
    [SerializeField] private TextMeshProUGUI counterText;

    private Stack<GameObject> inventory = new Stack<GameObject>();
    private SpriteRenderer spriteRenderer;

    public int BoxCount => inventory.Count;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[dropKey].wasPressedThisFrame)
        {
            DropLastBox();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(boxTag))
        {
            TryCollectBox(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(boxTag))
        {
            TryCollectBox(collision.gameObject);
        }
    }

    private void TryCollectBox(GameObject boxObject)
    {
        if (maxCapacity > 0 && inventory.Count >= maxCapacity)
        {
            return;
        }

        boxObject.SetActive(false);
        inventory.Push(boxObject);
        UpdateUI();
    }

    private void DropLastBox()
    {
        if (inventory.Count == 0)
        {
            return;
        }

        GameObject boxToDrop = inventory.Pop();
        float direction = GetFacingDirection();
        Vector3 spawnPosition = transform.position + new Vector3(dropOffset.x * direction, dropOffset.y, 0f);

        boxToDrop.transform.position = spawnPosition;
        boxToDrop.SetActive(true);
        UpdateUI();
    }

    private float GetFacingDirection()
    {
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            return -1f;
        }

        if (transform.localScale.x < 0f || transform.right.x < 0f)
        {
            return -1f;
        }

        return 1f;
    }

    private void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = $"Собрано: {inventory.Count}";
        }
    }
}
