using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICharacterState
{
    void Enter(Character character);
    void Update(Character character);
    void Exit(Character character);
}