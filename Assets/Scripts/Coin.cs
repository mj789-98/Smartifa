using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int value = 1;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private GameObject collectEffect;

    private Vector3 startPosition;
    private float bobTime;

    private void Start()
    {
        startPosition = transform.position;
        // Ensure the coin has a trigger collider
        if (!GetComponent<Collider2D>())
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
    }

    private void Update()
    {
        // Rotate the coin
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Bob up and down
        bobTime += Time.deltaTime;
        float newY = startPosition.y + Mathf.Sin(bobTime * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to get the coin collector component from the player
            CoinCollector collector = other.GetComponent<CoinCollector>();
            if (collector != null)
            {
                collector.CollectCoin(value);
            }

            // Spawn collection effect if assigned
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Destroy the coin
            Destroy(gameObject);
        }
    }
} 