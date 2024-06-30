using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileJuice : MonoBehaviour
{
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeMagnitude;
    [SerializeField] private float _freezeFrameDuration;
    
    [SerializeField] private ParticleSystem _explosionParticle;
    [SerializeField] private ParticleSystem _sparkParticle;
    [SerializeField] private TrailRenderer _trailRenderer;
    
    private GameManager _gameManager;
    private PlayAudio _playAudio;
    private CameraManager _cameraManager;

    private Animator _playerShieldAnimator;
    
    // Rider efficiency proposals
    private static readonly int IdleAnimHash = Animator.StringToHash("Idle");
    private static readonly int ShieldAnimHash = Animator.StringToHash("Shield");

    [SerializeField] private List<ColorStreakAmountPair> _colorList;
    
    [Serializable]
    struct ColorStreakAmountPair
    {
        public Color Color;
        public int StreakAmount;
    }

    private void Awake()
    {
        StreakLogic.StreakReached += UpdateTrailColorFromStreak;
    }
    
    private void OnDestroy()
    {
        StreakLogic.StreakReached -= UpdateTrailColorFromStreak;
    }

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playAudio = GetComponent<PlayAudio>();
        _cameraManager = Camera.main.GetComponent<CameraManager>();
        _playerShieldAnimator = GetComponent<Animator>();
    }

    public void ExplosionEffect(Vector2 pos)
    {
        ParticleSystem ps = Instantiate(_explosionParticle, pos, Quaternion.identity) as ParticleSystem;
        ps.Play();
        Destroy(ps.gameObject, 0.2f);
        
        ColorsController.Instance.StartProjectileColorFlash();
        
        _playAudio.PlayOneShotSound(1);
    }

    public void SparkEffect(Vector2 pos, Vector2 dir)
    {
        ParticleSystem ps = Instantiate(_sparkParticle, pos, Quaternion.identity) as ParticleSystem;
        ps.transform.up = dir;
        ps.Play();
        Destroy(ps.gameObject, 0.2f); 
        
    }

    public void CameraShake()
    {
        StartCoroutine(_cameraManager.Shake(_shakeDuration, _shakeMagnitude));
    }
    
    public void ShieldOpenAnim()
    {
        _playerShieldAnimator.SetTrigger(ShieldAnimHash);
    }

    public void ShieldCloseAnim()
    {
        _playerShieldAnimator.SetBool(ShieldAnimHash, false);
    }

    public void IdleAnim()
    {
        _playerShieldAnimator.SetTrigger(IdleAnimHash);
    }

    public void FreezeFrames()
    {
        _cameraManager.FreezeFrames(_freezeFrameDuration);
    }

    private void UpdateTrailColorFromStreak(int streakAmount)
    {
        _trailRenderer.startColor = GetColorFromStreak(streakAmount);
    }
    
    private Color GetColorFromStreak(int streakAmount)
    {
        for ( int i = _colorList.Count - 1; i > 0; i--)
        {
            if (streakAmount >= _colorList[i].StreakAmount)
            {
                return _colorList[i].Color;
            }
        }

        return _colorList[0].Color;
    }

}
