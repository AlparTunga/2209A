using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            RaceManager raceManager = FindObjectOfType<RaceManager>();
            if (raceManager != null && !raceManager.IsRaceFinished())
            {
                finished = true;
                string winner = car.name; // veya özel bir isim etiketi
                raceManager.FinishRace(winner);
            }
        }
    }
}
