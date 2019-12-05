using UnityEngine;


//���� �ӽŷ��׿� �����͸� ���������� ������...

    /*���� ���¸� �˷���, �� �� �ݶ��̴� �浹üũ�Ͽ� �� ������ - ������ �ϸ� ��� ���� ���� ������Ʈ���� ��������
     * 
     * 
     * 
     */

public class SoccerBallController : MonoBehaviour
{
    [HideInInspector]
    public SoccerFieldArea area; // �ʵ带 �������ְ�
    public AgentSoccer lastTouchedBy; //who was the last to touch the ball �������� ��ġ�� ���� �������� ����ϵ��� ����������. ���� ����ϰ� �־���� ��
    public string agentTag; //will be used to check if collided with a agent ������Ʈ�� ��Ҵ��� üũ�ϱ� ���� ��Ʈ���Լ�
    public string purpleGoalTag; //will be used to check if collided with red goal ������ ��Ҵ��� üũ, �ۺ����� �����Ѱ� �ٸ� ������ �� �� �ֵ���
    public string blueGoalTag; //will be used to check if collided with blue goal ���� ��Ҵ��� üũ

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched red goal
        {
            area.GoalTouched(AgentSoccer.Team.Blue);
        }
        if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            area.GoalTouched(AgentSoccer.Team.Purple); //����ġ �Լ��� ����ȴ�. �������� ����
        }
    }
}
