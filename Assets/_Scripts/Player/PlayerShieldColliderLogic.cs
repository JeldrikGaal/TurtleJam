using Unity.VisualScripting;
using UnityEngine;

public class PlayerShieldColliderLogic : MonoBehaviour
{
    private PlayerProjectile _playerProjectile;

    private void Start()
    {
        _playerProjectile = transform.parent.GetComponent<PlayerProjectile>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            _playerProjectile.ShieldHit(other);
        }
    }

}
