using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotationH : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // 回転速度

    private float rotationY;

    private void Update()
    {
        // マウスの移動量を取得
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;

        // Y軸回転（左右の動き）
        rotationY += mouseX;

        // 回転を適用
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}