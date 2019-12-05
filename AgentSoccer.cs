using UnityEngine;
using MLAgents;


/* ���� �����Ѵ�. ���ҵ� �����Ѵ�. ��
 * �����νİ� �ν��� ������Ʈ���� �����ش�.
 * 
 * 
 * 
 * 
 * 
 */

public class AgentSoccer : Agent // �� Ŭ������ SoccerField���� ���ȴ�. �������� ���ѰŴ�.
{
    public enum Team // ������, ������ �ϱ����� enum ���
    {
        Purple,
        Blue
    }
    public enum AgentRole //���Ҽ���
    {
        Striker,
        Goalie
    }


    public Team team;
    public AgentRole agentRole;  // ��Ʈ����Ŀ�� ������� ���Ҽ���
    float m_KickPower; 
    int m_PlayerIndex; //�÷��̾� �ε���
    public SoccerFieldArea area;

    [HideInInspector]
    public Rigidbody agentRb; // ������Ʈ�� ������ٵ� ���Ѵ�. ������ ȿ���� ��
    SoccerAcademy m_Academy; // ��Ŀ��ī������ Ŭ�󽺿��� ����Ǵ� ���� m_Academy ����>>>>> ��????
    Renderer m_AgentRenderer;
    RayPerception m_RayPer; // ���̰��� ��� ���� ����

    float[] m_RayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f }; // �����ɽ�Ʈ�� �پ��� ������ �־� �ɽ��õǴ� ��� �����ϱ� �������� ����Ȯ��

    string[] m_DetectableObjectsPurple = { "ball", "purpleGoal", "blueGoal",
                                           "wall", "purpleAgent", "blueAgent" }; //������ �ؾ��ϴ� ��
    string[] m_DetectableObjectsBlue = { "ball", "blueGoal", "purpleGoal",    //���� ������ �ؾ��ϴ� ������ ���ð� ��縦 ����
                                         "wall", "blueAgent", "purpleAgent" };

    public void ChooseRandomTeam() // �������� �������� ������ 0~2 ���̿� ������, �ʱ�ȭ �Լ� link: SoccdrFieldArea.cs >>> �׷��� �� �������� ���� ���ұ�????
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

    public void JoinPurpleTeam(AgentRole role) // �������� ������ �ο�, �ʱ�ȭ ���� �Լ�
    {
        agentRole = role; // ����
        team = Team.Purple; // �������� ����
        m_AgentRenderer.material = m_Academy.purpleMaterial; // ä��
        tag = "purpleAgent"; //���ÿ�����Ʈ�� �±�
    }

    public void JoinBlueTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.Blue;
        m_AgentRenderer.material = m_Academy.blueMaterial;
        tag = "blueAgent";
    }

    public override void InitializeAgent() // �ʱ�ȭ��. �ſ� �߿���. 
    {
        base.InitializeAgent(); //������Ʈ �ʱ�ȭ �̶��� ������Ʈ�� ���õ� ��絵 �ƴ� >>> ���⼭ base�� �ǹ̴� �θ��� �������̵�� InitializaAgnet()��� �޼ҵ带 �����Ѵٴ°�, �ʱ�ȭ���� �θ𿡰� ����
        m_AgentRenderer = GetComponentInChildren<Renderer>(); // �������� ȭ�鿡 ��Ÿ��?
        m_RayPer = GetComponent<RayPerception>(); //�����ν�
        m_Academy = FindObjectOfType<SoccerAcademy>(); // �౸��ī���� ����
        agentRb = GetComponent<Rigidbody>(); // // ��ü�� ����
        agentRb.maxAngularVelocity = 500; // �ְ� �ӵ��� 500���� ���� �׷��� �̰� �Ǽ�

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

    public override void CollectObservations() // �����Լ��ε� ���� ����� ������ó���ϱ�����. �����ϸ� �ڵ�����.
    {
        var rayDistance = 20f; // ������ �Ÿ�����
        string[] detectableObjects; // �ν��� ��� �����͸� �����ϱ����� ��Ʈ������ �迭���� Ȯ��
        if (team == Team.Purple) // �������̸�
        {
            detectableObjects = m_DetectableObjectsPurple; //�������� �ν��� ���� �־���
        }
        else
        {
            detectableObjects = m_DetectableObjectsBlue; // �׷��� ������ �翬 ������̰� ������� �ν����� �־���
        }
        
        // ������ �ִ� �͵��� �ν��ؼ� �н��ڷ�� �����ϴ� ���, ��ü�� �����ʰ� �� ������Ʈ�� �ν��ϴ� �͵��� ��, �׸��� �� ������Ʈ�� �ൿ�� �����ؾ��ϴϱ�.
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 0f, 0f)); //����ĳ���� �Ÿ�, ���밢��, �νİŸ�0F�϶� �ν��� ������Ʈ�� ���Ͱ����� �߰���
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 1f, 0f)); //�̰͵� �డ�ϴµ� 1f�� �ν��� ��ü���� �Ÿ��� 1f�϶��̰�,
    }

    public void MoveAgent(float[] act) // ������Ʈ�� �̵��ϰ� �ϴ� �Լ���  ���⿡ ���ڵ��� ���µ� 
    {
        var dirToGo = Vector3.zero; //�ʱⰪ�� ����  - ����
        var rotateDir = Vector3.zero; // �ʱⰪ�� ���η� -  ȸ��

        var action = Mathf.FloorToInt(act[0]); // �̰��� �����Լ��� ������ �����ε�, �Ҽ������ϸ� ����, �׸��� ���������� ��ȯ

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
        transform.Rotate(rotateDir, Time.deltaTime * 100f); //ȸ��
        agentRb.AddForce(dirToGo * m_Academy.agentRunSpeed, // ��ī���̿��� ������Ʈ�� �ִ� �޸��⽺�ǵ带 ���ؼ� ���� ���� �̷��� �ӵ��� ��ȭ��
            ForceMode.VelocityChange);
    }

    public override void AgentAction(float[] vectorAction, string textAction) // �̰� �׳� ������ �ִ��� �г��̸� ��. ������ ������ �ȵǴϱ�. 
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
        if (c.gameObject.CompareTag("ball")) // ������ ������� ���� �±׸� ����ؼ�
        {
            var dir = c.contacts[0].point - transform.position; //��ġ�� ����ؼ� ���� ������
            dir = dir.normalized; // ���� �ȹٷ� �������� ����, ����ȭ�� ���Ⱚ�� 1�� ��. ���� ������ �����ϴϱ�
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force); // ���� ���� ���ؼ� ���߻�
        }
    }

    public override void AgentReset() // ��������� ������Ʈ�� �����Ѵ�. ���� �������� ���ҵ� ��������. ���⼭ ����� ����ϴ� �Լ��� �ٸ� �ڵ忡 ����
    {
        if (m_Academy.randomizePlayersTeamForTraining)
        {
            ChooseRandomTeam();
        }

        if (team == Team.Purple)
        {
            JoinPurpleTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // ���� �������� ����������� ȸ���ؾ� ��. �׷��� ������ �ڼ�����
        }
        else
        {
            JoinBlueTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        transform.position = area.GetRandomSpawnPos(agentRole, team); // �����ϸ鼭 ���Ұ� ���� �������� ��ġ�� ������
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        SetResetParameters(); //�Ķ���Ͱ� �ʱ�ȭ
    }

    public void SetResetParameters()
    {
        area.ResetBall();
    }
}
