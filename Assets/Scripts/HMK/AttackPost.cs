using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenHitEffect : MonoBehaviour
{
    public Volume volume; //볼륨 가져오기
    private LensDistortion lensDistortion; //왜곡효과 변수
    private bool isHit = false; //효과가 켜진 상태 확인하는 변수
    private float intensity = 0f; //왜곡 기본 값

    private void Start()
    {
        if (volume.profile.TryGet<LensDistortion>(out lensDistortion))//볼륨 프로파일에서 Lens를 가져옴
        {
            lensDistortion.intensity.Override(0f); // 초기값 0
        }
        else
        {
            Debug.Log("파일없음");
        }
    }

    public void TriggerHitEffect()
    {
        isHit = true; //활성화 되면
        intensity = -0.6f; //-0.5만큼 왜곡
    }

    private void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.R))
        {
            TriggerHitEffect();
        }

        // 왜곡 효과가 켜져 있을 경우 서서히 강도를 줄임
        if (isHit)
        {
            intensity = Mathf.Lerp(intensity, 0f, Time.deltaTime * 2f); // 서서히 감소 하는 효과로 만듬
            lensDistortion.intensity.Override(intensity);

            // 강도가 거의 0에 도달하면 효과 종료
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

   
    playerScreenHitEffect.TriggerHitEffect(); 함수 실행

}
*/
}



