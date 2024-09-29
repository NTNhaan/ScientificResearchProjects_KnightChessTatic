using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;


public class BloodLoss : ActionNode
{
    public static bool isBloodLoss = false;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return isBloodLoss ? State.Success : State.Failure;
    }
}
