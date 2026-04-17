using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour {
    //Game Objects
    public GameObject Dialogue;
    public GameObject Item;

    public MedicalScenario[] Scenarios;

    //Inputs
    public InputActionReference InputClick;

    void Start() {
        InputClick.action.started += DoClick;

        //Spawns all the necessary medical items, and an extra dummy medical item
        var scenario = Scenarios[0];
        var itemCount = scenario.NecessaryItems.Length + 1;

        foreach (var item in scenario.NecessaryItems) {
            SpawnMedicalItem(item, new Vector2());
        }

        SpawnMedicalItem(scenario.DummyItem, Vector2.zero);
    }

    void Update() {
        
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf) {
            Dialogue.SetActive(false);
        } else {
            Debug.Log("Click!");
        }
    }

    void SpawnMedicalItem(MedicalItem item, Vector2 area) {
        var gameObject = Instantiate(Item);
        var circleObject = gameObject.transform.Find("Circle");
        circleObject.GetComponent<SpriteRenderer>().sprite = item.Icon;
    }
}