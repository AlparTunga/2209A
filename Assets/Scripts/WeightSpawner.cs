using UnityEngine;

public class WeightSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform spawnPoint;

    public void SpawnBlock()
    {
        Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity);
    }
}
