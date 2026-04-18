using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour {
    public Collider2D Collider;
    public InputActionReference InputClick;
    public Controller Controller;
    public GameObject Dialogue;
    public MedicalItem InfoItem;

    float startX;
    float startY;
    float angle = 0;
    const int AngleSpeed = 3;

    void Awake() {
        InputClick.action.started += DoClick;
        Controller = FindAnyObjectByType<Controller>();
        Dialogue = GameObject.Find("Dialogue");
    }

    void Start() {
        (startX, startY) = (gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
    }

    void Update() {
        var levitateY = startY + 0.05f * (float)Math.Sin(angle);
        gameObject.transform.localPosition = new Vector3(startX, levitateY, 0);
        angle = (angle + 360 + (AngleSpeed * Time.deltaTime)) % 360;
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf) {
            return;
        }

        var ray = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var hit = Physics2D.Raycast(ray, Vector2.zero);

        if (hit.collider == Collider) {
            if (Controller.ValidateScenarioStep(InfoItem)) {
                Controller.AdvanceScenarioStep();
            } else {
                Controller.WrongScenarioStep(InfoItem);
            }
        }
    }
}