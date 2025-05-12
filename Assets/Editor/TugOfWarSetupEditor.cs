// Assets/Editor/TugOfWarSetupEditor.cs
using UnityEngine;
using UnityEditor;

public static class TugOfWarSetupEditor
{
    [MenuItem("Tools/Setup TugOfWar Scene")]
    public static void SetupScene()
    {
        // 1) Sol Spawn Noktaları
        var left1 = new GameObject("LeftSpawn1").transform;
        left1.position = new Vector3(-5f, 1f, 0f);
        var left2 = new GameObject("LeftSpawn2").transform;
        left2.position = new Vector3(-5f, 1f, 1f);
        var left3 = new GameObject("LeftSpawn3").transform;
        left3.position = new Vector3(-5f, 1f, -1f);

        // 2) Sağ Spawn Noktaları
        var right1 = new GameObject("RightSpawn1").transform;
        right1.position = new Vector3(5f, 1f, 0f);
        var right2 = new GameObject("RightSpawn2").transform;
        right2.position = new Vector3(5f, 1f, 1f);
        var right3 = new GameObject("RightSpawn3").transform;
        right3.position = new Vector3(5f, 1f, -1f);

        // 3) Vagon ve Manager
        var wagon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wagon.name = "Wagon";
        var rb = wagon.AddComponent<Rigidbody>();
        rb.drag = rb.angularDrag = 2f;

        var mgrGO = new GameObject("TugOfWarManager");
        var mgr   = mgrGO.AddComponent<TugOfWarManager>();
        mgr.wagonRb = rb;

        // 4) Dizilere atama
        mgr.leftSpawnPoints  = new Transform[]{ left1, left2, left3 };
        mgr.rightSpawnPoints = new Transform[]{ right1, right2, right3 };

        // 5) Prefab yükleme (örnek)
        var leftPrefab  = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LeftCharacter.prefab");
        var rightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RightCharacter.prefab");
        mgr.leftPullerPrefab  = leftPrefab.GetComponent<CharacterPull>();
        mgr.rightPullerPrefab = rightPrefab.GetComponent<CharacterPull>();

        Debug.Log("✔ TugOfWar sahnesi oluşturuldu (3+3 spawn noktasıyla).");
    }
}