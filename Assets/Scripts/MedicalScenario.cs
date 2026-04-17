using UnityEngine;

[CreateAssetMenu(fileName = "MedicalScenario", menuName = "Scriptable Objects/MedicalScenario")]
public class MedicalScenario : ScriptableObject {
    public string Name;
    public MedicalItem[] NecessaryItems;
    public MedicalItem DummyItem;
}