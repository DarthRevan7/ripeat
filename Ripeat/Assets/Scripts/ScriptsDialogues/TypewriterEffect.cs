using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

// Questo script gestisce l'effetto "macchinetta da scrivere" che visualizza il testo gradualmente
public class TypewriterEffect : MonoBehaviour
{
    // Velocità dell'effetto macchinetta da scrivere; più alto è il valore, più velocemente il testo viene visualizzato
    [SerializeField] private float typewriterSpeed = 50f;

    // Lista di punteggiatura con i relativi tempi di attesa per creare pause naturali tra le frasi
    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        // Dopo punto, esclamativo o interrogativo, una pausa più lunga
        new Punctuation(new HashSet<char> { '.', '!', '?' }, 0.6f),
        // Dopo virgola, punto e virgola o due punti, una pausa leggermente minore
        new Punctuation(new HashSet<char> { ',', ';', ':' }, 0.3f),
    };

    // Flag per indicare se l'effetto è in esecuzione
    public bool IsRunning { get; private set; }

    // Riferimento alla coroutine in esecuzione
    private Coroutine typingCoroutine;

    // Variabili per memorizzare il testo corrente e il textLabel
    private string currentTextToType;
    private TMP_Text currentTextLabel;

    // Metodo pubblico per avviare l'effetto di scrittura
    public void Run(string textToType, TMP_Text textLabel)
    {
        currentTextToType = textToType;
        currentTextLabel = textLabel;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textLabel.text = string.Empty;
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    // Metodo per interrompere l'effetto e mostrare il testo completo
    public void Stop()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        if(currentTextLabel != null && !string.IsNullOrEmpty(currentTextToType))
            currentTextLabel.text = currentTextToType;

        IsRunning = false;
    }

    // Coroutine che gestisce l'effetto di scrittura carattere per carattere
    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            textLabel.text = textToType.Substring(0, charIndex);

            yield return null;
        }

        // Assicura che venga visualizzato l'intero testo
        textLabel.text = textToType;
        IsRunning = false;
    }

    // Struttura interna che rappresenta una regola di punteggiatura
    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}


