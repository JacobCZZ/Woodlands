using System.Collections;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private float lastCutTime = 0f;
    public Camera playerCamera;
    public float range = 5f;

    bool AxeInHand = false;
    public GameObject[] Axes;
    public GameObject ActualAxe;
    Axe ActualAxeInfo;
    public Transform AxeHolder;
    public Animator Anim;

    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)&& !AxeInHand)
        {
            foreach (GameObject item in Axes)
            {
                if (item.GetComponent<Axe>().IsActual)
                {
                    ActualAxe = item;
                    ActualAxeInfo = item.GetComponent<Axe>();
                    break;
                }
            }

            if (ActualAxe == null)
            {
                return;
            }

            AxeInHand = true;
            ActualAxe.SetActive(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)&& AxeInHand)
        {
            if (ActualAxe == null)
            {
                AxeInHand = false;
                return;
            }
            AxeInHand = false;
            ActualAxe.SetActive(false);
            return;
        }

        lastCutTime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && AxeInHand && lastCutTime >= ActualAxeInfo.CoolDown)
        {
            if (!ActualAxe.activeSelf)
            {
                return;
            }
            Anim.Play("Animator");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, ActualAxeInfo.Range))
            {
                if (hit.collider.CompareTag("Tree"))
                {
                    hit.collider.GetComponent<TreeSegment>().TakeDamage(ActualAxeInfo.Damage);
                    lastCutTime = 0;
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    PlayerFollow enemy = hit.collider.GetComponent<PlayerFollow>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(ActualAxe.GetComponent<Axe>().Damage);
                        lastCutTime = 0;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !AxeInHand)
        {
            Hold(1);
        }
        if (Input.GetMouseButtonUp(0) && !AxeInHand)
        {
            Hold(2);
        }
    }

    private void Hold(int a)
    {
        HoldingObject HoldingOB = gameObject.GetComponent<HoldingObject>();
        if (HoldingOB == null)
        {
            return;
        }

        if (playerCamera == null)
        {
            return;
        }
        if (a == 2)
        {
            HoldingOB.DeleteHolder();
        }
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            if (hit.transform.CompareTag("Holdable"))
            {
                if (a == 1)
                {
                    if (HoldingOB.ObjectHolder == null)
                    {
                        return;
                    }
                    HoldingOB.CreateHolder(hit.point, hit.transform.gameObject);
                }
                else if (a == 2)
                {
                    HoldingOB.DeleteHolder();
                }
            }
        }
    }
}
