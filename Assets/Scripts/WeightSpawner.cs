using UnityEngine;

public class WeightSpawner : MonoBehaviour
{
    [Header("Prefab ve Spawn Noktasý")]
    [SerializeField] private GameObject massBlockPrefab;
    [SerializeField] private Transform spawnPoint;

    public static MassBlock currentBlock;

    /// <summary>UI butonuna baðlayýn.</summary>
    public void SpawnBlock()
    {
        var go = Instantiate(massBlockPrefab, spawnPoint.position, Quaternion.identity);
        var mb = go.GetComponent<MassBlock>();
        if (mb != null)
            currentBlock = mb;
    }

    /// <summary>SlotClick çaðýrýr; mevcut bloðu yerleþtirir.</summary>
    public static void PlaceCurrentBlock(Transform slot)
    {
        if (currentBlock == null) return;
        currentBlock.PlaceOnSlot(slot);
        currentBlock = null;
    }
}
