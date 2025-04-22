using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public List<Transform> textsToRotate;
    public Transform player;

    void Update()
    {
        foreach (Transform text in textsToRotate)
        {
            if (text != null && player != null)
            {
                Vector3 direction = player.position - text.position;
                direction.y = 0; // nepřeklánět texty nahoru/dolů
                text.rotation = Quaternion.LookRotation(direction);
                text.Rotate(0, 180f, 0); // otočení o 180 stupňů kolem Y
            }
        }
    }
}
