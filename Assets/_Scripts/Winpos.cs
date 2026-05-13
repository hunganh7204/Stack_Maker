using UnityEngine;

public class Winpos : MonoBehaviour
{
    public static Winpos Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform standPos;
    [SerializeField] private ParticleSystem[] winEffects;

    private void OnEnable()
    {
        Instance = this;
    }

    public Vector3 GetStandPosition()
    {
        if (standPos != null) return standPos.position;
        return transform.position;
    }

    public void PlayWinEffects()
    {
        if (winEffects == null) return;
        foreach (ParticleSystem effect in winEffects)
        {
            if (effect != null) effect.Play();
        }
    }
}