using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loading = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private Vector3 TestPosition = Vector3.zero;
    [SerializeField] private Vector3 StartPosition = Vector3.zero; 
    
    private string ClickName = null;

    private void Start()
    {
        ClickName = PlayerPrefs.GetString("ClickBtn");
        SettingScene(ClickName);

        Invoke("DisableLoading", 2f);
    }

    private void SettingScene(string _ClickBtn)
    {
         if (_ClickBtn == "Test")
        {
            // 훈련장 위치로 플레이어 이동
            player.transform.position = TestPosition;
        }

         if (_ClickBtn == "Start")
        {
            // 보스룸 앞으로 플레이어 이동
            player.transform.position = StartPosition;
        }
    }

    private void DisableLoading()
    {
        loading.SetActive(false);
    }
}
