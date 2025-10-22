using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRProjectileShooter : MonoBehaviour
{
    [SerializeField] private InputActionReference shootAction; // Right Controller Aボタン
    [SerializeField] private Transform spawnPoint; // 弾を出す位置
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    private void OnEnable()
    {
        shootAction.action.performed += Shoot;
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        shootAction.action.performed -= Shoot;
        shootAction.action.Disable();
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = spawnPoint.forward * projectileSpeed;
            }
        }
    }
}