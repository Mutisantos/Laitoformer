using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{

    public AudioClip HoverAudioClip;
    public AudioClip ClickAudioClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlayEffectOnce(HoverAudioClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlayEffectOnce(ClickAudioClip);
    }
}
