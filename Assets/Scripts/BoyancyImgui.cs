using UnityEngine;
using ImGuiNET;

public class BuoyancyUI : MonoBehaviour
{
    [Header("References")]
    public Rigidbody blockRb;            // Kupun Rigidbody'si
    public Buoyancy buoyancy;            // Kupun uzerinde duran Buoyancy script'i
    public Transform waterVolume;        // Su hacmi objesi

    [Header("Renderers")]
    public Renderer blockRenderer;       // Kupun MeshRenderer'i
    public Renderer waterRenderer;       // Su hacmi objesinin MeshRenderer'i

    [Header("Block Materials (7 adet)")]
    public Material[] blockMaterials;    // Styrofoam→Iron sirasiyla atanacak

    [Header("Fluid Colors (7 adet)")]
    public Color[] fluidColors = new Color[7]
    {
        new Color(0.9f, 0.9f, 0.3f, 0.4f),   // Gasoline
        new Color(0.4f, 0.3f, 0.2f, 0.4f),   // Crude Oil
        new Color(0.9f, 0.8f, 0.5f, 0.4f),   // Olive Oil
        new Color(0.5f, 0.8f, 1f, 0.4f),     // Water
        new Color(0.9f, 0.7f, 0.3f, 0.4f),   // Honey
        new Color(1f, 0.6f, 0.6f, 0.4f),     // Nitric Acid
        new Color(0.6f, 0.6f, 0.6f, 0.4f)    // Mercury
    };
    private Material[] fluidMaterials;

    // Block types and densities (kg/m3)
    private string[] blockNames     = { "Styrofoam", "Wood", "Ice", "Plastic", "Brick", "Aluminium", "Iron" };
    private float[]  blockDensities = {    150f,    750f,   917f,   980f,   1800f,   2750f,  7870f };
    private int      selectedBlock;

    // Fluid types and densities (kg/m3)
    private string[] fluidNames     = { "Gasoline", "Crude Oil", "OliveOil", "Water", "Honey", "Nitric Acid", "Mercury" };
    private float[]  fluidDensities = {    770f,     870f,     930f,   1000f,   1360f,    1560f,   13590f };
    private int      selectedFluid = 3;

    [Header("Block Volume (m3)")]
    [Tooltip("1x1x1 m3 ise 1, degilse gercek hacmi girin")]
    public float blockVolume = 1f;

    private bool showIntro = true;
    private int prevBlock = -1, prevFluid = -1;

    void Awake()
    {
        if (waterVolume == null)
            waterVolume = GameObject.Find("WaterVolume")?.transform;

        fluidMaterials = new Material[fluidColors.Length];
        for (int i = 0; i < fluidColors.Length; i++)
        {
            var mat = new Material(Shader.Find("Standard"));
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
        // Her pencereyi icerige gore yeniden boyutlandir
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        if (showIntro)
        {
            ImGui.Begin("Sivilarin Kaldirma Kuvveti - Giris", flags);

            ImGui.TextWrapped("Bir cisme, icine daldirildigi sivi tarafindan yukari dogru uygulanan kuvvete 'kaldirma kuvveti' denir.");
            ImGui.TextWrapped("Bu kuvvet, cismin hacmine ve sivinin yogunluguna baglidir.");

            ImGui.BulletText("Fg = m * g           (Agirlik kuvveti)");
            ImGui.BulletText("Fb = rho * V * g     (Kaldirma kuvveti)");
            ImGui.BulletText("Fnet = Fb - Fg       (Net kuvvet)");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        ImGui.Begin("Forces on 1m3 Block", flags);

        // — Block Selection —
        ImGui.Text("Block Type:");
        ImGui.SliderInt("##blk", ref selectedBlock, 0, blockNames.Length - 1);
        ImGui.SameLine();
        ImGui.Text($"{blockNames[selectedBlock]}  {blockDensities[selectedBlock]:F0} kg/m3");

        ImGui.Separator();

        // — Fluid Selection —
        ImGui.Text("Fluid Type:");
        ImGui.SliderInt("##fld", ref selectedFluid, 0, fluidNames.Length - 1);
        ImGui.SameLine();
        ImGui.Text($"{fluidNames[selectedFluid]}  {fluidDensities[selectedFluid]:F0} kg/m3");

        ImGui.Separator();

        if (selectedBlock != prevBlock || selectedFluid != prevFluid)
        {
            ApplyChanges();
            prevBlock = selectedBlock;
            prevFluid = selectedFluid;
        }

        float g    = Physics.gravity.magnitude;
        float bD   = blockDensities[selectedBlock];
        float fD   = fluidDensities[selectedFluid];
        float gravF = bD * blockVolume * g;
        float buoyF = (bD < fD) ? gravF : fD * blockVolume * g;
        float netF  = buoyF - gravF;

        ImGui.Text($"Gravity:   {gravF:F0} N ▼");
        ImGui.Text($"Buoyancy:  {buoyF:F0} N ▲");
        ImGui.Text($"Net Force: {netF:F0} N");

        ImGui.End();
    }

    void ApplyChanges()
    {
        blockRb.mass          = blockDensities[selectedBlock] * blockVolume;
        buoyancy.fluidDensity = fluidDensities[selectedFluid];

        if (blockMaterials != null && blockMaterials.Length > selectedBlock)
            blockRenderer.sharedMaterial = blockMaterials[selectedBlock];
        if (fluidMaterials != null && fluidMaterials.Length > selectedFluid)
            waterRenderer.sharedMaterial = fluidMaterials[selectedFluid];

        RepositionBlock();
    }

    void RepositionBlock()
    {
        if (waterVolume == null) return;

        float halfH    = waterVolume.localScale.y * 0.5f;
        float centerY  = waterVolume.position.y;
        float surfaceY = centerY + halfH;
        float bottomY  = centerY - halfH;

        float hBlock = blockRb.transform.localScale.y;
        float bD     = blockDensities[selectedBlock];
        float fD     = fluidDensities[selectedFluid];
        float ratio  = bD / fD;

        float newY;
        if (ratio >= 1f)
            newY = bottomY + hBlock * 0.5f;
        else
        {
            float depth     = ratio * hBlock;
            float bottomPos = surfaceY - depth;
            newY            = bottomPos + hBlock * 0.5f;
        }

        Vector3 p = blockRb.transform.position;
        blockRb.transform.position  = new Vector3(p.x, newY, p.z);
        blockRb.velocity            = Vector3.zero;
        blockRb.angularVelocity     = Vector3.zero;
    }
}
