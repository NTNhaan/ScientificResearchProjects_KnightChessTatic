// public class SpeedUpState : ICharacterState
// {
//     private float speedUpDuration = 5f;
//     private float speedUpTimer;
//     private TimeBar timeBar;
//     private float speedMultiplier = 2f;

//     public void Enter(Character character)
//     {
//         speedUpTimer = speedUpDuration;
//         timeBar = FindObjectOfType<TimeBar>();

//         if (timeBar != null)
//         {
//             // Sử dụng phương thức SetTimeScale hiện có
//             timeBar.SetTimeScale(speedMultiplier);
//         }
//     }

//     public void Update(Character character)
//     {
//         if (speedUpTimer > 0)
//         {
//             speedUpTimer -= Time.deltaTime;
//         }
//         else
//         {
//             if (timeBar != null)
//             {
//                 timeBar.ResetTimeScale();
//             }
//             character.ChangeState(new IdleState());
//         }
//     }

//     public void Exit(Character character)
//     {
//         if (timeBar != null)
//         {
//             timeBar.ResetTimeScale();
//         }
//     }
// }