using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]

//여긴 클래스가 몇개 등장한다. 필드에어리어에서 벌어지는 것들을 반복적으로 처리하기 위함

/*
 Player의 상태를 정하는 클래스로 외부에서 참조가능
 SoccerFieldArea의 상태를 정하는 클래스로 역시 외부참조가능

 바닦의 색을 득점상황에 따라 변경하는 효과
 득점상황을 UI로 표시해주는 효과
 득점상화에따라 Reward, Punish를 주는 기능

 득점을 하면 플레이어 리셋하여 팀과 역할을 정해줌(reset team and role)

 초기화로 공을 리스폰
 초기화로 공의 값을 0으로 만듬(reset)
 초기화로 플레이어를 리스폰


*/


public class PlayerState
{
    public int playerIndex;// 플레이어는 인덱스로 정수값으로 처리하겠다고 정함
    [FormerlySerializedAs("agentRB")]// agentRB로 적용된 값을 모두 보존하기 위해서 사용한다. 프리팹과 씬의 모든 값을 보존하겠다는 말이다.
    public Rigidbody agentRb;// 이건 어디다 사용하나? 이 스크립트에 사용하지는 않음?????
    public Vector3 startingPos;// 시작위치의 벡터값을 저장하기 위해서 사용하는 변수
    public AgentSoccer agentScript;// 에이전트사커의 클래스로 팀과 에이전트의 역할을 정하는 것이다. 어떻게 정하느냐????? 아직모름
    public float ballPosReward;//볼의 포지션에따른 보상값
}

public class SoccerFieldArea : MonoBehaviour // 사커필드에어리어를 설정하는 클래스
{
    public GameObject ball; // 공은 당연히 있겠지
    [FormerlySerializedAs("ballRB")] // 이것도 공의 값을 보전하기위해서 적용.
    [HideInInspector] //여기서부터는 숨겨놓은 인스펙터. 그런데 왜 숨기나?
    public Rigidbody ballRb; // 공은 강체다라고 선언
    public GameObject ground; // 그라운드 선언
    public GameObject centerPitch; // 중앙에 동그란 부분 설정
    SoccerBallController m_BallController; //사커볼컨트럴러인데 공이 어디에 닿았는지 파악해줌. 그런데 왜 이걸 여기에 넣었는가?
    public List<PlayerState> playerStates = new List<PlayerState>(); //플레이어의 상태를 리스트에넣어줌
    [HideInInspector]
    public Vector3 ballStartingPos;
    public GameObject goalTextUI;
    [HideInInspector]
    public bool canResetBall; // 이건 왜쓰나
    Material m_GroundMaterial;
    Renderer m_GroundRenderer;
    SoccerAcademy m_Academy;

    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time) //별 필요 없어보이지만. 왜 작성했지? 시각효과 ㅎ
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    void Awake() // 무조건 실행되는 함수인데
    {
        m_Academy = FindObjectOfType<SoccerAcademy>();
        m_GroundRenderer = centerPitch.GetComponent<Renderer>();//중앙피치에서 랜더링 값을 가져와서 m_GroundRenderer에 넣어줌. 왜? 아 바닥의 색을 바꾸기 위해서???
        m_GroundMaterial = m_GroundRenderer.material;
        canResetBall = true; // ????
        if (goalTextUI) { goalTextUI.SetActive(false); } // ui가 처음에는 안나오고. 골을 넣으면 나옴
        ballRb = ball.GetComponent<Rigidbody>(); // 공의 강체값을 ballRb에 넣는다.
        m_BallController = ball.GetComponent<SoccerBallController>();
        m_BallController.area = this; //
        ballStartingPos = ball.transform.position; // 공의 위치를 시작위치로 정해줌
    }

    IEnumerator ShowGoalUI() // 골인을 넣으면 ui로 보여줬다가 사라짐
    {
        if (goalTextUI) goalTextUI.SetActive(true);
        yield return new WaitForSeconds(.25f);
        if (goalTextUI) goalTextUI.SetActive(false);
    }

    public void AllPlayersDone(float reward)
    {
        foreach (var ps in playerStates)
        {
            if (ps.agentScript.gameObject.activeInHierarchy)
            {
                if (reward != 0)
                {
                    ps.agentScript.AddReward(reward);
                }
                ps.agentScript.Done();
            }
        }
    }

    public void GoalTouched(AgentSoccer.Team scoredTeam) // 골터치 함수다. 득점에 따른 상과 벌점을 주고, 팀과 에이전트의 역할을 정하고, 이긴팀의 바닦색을 바꿔주고, UI에 득점했다고 표시한다.
    {
        foreach (var ps in playerStates)// 플레이어의 상태를 반복하는데 조건이 있다.
        {
            if (ps.agentScript.team == scoredTeam) //득점하면 상주고 >>>>>> 상을 준 값이 중요함.
            {
                RewardOrPunishPlayer(ps, m_Academy.strikerReward, m_Academy.goalieReward); // 스트라이크와 골라이에게 보상을 준다. 
            }
            else
            {
                RewardOrPunishPlayer(ps, m_Academy.strikerPunish, m_Academy.goaliePunish); //득점을 못했으니 벌 >>>>>> 벌준값도 중요함..
            }
            if (m_Academy.randomizePlayersTeamForTraining) //SoccerAcademdmy의 bool 값으로 기본설정은 True로 무조건 랜덤으로 팀이 정해짐, 이건 AgentsSoccer.cs에서 team and role정함
            {
                ps.agentScript.ChooseRandomTeam();
            }

            if (scoredTeam == AgentSoccer.Team.Purple)
            {
                StartCoroutine(GoalScoredSwapGroundMaterial(m_Academy.purpleMaterial, 1));
            }
            else
            {
                StartCoroutine(GoalScoredSwapGroundMaterial(m_Academy.blueMaterial, 1));
            }
            if (goalTextUI) // 골을 넣으면 UI에 표시
            {
                StartCoroutine(ShowGoalUI());
            }
        }
    }

    public void RewardOrPunishPlayer(PlayerState ps, float striker, float goalie) // 보상과 벌을 주는 함수
    {
        if (ps.agentScript.agentRole == AgentSoccer.AgentRole.Striker)
        {
            ps.agentScript.AddReward(striker);
        }
        if (ps.agentScript.agentRole == AgentSoccer.AgentRole.Goalie)
        {
            ps.agentScript.AddReward(goalie);
        }
        ps.agentScript.Done();  //all agents need to be reset
    }

    public Vector3 GetRandomSpawnPos(AgentSoccer.AgentRole role, AgentSoccer.Team team) // 에이전트 리스폰
    {
        var xOffset = 0f;
        if (role == AgentSoccer.AgentRole.Goalie)
        {
            xOffset = 13f;
        }
        if (role == AgentSoccer.AgentRole.Striker)
        {
            xOffset = 7f;
        }
        if (team == AgentSoccer.Team.Blue)
        {
            xOffset = xOffset * -1f;
        }
        var randomSpawnPos = ground.transform.position +
            new Vector3(xOffset, 0f, 0f)
            + (Random.insideUnitSphere * 2);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    public Vector3 GetBallSpawnPosition() // 공 리스폰
    {
        var randomSpawnPos = ground.transform.position +
            new Vector3(0f, 0f, 0f)
            + (Random.insideUnitSphere * 2);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    public void ResetBall() // 공 초기화, 공이 가지고 있는 값을 모두 0으로 만듬
    {
        ball.transform.position = GetBallSpawnPosition();
        ballRb.velocity = Vector3.zero; // 공의 속도를 초기화
        ballRb.angularVelocity = Vector3.zero; // 공의각속도를 초기화

        var ballScale = m_Academy.resetParameters["ball_scale"];
        ballRb.transform.localScale = new Vector3(ballScale, ballScale, ballScale); //스케일도 초기화로 설정해 놓은 값이 이미 있음
    }
}
