using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    private MassBlock selectedBlock;
    public LayerMask groundMask; // Sadece tahtayý algýlayacak Layer

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (selectedBlock != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                selectedBlock.PlaceAt(hit.point);
                selectedBlock = null;
            }
        }
    }

    public void SetSelectedBlock(MassBlock block)
    {
        selectedBlock = block;
    }
}
