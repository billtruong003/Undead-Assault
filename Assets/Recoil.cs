using UnityEngine;
using DG.Tweening;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Config")]
    public RecoilConfig recoilConfig;

    private bool isShaking = false;

    public void ApplyRecoil()
    {
        if (isShaking) return;
        isShaking = true;

        // Sử dụng DOTween để tạo hiệu ứng giật
        transform.DOShakePosition(recoilConfig.duration, recoilConfig.strength, recoilConfig.vibrato, recoilConfig.randomness, false, recoilConfig.fadeOut)
            .OnComplete(() => isShaking = false);
    }
}
