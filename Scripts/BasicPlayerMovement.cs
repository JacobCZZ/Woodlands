using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicPlayerMovement : MonoBehaviour
{
    public float speed = 5f;               // Movement speed
    public float jumpForce = 5f;           // Jump force
    public Transform groundCheck;          // Reference to the ground check object
    public float groundDistance = 0.1f;    // Distance to check for the ground

    private bool isGrounded = true;        // Is the player on the ground
    private Rigidbody rb;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public GameObject DEATHUI;
    public MouseLook mouseLook;
    public BasicPlayerMovement playermovement;
    public GameObject targetObject;
    public float delay = 3f;

    private bool isInWater = false; // ochrana proti opakovanému poškození vodou

    public bool IsDead = false;

    void Start()
    {
        Invoke("DeactivateTarget", delay);

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void DeactivateTarget()
    {
        if (targetObject != null)
            targetObject.SetActive(false);
    }

    void Update()
    {
        // Get user input for movement (WASD or arrow keys)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move player in the direction they face (forward/backward and left/right)
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        // Ground check
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Raycast pro kontrolu vody pod hráčem
        if (!isInWater)
        {
            RaycastHit hit;
            if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance + 0.2f))
            {
                if (hit.collider.CompareTag("Water"))
                {
                    isInWater = true;
                    TakeDamage(200); // nebo rovnou Die();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(20f);
            Destroy(other.gameObject);
        }

        // Pokud chceš zachovat i starý způsob detekce vody přes trigger
        // můžeš tento blok klidně nechat
        if (other.CompareTag("Water"))
        {
            TakeDamage(200);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!IsDead)
        {
            IsDead = true;
            Debug.Log("Player died.");
            DEATHUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mouseLook.enabled = false;
            transform.Rotate(0, 0, 90);
            playermovement.enabled = false;
        }
    }
}
