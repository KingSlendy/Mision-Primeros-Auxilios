using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour {
    public GameObject Dialogue;
    public GameObject Item;
    public SpriteRenderer Background;
    public Image TimeBarFill;

    public MedicalScenario[] Scenarios;

    public InputActionReference InputClick;

    TextMeshProUGUI dialogueText;
    MedicalScenario currentScenario;
    int scenarioStep;
    int scenarioTime;
    int scenarioFails;

    void Awake() {
        dialogueText = Dialogue.transform.Find("Region").Find("Text").GetComponent<TextMeshProUGUI>();
        InputClick.action.started += DoClick;
    }

    void Start() {
        StartScenario();
    }

    void Update() {
        //TimeBarFill.fillAmount = Mathf.Lerp(TimeBarFill.fillAmount, scenarioTime / currentScenario.TimeInSeconds, 0.75f * Time.deltaTime);
        TimeBarFill.fillAmount = (float)scenarioTime / currentScenario.TimeInSeconds;
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf) {
            Dialogue.SetActive(false);
        }
    }

    void StartDialogue(string message) {
        dialogueText.text = message;
        Dialogue.SetActive(true);
    }

    void StartScenario() {
        //Spawns all the necessary medical items, and an extra dummy medical item
        currentScenario = Scenarios[0];
        Background.sprite = currentScenario.Backgrounds[Random.Range(0, currentScenario.Backgrounds.Length)];
        Background.gameObject.transform.localScale = new Vector3(1920f / Background.sprite.rect.width, 1080f / Background.sprite.rect.height, 1f);

        var itemCount = currentScenario.NecessaryItems.Length + 1;
        var shuffledItems = currentScenario.NecessaryItems.OrderBy(x => Random.Range(0, itemCount)).Append(currentScenario.DummyItem).ToArray();

        for (var i = 0; i < itemCount; i++) {
            SpawnMedicalItem(shuffledItems[i], new Vector2(-7.5f + 3.5f * i + Random.Range(-1f, 1f), Random.Range(-1f, 2f)));
        }

        scenarioStep = 0;
        StartDialogue(currentScenario.StartMessage);
        //StartScenarioTime();
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

    void StartScenarioTime() {
        scenarioTime = currentScenario.TimeInSeconds;
        InvokeRepeating(nameof(SubtractScenarioTime), 1f, 1f);
    }

    void SubtractScenarioTime() {
        scenarioTime--;

        if (scenarioTime < 0) {
            scenarioTime = 0;
            Debug.Log("Time's up!");
            StartDialogue("¡Se acabó el tiempo! No lograste curar al paciente...");
        }
    }

    public bool ValidateScenarioStep(MedicalItem item) {
        return (currentScenario.NecessaryItems[scenarioStep] == item);
    }

    public void AdvanceScenarioStep(GameObject item) {
        var itemComponent = item.GetComponent<Item>();
        itemComponent.ShowValid();
        scenarioFails = 0;

        if (scenarioStep >= currentScenario.NecessaryItems.Length - 1) {
            scenarioStep = 0;
            StartDialogue(currentScenario.SuccessMessage);
            //Complete Scenario
            return;
        }

        scenarioStep++;
    }

    public void WrongScenarioStep(GameObject item) {
        var itemComponent = item.GetComponent<Item>();
        itemComponent.ShowInvalid();
        scenarioTime -= 30;
        scenarioFails++;

        if (scenarioFails % 3 == 0) {
            if (itemComponent.InfoItem != currentScenario.DummyItem) {
                StartDialogue(currentScenario.FailMessages[scenarioStep]);
            } else {
                StartDialogue(currentScenario.FailMessages[^1]);
            }
        }
    }
}