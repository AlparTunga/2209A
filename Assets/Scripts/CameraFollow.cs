using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform car1;
    public Transform car2;

    public Vector3 offset = new Vector3(0f, 25f, -10f); // Daha dik ve yukar�dan
    public float followSpeed = 2f;

    private bool zoomToWinner = false;
    private Transform winnerTarget;

    void LateUpdate()
    {
        if (zoomToWinner && winnerTarget != null)
        {
            // Kazanan arabaya ku� bak��� zoom
            Vector3 targetPos = winnerTarget.position + new Vector3(0f, 15f, -5f); // Hafif �apraz yukar�dan
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed * 2f);
            transform.LookAt(winnerTarget.position + Vector3.up * 2f); // Arabaya bakarken biraz yukar�y� hedefle
        }
        else
        {
            // �ki araban�n ortas�n� takip et
            Vector3 midPoint = (car1.position + car2.position) / 2f;
            Vector3 targetPosition = midPoint + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

            // Ku� bak��� bak�� y�n� (a�a��ya do�ru)
            transform.rotation = Quaternion.Euler(75f, 0f, 0f); // 75 derece e�im
        }
    }

    public void FocusOnWinner(Transform winner)
    {
        winnerTarget = winner;
        zoomToWinner = true;
    }

    public void ResetCamera()
    {
        zoomToWinner = false;
    }
}
