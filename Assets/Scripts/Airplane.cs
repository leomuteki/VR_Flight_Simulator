using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : Destroyable
{
    public int Score = 0;
    public int maxHP = 500;
    [SerializeField]
    private float RotateSpeed = 0.5f;
    [SerializeField]
    private float WarpTime = 1;
    [SerializeField]
    private Follow PortalParent;
    [SerializeField]
    private Transform PortalPlane;
    [SerializeField]
    private Transform ResetPortalPosition;
    [SerializeField]
    private Transform PortalCamera;
    [SerializeField]
    private Transform TeleportPosition;
    [SerializeField]
    private ParticleSystem TeleportParticle;
    [SerializeField]
    private ParticleSystem SnowStill;
    [SerializeField]
    private ParticleSystem SnowMoving;
    [SerializeField]
    private ParticleSystem EnemyExplosion;
    [SerializeField]
    private float movingSnowMaxSpeed = 5;
    [SerializeField]
    private List<Transform> FirePoints;
    private Vector3 portalMaxScale;
    private bool portalAnimating = false;
    [SerializeField]
    private float maxSpeed = 30;
    public float MaxSpeed
    {
        get { return maxSpeed; }
    }
    [SerializeField]
    private float currentSpeed = 0;
    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }

    private void Awake()
    {
        Debug.LogWarning("yay");
        Blackboard.PlaneControls = this;
        Blackboard.Portal = PortalPlane.gameObject;
    }

    void Start()
    {
        ResetPortalPosition.position = PortalParent.transform.position;
        ResetPortalPosition.rotation = PortalParent.transform.rotation;
        portalMaxScale = PortalPlane.localScale;
        SnowMoving.Stop();
        TeleportParticle.Stop();
        Blackboard.EnemyExplosion = EnemyExplosion;
    }

    void Update()
    {
        if (Blackboard.canMove)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!PortalPlane.gameObject.activeSelf)
                {
                    OpenPortal();
                }
                else
                {
                    ClosePortal();
                }
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (PortalPlane.gameObject.activeSelf)
                {
                    Warp();
                }
            }
            if (Input.GetKey(KeyCode.F))
            {
                currentSpeed = MaxSpeed;
                SnowMoving.Play();
                SnowStill.Stop();
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                currentSpeed = 0;
                SnowStill.Play();
                StartCoroutine(SnowStopDelay(SnowMoving, 2));
            }
            if (currentSpeed > float.Epsilon)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * currentSpeed);
            }
            if (Input.GetKeyDown(KeyCode.M) || OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Two))
            {
                Blackboard.canMove = false;
                Blackboard.Menu.OpenMenu();
            }
            // Rotate plane with primary thumbstick
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0)
            {
                transform.Rotate(-Vector3.right * RotateSpeed);
            }
            else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < 0)
            {
                transform.Rotate(Vector3.right * RotateSpeed);
            }
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < 0)
            {
                transform.Rotate(-Vector3.up * RotateSpeed);
            }
            else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0)
            {
                transform.Rotate(Vector3.up * RotateSpeed);
            }
            if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < 0)
            {
                transform.Rotate(-Vector3.forward * RotateSpeed);
            }
            else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0)
            {
                transform.Rotate(Vector3.forward * RotateSpeed);
            }
        }
    }

    public override void Destroy()
    {
        hP = maxHP;
        Blackboard.Menu.AirplaneReset();
        Blackboard.Menu.OpenMenu();
        Blackboard.Menu.OpenGameOver();
    }

    public void ChangeSpeed(float speed)
    {
        if (Blackboard.canMove)
        {
            if (Mathf.Abs(speed - currentSpeed) > 1 && speed > currentSpeed)
            {
                Blackboard.Sounds.PlaySound("SpeedUp");
            }
            else if (Mathf.Abs(speed - currentSpeed) > 1 && speed < currentSpeed)
            {
                Blackboard.Sounds.PlaySound("SlowDown");
            }
            if (speed <= 10)
            {
                SnowStill.Play();
                StartCoroutine(SnowStopDelay(SnowMoving, 2));
            }
            if (speed > MaxSpeed)
            {
                if (currentSpeed == 0)
                {
                    SnowMoving.Play();
                    SnowStill.Stop();
                }
                currentSpeed = MaxSpeed;
                var main = SnowMoving.main;
                main.startSpeed = new ParticleSystem.MinMaxCurve(movingSnowMaxSpeed, movingSnowMaxSpeed);
            }
            else
            {
                if (currentSpeed == 0)
                {
                    SnowMoving.Play();
                    SnowStill.Stop();
                }
                currentSpeed = speed;
                float frac = speed / MaxSpeed;
                var main = SnowMoving.main;
                main.startSpeed = new ParticleSystem.MinMaxCurve(movingSnowMaxSpeed * frac, movingSnowMaxSpeed * frac);
            }
        }
    }

    public void Warp()
    {
        if (Blackboard.canMove && PortalPlane.gameObject.activeSelf)
        {
            Blackboard.canMove = false;
            PortalParent.isFollowing = false;
            StartCoroutine(WarpCoroutine());
        }
    }

    public void PlayTeleportParticles()
    {
        TeleportParticle.Play();
    }

    private IEnumerator SnowStopDelay(ParticleSystem snow, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        snow.Stop();
    }

    public void Teleport()
    {
        StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        Blackboard.Sounds.PlaySound("Teleport");
        yield return new WaitForSecondsRealtime(0.1f);
        transform.position = PortalCamera.position;
        transform.rotation = PortalCamera.rotation;
        PortalParent.transform.position = ResetPortalPosition.position;
        PortalParent.transform.rotation = ResetPortalPosition.rotation;
        PortalParent.isFollowing = true;
        Blackboard.canMove = true;
    }

    private IEnumerator WarpCoroutine()
    {
        Vector3 startPosition = transform.position;
        float startTime = Time.time;
        float fracJourney = 0;
        SnowMoving.Play();
        SnowStill.Play();
        while (fracJourney < 1)
        {
            fracJourney = (Time.time - startTime) / WarpTime;
            fracJourney *= fracJourney;
            transform.position = Vector3.Lerp(startPosition, TeleportPosition.position, fracJourney);
            if (fracJourney > 0.8)
            {
                TeleportParticle.Play();
            }
            yield return null;
        }
        Teleport();
        SnowMoving.Stop();
    }

    public void OpenPortal()
    {
        if (!PortalPlane.gameObject.activeInHierarchy && !portalAnimating)
        {
            Blackboard.ResetPortal();
            portalAnimating = true;
            Blackboard.Sounds.PlaySoundFade(2, "Portal", 0.5f);
            StartCoroutine(InterpolateSizeCoroutine(PortalPlane, portalMaxScale, true, 0.5f, true));
        }
    }

    public void ClosePortal()
    {
        if (PortalPlane.gameObject.activeInHierarchy && !portalAnimating)
        {
            portalAnimating = true;
            Blackboard.Sounds.StopSoundFade(2, "Portal", 0.5f);
            StartCoroutine(InterpolateSizeCoroutine(PortalPlane, portalMaxScale, false, 0.5f, true));
        }
    }

    public IEnumerator InterpolateSizeCoroutine(Transform scaleTransform, Vector3 maxSize, bool grow, float seconds, bool deactivate)
    {
        if (grow)
        {
            scaleTransform.localScale = Vector3.zero;
            scaleTransform.gameObject.SetActive(true);
        }
        float startTime = Time.time;
        float interpolator = grow ? 0 : 1;
        while ((!grow && interpolator > float.Epsilon) || (grow && interpolator < (1 - float.Epsilon)))
        {
            scaleTransform.localScale = interpolator * maxSize;
            yield return null;
            if (grow)
            {
                interpolator = (Time.time - startTime) / seconds;
            }
            else
            {
                interpolator = 1 - ((Time.time - startTime) / seconds);
            }
        }
        if (!grow && deactivate)
        {
            scaleTransform.gameObject.SetActive(false);
            scaleTransform.localScale = maxSize;
        }
        portalAnimating = false;
    }
}
