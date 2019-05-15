using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class highscore : MonoBehaviour
{
    public TMPro.TextMeshProUGUI score1, score2, score3, score4, score5;

    // Start is called before the first frame update
    void Start()
    {
        if (!System.IO.File.Exists(@"highscores.txt"))
        {
            System.IO.File.Create(@"highscores.txt");

        }
        string[] score = System.IO.File.ReadAllLines(@"highscores.txt");
        score1.text = score[0];
        score2.text = score[1];
        score3.text = score[2];
        score4.text = score[3];
        score5.text = score[4];
    }

}
