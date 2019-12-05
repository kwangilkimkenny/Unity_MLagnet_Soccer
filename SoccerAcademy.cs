using UnityEngine;
using MLAgents; // 아카데미니가 머신러닝에이전트를 사용

/* 아카데미임, 쵝화로 에이전트의 색, 중력, 랜덤플레이어 정하기, 스피드 설정
 * 에이전트의 각각의 보상과 징벌을 실수형 값으로 선언해줌
 * 중력도 정해줌(무거워야 공이 튀지 않으니까) 무겁게무겁게
 * 아카데미 초기화
 * 
 * 
 */


public class SoccerAcademy : Academy
{
    public Material purpleMaterial; //재질
    public Material blueMaterial; //재질설정
    public float gravityMultiplier = 1; //중력의 실수형 기본값은 1
    public bool randomizePlayersTeamForTraining = true; //트레이닝을 위해서 플레이어는 랜덤으로 만든다로 불함수사용. 관련: SoccerFieldArea.cs,  ????>>>>>>>그런데 이건 왜 여기서 정해야 하는가? 사커필드에러리어에서는 못 정하나?

    public float agentRunSpeed; // 스피드 설정


    // 아하 여긴는 보상부분이구만. 

    public float strikerPunish; //if opponents scores, the striker gets this neg reward (-1) 상대방득점하면 당연히 -1점을 받음
    public float strikerReward; //if team scores a goal they get a reward (+1) 팀이 골을 넣으면 보상
    public float goaliePunish; //if opponents score, goalie gets this neg reward (-1) 상대방이 득점하면 골라이는 -1
    public float goalieReward; //if team scores, goalie gets this reward (currently 0...no reward. can play with this later)팀이 이기면 골라도 득점 현재는 0인데 나중에 이것가지로 플레이가능

    void Start()
    {
        Physics.gravity *= gravityMultiplier; //for soccer a multiplier of 3 looks good 그래? 3이 좋타고? 공이 날라다니지 않게?
    }

    public override void AcademyReset()
    {
        Physics.gravity = new Vector3(0, -resetParameters["gravity"], 0);
    }

    public override void AcademyStep()
    {
    }
}
