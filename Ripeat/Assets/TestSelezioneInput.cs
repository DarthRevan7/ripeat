using UnityEngine;
using TMPro;

public class TestSelezioneInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField campoDaSelezionare;

    public void EseguiTestSelezione()
    {
        if (campoDaSelezionare != null && campoDaSelezionare.gameObject.activeInHierarchy)
        {
            campoDaSelezionare.ActivateInputField();
            Debug.Log("Tentativo di selezione eseguito su: " + campoDaSelezionare.name);
        }
        else
        {
            Debug.LogError("Campo da selezionare non assegnato o non attivo!");
        }
    }
}
