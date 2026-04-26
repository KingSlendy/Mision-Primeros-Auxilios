using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class Controller : MonoBehaviour {
    public GameObject Dialogue;
    public GameObject Item;
    public SpriteRenderer Background;
    public GameObject TimeBar;
    public Image TimeBarFill;
    public GameObject RetryButton;
    public GameObject ReturnButton;

    public MedicalScenario[] Scenarios;

    public Sprite Victory;
    public Sprite Loss;

    public InputActionReference InputClick;

    TextMeshProUGUI dialogueText;
    MedicalScenario currentScenario;
    int scenarioStep;
    int scenarioTime;

    void Awake() {
        dialogueText = Dialogue.transform.Find("Region").Find("Text").GetComponent<TextMeshProUGUI>();
        InputClick.action.started += DoClick;
    }

    void Start() {
        SetupScenario();
    }

    void Update() {
        //TimeBarFill.fillAmount = Mathf.Lerp(TimeBarFill.fillAmount, scenarioTime / currentScenario.TimeInSeconds, 0.75f * Time.deltaTime);
        TimeBarFill.fillAmount = (float)scenarioTime / currentScenario.TimeInSeconds;
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf) {
            Dialogue.SetActive(false);

            if (!StartedScenario() && !FailedScenario() && !CompletedScenario()) {
                StartScenarioTime();
            }

            if (FailedScenario()) {
                ResultsChange(false);
            }

            if (CompletedScenario()) {
                ResultsChange(true);
            }
        }
    }

    void StartDialogue(string message) {
        dialogueText.text = message;
        Dialogue.SetActive(true);
    }

    void SetupScenario() {
        //Spawns all the necessary medical items, and an extra dummy medical item
        currentScenario = Scenarios[0];
        currentScenario.Started = false;
        currentScenario.Failed = false;
        currentScenario.Completed = false;
        ChangeBackground(currentScenario.Backgrounds[Random.Range(0, currentScenario.Backgrounds.Length)]);

        var itemCount = currentScenario.NecessaryItems.Length + 1;
        var shuffledItems = currentScenario.NecessaryItems.Append(currentScenario.DummyItem).OrderBy(x => Random.Range(0, itemCount)).ToArray();

        for (var i = 0; i < itemCount; i++) {
            SpawnMedicalItem(shuffledItems[i], new Vector2(-7f + 2.25f * i + Random.Range(-0.5f, 0.5f), Random.Range(-1f, 2f)));
        }

        scenarioStep = 0;
        scenarioTime = currentScenario.TimeInSeconds;
        StartDialogue(currentScenario.StartMessage);
    }

    void ChangeBackground(Sprite background) {
        Background.sprite = background;
        Background.gameObject.transform.localScale = new Vector3(1920f / Background.sprite.rect.width, 1080f / Background.sprite.rect.height, 1f);
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
        InvokeRepeating(nameof(InvokeScenarioTime), 1f, 1f);
        //currentScenario.Started = true;
    }

    void InvokeScenarioTime() {
        SubtractScenarioTime(1);
    }

    bool SubtractScenarioTime(int amount = 1) {
        scenarioTime -= amount;

        if (scenarioTime <= 0) {
            scenarioTime = 0;
            StartDialogue(currentScenario.FailMessage);
            StopScenarioTime();
            currentScenario.Failed = true;
            return false;
        }

        return true;
    }

    void StopScenarioTime() {
        CancelInvoke(nameof(InvokeScenarioTime));
        currentScenario.Started = false;
    }

    public bool StartedScenario() {
        return (currentScenario.Started);
    }

    public bool FailedScenario() {
        return (currentScenario.Failed);
    }

    public bool CompletedScenario() {
        return (currentScenario.Completed);
    }

    public bool ValidateScenarioStep(MedicalItem item) {
        return (currentScenario.NecessaryItems[scenarioStep] == item);
    }

    public void AdvanceScenarioStep(GameObject item) {
        var itemComponent = item.GetComponent<Item>();
        itemComponent.ShowValid();

        if (scenarioStep >= currentScenario.NecessaryItems.Length - 1) {
            scenarioStep = 0;
            currentScenario.Completed = true;
            StopScenarioTime();
            StartDialogue(currentScenario.SuccessMessage);
            return;
        }

        scenarioStep++;
    }

    public void WrongScenarioStep(GameObject item) {
        var itemComponent = item.GetComponent<Item>();
        itemComponent.ShowInvalid();
        SubtractScenarioTime(5);
    }

    void ResultsChange(bool victory) {
        var background = (victory) ? Victory : Loss;
        ChangeBackground(background);

        foreach (var item in FindObjectsByType<Item>()) {
            item.gameObject.SetActive(false);
        }

        TimeBar.SetActive(false);

        if (victory) {
            ReturnButton.SetActive(true);
        } else {
            RetryButton.SetActive(true);
        }
    }

    public void Retry() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnMenu() {
        SceneManager.LoadScene("TitleScene");
    }
}