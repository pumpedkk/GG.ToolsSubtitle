using UnityEngine;
using GGTools.Subtitle;

public abstract class SubtitleExtension : MonoBehaviour
{
    protected Subtitle subtitle;
    public virtual void Reset()
    {
        subtitle = GetComponent<Subtitle>();
    }

}
