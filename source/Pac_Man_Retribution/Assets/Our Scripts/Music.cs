using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{

    private AudioSource[] songs;
    private float deltaT;
    private AudioSource currentSong;
    private int currentSIndex;
    private System.Random rand;
    public int numSongs;
    private bool started;
    public bool isStopped;
    // Start is called before the first frame update
    void Start()
    {
        songs = this.GetComponentsInChildren<AudioSource>();
        rand = new System.Random(System.DateTime.Now.Millisecond);

        deltaT = 0.0f;
        started = false;
        isStopped = false;

    }

    void SelectSong(int currentS)
    {
        int nextSong = rand.Next() % numSongs;
        if (nextSong == currentS) SelectSong(currentS);
        currentSong = songs[nextSong];
        currentSIndex = nextSong;
        currentSong.Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if (deltaT < 2.5f) { 
                deltaT += Time.deltaTime; // wait 
                return;  
            }
            SelectSong(-1);
            started = true;
        }
        if (isStopped)
        {
            return;
        }
        if (!currentSong.isPlaying)
        {
            SelectSong(currentSIndex);
        }
    }

    public void StopMusic()
    {
        currentSong.Stop();
        isStopped = true;
        songs[numSongs].PlayDelayed(5.0f);
    }
}
