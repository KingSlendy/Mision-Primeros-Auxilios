using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour {
    public GameObject Dialogue;
    public GameObject Item;
    public Image Back;

    public MedicalScenario[] Scenarios;

    public InputActionReference InputClick;

    MedicalScenario currentScenario;
    int scenarioStep = 0;

    void Awake() {
        InputClick.action.started += DoClick;
    }

    void Start() {
        StartScenario();
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf) {
            Dialogue.SetActive(false);
        } else {
            //Debug.Log("Click!");
        }
    }

    void StartScenario() {
        //Spawns all the necessary medical items, and an extra dummy medical item
        currentScenario = Scenarios[0];
        Back.sprite = currentScenario.Backgrounds[Random.Range(0, currentScenario.Backgrounds.Length)];
        var itemCount = currentScenario.NecessaryItems.Length + 1;
        var shuffledItems = currentScenario.NecessaryItems.OrderBy(x => Random.Range(0, itemCount)).Append(currentScenario.DummyItem).ToArray();

        for (var i = 0; i < itemCount; i++) {
            SpawnMedicalItem(shuffledItems[i], new Vector2(-7.5f + 3.5f * i + Random.Range(-1f, 1f), Random.Range(-1f, 2f)));
        }

        scenarioStep = 0;
    }

    void SpawnMedicalItem(MedicalItem item, Vector2 area) {
        var itemObject = Instantiate(Item);
        itemObject.GetComponent<Item>().InfoItem = item;
        itemObject.transform.localPosition = (Vector3)area;
        var circleObject = itemObject.transform.Find("Circle");
        circleObject.GetComponent<SpriteRenderer>().sprite = item.Icon;
        var shadowObject = itemObject.transform.Find("Shadow");
        shadowObject.GetComponent<SpriteRenderer>().sprite = item.Icon;
    }

    public bool ValidateScenarioStep(MedicalItem item) {
        return (currentScenario.NecessaryItems[scenarioStep] == item);
    }

    public void AdvanceScenarioStep() {
        Debug.Log("Correct Item!");

        if (scenarioStep++ >= currentScenario.NecessaryItems.Length - 1) {
            scenarioStep = 0;
            Debug.Log(currentScenario.SuccessMessage);
            //Complete Scenario
        }
    }

    public void WrongScenarioStep(MedicalItem item) {
        if (item != currentScenario.DummyItem) {
            Debug.Log(currentScenario.FailMessages[scenarioStep]);
        } else {
            Debug.Log(currentScenario.FailMessages[^1]);
        }
    }
}