using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverFade : MonoBehaviour
{
    [SerializeField] private Image backGround;
    [SerializeField] private Color fadeInColour;


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (backGround.color == fadeInColour)
            return;

        backGround.color = Color.Lerp(backGround.color, fadeInColour, 1.5f * Time.deltaTime);
    }
}
