using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Portal Blocker")]
    [SerializeField] private List<GameObject> portalBlockers;

    [Header("Blue Room")]
    [SerializeField] private Button blueRoomBtn;
    [SerializeField] private List<Renderer> blueSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEBlueColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogBlueColor;

    [Header("Red Room")]
    [SerializeField] private Button redRoomBtn;
    [SerializeField] private List<Renderer> redSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newERedColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogRedColor;

    [Header("Green Room")]
    [SerializeField] private PadManager greenRoomPads;
    [SerializeField] private List<Renderer> greenSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEGreenColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogGreenColor;

    [Header("Orange Room")]
    [SerializeField] private PadManager orangeRoomPads;
    [SerializeField] private List<Renderer> orangeSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEOrangeColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogOrangeColor;

    private bool _allRoomClear = false;

    private void Awake()
    {
        _allRoomClear = false;

        foreach (var pb in portalBlockers)
        {
            if (!pb.activeSelf)
                pb.SetActive(true);
        }

        foreach (var bsr in blueSignRenderers)
            bsr.material.SetColor("_EmissionColor", ogBlueColor);

        foreach (var rsr in redSignRenderers)
            rsr.material.SetColor("_EmissionColor", ogRedColor);

        foreach (var gsr in greenSignRenderers)
            gsr.material.SetColor("_EmissionColor", ogGreenColor);

        foreach (var osr in orangeSignRenderers)
            osr.material.SetColor("_EmissionColor", ogOrangeColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (_allRoomClear)
        {
            foreach (var pb in portalBlockers)
                pb.SetActive(false);
        }

        if (blueRoomBtn.BtnPressed() && redRoomBtn.BtnPressed() &&
            greenRoomPads.GetRoomClear() && orangeRoomPads.GetRoomClear())
        {
            _allRoomClear = true;
        }
        else
            _allRoomClear = false;

        if(blueRoomBtn.BtnPressed())
        {
            foreach (var bsr in blueSignRenderers)
                bsr.material.SetColor("_EmissionColor", newEBlueColor);
        }

        if(redRoomBtn.BtnPressed())
        {
            foreach (var rsr in redSignRenderers)
                rsr.material.SetColor("_EmissionColor", newERedColor);
        }

        if (greenRoomPads.GetRoomClear())
        {
            foreach (var gsr in greenSignRenderers)
                gsr.material.SetColor("_EmissionColor", newEGreenColor);
        }

        if (orangeRoomPads.GetRoomClear())
        {
            foreach (var osr in orangeSignRenderers)
                osr.material.SetColor("_EmissionColor", newEOrangeColor);
        }
    }
}
