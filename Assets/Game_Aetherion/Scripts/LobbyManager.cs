using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void StartBtnClick()
    {
        // ���� ��ư ������ ����
        PlayerPrefs.SetString("ClickBtn", "Start");

        // �� �ε�
        SceneManager.LoadScene("MainScene");
    }
    public void TestBtnClick()
    {
        // ���� ��ư ������ ����   
        PlayerPrefs.SetString("ClickBtn", "Test");

        // �� �ε�
        SceneManager.LoadScene("MainScene");
    }
}
