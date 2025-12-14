using Assets.CoreGame.Scripts.Signals;
using DG.Tweening;
using UnityEngine;

namespace Assets.CoreGame.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private float orthoSizeShakeAmount = 0.3f;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private Transform followObject;
        [SerializeField] private Vector3 followOffset;
        [SerializeField] private bool canFollow;

        Sequence _camShakeSeq;
        private float _initOrthoSize;

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
            followOffset = transform.position;

            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            _initOrthoSize = mainCam.orthographicSize;

            _camShakeSeq = DOTween.Sequence();
            _camShakeSeq.SetAutoKill(false);
            _camShakeSeq.Pause();
            _camShakeSeq.Append(mainCam.transform.DOShakePosition(shakeDuration, 0.2f, 14, 90, false, true));
            _camShakeSeq.Join(mainCam.DOOrthoSize(_initOrthoSize + orthoSizeShakeAmount, shakeDuration).SetLoops(2, LoopType.Yoyo));
        }

        private void LateUpdate()
        {
            if (canFollow)
                transform.position = followObject.position + followOffset;
        }

        private void Shake()
        {
            _camShakeSeq.Restart();
        }
    }
}