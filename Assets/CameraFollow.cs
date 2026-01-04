using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 4, -8);
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;

    private float driftCameraOffset = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // -------------------------------
        // 1) Drift sýrasýnda kamera sola kayar
        // -------------------------------
        bool isDrifting = Input.GetKey(KeyCode.Space);

        float targetDriftOffset = isDrifting ? 2f : 0f;      // Drift açýsý
        driftCameraOffset = Mathf.Lerp(driftCameraOffset, targetDriftOffset, Time.deltaTime * 3);

        // -------------------------------
        // 2) Pozisyon hesaplama
        // -------------------------------
        Vector3 desiredPos = target.position
            + target.right * driftCameraOffset
            + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // -------------------------------
        // 3) Rotasyonu yumuþat
        // -------------------------------
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}
