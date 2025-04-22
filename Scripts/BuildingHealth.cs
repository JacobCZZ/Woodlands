using UnityEngine;
using UnityEngine.UI;

public class BuildingHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public GameObject healthBarUI;         // Celý canvas healthbaru
    public Slider healthSlider;            // Samotný slider

    private Camera mainCamera;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        healthBarUI.SetActive(false); // Na začátku skryté

        mainCamera = Camera.main; // Reference na hlavní kameru
    }

    private void Update()
    {
        // Otáčení health baru směrem ke kameře
        if (healthBarUI.activeSelf)
        {
            healthBarUI.transform.rotation = Quaternion.LookRotation(healthBarUI.transform.position - mainCamera.transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("EnemyBullet"))
        {
            TakeDamage(10f); // Např. každá kulka ubere 10 HP
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = currentHealth / maxHealth;

        // Zobraz healthbar jen pokud není na 100 %
        healthBarUI.SetActive(currentHealth < maxHealth);
    }

    private void DestroyBuilding()
    {
        // Zničení budovy
        Destroy(gameObject);
    }
}
