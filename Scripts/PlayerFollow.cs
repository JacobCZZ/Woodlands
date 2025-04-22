using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerFollow : MonoBehaviour
{
    public string[] targetTags = { "Player", "Building" };
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float stoppingDistance = 2f;
    public float fireRate = 1f;
    public float followRange = 10f;
    public int maxHP = 100;
    public Slider hpSlider;
    public GameObject characterModel;

    public DayNightCycle dayNightCycle;
    public int dayDamageAmount = 20;
    public float dayDamageInterval = 1f;
    private float dayDamageTimer = 0f;

    private NavMeshAgent agent;
    private float fireCooldown;
    private int currentHP;
    private Vector3 randomDestination;
    private float wanderTimer = 0f;
    private Animator animator;
    private bool isDead = false;
    public GameObject fireefekt;

    void Start()
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = FindObjectOfType<DayNightCycle>();
        }
        agent = GetComponent<NavMeshAgent>();
        animator = characterModel.GetComponent<Animator>();
        currentHP = maxHP;
        UpdateHPUI();
        SetRandomDestination();
    }

    void Update()
    {
        if (isDead) return;

        // >>> Denní poškození
        if (dayNightCycle != null && dayNightCycle.isDay)
        {
            dayDamageTimer -= Time.deltaTime;
            if (dayDamageTimer <= 0f)
            {
                fireefekt.SetActive(true);
                TakeDamage(dayDamageAmount);
                dayDamageTimer = dayDamageInterval;
                
            }
        }
        else
        {
            dayDamageTimer = 0f; // reset časovače, když není den
        }

        Transform nearestTarget = GetNearestTarget();

        if (nearestTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, nearestTarget.position);

            if (distanceToTarget > stoppingDistance)
            {
                agent.destination = nearestTarget.position;
            }
            else
            {
                agent.destination = transform.position;
                RotateTowards(nearestTarget);
                ShootAt(nearestTarget);
            }
        }
        else
        {
            Wander();
        }

        fireCooldown -= Time.deltaTime;
    }

    Transform GetNearestTarget()
    {
        Transform nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in targetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance && distance <= followRange)
                {
                    shortestDistance = distance;
                    nearestTarget = target.transform;
                }
            }
        }

        return nearestTarget;
    }

    void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void ShootAt(Transform target)
    {
        if (fireCooldown <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = (target.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody>().linearVelocity = direction * 10f;

            fireCooldown = 1f / fireRate;
            Destroy(bullet, 2f);
        }
    }

    void Wander()
    {
        if (wanderTimer <= 0f)
        {
            SetRandomDestination();
            wanderTimer = Random.Range(1f, 2f);
        }
        else
        {
            wanderTimer -= Time.deltaTime;
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomPoint = transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        agent.destination = randomPoint;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
        UpdateHPUI();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;

        if (animator != null)
        {
            animator.Play("Die");

            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(DestroyAfterAnimation(animationLength - 0.8f));
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    System.Collections.IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)currentHP / maxHP;
        }
    }
}
