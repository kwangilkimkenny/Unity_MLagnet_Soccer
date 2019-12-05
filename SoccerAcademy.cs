using UnityEngine;
using MLAgents; // ��ī���̴ϰ� �ӽŷ��׿�����Ʈ�� ���

/* ��ī������, ��ȭ�� ������Ʈ�� ��, �߷�, �����÷��̾� ���ϱ�, ���ǵ� ����
 * ������Ʈ�� ������ ����� ¡���� �Ǽ��� ������ ��������
 * �߷µ� ������(���ſ��� ���� Ƣ�� �����ϱ�) ���̰Թ��̰�
 * ��ī���� �ʱ�ȭ
 * 
 * 
 */


public class SoccerAcademy : Academy
{
    public Material purpleMaterial; //����
    public Material blueMaterial; //��������
    public float gravityMultiplier = 1; //�߷��� �Ǽ��� �⺻���� 1
    public bool randomizePlayersTeamForTraining = true; //Ʈ���̴��� ���ؼ� �÷��̾�� �������� ����ٷ� ���Լ����. ����: SoccerFieldArea.cs,  ????>>>>>>>�׷��� �̰� �� ���⼭ ���ؾ� �ϴ°�? ��Ŀ�ʵ忡��������� �� ���ϳ�?

    public float agentRunSpeed; // ���ǵ� ����


    // ���� ����� ����κ��̱���. 

    public float strikerPunish; //if opponents scores, the striker gets this neg reward (-1) ��������ϸ� �翬�� -1���� ����
    public float strikerReward; //if team scores a goal they get a reward (+1) ���� ���� ������ ����
    public float goaliePunish; //if opponents score, goalie gets this neg reward (-1) ������ �����ϸ� ����̴� -1
    public float goalieReward; //if team scores, goalie gets this reward (currently 0...no reward. can play with this later)���� �̱�� ��� ���� ����� 0�ε� ���߿� �̰Ͱ����� �÷��̰���

    void Start()
    {
        Physics.gravity *= gravityMultiplier; //for soccer a multiplier of 3 looks good �׷�? 3�� ��Ÿ��? ���� ����ٴ��� �ʰ�?
    }

    public override void AcademyReset()
    {
        Physics.gravity = new Vector3(0, -resetParameters["gravity"], 0);
    }

    public override void AcademyStep()
    {
    }
}
