using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckArrays : CompositeNode
{
    private bool ArraysisNull=false;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return ArraysisNull ? State.Success : State.Failure;
    }
}
