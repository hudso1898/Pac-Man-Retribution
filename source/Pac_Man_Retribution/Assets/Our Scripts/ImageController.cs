using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{

    public Image titleImg;
    public int numFrames;
    private int currentFrame;
    private SpriteRenderer[] frames;
    private int frameCounter;
    // Start is called before the first frame update
    void Start()
    {
        frames = GetComponentsInChildren<SpriteRenderer>();
        currentFrame = 0;
        frameCounter = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if(frameCounter < 3)
        {
            frameCounter++;
            return;
        }
        if (currentFrame >= numFrames) currentFrame = 0;
        titleImg.sprite = frames[currentFrame].sprite;
        currentFrame++;
        frameCounter = 0;
    }
}
