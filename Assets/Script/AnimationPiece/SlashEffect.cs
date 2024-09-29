using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public GameObject slashEffect;
    public Transform targetPos;
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        Debug.Log(animator.runtimeAnimatorController.name);
    }
    public void PlaySlashEffect()
    {
        GameObject SlashEffect = Instantiate(slashEffect, transform.position, Quaternion.identity);
        StartCoroutine(MoveSlashEffect(SlashEffect));
    }
    private IEnumerator MoveSlashEffect(GameObject vfx)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = vfx.transform.position;
        Vector3 endPos = targetPos.position;
        while (elapsed < duration)
        {
            vfx.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vfx.transform.position = endPos;
        Destroy(vfx);
    }
}
