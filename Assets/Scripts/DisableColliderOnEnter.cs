using UnityEngine;

public class DisableColliderOnEnter : MonoBehaviour
{
    public Collider colliderToDisable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            colliderToDisable.enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            colliderToDisable.enabled = true;
    }
}