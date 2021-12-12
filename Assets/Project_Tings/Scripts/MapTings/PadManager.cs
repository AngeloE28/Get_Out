using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadManager : MonoBehaviour
{
    [SerializeField] private List<Pads> pads;

    private bool _roomCleared;    

    private void Awake()
    {
        _roomCleared = false;        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_roomCleared)
            return;

        bool allClear = true;
        foreach (var p in pads)
        {
            if (!p.GetActuatedState())
            {
                allClear = false;
                break;
            }
        }
        _roomCleared = allClear;
    }

    public bool GetRoomClear()
    {
        return _roomCleared;
    }
}
