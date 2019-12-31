using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public string newGameScene;
    public string loadGameScene;

    public GameObject ContinueButton;

    // Start is called before the first frame update
    void Start() {
        if (PlayerPrefs.HasKey("Current_Scene")) {
            ContinueButton.SetActive(true);
        } else {
            ContinueButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void NewGame() {
        SceneManager.LoadScene(newGameScene);
    }

    public void ContinueGame() {
        SceneManager.LoadScene(loadGameScene);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
