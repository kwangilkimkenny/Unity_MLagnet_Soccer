using UnityEngine;
using MLAgents;


/* 팀을 설정한다. 역할도 설정한다. ㅎ
 * 레이인식과 인식할 오브젝트들을 정해준다.
 * 
 * 
 * 
 * 
 * 
 */

public class AgentSoccer : Agent // 이 클래스는 SoccerField에서 사용된다. 팀설정을 위한거다.
{
    public enum Team // 팀설정, 구분을 하기위해 enum 사용
    {
        Purple,
        Blue
    }
    public enum AgentRole //역할설정
    {
        Striker,
        Goalie
    }


    public Team team;
    public AgentRole agentRole;  // 스트라이커와 골라이의 역할설정
    float m_KickPower; 
    int m_PlayerIndex; //플레이어 인덱스
    public SoccerFieldArea area;

    [HideInInspector]
    public Rigidbody agentRb; // 에이전트의 리지드바디를 정한다. 물리적 효과를 줌
    SoccerAcademy m_Academy; // 싸커아카데미의 클라스에서 선언되는 변수 m_Academy 선언>>>>> 왜????
    Renderer m_AgentRenderer;
    RayPerception m_RayPer; // 레이값을 담는 변수 설정

    float[] m_RayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f }; // 레이케스트로 다양한 각도를 주어 케스팅되는 대로 제어하기 위함으로 공간확보

    string[] m_DetectableObjectsPurple = { "ball", "purpleGoal", "blueGoal",
                                           "wall", "purpleAgent", "blueAgent" }; //디텍팅 해야하는 것
    string[] m_DetectableObjectsBlue = { "ball", "blueGoal", "purpleGoal",    //역시 디텍팅 해야하는 것으로 퍼플과 블루를 구분
                                         "wall", "blueAgent", "purpleAgent" };

    public void ChooseRandomTeam() // 랜덤으로 선택으로 레인지 0~2 사이에 결정함, 초기화 함수 link: SoccdrFieldArea.cs >>> 그런데 왜 랜덤으로 팀을 정할까????
    {
        team = (Team)Random.Range(0, 2);
        if (team == Team.Purple)
        {
            JoinPurpleTeam(agentRole);
        }
        else
        {
            JoinBlueTeam(agentRole);
        }
    }

    public void JoinPurpleTeam(AgentRole role) // 퍼플팀의 역할을 부여, 초기화 관련 함수
    {
        agentRole = role; // 역할
        team = Team.Purple; // 퍼플팀에 넣음
        m_AgentRenderer.material = m_Academy.purpleMaterial; // 채색
        tag = "purpleAgent"; //퍼플에이전트로 태깅
    }

    public void JoinBlueTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.Blue;
        m_AgentRenderer.material = m_Academy.blueMaterial;
        tag = "blueAgent";
    }

    public override void InitializeAgent() // 초기화됨. 매우 중요함. 
    {
        base.InitializeAgent(); //에이전트 초기화 이때는 에이전트가 퍼플도 블루도 아님 >>> 여기서 base의 의미는 부모의 오버라이드된 InitializaAgnet()라는 메소드를 지정한다는거, 초기화값을 부모에게 전달
        m_AgentRenderer = GetComponentInChildren<Renderer>(); // 렌더러로 화면에 나타남?
        m_RayPer = GetComponent<RayPerception>(); //레이인식
        m_Academy = FindObjectOfType<SoccerAcademy>(); // 축구아카데미 설정
        agentRb = GetComponent<Rigidbody>(); // // 강체를 적용
        agentRb.maxAngularVelocity = 500; // 최고 속도를 500으로 설정 그런데 이거 실수

        var playerState = new PlayerState //???
        {
            agentRb = agentRb,
            startingPos = transform.position,
            agentScript = this,
        };

        area.playerStates.Add(playerState);
        m_PlayerIndex = area.playerStates.IndexOf(playerState); //????
        playerState.playerIndex = m_PlayerIndex;
    }

    public override void CollectObservations() // 관찰함수인데 관찰 결과를 딥러닝처리하기위함. 설정하면 자동전달.
    {
        var rayDistance = 20f; // 레이의 거리설정
        string[] detectableObjects; // 인식한 모든 데이터를 저장하기위해 스트링으로 배열공간 확보
        if (team == Team.Purple) // 퍼플팀이면
        {
            detectableObjects = m_DetectableObjectsPurple; //퍼플팀이 인식할 것을 넣어줌
        }
        else
        {
            detectableObjects = m_DetectableObjectsBlue; // 그렇지 않으면 당연 블루팀이고 블루팀의 인식템을 넣어줌
        }
        
        // 가까이 있는 것들을 인식해서 학습자료로 전달하는 기능, 전체를 보지않고 각 에에전트가 인식하는 것들을 봄, 그리고 각 에이전트의 행동을 제어해야하니까.
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 0f, 0f)); //레이캐스팅 거리, 적용각도, 인식거리0F일때 인식한 오브젝트를 벡터값으로 추가함
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 1f, 0f)); //이것도 축가하는데 1f는 인식한 물체와의 거리가 1f일때이고,
    }

    public void MoveAgent(float[] act) // 에이전트가 이동하게 하는 함수로  여기에 숫자들이 들어가는데 
    {
        var dirToGo = Vector3.zero; //초기값은 제로  - 직진
        var rotateDir = Vector3.zero; // 초기값은 제로로 -  회전

        var action = Mathf.FloorToInt(act[0]); // 이것은 수학함수를 적용한 엑션인데, 소수점이하를 버림, 그리고 정수값으로 변환

        // Goalies and Strikers have slightly different action spaces.
        if (agentRole == AgentRole.Goalie)
        {
            m_KickPower = 0f;
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    m_KickPower = 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 4:
                    dirToGo = transform.right * -1f;
                    break;
                case 3:
                    dirToGo = transform.right * 1f;
                    break;
            }
        }
        else
        {
            m_KickPower = 0f;
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
                    m_KickPower = 1f;
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
                case 5:
                    dirToGo = transform.right * -0.75f;
                    break;
                case 6:
                    dirToGo = transform.right * 0.75f;
                    break;
            }
        }
        transform.Rotate(rotateDir, Time.deltaTime * 100f); //회전
        agentRb.AddForce(dirToGo * m_Academy.agentRunSpeed, // 아카데미에서 에이전트가 주는 달리기스피드를 곱해서 힘을 제공 이러면 속도가 변화됨
            ForceMode.VelocityChange);
    }

    public override void AgentAction(float[] vectorAction, string textAction) // 이건 그냥 가만이 있더도 패널이를 줌. 가만히 있으면 안되니까. 
    {
        // Existential penalty for strikers.
        if (agentRole == AgentRole.Striker)
        {
            AddReward(-1f / 3000f);
        }
        // Existential bonus for goalies.
        if (agentRole == AgentRole.Goalie)
        {
            AddReward(1f / 3000f);
        }
        MoveAgent(vectorAction);
    }

    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        var force = 2000f * m_KickPower;
        if (c.gameObject.CompareTag("ball")) // 공차는 기능으로 공의 태그를 비고해서
        {
            var dir = c.contacts[0].point - transform.position; //위치를 계산해서 공을 차게함
            dir = dir.normalized; // 공을 똑바로 차기위한 정렬, 정규화로 방향값을 1로 줌. 공을 제데로 차야하니까
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force); // 공에 힘을 더해서 공발사
        }
    }

    public override void AgentReset() // 결과에따라 에이전트를 리셋한다. 팀이 정해지고 역할도 정해진다. 여기서 결과를 명령하는 함수는 다른 코드에 있음
    {
        if (m_Academy.randomizePlayersTeamForTraining)
        {
            ChooseRandomTeam();
        }

        if (team == Team.Purple)
        {
            JoinPurpleTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // 팀이 정해지면 상대골대쪽으로 회전해야 함. 그래야 선수들 자세정렬
        }
        else
        {
            JoinBlueTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        transform.position = area.GetRandomSpawnPos(agentRole, team); // 스폰하면서 역할과 팀이 정해지고 위치도 정해짐
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        SetResetParameters(); //파라미터값 초기화
    }

    public void SetResetParameters()
    {
        area.ResetBall();
    }
}
