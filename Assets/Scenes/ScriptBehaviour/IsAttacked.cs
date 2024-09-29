using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsAttacked : ActionNode
{
    private bool isAttacked = false;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return isAttacked ? State.Success : State.Failure;  
    }
}
