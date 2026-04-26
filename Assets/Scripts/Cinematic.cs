using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour {
    public GameObject Dialogue;
    public InputActionReference InputClick;
    public string[] DialogueSequence;
    public SpriteRenderer[] IntroSequence;
    public GameObject Fade; 

    int currentSequence;
    TextMeshProUGUI dialogueText;
    SpriteRenderer fadeSprite;
    int currentFade;
    float alphaFade;
    const float alphaFadeSpeed = 0.6f;

    void Awake() {
        dialogueText = Dialogue.transform.Find("Region").Find("Text").GetComponent<TextMeshProUGUI>();
        fadeSprite = Fade.GetComponent<SpriteRenderer>();
        InputClick.action.started += DoClick;
    }

    void Start() {
        StartDialogue();
    }

    void Update() {
        if (currentFade == 1) {
            alphaFade += alphaFadeSpeed * Time.deltaTime;

            if (alphaFade >= 1f) {
                if (currentSequence != 2) {
                    AdvanceSequence();
                } else {
                    currentFade = 0;
                    Invoke(nameof(AdvanceSequence), 2f);    
                }

                alphaFade = 1f;
            }
        } else if (currentFade == -1) {
            alphaFade -= alphaFadeSpeed * Time.deltaTime;

            if (alphaFade <= 0f) {
                StopFade();

                if (currentSequence != 2) {
                    StartDialogue();
                } else {
                    Invoke(nameof(StartFade), 3f);
                }

                alphaFade = 0f;
            }
        }

        if (currentSequence < 3) {
            fadeSprite.color = new Vector4(0f, 0f, 0f, alphaFade);
        } else {
            fadeSprite.color = new Vector4(1f, 1f, 1f, alphaFade);
        }
    }

    void DoClick(InputAction.CallbackContext context) {
        if (Dialogue.activeSelf && (!Fade.activeSelf || currentSequence == 3)) {
            Dialogue.SetActive(false);

            if (currentSequence != 3 || alphaFade < 1f) {
                StartFade();
            } else {
                currentFade = -1;
            }
        }
    }

    void StartDialogue() {
        dialogueText.text = DialogueSequence[currentSequence];
        Dialogue.SetActive(true);
    }

    void StartFade() {
        Fade.SetActive(true);
        currentFade = 1;
    }

    void StopFade() {
        Fade.SetActive(false);
        currentFade = 0;
    }

    void AdvanceSequence() {
        if (currentSequence != 2) {
            currentFade = -1;
        } else {
            StartDialogue();
        }

        IntroSequence[currentSequence].transform.gameObject.SetActive(false);
        currentSequence++;

        if (currentSequence >= DialogueSequence.Length) {
            SceneManager.LoadScene("MainScene");
            return;
        }

        IntroSequence[currentSequence].transform.gameObject.SetActive(true);
    }
}