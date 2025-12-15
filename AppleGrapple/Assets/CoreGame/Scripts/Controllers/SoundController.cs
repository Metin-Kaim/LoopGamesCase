using Assets.CoreGame.Scripts.Enums;
using Assets.CoreGame.Scripts.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private AudioClip collectBubbleSound;
        [SerializeField] private AudioClip swordHitSound;
        [SerializeField] private AudioClip characterHitSound;

        private AudioSource _audioSource;
        private Dictionary<SoundType, AudioClip> _soundClips;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            _soundClips = new Dictionary<SoundType, AudioClip>()
            {
                {SoundType.Bubble, collectBubbleSound },
                {SoundType.SwordHit, swordHitSound },
                {SoundType.CharacterHit, characterHitSound },
            };
        }

        private void OnEnable()
        {
            SoundSignals.Instance.onPlaySoundByType += OnPlaySoundByType;
        }

        private void OnPlaySoundByType(SoundType soundType)
        {
            _audioSource.PlayOneShot(_soundClips[soundType]);
        }

        private void OnDisable()
        {
            SoundSignals.Instance.onPlaySoundByType -= OnPlaySoundByType;
        }
    }
}