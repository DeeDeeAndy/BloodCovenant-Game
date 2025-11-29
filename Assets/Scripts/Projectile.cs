using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    public float speed = 20f;
    public float lifetime = 5f;
    public GameObject hitEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead())
        {
            enemy.TakeDamage(damage);

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}