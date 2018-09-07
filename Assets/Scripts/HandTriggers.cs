using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class HandTriggers : MonoBehaviour {

    [SerializeField]
    private bool amIRightHand = true;
    [SerializeField]
    private float PortalSpeed = 1.5f;
    [SerializeField]
    private Transform Portal;
    [SerializeField]
    private GameObject RollDetectionStuff;
    [SerializeField]
    private Transform DetectorShells;
    [SerializeField]
    private Transform Knob;
    [SerializeField]
    private Transform SpeedHandle;
    [SerializeField]
    private Vector2 SpeedHandleAngleRange = new Vector2(0.13f, 0.92f);
    [SerializeField]
    private float recenterSpeed = 5;
    [SerializeField]
    private Gun RightGun;
    [SerializeField]
    private Gun LeftGun;
    TrailRenderer Trail;
    private Quaternion knobCenteredRotation;
    private bool buttonAnimating = false;
    private enum DrawShape { linear, circular, ambiguous };
    [SerializeField]
    private float DrawThreshold = 0.3f;
    [SerializeField]
    private PostProcessingBehaviour postProcessing;
    [SerializeField]
    private PostProcessingProfile NoVignette;
    [SerializeField]
    private PostProcessingProfile Vignette;

    private void Start()
    {
        knobCenteredRotation = Knob.localRotation;
        Trail = GetComponent<TrailRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "button" && !buttonAnimating)
        {
            ButtonPress hit_button = other.GetComponent<ButtonPress>();
            if (hit_button)
            {
                buttonAnimating = true;
                StartCoroutine(ButtonAimation(hit_button.transform, hit_button.NotPressed, hit_button.Pressed, 0.2f));
                if (hit_button.Type == ButtonPress.ButtonType.Speed)
                {
                    float interpolatedSpeed = (Blackboard.PlaneControls.MaxSpeed * (SpeedHandle.rotation.w - SpeedHandleAngleRange.x)) / (SpeedHandleAngleRange.y - SpeedHandleAngleRange.x);
                    if (interpolatedSpeed > Blackboard.PlaneControls.MaxSpeed)
                    {
                        interpolatedSpeed = Blackboard.PlaneControls.MaxSpeed;
                    }
                    else if (interpolatedSpeed < 0)
                    {
                        interpolatedSpeed = 0;
                    }
                    Blackboard.PlaneControls.ChangeSpeed(interpolatedSpeed);
                }
                else if (hit_button.Type == ButtonPress.ButtonType.Warp)
                {
                    Blackboard.PlaneControls.Warp();
                }
                else if (hit_button.Type == ButtonPress.ButtonType.ResetPortal)
                {
                    Blackboard.ResetPortal();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.75f || OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.75f))
        {
            if (other.gameObject.tag == "knob" && !DetectorShells.gameObject.activeInHierarchy)
            {
                ActivateDetectorShells();
            }
            else if (other.gameObject.tag == "portalUp")
            {
                Portal.Rotate(-Vector3.right * PortalSpeed);
            }
            else if (other.gameObject.tag == "portalDown")
            {
                Portal.Rotate(Vector3.right * PortalSpeed);
            }
            else if (other.gameObject.tag == "portalLeft")
            {
                Portal.Rotate(-Vector3.up * PortalSpeed);
            }
            else if (other.gameObject.tag == "portalRight")
            {
                Portal.Rotate(Vector3.up * PortalSpeed);
            }
            else if (other.gameObject.tag == "rightGunTrigger")
            {
                if (!RightGun.IsFiring)
                {
                    RightGun.StartFiring();
                }
            }
            else if (other.gameObject.tag == "leftGunTrigger")
            {
                if (!LeftGun.IsFiring)
                {
                    LeftGun.StartFiring();
                }
            }
        }
        else if (other.gameObject.tag == "knob" && DetectorShells.gameObject.activeInHierarchy)
        {
            ResetDetectorShells();
        }
        else if (other.gameObject.tag == "rightGunTrigger")
        {
            if (RightGun.IsFiring)
            {
                RightGun.StopFiring();
            }
        }
        else if (other.gameObject.tag == "leftGunTrigger")
        {
            if (LeftGun.IsFiring)
            {
                LeftGun.StopFiring();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "knob")
        {
            ResetDetectorShells();
        }
        else if (other.gameObject.tag == "rightGunTrigger" && RightGun.IsFiring)
        {
            RightGun.StopFiring();
        }
        else if (other.gameObject.tag == "leftGunTrigger" && LeftGun.IsFiring)
        {
            LeftGun.StopFiring();
        }
    }

    private void ActivateDetectorShells()
    {
        DetectorShells.SetParent(null);
        RollDetectionStuff.SetActive(true);
    }

    private void ResetDetectorShells()
    {
        RollDetectionStuff.SetActive(false);
        DetectorShells.SetParent(RollDetectionStuff.transform);
        DetectorShells.localPosition = Vector3.zero;
        DetectorShells.localEulerAngles = Vector3.zero;
        StartCoroutine(RecenterKnob());
    }

    private IEnumerator RecenterKnob()
    {
        while (Vector3.Distance(Knob.localEulerAngles, knobCenteredRotation.eulerAngles) > 0.1f)
        {
            Knob.localRotation = Quaternion.Lerp(Knob.localRotation, knobCenteredRotation, Time.deltaTime * recenterSpeed);
            yield return null;
        }
    }

    private void Update()
    {
        if (DetectorShells.gameObject.activeInHierarchy)
        {
            DetectorShells.position = transform.position;
        }
        if (Blackboard.canMove && ((!amIRightHand && (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.75f)) || (amIRightHand && (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.75f))))
        {
            Blackboard.canMove = false;
            if (postProcessing.profile == NoVignette)
            {
                postProcessing.profile = Vignette;
            }
            StartCoroutine(DrawTrail());
        }
    }

    private IEnumerator DrawTrail()
    {
        Trail.enabled = true;
        Trail.Clear();
        while ((!amIRightHand && (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.75f)) || (amIRightHand && (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.75f)))
        {
            yield return null;
        }
        if (Trail.positionCount > 2)
        {
            Vector3[] points = new Vector3[Trail.positionCount + 1];
            Trail.GetPositions(points);
            switch (ApproximateShape(points))
            {
                case (DrawShape.circular):
                    Blackboard.PlaneControls.OpenPortal();
                    print("circle");
                    break;
                case (DrawShape.linear):
                    Blackboard.PlaneControls.ClosePortal();
                    print("line");
                    break;
                case (DrawShape.ambiguous):
                    print("ambiguous");
                    break;
                default:
                    break;
            }
        }
        Trail.Clear();
        Trail.enabled = false;
        Blackboard.canMove = true;
        if (postProcessing.profile == Vignette)
        {
            postProcessing.profile = NoVignette;
        }
    }

    private IEnumerator ButtonAimation(Transform button, Transform notPoressed, Transform pressed, float seconds)
    {
        float startTime = Time.time;
        float fraction = 0;
        while (fraction < (1 - float.Epsilon))
        {
            button.position = Vector3.Lerp(button.position, pressed.position, fraction);
            fraction = (Time.time - startTime) / seconds;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        startTime = Time.time;
        fraction = 0;
        while (fraction < (1 - float.Epsilon))
        {
            button.position = Vector3.Lerp(button.position, notPoressed.position, fraction);
            fraction = (Time.time - startTime) / seconds;
            yield return null;
        }
        buttonAnimating = false;
    }
    
    private DrawShape ApproximateShape(Vector3[] points)
    {
        int size = points.Length;
        Vector3 midPoint = (points[0] + points[size - 2]) / 2;
        if (Blackboard.Portal.activeInHierarchy && ((Vector3.Distance(midPoint, points[size / 2]) < DrawThreshold / 2) && (Vector3.Distance(points[0], points[size - 2]) > DrawThreshold)))
        {
            return DrawShape.linear;
        }
        else if (!Blackboard.Portal.activeInHierarchy) {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < size; i += 2)
            {
                center.x += points[i].x;
                center.y += points[i].y;
                center.z += points[i].z;
            }
            center = new Vector3(center.x / (size / 2), center.y / (size / 2), center.z / (size / 2));
            float radius = Vector3.Distance(center, points[0]);
            if (radius > DrawThreshold / 5)
            {
                for (int i = 0; i < size; i += 2)
                {
                    if (Mathf.Abs(Vector3.Distance(center, points[i]) - radius) > DrawThreshold)
                    {
                        return DrawShape.ambiguous;
                    }
                }
                return DrawShape.circular;
            }
        }
        return DrawShape.ambiguous;
    }
}
