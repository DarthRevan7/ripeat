using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class FinalSceneHandler : MonoBehaviour
{

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private float waitingSecondsBeforeMenuScene = 2f;
    [SerializeField] private string menuSceneName = "Menu";


    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += ChangeScene;
    }

    void ChangeScene(VideoPlayer vp)
    {
        StartCoroutine(WaitingCoroutine());
    }

    IEnumerator WaitingCoroutine()
    {
        yield return new WaitForSeconds(waitingSecondsBeforeMenuScene);
        SceneManager.LoadScene(menuSceneName);
    }
}
