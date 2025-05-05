using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public CarController car1;
    public CarController car2;
    public Transform roadVisual;    // Yolun görsel objesi (örn: Plane)
    public Transform finishLine;
    public CameraFollow cameraFollow; // Inspector'dan atayın// Bitiş çizgisi objesi

    public float raceDistance = 50f;

    public float countdownTime = 3f;
    private float countdown = 0f;

    private bool raceStarted = false;
    private bool raceFinished = false;
    private string winner = "";

    private bool raceTriggered = false;

    void Start()
    {
        ResetRace();
    }

    void Update()
    {
        if (!raceTriggered) return;

        if (!raceStarted)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                raceStarted = true;
                car1.SetRaceStarted(true);
                car2.SetRaceStarted(true);
            }
            return;
        }

        if (raceFinished) return;

        // Bitiş çizgisini geçen ilk aracı belirle
        if (car1.transform.position.x >= raceDistance)
        {
            raceFinished = true;
            winner = "Arac 1";
            car1.SetRaceStarted(false);
            car2.SetRaceStarted(false);
        }
        else if (car2.transform.position.x >= raceDistance)
        {
            raceFinished = true;
            winner = "Arac 2";
            car1.SetRaceStarted(false);
            car2.SetRaceStarted(false);
        }
    }

    public void ResetRace()
    {
        Time.timeScale = 1f;

        raceStarted = false;
        raceFinished = false;
        raceTriggered = false;
        winner = "";
        countdown = countdownTime;

        car1.ResetCar();
        car2.ResetCar();

        car1.SetRaceStarted(false);
        car2.SetRaceStarted(false);

        if (cameraFollow != null)
            cameraFollow.ResetCamera();

        UpdateRoadLength();
    }

    public void TriggerRace()
    {
        countdown = countdownTime;
        raceTriggered = true;
    }

    public float extraBackLength = 20f; // Geride bırakılacak ekstra uzunluk

    public void UpdateRoadLength()
    {
        float extraBackLength = 20f;
        float carLength = car1.GetComponent<Renderer>().bounds.size.x;
        float adjustedRaceDistance = raceDistance + (carLength / 2f); // Ön uç hizası

        float totalLength = adjustedRaceDistance + extraBackLength;

        if (roadVisual != null)
        {
            Vector3 scale = roadVisual.localScale;
            scale.x = totalLength;
            roadVisual.localScale = scale;

            Vector3 start = (car1.transform.position + car2.transform.position) / 2f;
            Vector3 newPos = start;
            newPos.x += (adjustedRaceDistance - extraBackLength) / 2f;
            roadVisual.position = new Vector3(newPos.x, roadVisual.position.y, roadVisual.position.z);
        }

        if (finishLine != null)
        {
            float finishWidth = finishLine.localScale.x;
            Vector3 finishPos = (car1.transform.position + car2.transform.position) / 2f;
            finishPos.x += adjustedRaceDistance + (finishWidth / 2f);
            finishLine.position = new Vector3(finishPos.x, finishLine.position.y, finishLine.position.z);
        }
    }

    public void FinishRace(string winnerName)
    {
        if (raceFinished) return;

        raceFinished = true;
        raceStarted = false;
        winner = winnerName;

        car1.SetRaceStarted(false);
        car2.SetRaceStarted(false);

        Time.timeScale = 0f;

        if (cameraFollow != null)
        {
            if (winnerName == car1.gameObject.name)
                cameraFollow.FocusOnWinner(car1.transform);
            else if (winnerName == car2.gameObject.name)
                cameraFollow.FocusOnWinner(car2.transform);
        }

        Debug.Log("Yarışı kazanan: " + winnerName);
    }




    // Getterlar
    public string GetWinner() => winner;
    public bool IsRaceFinished() => raceFinished;
    public bool IsRaceStarted() => raceStarted;
    public float GetCountdown() => countdown;
    public bool IsRaceTriggered() => raceTriggered;

}