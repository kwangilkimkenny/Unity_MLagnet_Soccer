using UnityEngine;


//여긴 머신러닝에 데이터를 직접주지는 않지만...

    /*공의 상태를 알려줌, 즉 공 콜라이더 충돌체크하여 골에 닿으면 - 골인을 하면 결과 값을 값을 에이전트에게 전달해줌
     * 
     * 
     * 
     */

public class SoccerBallController : MonoBehaviour
{
    [HideInInspector]
    public SoccerFieldArea area; // 필드를 설정해주고
    public AgentSoccer lastTouchedBy; //who was the last to touch the ball 마지막에 터치한 놈이 누구인지 기억하도록 변수선언함. 공은 기억하고 있어야함 ㅎ
    public string agentTag; //will be used to check if collided with a agent 에이전트에 닿았는지 체크하기 위한 스트링함수
    public string purpleGoalTag; //will be used to check if collided with red goal 빨강골에 닿았는지 체크, 퍼블릭으로 선언한건 다른 곳에서 볼 수 있도록
    public string blueGoalTag; //will be used to check if collided with blue goal 블루골에 닿았는지 체크

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched red goal
        {
            area.GoalTouched(AgentSoccer.Team.Blue);
        }
        if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            area.GoalTouched(AgentSoccer.Team.Purple); //골터치 함수가 실행된다. 득점으로 연결
        }
    }
}
