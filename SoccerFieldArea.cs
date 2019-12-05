using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]

//���� Ŭ������ � �����Ѵ�. �ʵ忡���� �������� �͵��� �ݺ������� ó���ϱ� ����

/*
 Player�� ���¸� ���ϴ� Ŭ������ �ܺο��� ��������
 SoccerFieldArea�� ���¸� ���ϴ� Ŭ������ ���� �ܺ���������

 �ٴ��� ���� ������Ȳ�� ���� �����ϴ� ȿ��
 ������Ȳ�� UI�� ǥ�����ִ� ȿ��
 ������ȭ������ Reward, Punish�� �ִ� ���

 ������ �ϸ� �÷��̾� �����Ͽ� ���� ������ ������(reset team and role)

 �ʱ�ȭ�� ���� ������
 �ʱ�ȭ�� ���� ���� 0���� ����(reset)
 �ʱ�ȭ�� �÷��̾ ������


*/


public class PlayerState
{
    public int playerIndex;// �÷��̾�� �ε����� ���������� ó���ϰڴٰ� ����
    [FormerlySerializedAs("agentRB")]// agentRB�� ����� ���� ��� �����ϱ� ���ؼ� ����Ѵ�. �����հ� ���� ��� ���� �����ϰڴٴ� ���̴�.
    public Rigidbody agentRb;// �̰� ���� ����ϳ�? �� ��ũ��Ʈ�� ��������� ����?????
    public Vector3 startingPos;// ������ġ�� ���Ͱ��� �����ϱ� ���ؼ� ����ϴ� ����
    public AgentSoccer agentScript;// ������Ʈ��Ŀ�� Ŭ������ ���� ������Ʈ�� ������ ���ϴ� ���̴�. ��� ���ϴ���????? ������
    public float ballPosReward;//���� �����ǿ����� ����
}

public class SoccerFieldArea : MonoBehaviour // ��Ŀ�ʵ忡�� �����ϴ� Ŭ����
{
    public GameObject ball; // ���� �翬�� �ְ���
    [FormerlySerializedAs("ballRB")] // �̰͵� ���� ���� �����ϱ����ؼ� ����.
    [HideInInspector] //���⼭���ʹ� ���ܳ��� �ν�����. �׷��� �� ���⳪?
    public Rigidbody ballRb; // ���� ��ü�ٶ�� ����
    public GameObject ground; // �׶��� ����
    public GameObject centerPitch; // �߾ӿ� ���׶� �κ� ����
    SoccerBallController m_BallController; //��Ŀ����Ʈ�����ε� ���� ��� ��Ҵ��� �ľ�����. �׷��� �� �̰� ���⿡ �־��°�?
    public List<PlayerState> playerStates = new List<PlayerState>(); //�÷��̾��� ���¸� ����Ʈ���־���
    [HideInInspector]
    public Vector3 ballStartingPos;
    public GameObject goalTextUI;
    [HideInInspector]
    public bool canResetBall; // �̰� �־���
    Material m_GroundMaterial;
    Renderer m_GroundRenderer;
    SoccerAcademy m_Academy;

    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time) //�� �ʿ� ���������. �� �ۼ�����? �ð�ȿ�� ��
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    void Awake() // ������ ����Ǵ� �Լ��ε�
    {
        m_Academy = FindObjectOfType<SoccerAcademy>();
        m_GroundRenderer = centerPitch.GetComponent<Renderer>();//�߾���ġ���� ������ ���� �����ͼ� m_GroundRenderer�� �־���. ��? �� �ٴ��� ���� �ٲٱ� ���ؼ�???
        m_GroundMaterial = m_GroundRenderer.material;
        canResetBall = true; // ????
        if (goalTextUI) { goalTextUI.SetActive(false); } // ui�� ó������ �ȳ�����. ���� ������ ����
        ballRb = ball.GetComponent<Rigidbody>(); // ���� ��ü���� ballRb�� �ִ´�.
        m_BallController = ball.GetComponent<SoccerBallController>();
        m_BallController.area = this; //
        ballStartingPos = ball.transform.position; // ���� ��ġ�� ������ġ�� ������
    }

    IEnumerator ShowGoalUI() // ������ ������ ui�� ������ٰ� �����
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

    public void GoalTouched(AgentSoccer.Team scoredTeam) // ����ġ �Լ���. ������ ���� ��� ������ �ְ�, ���� ������Ʈ�� ������ ���ϰ�, �̱����� �ٴۻ��� �ٲ��ְ�, UI�� �����ߴٰ� ǥ���Ѵ�.
    {
        foreach (var ps in playerStates)// �÷��̾��� ���¸� �ݺ��ϴµ� ������ �ִ�.
        {
            if (ps.agentScript.team == scoredTeam) //�����ϸ� ���ְ� >>>>>> ���� �� ���� �߿���.
            {
                RewardOrPunishPlayer(ps, m_Academy.strikerReward, m_Academy.goalieReward); // ��Ʈ����ũ�� ����̿��� ������ �ش�. 
            }
            else
            {
                RewardOrPunishPlayer(ps, m_Academy.strikerPunish, m_Academy.goaliePunish); //������ �������� �� >>>>>> ���ذ��� �߿���..
            }
            if (m_Academy.randomizePlayersTeamForTraining) //SoccerAcademdmy�� bool ������ �⺻������ True�� ������ �������� ���� ������, �̰� AgentsSoccer.cs���� team and role����
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
            if (goalTextUI) // ���� ������ UI�� ǥ��
            {
                StartCoroutine(ShowGoalUI());
            }
        }
    }

    public void RewardOrPunishPlayer(PlayerState ps, float striker, float goalie) // ����� ���� �ִ� �Լ�
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

    public Vector3 GetRandomSpawnPos(AgentSoccer.AgentRole role, AgentSoccer.Team team) // ������Ʈ ������
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

    public Vector3 GetBallSpawnPosition() // �� ������
    {
        var randomSpawnPos = ground.transform.position +
            new Vector3(0f, 0f, 0f)
            + (Random.insideUnitSphere * 2);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    public void ResetBall() // �� �ʱ�ȭ, ���� ������ �ִ� ���� ��� 0���� ����
    {
        ball.transform.position = GetBallSpawnPosition();
        ballRb.velocity = Vector3.zero; // ���� �ӵ��� �ʱ�ȭ
        ballRb.angularVelocity = Vector3.zero; // ���ǰ��ӵ��� �ʱ�ȭ

        var ballScale = m_Academy.resetParameters["ball_scale"];
        ballRb.transform.localScale = new Vector3(ballScale, ballScale, ballScale); //�����ϵ� �ʱ�ȭ�� ������ ���� ���� �̹� ����
    }
}
