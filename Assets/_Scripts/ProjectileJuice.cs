using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileJuice : MonoBehaviour
{
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeMagnitude;
    
    private GameManager _gameManager;
    private PlayAudio _playAudio;
    private CameraManager _cameraManager;
    
    private ParticleSystem _explosionParticle;
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playAudio = GetComponent<PlayAudio>();
        _explosionParticle = _gameManager.explosion;
        _cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    public void ExplosionEffect(Vector2 pos)
    {
        ParticleSystem ps = Instantiate(_explosionParticle, pos, Quaternion.identity) as ParticleSystem;
        ps.Play();
        Destroy(ps.gameObject, 0.2f);
        StartCoroutine(_gameManager.FlashWalls(0.05f, Color.red));
        
        _playAudio.PlayOneShotSound(1);
    }

    public void CameraShake()
    {
        StartCoroutine(_cameraManager.Shake(_shakeDuration, _shakeMagnitude));
    }
}
