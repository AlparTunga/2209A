using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using UnityEngine.SceneManagement;

public class BoxCollisionImguı : MonoBehaviour
{
    public GameObject elasticPrefab;
    public GameObject inelasticPrefab;
    public GameObject perfectlyInelasticPrefab;
    public Transform   spawnPointLeft;
    public Transform   spawnPointRight;

    public Renderer       floorRenderer;
    public Material       iceMat;
    public Material       normalMat;
    public Material       roughMat;

    public PhysicMaterial icePhysicMat;
    public PhysicMaterial normalPhysicMat;
    public PhysicMaterial roughPhysicMat;

    public float leftMass     = 2f;
    public float rightMass    = 1f;
    public float leftInitVel  = 5f;
    public float rightInitVel = 3f;
    [Range(0f, 1f)] public float restitution = 1f;

    private enum BoxType   { Elastic, Inelastic, PerfectlyInelastic }
    private enum FloorType { Ice, Normal, Rough      }

    private BoxType   leftBoxType   = BoxType.Elastic;
    private BoxType   rightBoxType  = BoxType.Elastic;
    private FloorType selectedFloor = FloorType.Ice;

    private readonly List<GameObject> spawnedBoxes = new List<GameObject>();
    private struct CollisionInfo { public float impulse; public Vector3 v1After, v2After; }
    private CollisionInfo lastCollision;

    private float momentumLeft, momentumRight;
    private float energyLeft, energyRight;

    private static bool isLayoutRegistered = false;
    private bool showIntro       = true;  // entry screen flag
    private bool showGuideWindow = true;  // guide window flag

    void OnEnable()
    {
        if (!isLayoutRegistered)
        {
            ImGuiUn.Layout += OnLayout;
            isLayoutRegistered = true;
        }
    }

    void OnDisable()
    {
        if (isLayoutRegistered)
        {
            ImGuiUn.Layout -= OnLayout;
            isLayoutRegistered = false;
        }
    }

    void OnLayout()
    {
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- ENTRY SCREEN ---
        if (showIntro)
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(
                new Vector2(io.DisplaySize.x * 0.5f, io.DisplaySize.y * 0.5f),
                ImGuiCond.Once,
                new Vector2(0.5f, 0.5f)
            );
            ImGui.Begin("Çarpışma - Giriş", flags);
            ImGui.TextWrapped("Bu simülasyonda kutuların çarpışma çeşitleri incelenir.");
            ImGui.TextWrapped("Momentum, enerji ve itki değerleri hesaplanır.");
            ImGui.BulletText("p  = m * v                 (Momentum)");
            ImGui.BulletText("KE = 0.5 * m * v * v       (Kinetik Enerji)");
            ImGui.BulletText("J  = Δp = m * (v2 - v1)    (İtki)");
            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;
            ImGui.End();
            return;
        }


        // --- MAIN WINDOW ---
        if (!ImGui.Begin("Carpisma", flags))
        {
            ImGui.End();
            return;
        }

        ImGui.Text("Carpisma Oyunu Ayarlari:");
        ImGui.Separator();

        ImGui.SliderFloat("Sol Kutle (m1) [kg]",               ref leftMass,    0.1f, 10f);
        ImGui.SliderFloat("Sag Kutle (m2) [kg]",               ref rightMass,   0.1f, 10f);
        ImGui.SliderFloat("Sol Baslangic Hizi (u1) [m/s]",     ref leftInitVel, 0f,   20f);
        ImGui.SliderFloat("Sag Baslangic Hizi (u2) [m/s]",     ref rightInitVel,0f,   20f);
        ImGui.SliderFloat("Esneklik (e)",                      ref restitution, 0f,    1f);

        ImGui.Separator();
        ImGui.Text("Zemin Secimi:");
        if (ImGui.RadioButton("Ice",    selectedFloor == FloorType.Ice))   SetFloor(FloorType.Ice);
        if (ImGui.RadioButton("Normal", selectedFloor == FloorType.Normal))SetFloor(FloorType.Normal);
        if (ImGui.RadioButton("Rough",  selectedFloor == FloorType.Rough)) SetFloor(FloorType.Rough);

        ImGui.Separator();
        ImGui.Text("Sol Kutu Tipi:");
        if (ImGui.RadioButton("Esnek",             leftBoxType == BoxType.Elastic))            leftBoxType  = BoxType.Elastic;
        if (ImGui.RadioButton("Inelastik",         leftBoxType == BoxType.Inelastic))          leftBoxType  = BoxType.Inelastic;
        if (ImGui.RadioButton("Tamamen Inelastik", leftBoxType == BoxType.PerfectlyInelastic)) leftBoxType  = BoxType.PerfectlyInelastic;

        ImGui.Separator();
        ImGui.Text("Sag Kutu Tipi:");
        if (ImGui.RadioButton("Esnek##R",          rightBoxType == BoxType.Elastic))            rightBoxType = BoxType.Elastic;
        if (ImGui.RadioButton("Inelastik##R",      rightBoxType == BoxType.Inelastic))          rightBoxType = BoxType.Inelastic;
        if (ImGui.RadioButton("Tamamen Inelastik##R", rightBoxType == BoxType.PerfectlyInelastic)) rightBoxType = BoxType.PerfectlyInelastic;

        ImGui.Separator();
        if (ImGui.Button("Secilen Kutulari Olustur")) SpawnSelectedBoxes();
        ImGui.SameLine();
        if (ImGui.Button("Temizle"))                 ClearBoxes();

        ImGui.Separator();
        if (ImGui.Button("Baslat"))   LaunchSpawnedBoxes();
        ImGui.SameLine();
        if (ImGui.Button("Durdur"))   PauseAllBoxes();
        ImGui.SameLine();
        if (ImGui.Button("Sifirla"))  RestartScene();

        ImGui.Separator();
        if (spawnedBoxes.Count >= 2)
        {
            var rbL = spawnedBoxes[0].GetComponent<Rigidbody>();
            var rbR = spawnedBoxes[1].GetComponent<Rigidbody>();
            momentumLeft  = rbL.mass * rbL.velocity.magnitude;
            momentumRight = rbR.mass * rbR.velocity.magnitude;
            energyLeft    = 0.5f * rbL.mass * rbL.velocity.sqrMagnitude;
            energyRight   = 0.5f * rbR.mass * rbR.velocity.sqrMagnitude;
        }

        ImGui.Text("Anlik Momentum & Enerji:");
        ImGui.Text($" p1 = {momentumLeft:F2} kg·m/s   KE1 = {energyLeft:F2} J");
        ImGui.Text($" p2 = {momentumRight:F2} kg·m/s  KE2 = {energyRight:F2} J");

        ImGui.Separator();
        ImGui.Text("Son Carpisma:");
        ImGui.Text($" Itki J = {lastCollision.impulse:F2} N·s");
        ImGui.Text($" v1'   = {lastCollision.v1After:F2}");
        ImGui.Text($" v2'   = {lastCollision.v2After:F2}");

        ImGui.End();

        // --- GUIDE WINDOW (fixed top-right) ---
        if (showGuideWindow)
        {
            var io = ImGui.GetIO();
            ImGui.SetNextWindowPos(
                new Vector2(io.DisplaySize.x, 0f),
                ImGuiCond.Always,
                new Vector2(1f, 0f)
            );
            ImGui.Begin(
                "Kullanim Kilavuzu",
                ref showGuideWindow,
                ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize
            );
            ImGui.TextWrapped("Nasıl kullanılır?");
            ImGui.BulletText("Sol/Sag kutu kutlesi ve hizini ayarlayin.");
            ImGui.BulletText("Esneklik (e): 1 tam esnek, 0 tam inelastik.");
            ImGui.BulletText("Secilen Kutulari Olustur: Sahneye kutu ekler.");
            ImGui.BulletText("Baslat/Durdur/Sifirla: Hareketi kontrol eder.");
            ImGui.BulletText("Zemin Secimi: Ice/Normal/Rough secerek zemin materyalini degistirir.");
            ImGui.BulletText("Momentum & Enerji: Anlik fiziksel degerleri gosterir.");
            ImGui.BulletText("Son Carpisma: Itki ve son hizlar gosterilir.");
            ImGui.End();
        }
    }

    void SetFloor(FloorType type)
    {
        selectedFloor = type;
        if (floorRenderer != null)
        {
            floorRenderer.material = type == FloorType.Ice    ? iceMat
                                   : type == FloorType.Normal ? normalMat
                                                               : roughMat;
            var col = floorRenderer.GetComponent<Collider>();
            if (col != null)
                col.material = type == FloorType.Ice    ? icePhysicMat
                             : type == FloorType.Normal ? normalPhysicMat
                                                         : roughPhysicMat;
        }
    }

    public bool IsManagedBox(GameObject obj) => spawnedBoxes.Contains(obj);

    void SpawnSelectedBoxes()
    {
        if (spawnPointLeft == null || spawnPointRight == null) return;
        ClearBoxes();

        var leftPf  = leftBoxType   switch
        {
            BoxType.Elastic            => elasticPrefab,
            BoxType.Inelastic          => inelasticPrefab,
            BoxType.PerfectlyInelastic => perfectlyInelasticPrefab,
            _ => null
        };
        var rightPf = rightBoxType   switch
        {
            BoxType.Elastic            => elasticPrefab,
            BoxType.Inelastic          => inelasticPrefab,
            BoxType.PerfectlyInelastic => perfectlyInelasticPrefab,
            _ => null
        };
        if (leftPf == null || rightPf == null) return;

        var bL = Instantiate(leftPf,  spawnPointLeft.position,  Quaternion.identity);
        var bR = Instantiate(rightPf, spawnPointRight.position, Quaternion.identity);

        bL.GetComponent<Rigidbody>().mass = leftMass;
        bR.GetComponent<Rigidbody>().mass = rightMass;

        bL.GetComponent<Collider>().material.bounciness = restitution;
        bR.GetComponent<Collider>().material.bounciness = restitution;

        bL.AddComponent<BoxCollisionHandler>().Init(this);
        bR.AddComponent<BoxCollisionHandler>().Init(this);

        spawnedBoxes.Add(bL);
        spawnedBoxes.Add(bR);
    }

    void ClearBoxes()
    {
        foreach (var b in spawnedBoxes) if (b != null) Destroy(b);
        spawnedBoxes.Clear();
    }

    void LaunchSpawnedBoxes()
    {
        if (spawnedBoxes.Count < 2) return;
        spawnedBoxes[0].GetComponent<Rigidbody>().velocity = Vector3.right * leftInitVel;
        spawnedBoxes[1].GetComponent<Rigidbody>().velocity = Vector3.left  * rightInitVel;
    }

    void PauseAllBoxes()
    {
        foreach (var b in spawnedBoxes)
        {
            var rb = b.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LogCollision(float impulse, Vector3 v1After, Vector3 v2After)
    {
        lastCollision.impulse  = impulse;
        lastCollision.v1After  = v1After;
        lastCollision.v2After  = v2After;
    }
}
