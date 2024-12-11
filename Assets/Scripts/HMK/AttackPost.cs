using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenHitEffect : MonoBehaviour
{
    public Volume volume; //���� ��������
    private LensDistortion lensDistortion; //�ְ�ȿ�� ����
    private bool isHit = false; //ȿ���� ���� ���� Ȯ���ϴ� ����
    private float intensity = 0f; //�ְ� �⺻ ��

    private void Start()
    {
        if (volume.profile.TryGet<LensDistortion>(out lensDistortion))//���� �������Ͽ��� Lens�� ������
        {
            lensDistortion.intensity.Override(0f); // �ʱⰪ 0
        }
        else
        {
            Debug.Log("���Ͼ���");
        }
    }

    public void TriggerHitEffect()
    {
        isHit = true; //Ȱ��ȭ �Ǹ�
        intensity = -0.6f; //-0.5��ŭ �ְ�
    }

    private void Update()
    {
        // �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.R))
        {
            TriggerHitEffect();
        }

        // �ְ� ȿ���� ���� ���� ��� ������ ������ ����
        if (isHit)
        {
            intensity = Mathf.Lerp(intensity, 0f, Time.deltaTime * 2f); // ������ ���� �ϴ� ȿ���� ����
            lensDistortion.intensity.Override(intensity);

            // ������ ���� 0�� �����ϸ� ȿ�� ����
            if (intensity >= -0.01f)
            {
                isHit = false;
                lensDistortion.intensity.Override(0f);
            }
        }
    }
    /*    
{
    public ScreenHitEffect playerScreenHitEffect; 

   
    playerScreenHitEffect.TriggerHitEffect(); �Լ� ����

}
*/
}



