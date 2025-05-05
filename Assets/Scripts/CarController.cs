using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public float speed = 5f; // m/s
    private Rigidbody rb;

    private float distanceTravelled = 0f;
    private float timeElapsed = 0f;
    private Vector3 startPosition;

    private bool raceStarted = false;

    // UI Elemanlar� (opsiyonel)
    public Text infoText;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("CarController: Rigidbody bile�eni eksik!");
        }

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!raceStarted || rb == null) return;

        timeElapsed += Time.fixedDeltaTime;

        Vector3 movement = Vector3.right * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        distanceTravelled = Vector3.Distance(startPosition, rb.position);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (infoText != null)
        {
            infoText.text = $"Zaman: {timeElapsed:F1} s\n" +
                            $"H�z: {speed} m/s\n" +
                            $"Yol: {distanceTravelled:F2} m\n" +
                            $"Form�l: Yol = H�z x Zaman";
        }
    }

    public float GetElapsedTime() => timeElapsed;
    public float GetDistanceTravelled() => distanceTravelled;

    public void ResetCar()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("CarController: Reset s�ras�nda Rigidbody bulunamad�!");
                return;
            }
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Yeni ba�lang�� pozisyonunu hesapla (yolun �st�nde ve sabit bir y'de)
        Vector3 resetPos = new Vector3(-80.4f, 0.6f, transform.position.z); // 0.6f zemin y�ksekli�i (ayarlayabilirsin)
        transform.position = resetPos;

        timeElapsed = 0f;
        distanceTravelled = 0f;
        raceStarted = false;
    }

    public void SetRaceStarted(bool started)
    {
        raceStarted = started;
    }
}
