using UnityEngine;
using Cinemachine;

public class impulsetest : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            impulseSource.GenerateImpulse();
        }
    }
}
