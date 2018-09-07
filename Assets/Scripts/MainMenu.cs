using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private float ShrinkTime = 0.5f;
    [SerializeField]
    private Text TitleText;
    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject QuitPanel;
    [SerializeField]
    private GameObject CreditsPanel;
    [SerializeField]
    private GameObject GameOverPanel;
    [SerializeField]
    private Text ScoreText;
    [SerializeField]
    private GameObject LeftHand;
    [SerializeField]
    private GameObject RightHand;
    private List<GameObject> AllPanels = new List<GameObject>();
    [SerializeField]
    private Transform MenuShrinker;
    private Vector3 AirplaneStartPosition;
    private Quaternion AirplaneStartRotation;

    private void Awake()
    {
        Blackboard.Menu = this;
        Blackboard.canMove = false;
    }

    private void Start()
    {
        AirplaneStartPosition = Blackboard.PlaneControls.transform.position;
        AirplaneStartRotation = Blackboard.PlaneControls.transform.rotation;
        AllPanels.Add(MainPanel);
        AllPanels.Add(QuitPanel);
        AllPanels.Add(CreditsPanel);
        AllPanels.Add(GameOverPanel);
        OpenMenu();
    }

    public void OpenMenu()
    {
        ResetMenu();
        Blackboard.canMove = false;
        StartCoroutine(Blackboard.PlaneControls.InterpolateSizeCoroutine(MenuShrinker, Vector3.one, true, ShrinkTime, false));
        Blackboard.PlaneControls.ClosePortal();
        TogglePointers(LeftHand, true);
        TogglePointers(RightHand, true);
    }

    public void GameStart()
    {
        Blackboard.Sounds.PlaySound("GameStart");
        StartCoroutine(Blackboard.PlaneControls.InterpolateSizeCoroutine(MenuShrinker, Vector3.one, false, 0.5f, false));
        Invoke("DeactivateMenu", ShrinkTime + 0.001f);
        TogglePointers(LeftHand, false);
        TogglePointers(RightHand, false);
    }

    public void AirplaneReset()
    {
        Blackboard.PlaneControls.PlayTeleportParticles();
        Blackboard.Sounds.PlaySound("Teleport");
        Blackboard.PlaneControls.transform.position = AirplaneStartPosition;
        Blackboard.PlaneControls.transform.rotation = AirplaneStartRotation;
    }

    public void OpenCredits()
    {
        SelectPanel(CreditsPanel);
        Blackboard.Sounds.PlaySound("MenuSelect");
    }

    public void ResetMenu()
    {
        SelectPanel(MainPanel);
        Blackboard.Sounds.PlaySound("MenuSelect");
    }

    public void OpenQuitPanel()
    {
        SelectPanel(QuitPanel);
        Blackboard.Sounds.PlaySound("MenuSelect");
    }

    public void AcceptQuit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                 Application.OpenURL(webplayerQuitURL);
        #else
                 Application.Quit();
        #endif

    }

    public void OpenGameOver()
    {
        ScoreText.text = "You scored:\n"+Blackboard.PlaneControls.Score;
        SelectPanel(GameOverPanel);
    }

    private void SelectPanel(GameObject currentPanel)
    {
        foreach (GameObject panel in AllPanels)
        {
            if (panel != currentPanel)
            {
                panel.SetActive(false);
            }
        }
        currentPanel.SetActive(true);
    }

    private void DeactivateMenu()
    {
        Blackboard.canMove = true;
    }

    private void TogglePointers(GameObject hand, bool on)
    {
        hand.GetComponent<VRTK.VRTK_Pointer>().enabled = on;
        hand.GetComponent<VRTK.VRTK_UIPointer>().enabled = on;
        hand.GetComponent<VRTK.VRTK_StraightPointerRenderer>().enabled = on;
        hand.GetComponent<VRTK.VRTK_InteractGrab>().enabled = !on;
        hand.GetComponent<VRTK.VRTK_InteractUse>().enabled = !on;
    }
}
