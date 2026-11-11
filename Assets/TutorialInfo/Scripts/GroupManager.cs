using UnityEngine;

public class GroupManager : MonoBehaviour
{
    public GameObject titleGroup;
    public GameObject lobbyGroup;
    public GameObject playGroup;

    void Start()
    {
        ShowTitle();
    }

    void Update()
    {
        if (InputManager.instance.TapDown)
        {
            // title -> lobby
            if (titleGroup.activeSelf)
            {
                ShowLobby();
            }
            // lobby -> play
            else if (lobbyGroup.activeSelf)
            {
                // ShowPlay() 실행은 GameStart button inspector 에서 설정 
                // ShowPlay();
            }
        }
    }

    public void ShowTitle()
    {
        titleGroup.SetActive(true);
        lobbyGroup.SetActive(false);
        playGroup.SetActive(false);

        BGMManager.instance.PlayMainBGM(0);
    }

    public void ShowLobby()
    {
        titleGroup.SetActive(false);
        lobbyGroup.SetActive(true);
        playGroup.SetActive(false);

        BGMManager.instance.PlayMainBGM(1);
    }

    public void ShowPlay()
    {
        titleGroup.SetActive(false);
        lobbyGroup.SetActive(false);
        playGroup.SetActive(true);

        // 일단 bgm이 끊어지지 않도록 주석처리
        // BGMManager.instance.PlayMainBGM(1);
    }
}
