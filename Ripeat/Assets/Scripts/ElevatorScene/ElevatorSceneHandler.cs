using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ElevatorSceneHandler : MonoBehaviour
{

    [SerializeField] private float secondsToStartVideo = 1.5f;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private MenuScript menuScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        menuScript = GameObject.Find("FadingImage").GetComponent<MenuScript>();

        StartCoroutine(DelayedPlayVideo());

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        menuScript.LoadScene();
    }

    IEnumerator DelayedPlayVideo()
    {
        yield return new WaitForSeconds(secondsToStartVideo);
        videoPlayer.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
