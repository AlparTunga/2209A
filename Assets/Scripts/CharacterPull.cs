// Assets/Scripts/CharacterPull.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CharacterPull : MonoBehaviour
{
    [Header("Wagon Rigidbody (assign in Inspector)")]
    public Rigidbody wagonRb;

    [Header("Strength of this puller (N)")]
    public float pullForce = 100f;

    Animator _anim;
    bool     _pulling;

    void Awake()
    {
        // Animator bileşenini al
        _anim = GetComponent<Animator>();

        // Karakterin hareketsiz kalması için kinematik yap
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Start()
    {
        if (wagonRb == null)
            Debug.LogError($"{name}: wagonRb atanmamış!");
    }

    /// <summary>GUI’den veya manager’dan çekiş başlatmak için çağrılır</summary>
    public void StartPull()
    {
        _pulling = true;
        if (_anim != null)
            _anim.SetBool("isPulling", true);
    }

    /// <summary>Çekişi durdurmak için çağrılır</summary>
    public void StopPull()
    {
        _pulling = false;
        if (_anim != null)
            _anim.SetBool("isPulling", false);
    }

    /// <summary>Şu an çekiş yapıyor mu?</summary>
    public bool IsPulling => _pulling;
}