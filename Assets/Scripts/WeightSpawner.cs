using UnityEngine;

public class WeightSpawner : MonoBehaviour
{
    [Header("Prefab ve Spawn Noktas�")]
    [SerializeField] private GameObject massBlockPrefab;
    [SerializeField] private Transform spawnPoint;

    public static MassBlock currentBlock;

    /// <summary>UI butonuna ba�lay�n.</summary>
    public void SpawnBlock()
    {
        var go = Instantiate(massBlockPrefab, spawnPoint.position, Quaternion.identity);
        var mb = go.GetComponent<MassBlock>();
        if (mb != null)
            currentBlock = mb;
    }

    /// <summary>SlotClick �a��r�r; mevcut blo�u yerle�tirir.</summary>
    public static void PlaceCurrentBlock(Transform slot)
    {
        if (currentBlock == null) return;
        currentBlock.PlaceOnSlot(slot);
        currentBlock = null;
    }
}
