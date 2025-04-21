using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private float _projectileSpeed = 15f;

    [Header("References")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Projectile _projectilePrefab;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _muzzleFlashEffect;
    [SerializeField] private float _muzzleFlashDuration = 0.1f;

    private float _nextFireTime;

    public void Shoot(float direction)
    {
        if (Time.time < _nextFireTime) return;

        ShowMuzzleFlash();

        var projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        projectile.Initialize(direction * _projectileSpeed);
        _nextFireTime = Time.time + _fireRate;
    }

    private void ShowMuzzleFlash()
    {
        if (_muzzleFlashEffect == null) return;

        _muzzleFlashEffect.SetActive(true);
        CancelInvoke(nameof(HideMuzzleFlash));
        Invoke(nameof(HideMuzzleFlash), _muzzleFlashDuration);
    }

    private void HideMuzzleFlash()
    {
        _muzzleFlashEffect.SetActive(false);
    }
}