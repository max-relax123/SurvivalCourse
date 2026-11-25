using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxCollector : MonoBehaviour
{
    [SerializeField] private string boxTag = "Box";
    [SerializeField] private int maxCapacity = 10;
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private Key pickupSingleKey = Key.F;
    [SerializeField] private Key pickupAllKey = Key.R;
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
        if (Keyboard.current == null) return;

        if (Keyboard.current[pickupSingleKey].wasPressedThisFrame)
        {
            CollectNearestBox();
        }

        if (Keyboard.current[pickupAllKey].wasPressedThisFrame)
        {
            CollectAllBoxesInRadius();
        }

        if (Keyboard.current[dropKey].wasPressedThisFrame)
        {
            DropLastBox();
        }
    }

    private void CollectNearestBox()
    {
        if (maxCapacity > 0 && inventory.Count >= maxCapacity)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        GameObject nearestBox = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(boxTag) && col.gameObject.activeInHierarchy)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestBox = col.gameObject;
                }
            }
        }

        if (nearestBox != null)
        {
            TryCollectBox(nearestBox);
        }
    }

    private void CollectAllBoxesInRadius()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D col in colliders)
        {
            if (maxCapacity > 0 && inventory.Count >= maxCapacity)
            {
                break;
            }

            if (col.CompareTag(boxTag) && col.gameObject.activeInHierarchy)
            {
                TryCollectBox(col.gameObject);
            }
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
