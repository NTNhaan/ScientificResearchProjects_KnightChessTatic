using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage buttonImage;
    [SerializeField] private float x, y;

    void Update()
    {
        buttonImage.uvRect = new Rect(buttonImage.uvRect.position + new Vector2(x, y) * Time.deltaTime, buttonImage.uvRect.size);
    }
}
