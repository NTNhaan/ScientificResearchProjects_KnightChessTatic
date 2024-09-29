using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Text.RegularExpressions;
public class IdelAction : ActionNode
{   
    public string animationTriggerIdel; //idle block
    private bool isIdle=false;
    private Animator animator; //animator find animationController
    public RuntimeAnimatorController animatorController;
    protected override void OnStart() {
        GameObject demon = GameObject.Find("Demon");
        animator = demon.GetComponent<Animator>();
        Debug.Log("Animator is acttached to: " + animator.gameObject.name);
        animator.runtimeAnimatorController = animatorController;
    }
    protected override void OnStop() {
    }
    protected override State OnUpdate() {
        if(animator == null || animationTriggerIdel == null) {
            Debug.LogError("Animator or condition idle is null");
        }
        else {
            isIdle = true;
            animator.SetTrigger(animationTriggerIdel);
            Debug.Log("Idle is running");
        }
        return isIdle ? State.Success : State.Failure;
    }
}