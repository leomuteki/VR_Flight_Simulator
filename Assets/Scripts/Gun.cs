using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool IsFiring = false;
    private GameObject Explosion;
    private const float MaxDistance = 100;
    [SerializeField]
    private Transform FirePoint;
    [SerializeField]
    private float RotationSpeed = 100;
    [SerializeField]
    private int AttackPower = 10;
    [SerializeField]
    private float DamageDelay = 1;
    private float lastTime = 0;
    private AudioSource GunSound;

    private void Start()
    {
        Explosion = transform.Find("Explosion").gameObject;
        Explosion.SetActive(false);
        GunSound = GetComponent<AudioSource>();
        GunSound.Stop();
    }

    public void StartFiring()
    {
        Explosion.SetActive(true);
        GunSound.Play();
        IsFiring = true;
        lastTime = Time.time;
    }

    public void StopFiring()
    {
        Explosion.SetActive(false);
        GunSound.Stop();
        IsFiring = false;
    }

    private void Update()
    {
        if (IsFiring)
        {
            //int layerMask = 1 << (LayerMask.NameToLayer("Airplane") | LayerMask.NameToLayer("Enemy"));

            RaycastHit hit;
            if ((Time.time - lastTime) > DamageDelay && Physics.Raycast(FirePoint.position, FirePoint.TransformDirection(Vector3.forward), out hit, MaxDistance))//, layerMask))
            {
                Destroyable foe = hit.transform.GetComponent<Destroyable>();
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    foe = hit.transform.parent.GetComponent<Destroyable>();
                }
                if (foe)
                {
                    foe.Damage(AttackPower);
                    lastTime = Time.time;
                }
            }
            transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
        }
    }
}
