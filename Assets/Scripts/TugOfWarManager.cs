using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TugOfWarManager : MonoBehaviour
{
    [Header("Wagon Rigidbody (assign in Inspector)")]
    public Rigidbody wagonRb;

    [Header("Puller Prefabs")]
    public CharacterPull leftPullerPrefab;
    public CharacterPull rightPullerPrefab;

    [Header("Left Spawn Points (assign multiple)")]
    public Transform[] leftSpawnPoints;

    [Header("Right Spawn Points (assign multiple)")]
    public Transform[] rightSpawnPoints;

    [Header("Max pullers per side")]
    public int maxPullersPerSide = 4;

    [Header("How many Newton → 1 m/s")]
    public float speedScale = 0.01f;

    [Header("Active Pullers (do NOT assign manually)")]
    public List<CharacterPull> pullers = new List<CharacterPull>();

    Vector3   _startPos;
    Quaternion _startRot;

    void Start()
    {
        if (wagonRb == null)
            Debug.LogError("🚨 wagonRb atanmamış!");

        wagonRb.isKinematic = false;
        _startPos = wagonRb.position;
        _startRot = wagonRb.rotation;
    }

    void FixedUpdate()
    {
        float leftSum  = pullers
            .Where(p => p.IsPulling && p.transform.position.x < wagonRb.position.x)
            .Sum(p => p.pullForce);
        float rightSum = pullers
            .Where(p => p.IsPulling && p.transform.position.x > wagonRb.position.x)
            .Sum(p => p.pullForce);

        // Sağ kuvvet ağır basarsa +X, sol ağır basarsa –X
        float netForce = rightSum - leftSum;
        float friction = -wagonRb.drag * wagonRb.velocity.x;
        float totalForce = netForce + friction;

        Vector3 v = wagonRb.velocity;
        v.x = totalForce * speedScale;
        wagonRb.velocity = v;
    }

    public CharacterPull AddLeftPuller(float force)
    {
        if (leftPullerPrefab == null || leftSpawnPoints == null || leftSpawnPoints.Length == 0)
        {
            Debug.LogError("🚨 leftPullerPrefab veya leftSpawnPoints eksik!");
            return null;
        }

        int count = pullers.Count(p => p.transform.position.x < wagonRb.position.x);
        if (count >= maxPullersPerSide)
        {
            Debug.LogWarning("⚠️ Solda limit dolu");
            return null;
        }
        if (count >= leftSpawnPoints.Length)
        {
            Debug.LogWarning($"⚠️ Soldaki spawnPoints dizisi ({leftSpawnPoints.Length}) yetersiz");
            return null;
        }

        var spawn = leftSpawnPoints[count];
        var go = Instantiate(leftPullerPrefab.gameObject, spawn.position, spawn.rotation);
        var cp = go.GetComponent<CharacterPull>();
        cp.wagonRb   = wagonRb;
        cp.pullForce = force;
        pullers.Add(cp);
        return cp;
    }

    public CharacterPull AddRightPuller(float force)
    {
        if (rightPullerPrefab == null || rightSpawnPoints == null || rightSpawnPoints.Length == 0)
        {
            Debug.LogError("🚨 rightPullerPrefab veya rightSpawnPoints eksik!");
            return null;
        }

        int count = pullers.Count(p => p.transform.position.x > wagonRb.position.x);
        if (count >= maxPullersPerSide)
        {
            Debug.LogWarning("⚠️ Sağda limit dolu");
            return null;
        }
        if (count >= rightSpawnPoints.Length)
        {
            Debug.LogWarning($"⚠️ Sağdaki spawnPoints dizisi ({rightSpawnPoints.Length}) yetersiz");
            return null;
        }

        var spawn = rightSpawnPoints[count];
        var go = Instantiate(rightPullerPrefab.gameObject, spawn.position, spawn.rotation);
        var cp = go.GetComponent<CharacterPull>();
        cp.wagonRb   = wagonRb;
        cp.pullForce = force;
        pullers.Add(cp);
        return cp;
    }

    public void StartRace() => pullers.ForEach(p => p.StartPull());
    public void StopRace()  => pullers.ForEach(p => p.StopPull());

    public void ResetRace()
    {
        foreach (var p in pullers)
            if (p != null)
                Destroy(p.gameObject);
        pullers.Clear();

        wagonRb.velocity        = Vector3.zero;
        wagonRb.angularVelocity = Vector3.zero;
        wagonRb.position        = _startPos;
        wagonRb.rotation        = _startRot;
    }
}
