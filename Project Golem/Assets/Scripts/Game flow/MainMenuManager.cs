using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton; // still optional, for fallback
    public HotasWeaponInput hotasInput;

    private bool started = false;

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    void Update()
    {
        if (!started && hotasInput != null && hotasInput.TriggerDown)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        if (started) return;
        started = true;

        SceneLoader.Instance.LoadNextScene();
    }
}
