using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockState : ICharacterState
{
    private bool isBlocking = false;

    public void Enter(Character character)
    {
        character.GetComponent<Animator>().SetTrigger("Block");
        isBlocking = true;
    }

    public void Update(Character character)
    {
        // Check if player releases block button (right mouse button)
        if (Input.GetMouseButtonUp(1))
        {
            character.GetComponent<Animator>().SetBool("IdleBlock", false);
            character.ChangeState(new IdleState());
        }
        else if (Input.GetMouseButton(1))
        {
            character.GetComponent<Animator>().SetBool("IdleBlock", true);
        }
    }

    public void Exit(Character character)
    {
        isBlocking = false;
        character.GetComponent<Animator>().SetBool("IdleBlock", false);
    }
} 