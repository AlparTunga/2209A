using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public float rotationSpeed = 1;
    public float BlastPower = 5;

    public GameObject Cannonball;
    public Transform ShotPoint;

    public GameObject Explosion;

    // Son atılan topu saklamak için
    public GameObject LastFiredBall { get; private set; }

    private void Update()
    {
        float HorizontalRotation = Input.GetAxis("Horizontal");
        float VericalRotation = Input.GetAxis("Vertical");

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles +
                                              new Vector3(0, HorizontalRotation * rotationSpeed, VericalRotation * rotationSpeed));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject CreatedCannonball = Instantiate(Cannonball, ShotPoint.position, ShotPoint.rotation);
            CreatedCannonball.GetComponent<Rigidbody>().velocity = ShotPoint.transform.up * BlastPower;

            // Son topu sakla
            LastFiredBall = CreatedCannonball;

            Destroy(Instantiate(Explosion, ShotPoint.position, ShotPoint.rotation), 2);
            Screenshake.ShakeAmount = 5;
        }
    }
}