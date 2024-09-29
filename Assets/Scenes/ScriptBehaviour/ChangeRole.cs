using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.UI;


public class ChangeRole : ActionNode
{
    public static TimeBar.Role role;
    public Slider TimeSliderDemon;
    public Slider TimeSliderHero;   
    private float MaxTime;
    protected void Awake() {
        TimeSliderHero.value = TimeSliderHero.GetComponent<Slider>().value;
        TimeSliderDemon.value = TimeSliderDemon.GetComponent<Slider>().value;
        TimeSliderHero.value = MaxTime;
        TimeSliderDemon.value = MaxTime;
        MaxTime=100;
        role = TimeBar.Role.Player;
    }

    protected override void OnStart() {
        
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (role == TimeBar.Role.Player)
        {
            Debug.Log("RoleValue 1: " + TimeSliderHero.value);
            Debug.Log("Roleplayer: " + role);
            if (TimeSliderHero.value <= 0)
            {
                Debug.Log("Role Changing role to DEMON");
                role = TimeBar.Role.Demon;
                TimeSliderHero.value = MaxTime;
                Debug.Log("Role New TimeSliderHero value: " + TimeSliderHero.value);
            }
            else
                TimeSliderHero.value -= Time.deltaTime * 10;
        }
        else
        {
            Debug.Log("RoleValue 2: " + TimeSliderDemon.value);
            Debug.Log("Roleplayer: " + role);
            if (TimeSliderDemon.value <= 0)
            {
                Debug.Log("Role Changing role to PLAYER");
                role = TimeBar.Role.Player;
                TimeSliderDemon.value = MaxTime;
                Debug.Log("Role New TimeSliderDemon value: " + TimeSliderDemon.value);
            }
            else
                TimeSliderDemon.value -= Time.deltaTime * 10;
        }
        return State.Success;
    }
}
