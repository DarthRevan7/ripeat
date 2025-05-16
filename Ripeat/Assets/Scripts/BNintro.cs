using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BNintro : MonoBehaviour
{
    
    [SerializeField] private TMP_Text write1;
    [SerializeField] private TMP_Text write2;
    [SerializeField] private string string1 = "";
    [SerializeField] private string string2 = "";

    private TypewriterEffect typewriterEffect;
    private MenuScript menuScript;

    private void Awake()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        menuScript = GetComponent<MenuScript>();

        write1.text = string.Empty;
        write2.text = string.Empty;
        Write1();
    }
    
    private void Write1()
    {
        Debug.Log("Write1 called");
        StartCoroutine(Write1Coroutine());
    }

    private IEnumerator Write1Coroutine()
    {
        typewriterEffect.Run(string1, write1);
        // Wait until user clicks (mouse button down or screen tap)
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        SceneManager.LoadScene("NewIntro");
        
    }

    private void Write2()
    {
        Debug.Log("Write2 called");
        StartCoroutine(Write2Coroutine());
    }

    private IEnumerator Write2Coroutine()
    {
        typewriterEffect.Run(string2, write2);
        yield return new WaitForSeconds(10f);
    }
}
