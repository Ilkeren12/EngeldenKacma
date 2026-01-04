using UnityEngine;

public class CarCollision : MonoBehaviour
{
    Rigidbody rb;

    [Header("Barrier Collision Settings")]
    public float bounceForce = 8f;
    public float speedLossMultiplier = 0.6f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier"))
        {
            // �arpma y�n� (duvardan d��ar� do�ru)
            Vector3 hitNormal = collision.contacts[0].normal;

            // Mevcut h�z� al
            Vector3 currentVelocity = rb.linearVelocity;

            // H�z� biraz d���r
            rb.linearVelocity = currentVelocity * speedLossMultiplier;

            // Sekme kuvveti uygula
            rb.AddForce(hitNormal * bounceForce, ForceMode.Impulse);
        }
    }
}
