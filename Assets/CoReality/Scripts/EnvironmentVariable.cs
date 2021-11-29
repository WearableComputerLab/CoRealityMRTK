using UnityEngine;

[System.Serializable]
public class EnvironmentVariable
{
    [SerializeField]
    public string name;

    [SerializeField]
    public bool lockedName;
    public EnvironmentVariable() {}

    public EnvironmentVariable(string name, bool lockName = false) {
        this.name = name;
        this.lockedName = lockName;
    }

    public void Set(string value) {
        if (name != null) {
            System.Environment.SetEnvironmentVariable(name, value, System.EnvironmentVariableTarget.User);
        }
    }

    public string Get() {
        return System.Environment.GetEnvironmentVariable(name, System.EnvironmentVariableTarget.User);
    }
}