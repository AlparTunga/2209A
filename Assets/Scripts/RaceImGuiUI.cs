using UnityEngine;
using ImGuiNET;

public class RaceImGuiUI : MonoBehaviour
{
    public RaceManager raceManager;

    private bool introComplete = false;
    private float introAlpha = 0f;
    private float introFadeSpeed = 0.5f;

    void OnEnable() => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        if (raceManager == null) return;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 windowSize = new Vector2(480, 300);
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.Always);

        if (!introComplete)
        {
            Vector2 centerPos = (screenSize - windowSize) * 0.5f;
            ImGui.SetNextWindowPos(centerPos, ImGuiCond.Always);
        }
        else
        {
            Vector2 topLeftPos = new Vector2(20, 20);
            ImGui.SetNextWindowPos(topLeftPos, ImGuiCond.Always);
        }

        ImGui.Begin("Race Intro", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);

        if (!introComplete)
        {
            introAlpha = Mathf.Clamp01(introAlpha + Time.deltaTime * introFadeSpeed);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, introAlpha);

            ImGui.TextWrapped("Bu bir hiz - zaman - yol iliskisini gosteren yaristirma uygulamasidir.");
            ImGui.Spacing();
            ImGui.TextWrapped("Fizik formulu:");
            ImGui.Text("Yol = Hiz x Zaman");
            ImGui.Spacing();
            ImGui.TextWrapped("Iki araci farkli hizlarla yaristirarak hedef mesafeye kim daha once varir gozlemleyebilirsin.");
            ImGui.Spacing();

            if (introAlpha >= 1f && ImGui.Button("Devam Et"))
            {
                introComplete = true;
            }

            ImGui.PopStyleVar();
            ImGui.End();
            return;
        }

        ShowMainRaceUI();
        ImGui.End();

        // Intro sonrası sağ üst köşede denklem penceresi
        if (introComplete)
        {
            Vector2 equationWindowSize = new Vector2(220, 60);
            Vector2 equationWindowPos = new Vector2(screenSize.x - equationWindowSize.x - 20, 20);
            ImGui.SetNextWindowSize(equationWindowSize, ImGuiCond.Always);
            ImGui.SetNextWindowPos(equationWindowPos, ImGuiCond.Always);
            ImGui.Begin("Fizik Denklemi", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);
            ImGui.Text("Yol = Hiz x Zaman");
            ImGui.End();
        }
    }

    void ShowMainRaceUI()
    {
        float speed1 = raceManager.car1.speed;
        float speed2 = raceManager.car2.speed;
        float distance = raceManager.raceDistance;

        if (!raceManager.IsRaceTriggered())
        {
            ImGui.Text("Yaris Ayarlari:");
            if (ImGui.SliderFloat("Arac 1 Hiz", ref speed1, 0f, 20f))
                raceManager.car1.speed = speed1;

            if (ImGui.SliderFloat("Arac 2 Hiz", ref speed2, 0f, 20f))
                raceManager.car2.speed = speed2;

            if (ImGui.SliderFloat("Mesafe", ref distance, 10f, 500f))
            {
                raceManager.raceDistance = distance;
                raceManager.UpdateRoadLength();
            }

            if (ImGui.Button("Yarisi Baslat"))
            {
                raceManager.TriggerRace();
            }
        }
        else
        {
            if (!raceManager.IsRaceStarted())
            {
                ImGui.Text($"Yaris basliyor: {raceManager.GetCountdown():F1} saniye");
            }
            else
            {
                ImGui.Text("Arac Bilgileri");
                ImGui.Separator();

                ImGui.Text("Arac 1");
                ImGui.Text($"Zaman: {raceManager.car1.GetElapsedTime():F1} s");
                ImGui.Text($"Hiz: {raceManager.car1.speed} m/s");
                ImGui.Text($"Yol: {raceManager.car1.GetDistanceTravelled():F2} m");

                ImGui.Separator();
                ImGui.Text("Arac 2");
                ImGui.Text($"Zaman: {raceManager.car2.GetElapsedTime():F1} s");
                ImGui.Text($"Hiz: {raceManager.car2.speed} m/s");
                ImGui.Text($"Yol: {raceManager.car2.GetDistanceTravelled():F2} m");
            }

            if (raceManager.IsRaceFinished())
            {
                ImGui.Separator();
                ImGui.Text($"Kazanan: {raceManager.GetWinner()}");
            }
        }

        ImGui.Separator();
        if (ImGui.Button("Yarisi Sifirla"))
        {
            raceManager.ResetRace();
        }
    }
}
