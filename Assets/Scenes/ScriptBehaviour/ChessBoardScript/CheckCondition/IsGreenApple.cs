using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsGreenApple : CompositeNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return BloodLoss.isBloodLoss ? State.Success : State.Failure;
    }
}
