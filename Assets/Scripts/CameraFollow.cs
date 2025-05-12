using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target1;
    public Transform target2;

    public float baseHeight = 15f;              // Art�r�ld�
    public float maxHeight = 60f;               // Art�r�ld�
    public float distanceMultiplier = 1.2f;     // Daha h�zl� y�kselme

    public float followSpeed = 4f;
    public float baseZDistance = 25f;           // Kamera Z'de daha geride
    public float maxZDistance = 80f;            // Zoom-out s�n�r�

    private bool focusOnWinner = false;
    private Transform winnerTarget;

    void LateUpdate()
    {
        if (focusOnWinner && winnerTarget != null)
        {
            Vector3 focusPos = winnerTarget.position + new Vector3(-15f, 8f, -15f);
            transform.position = Vector3.Lerp(transform.position, focusPos, Time.deltaTime * followSpeed);
            transform.LookAt(winnerTarget);
            return;
        }

        if (target1 == null || target2 == null) return;

        Vector3 centerPoint = (target1.position + target2.position) / 2f;

        float horizontalDist = Mathf.Abs(target1.position.x - target2.position.x);
        float dynamicHeight = Mathf.Clamp(baseHeight + horizontalDist * distanceMultiplier, baseHeight, maxHeight);
        float dynamicZ = Mathf.Clamp(baseZDistance + horizontalDist * 1.2f, baseZDistance, maxZDistance); // Z uzakl��� da b�y�s�n

        Vector3 desiredPos = centerPoint + new Vector3(0f, dynamicHeight, -dynamicZ);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
        transform.LookAt(centerPoint);
    }

    public void FocusOnWinner(Transform winner)
    {
        focusOnWinner = true;
        winnerTarget = winner;
    }

    public void ResetCamera()
    {
        focusOnWinner = false;
        winnerTarget = null;
    }
}
