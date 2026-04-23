using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour {
    public Collider2D Collider;
    public GameObject Valid;
    public GameObject Invalid;
    public InputActionReference InputClick;
    public InputActionReference InputPoint;
    public Controller Controller;
    public GameObject Dialogue;
    public MedicalItem InfoItem;

    float startX;
    float startY;
    float angle = 0;
    const int AngleSpeed = 3;
    bool dialoguePreviousActive = true;

    void Awake() {
        InputClick.action.started += DoClick;
        Controller = FindAnyObjectByType<Controller>();
        Dialogue = Controller.Dialogue;
    }

    void Start() {
        (startX, startY) = (gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
    }

    void Update() {
        var levitateY = startY + 0.05f * (float)Math.Sin(angle);
        gameObject.transform.localPosition = new Vector3(startX, levitateY, gameObject.transform.localPosition.z);
        angle = (angle + 360 + (AngleSpeed * Time.deltaTime)) % 360;
    }

    void DoClick(InputAction.CallbackContext context) {
        if (!dialoguePreviousActive) {
            var ray = Camera.main.ScreenToWorldPoint(InputPoint.action.ReadValue<Vector2>());
            var hit = Physics2D.Raycast(ray, Vector2.zero);

            if (hit.collider == Collider) {
                if (Controller.ValidateScenarioStep(InfoItem)) {
                    Controller.AdvanceScenarioStep(gameObject);
                } else {
                    Controller.WrongScenarioStep(gameObject);
                }
            }
        }

        dialoguePreviousActive = Dialogue.activeSelf;
    }

    public void ShowValid() {
        Valid.SetActive(true);
    }

    public void ShowInvalid() {
        Invalid.SetActive(true);
        Invoke(nameof(HideValidity), 1f);
    }

    void HideValidity()  {
        Valid.SetActive(false);
        Invalid.SetActive(false);
    }
}