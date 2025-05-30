using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;               // L'immagine UI da animare
    public Sprite[] m_SpriteArray;      // Le sprite dell'animazione
    public float m_Speed = 0.1f;        // Tempo tra frame

    private int m_IndexSprite = 0;

    void Start()
    {
        StartCoroutine(PlayLoopingAnimation());
    }

    IEnumerator PlayLoopingAnimation()
    {
        while (true)
        {
            if (m_SpriteArray.Length == 0) yield break;

            m_Image.sprite = m_SpriteArray[m_IndexSprite];
            m_IndexSprite = (m_IndexSprite + 1) % m_SpriteArray.Length;

            yield return new WaitForSeconds(m_Speed);
        }
    }
}
