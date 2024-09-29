using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsTurnHero : ActionNode
{
    private State StateRole = State.Failure;
    protected override void OnStart() {
        if(ChangeRole.role == TimeBar.Role.Demon)
            StateRole = State.Failure;
    }

    protected override void OnStop() {
        if(ChangeRole.role == TimeBar.Role.Demon)
            StateRole = State.Failure;
    }

    protected override State OnUpdate() {
        if(ChangeRole.role == TimeBar.Role.Player)
        {                           
            if(ChangeRole.role != TimeBar.Role.Player)
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
