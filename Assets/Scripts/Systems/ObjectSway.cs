using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSway : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float amplitude = 0.5f; // 上下の振れ幅
    [SerializeField] private float frequency = 1f; // 1秒あたりの周期数
    [SerializeField] private bool useWorldSpace = false; // ワールド空間基準か

    [Header("Advanced")]
    [SerializeField] private Vector3 motionAxis = Vector3.up; // 動かす軸
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private bool swayRotation = false;
    [SerializeField] private float phaseOffset = 0f; // 開始位相オフセット
    [SerializeField] private float min = 0f; // ランダムな位相のずれ幅(最小)
    [SerializeField] private float max = 0f; // ランダムな位相のずれ幅(最大)

    [Header("Start Settings")]
    [SerializeField] private float fadeInTime = 1f;
    private float swayFactor = 0f;

    [Header("Active")]
    public bool isActive = true;

    private Vector3 initialPosition;
    private float timer;

    private void Awake()
    {
        StoreInitialPosition();
        timer = Random.Range(min, max);
    }

    private void StoreInitialPosition()
    {
        initialPosition = useWorldSpace ?
            transform.position :
            transform.localPosition;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        swayFactor = Mathf.Clamp01(swayFactor + Time.deltaTime / fadeInTime);

        ApplyMovement();
    }

    private void ApplyMovement()
    {
        float sinValue = Mathf.Sin((timer + phaseOffset) * frequency * Mathf.PI * 2);
        float offset = sinValue * amplitude * swayFactor;

        Vector3 newPosition = initialPosition + motionAxis * offset;
        if (useWorldSpace)
            transform.position = newPosition;
        else
            transform.localPosition = newPosition;

        if (swayRotation)
            transform.localRotation = Quaternion.AngleAxis(offset * 10f, rotationAxis);
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            StoreInitialPosition();
        }
    }
}