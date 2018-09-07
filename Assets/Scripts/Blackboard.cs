using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Blackboard
{
    public static SoundManager Sounds;
    public static Airplane PlaneControls;
    public static GameObject Portal;
    public static Transform PortalCam;
    public static Quaternion PortalCamReset;
    public static MainMenu Menu;
    public static ParticleSystem EnemyExplosion;
    public static bool canMove = true;

    public static void ResetPortal()
    {
        PortalCam.rotation = PortalCamReset;
    }
}
