using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float targetSpeed = 18f;     // sürekli gidiş hızı
    public float accel = 8f;            // hedef hıza yaklaşma
    public float maxSpeed = 25f;        // kesin hız sınırı
    public float steering = 80f;

    [Header("Drift")]
    public float driftFactor = 0.88f;

    [Header("Score")]
    public float driftScore = 0f;
    public float driftMultiplier = 1f;
    public TextMeshProUGUI scoreText;

    private Rigidbody rb;
    private float startY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        // başlangıç Y’yi sabitlemek için
        startY = transform.position.y;

        // Rigidbody stabilizasyon (script içinden garanti)
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (scoreText == null)
            Debug.LogWarning("ScoreText atanmadı (puan yazısı boş kalır).");
    }

    void FixedUpdate()
    {
        // 1) İleri hız: W basılıysa hedef hızı artır, değilse normal targetSpeed
        float desiredSpeed = Input.GetKey(KeyCode.W) ? targetSpeed * 1.8f : targetSpeed;

        // mevcut velocity’den sadece XZ düzleminde bak
        Vector3 v = rb.linearVelocity;
        Vector3 flatV = new Vector3(v.x, 0f, v.z);

        // aracın ileri yönündeki hızını hedefe yaklaştır
        float currentForward = Vector3.Dot(flatV, transform.forward);
        float speedDelta = desiredSpeed - currentForward;

        // hız yaklaşımı
        float add = Mathf.Clamp(speedDelta, -accel, accel);
        rb.AddForce(transform.forward * add, ForceMode.VelocityChange);

        // 2) Direksiyon
        float turn = 0f;
        if (Input.GetKey(KeyCode.A)) turn = -1f;
        if (Input.GetKey(KeyCode.D)) turn = 1f;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * steering * Time.fixedDeltaTime, 0f));

        // 3) Drift: sadece yanal hızı azalt
        v = rb.linearVelocity;
        Vector3 forwardVel = transform.forward * Vector3.Dot(v, transform.forward);
        Vector3 sideVel = transform.right * Vector3.Dot(v, transform.right);

        if (Input.GetKey(KeyCode.Space))
            rb.linearVelocity = forwardVel + sideVel * driftFactor + Vector3.up * v.y;
        else
            rb.linearVelocity = forwardVel + sideVel + Vector3.up * v.y;

        // 4) Mutlak hız limiti (uçmayı bitirir)
        v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
        }

        // 5) Y eksenini sabitle (zeminden kopma/sekme azaltır)
        Vector3 p = rb.position;
        p.y = startY;
        rb.position = p;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // 6) Skor (drift basılıyken artar)
        if (Input.GetKey(KeyCode.Space))
            driftScore += rb.linearVelocity.magnitude * driftMultiplier * Time.fixedDeltaTime;
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = Mathf.FloorToInt(driftScore).ToString();
    }
}
