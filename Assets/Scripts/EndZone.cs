using UnityEngine;
public class EndZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var e = other.GetComponent<Enemy>();
        if (e) e.ReachEnd();
    }
}