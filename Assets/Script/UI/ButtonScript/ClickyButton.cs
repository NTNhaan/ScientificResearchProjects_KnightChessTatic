using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite defaultBT, pressBT;
    [SerializeField] private AudioClip compressClip, uncompessClip;
    [SerializeField] private AudioSource audioSource;
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.sprite = pressBT;
        audioSource.PlayOneShot(compressClip);  // just one play sound
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        buttonImage.sprite = defaultBT;
        audioSource.PlayOneShot(uncompessClip);
    }
    public void text()
    {
        Debug.Log("Button Clicked");
    }
}

