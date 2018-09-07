using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public enum ButtonType { Speed, Warp, ResetPortal };
    public Transform NotPressed;
    public Transform Pressed;
    public ButtonType Type = ButtonType.Speed;
}
