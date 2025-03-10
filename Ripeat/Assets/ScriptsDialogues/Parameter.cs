using UnityEngine;

[System.Serializable]
public class Parameter
{
    [SerializeField] private string parameterName;
    [SerializeField] private int value;

    public Parameter(string parameterName, int value)
    {
        this.parameterName = parameterName;
        this.value = value;
    }

    public void Apply()
    {
        // Logic to apply the parameter change
        Debug.Log($"Parameter {parameterName} changed by {value}");
        // Implement the actual parameter modification logic here
    }
}
