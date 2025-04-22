using UnityEngine;

public class FireHealing : MonoBehaviour
{
    public float healAmountPerSecond = 5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BasicPlayerMovement player = other.GetComponent<BasicPlayerMovement>();
            if (player != null)
            {
                if (player.currentHealth < player.maxHealth)
                {
                    float healAmount = healAmountPerSecond * Time.deltaTime;

                    // Ošetření, aby nepřesáhl maxHealth
                    if (player.currentHealth + healAmount > player.maxHealth)
                    {
                        healAmount = player.maxHealth - player.currentHealth;
                    }

                    player.TakeDamage(-healAmount); // záporné číslo = léčení
                    Debug.Log("Léčím hráče o " + healAmount.ToString("F2") + ". Aktuální HP: " + player.currentHealth.ToString("F2"));
                }
                else
                {
                    Debug.Log("Hráč má plné zdraví.");
                }
            }
            else
            {
                Debug.LogWarning("Objekt s tagem Player nemá BasicPlayerMovement.");
            }
        }
    }
}
