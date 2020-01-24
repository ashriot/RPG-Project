using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public string newGameScene;
    public string loadGameScene;
    public string musicNameToPlay;
    public GameObject towerSprite;
    public float moveTime = 30f;

    private bool musicStarted = false;

    public GameObject ContinueButton;

    // Start is called before the first frame update
    void Start() {
        // if (PlayerPrefs.HasKey("Current_Scene")) {
        //     ContinueButton.SetActive(true);
        // } else {
        //     ContinueButton.SetActive(false);
        // }

        StartCoroutine(SmoothMovement());
    }

    // Update is called once per frame
    void Update() {
        
    }

    private IEnumerator SmoothMovement() {
        var endPos = new Vector3(-17f, -19f, 0f);
        float sqrRemainingDistance = (towerSprite.transform.localPosition - endPos).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(towerSprite.transform.position, endPos, inverseMoveTime * Time.deltaTime);
            towerSprite.transform.position = newPosition;
            sqrRemainingDistance = (towerSprite.transform.position - endPos).sqrMagnitude;

            yield return null;
        }
    }
    
    void LateUpdate() {
        // this.transform.position = new Vector3(target.position.x, target.position.y, this.transform.position.z);

        // // keep the camera inside the bounds
        // transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
        // Mathf.Clamp(transform.position.y, bottomLeftLimit.y,
        // topRightLimit.y), transform.position.z);

        if (!musicStarted)
        {
            AudioManager.instance.PlayBgm(musicNameToPlay);
            musicStarted = true;
        }
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
