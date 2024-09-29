using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class IsTurn : ActionNode
{
    private State StateRole = State.Failure;

    protected override void OnStart() {
        if(ChangeRole.role == TimeBar.Role.Player)
            StateRole = State.Failure;
    }

    protected override void OnStop() {
        if(ChangeRole.role == TimeBar.Role.Player)
            StateRole = State.Failure;
    }

    protected override State OnUpdate() {
        if(ChangeRole.role == TimeBar.Role.Demon)
        {
            if(ChangeRole.role != TimeBar.Role.Demon)
            {
                return State.Failure;
            }
            StateRole = State.Success;
        }
        else
        {
            return State.Failure;
        }
        return StateRole;
        
    }
}
