using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class AttackAction : ActionNode
{
    private bool isAttack = false;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return isAttack ? State.Success : State.Failure;
    }
}
