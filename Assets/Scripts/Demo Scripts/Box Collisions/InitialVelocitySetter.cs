using System.Collections.Generic;
using UnityEngine;

public class InitialVelocitySetter : MonoBehaviour
{
    [Header("Kutuların Rigidbody Listeleri (opsiyonel)")]
    [SerializeField] private List<Rigidbody> _elasticBoxes = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> _inelasticBoxes = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> _perfectlyInelasticBoxes = new List<Rigidbody>();

    [Header("Başlangıç Hızları ve Kütleler")]
    [SerializeField] private float _box1InitialVelocity = 0f;
    [SerializeField] private float _box2InitialVelocity = 0f;
    [Min(0.0001f)]
    [SerializeField] private float _box1mass = 1f;
    [Min(0.0001f)]
    [SerializeField] private float _box2mass = 1f;

    void Start()
    {
        // Başlangıçta sadece kütleleri ayarla
        SetMasses();
    }

    public void SetMasses()
    {
        // Eğer listeler boşsa uyar, çık
        if (_elasticBoxes.Count == 0 &&
            _inelasticBoxes.Count == 0 &&
            _perfectlyInelasticBoxes.Count == 0)
        {
            Debug.LogWarning("InitialVelocitySetter: Rigidbody listeleri atanmadı (Inspector).");
            return;
        }

        // Elastik
        if (_elasticBoxes.Count > 0) _elasticBoxes[0].mass = _box1mass;
        if (_elasticBoxes.Count > 1) _elasticBoxes[1].mass = _box2mass;

        // İnelastik
        if (_inelasticBoxes.Count > 0) _inelasticBoxes[0].mass = _box1mass;
        if (_inelasticBoxes.Count > 1) _inelasticBoxes[1].mass = _box2mass;

        // Tamamen İnelastik
        if (_perfectlyInelasticBoxes.Count > 0) _perfectlyInelasticBoxes[0].mass = _box1mass;
        if (_perfectlyInelasticBoxes.Count > 1) _perfectlyInelasticBoxes[1].mass = _box2mass;
    }

    public void ApplyVelocities()
    {
        // Eğer listeler boşsa uyar, çık
        if (_elasticBoxes.Count == 0 &&
            _inelasticBoxes.Count == 0 &&
            _perfectlyInelasticBoxes.Count == 0)
        {
            Debug.LogWarning("InitialVelocitySetter: Rigidbody listeleri atanmadı (Inspector).");
            return;
        }

        // Elastik
        if (_elasticBoxes.Count > 0) _elasticBoxes[0].velocity = new Vector3(_box1InitialVelocity, 0, 0);
        if (_elasticBoxes.Count > 1) _elasticBoxes[1].velocity = new Vector3(_box2InitialVelocity, 0, 0);

        // İnelastik
        if (_inelasticBoxes.Count > 0) _inelasticBoxes[0].velocity = new Vector3(_box1InitialVelocity, 0, 0);
        if (_inelasticBoxes.Count > 1) _inelasticBoxes[1].velocity = new Vector3(_box2InitialVelocity, 0, 0);

        // Tamamen İnelastik
        if (_perfectlyInelasticBoxes.Count > 0) _perfectlyInelasticBoxes[0].velocity = new Vector3(_box1InitialVelocity, 0, 0);
        if (_perfectlyInelasticBoxes.Count > 1) _perfectlyInelasticBoxes[1].velocity = new Vector3(_box2InitialVelocity, 0, 0);
    }
}
