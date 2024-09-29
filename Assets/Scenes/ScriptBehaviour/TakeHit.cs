using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class TakeHit : ActionNode
{
    private bool isHit = false;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return isHit ? State.Success : State.Failure;
    }
}
