using UnityEngine;

[CreateAssetMenu(fileName = "MedicalScenario", menuName = "Scriptable Objects/MedicalScenario")]
public class MedicalScenario : ScriptableObject {
    public string Name;
    public MedicalItem[] NecessaryItems;
    public MedicalItem DummyItem;
    public int TimeInSeconds;
    public Sprite[] Backgrounds;
    public string StartMessage;
    public string FailMessage;
    public string SuccessMessage;

    [HideInInspector]
    public bool Started;

    [HideInInspector]
    public bool Failed;

    [HideInInspector]
    public bool Completed;
}