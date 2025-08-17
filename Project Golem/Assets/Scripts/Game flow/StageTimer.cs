using UnityEngine;

public class StageTimer : MonoBehaviour
{
    public float timeToNextStage = 180f; // 3 minutes default
    private float timer;

    void Start()
    {
        timer = timeToNextStage;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SceneLoader.Instance.LoadNextScene();
        }
    }
}
