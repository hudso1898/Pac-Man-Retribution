using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Colorer : MonoBehaviour
{
    private TextMeshProUGUI scoreTitle;
    private int i = 0;
    private float deltaT = 0.0f;
    private float t = 0;
    private float duration = 100;
    private Color[] colors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta, Color.white };
    private Color currentCol;

    // Start is called before the first frame update
    void Start()
    {
        scoreTitle = GetComponent<TextMeshProUGUI>();
        currentCol = scoreTitle.fontSharedMaterial.GetColor(ShaderUtilities.ID_GlowColor);

    }

    // Update is called once per frame
    void Update()
    {

        if (deltaT >= 1.6f)
        {
            if (i == 6) i = 0;
            i++;
            deltaT = 0.0f;
            t = 0;
        }
        else deltaT += Time.deltaTime;
        currentCol = Color.Lerp(currentCol, colors[i], t);
        if (t < 1) t += Time.deltaTime / duration;
        scoreTitle.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, currentCol);
    }

   
}
