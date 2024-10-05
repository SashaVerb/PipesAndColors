using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;
    [SerializeField] float duration;

    private CinemachineVirtualCamera virtCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private void Awake()
    {
        virtCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        EventManager.EnemyHitedPipe.AddListener(ReactAtEnemyDeath);
    }

    private void ReactAtEnemyDeath()
    {
        StopCoroutine(Shaking());
        StartCoroutine(Shaking());
    }

    private IEnumerator Shaking()
    {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
}
