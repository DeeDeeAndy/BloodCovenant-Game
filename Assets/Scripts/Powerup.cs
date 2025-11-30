using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpData powerUpData;
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBuffs playerBuffs = other.GetComponent<PlayerBuffs>();
            if (playerBuffs != null)
            {
                playerBuffs.ApplyPowerUp(powerUpData);

                if (powerUpData.pickupSound != null)
                    AudioSource.PlayClipAtPoint(powerUpData.pickupSound, transform.position);

                Destroy(gameObject);
            }
        }
    }
}