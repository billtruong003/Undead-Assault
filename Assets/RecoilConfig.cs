using UnityEngine;

[CreateAssetMenu(fileName = "RecoilConfig", menuName = "ScriptableObjects/RecoilConfig", order = 1)]
public class RecoilConfig : ScriptableObject
{
    public float duration = 0.1f; // Thời gian giật
    public float strength = 1f; // Độ mạnh của giật
    public int vibrato = 10; // Độ rung
    public float randomness = 90f; // Độ ngẫu nhiên
    public bool fadeOut = true; // Mờ dần
}
