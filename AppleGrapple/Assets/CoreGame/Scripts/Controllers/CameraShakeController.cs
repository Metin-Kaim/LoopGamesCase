using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class CameraShakeController : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private float orthoSizeShakeAmount = 0.3f;
        [SerializeField] private float shakeDuration = 0.2f;

        Sequence _camShakeSeq;

        private float initOrthoSize;

        private void OnEnable()
        {
            CameraSignals.Instance.onCameraShake += Shake;
        }
        private void OnDisable()
        {
            CameraSignals.Instance.onCameraShake -= Shake;
        }

        private void Start()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            initOrthoSize = mainCam.orthographicSize;

            _camShakeSeq = DOTween.Sequence();
            _camShakeSeq.SetAutoKill(false);
            _camShakeSeq.Pause();
            _camShakeSeq.Append(mainCam.transform.DOShakePosition(shakeDuration, 0.2f, 14, 90, false, true));
            _camShakeSeq.Join(mainCam.DOOrthoSize(initOrthoSize + orthoSizeShakeAmount, shakeDuration).SetLoops(2, LoopType.Yoyo));
        }

        private void Shake()
        {
            _camShakeSeq.Restart();
        }
    }
}