using UnityEngine;
using ImGuiNET;

public class BuoyancyUI : MonoBehaviour
{
    [Header("Referanslar")]
    public Rigidbody blockRb;            // Küpün Rigidbody'si
    public Buoyancy buoyancy;            // Küpün üzerinde duran Buoyancy script'i
    public Transform waterVolume;        // Su hacmi objesi

    [Header("Renderer'lar")]
    public Renderer blockRenderer;       // Küpün MeshRenderer'ı
    public Renderer waterRenderer;       // Su hacmi objesinin MeshRenderer'ı

    [Header("Block Materials (7 adet)")]
    public Material[] blockMaterials;    // Styrofoam→Iron sırasıyla atanacak

    [Header("Fluid Colors (7 adet)")]
    public Color[] fluidColors = new Color[7]
    {
        new Color(0.9f, 0.9f, 0.3f, 0.4f),   // Gasoline
        new Color(0.4f,0.3f,0.2f,0.4f), // Crude Oil
        new Color(0.9f,0.8f,0.5f,0.4f), // Olive Oil
        new Color(0.5f,0.8f,1f,0.4f),   // Water
        new Color(0.9f,0.7f,0.3f,0.4f), // Honey
        new Color(1f,0.6f,0.6f,0.4f),   // Nitric Acid
        new Color(0.6f,0.6f,0.6f,0.4f)  // Mercury
    };
    Material[] fluidMaterials;

    // Blok türleri ve yoğunlukları (kg/m³)
    string[] blockNames     = { "Styrofoam","Wood","Ice","Plastic","Brick","Aluminium","Iron" };
    float[]  blockDensities = {     150f,     750f,   917f,   980f,   1800f,   2750f,  7870f };
    int      selectedBlock;

    // Sıvı türleri ve yoğunlukları (kg/m³)
    string[] fluidNames     = { "Gasoline","Crude Oil","OliveOil","Water","Honey","Nitric Acid","Mercury" };
    float[]  fluidDensities = {     770f,     870f,    930f,  1000f,   1360f,    1560f, 13590f };
    int      selectedFluid = 3;  

    [Header("Blok Hacmi (m³)")]
    [Tooltip("1×1×1 m³ ise 1, değilse gerçek hacmi girin")]
    public float blockVolume = 1f;

    int prevBlock = -1, prevFluid = -1;

    void Awake()
    {
        // WaterVolume referansı yoksa sahneden bul
        if (waterVolume == null)
            waterVolume = GameObject.Find("WaterVolume")?.transform;

        // Fluid materyallerini renk tablosuna göre oluştur
        fluidMaterials = new Material[fluidColors.Length];
        for (int i = 0; i < fluidColors.Length; i++)
        {
            var mat = new Material(Shader.Find("Standard"));
            // Transparan ayarları
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.color = fluidColors[i];
            fluidMaterials[i] = mat;
        }
    }

    void Start()
    {
        ApplyChanges();
    }

    void OnEnable()  => ImGuiUn.Layout += LayoutUI;
    void OnDisable() => ImGuiUn.Layout -= LayoutUI;

    void LayoutUI()
    {
        ImGui.Begin("Forces on 1m³ Block");

        // — Blok Seçimi —
        ImGui.Text("Block Type:");
        ImGui.SliderInt("##blk", ref selectedBlock, 0, blockNames.Length - 1);
        ImGui.SameLine();
        ImGui.Text($"{blockNames[selectedBlock]}  {blockDensities[selectedBlock]:F0} kg/m³");

        ImGui.Separator();

        // — Sıvı Seçimi —
        ImGui.Text("Fluid Type:");
        ImGui.SliderInt("##fld", ref selectedFluid, 0, fluidNames.Length - 1);
        ImGui.SameLine();
        ImGui.Text($"{fluidNames[selectedFluid]}  {fluidDensities[selectedFluid]:F0} kg/m³");

        ImGui.Separator();

        // Seçim değiştiyse hemen uygula
        if (selectedBlock != prevBlock || selectedFluid != prevFluid)
        {
            ApplyChanges();
            prevBlock = selectedBlock;
            prevFluid = selectedFluid;
        }

        // — Kuvvetleri Göster —
        float g        = Physics.gravity.magnitude;
        float bD       = blockDensities[selectedBlock];
        float fD       = fluidDensities[selectedFluid];
        float gravF    = bD * blockVolume * g;
        float buoyF    = (bD < fD) ? gravF : fD * blockVolume * g;
        float netF     = buoyF - gravF;

        ImGui.Text($"Gravity:   {gravF:F0} N ▼");
        ImGui.Text($"Buoyancy:  {buoyF:F0} N ▲");
        ImGui.Text($"Net Force: {netF:F0} N");

        ImGui.End();
    }

    void ApplyChanges()
    {
        // 1) Kütleyi ve fluidDensity'i güncelle
        blockRb.mass          = blockDensities[selectedBlock] * blockVolume;
        buoyancy.fluidDensity = fluidDensities[selectedFluid];

        // 2) Malzemeleri ata
        if (blockMaterials != null && blockMaterials.Length > selectedBlock)
            blockRenderer.sharedMaterial = blockMaterials[selectedBlock];
        if (fluidMaterials != null && fluidMaterials.Length > selectedFluid)
            waterRenderer.sharedMaterial = fluidMaterials[selectedFluid];

        // 3) Konumu güncelle
        RepositionBlock();
    }

    void RepositionBlock()
    {
        if (waterVolume == null) return;

        float halfH    = waterVolume.localScale.y * 0.5f;
        float centerY  = waterVolume.position.y;
        float surfaceY = centerY + halfH;
        float bottomY  = centerY - halfH;

        float hBlock   = blockRb.transform.localScale.y;
        float bD       = blockDensities[selectedBlock];
        float fD       = fluidDensities[selectedFluid];
        float ratio    = bD / fD;

        float newY;
        if (ratio >= 1f)
            newY = bottomY + hBlock * 0.5f;               // Tamamen batar
        else
        {
            float depth     = ratio * hBlock;             // Kısmen batar derinliği
            float bottomPos = surfaceY - depth;
            newY            = bottomPos + hBlock * 0.5f;
        }

        Vector3 p = blockRb.transform.position;
        blockRb.transform.position  = new Vector3(p.x, newY, p.z);
        blockRb.velocity            = Vector3.zero;
        blockRb.angularVelocity     = Vector3.zero;
    }
}
