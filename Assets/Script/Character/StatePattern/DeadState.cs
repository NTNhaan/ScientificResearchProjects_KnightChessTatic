using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadState : ICharacterState
{
    public void Enter(Character character)
    {
        character.GetComponent<Animator>().SetTrigger("Death");
    }
    public void Update(Character character)
    {
    }
    public void Exit(Character character)
    { }
}