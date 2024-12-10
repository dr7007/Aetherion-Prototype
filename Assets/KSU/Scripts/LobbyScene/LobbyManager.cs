using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void StartBtnClick()
    {
        // 누른 버튼 정보를 저장
        PlayerPrefs.SetString("ClickBtn", "Start");

        // 씬 로드
        SceneManager.LoadScene("MainScene");
    }
    public void TestBtnClick()
    {
        // 누른 버튼 정보를 저장   
        PlayerPrefs.SetString("ClickBtn", "Test");

        // 씬 로드
        SceneManager.LoadScene("MainScene");
    }
}
