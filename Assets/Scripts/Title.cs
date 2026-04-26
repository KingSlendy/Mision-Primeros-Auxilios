using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour {
    public GameObject LineBackground;
    public GameObject GuideBackground;
    public GameObject PlayButton;
    public GameObject LineButton;
    public GameObject GuideButton;
    public GameObject ReturnButton;
    public void PlayAction() {
        SceneManager.LoadScene("IntroScene");
    }

    public void LineAction() {
        LineBackground.SetActive(true);
        ChangeState();
    }

    public void GuideAction() {
        GuideBackground.SetActive(true);
        ChangeState();
    }

    public void ReturnAction() {
        LineBackground.SetActive(false);
        GuideBackground.SetActive(false);
        ChangeState();
    }

    void ChangeState() {
        PlayButton.SetActive(!PlayButton.activeSelf);
        LineButton.SetActive(!LineButton.activeSelf);
        GuideButton.SetActive(!GuideButton.activeSelf);
        ReturnButton.SetActive(!ReturnButton.activeSelf);
    }
}