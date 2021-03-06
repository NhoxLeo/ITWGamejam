﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UiStageController : MonoBehaviour
{
    [SerializeField]
    private Text remainTimeText;
    [SerializeField]
    private NoticeBox noticeBox;
    [SerializeField]
    private InGameOption option;

    private void Awake()
    {
        StageController.Instance.OnStageEnd += OnStageEnd;

        if(BgmManager.Instance.Initialized == false)
        {
            BgmManager.Instance.LoadClips();
        }

        noticeBox.gameObject.SetActive(true);
        noticeBox.InitNoticeBox();
    }

    private void Start()
    {
        remainTimeText.text = ((int)StageController.Instance.RemainElapsedTime).ToString();
    }

    public void ShowReadyStart()
    {
        StartCoroutine(ReadyStartProcess());
    }

    private IEnumerator ReadyStartProcess()
    {
        yield return noticeBox.ShowNoticeBox(NoticeType.ReadyAndFight);
        // yield break;
    }

    private bool isSceneChanging = false;
    private void Update()
    {
        if(StageController.Instance.IsStageProcessing == true)
        {
            remainTimeText.text = ((int)StageController.Instance.RemainElapsedTime).ToString();

            if (Input.GetKeyDown(KeyCode.Escape) == true || Input.GetButtonDown("Back") == true)
            {
                if (option.IsShowing == false)
                {
                    option.Show();
                }
                else
                {
                    option.Hide();
                }
            }
        }

        if(isSceneChanging == false)
        {
            if(Input.GetKeyDown(KeyCode.Return) == true || Input.GetButtonDown("Back") == true)
            {
                SceneUtil.LoadScene("MainMenu");
            }
        }
    }

    private void OnStageEnd(int winTeamNumber)
    {
        BgmManager.Instance.Play(BgmType.GameOver);
        noticeBox.gameObject.SetActive(true);

        Debug.Log("WinTeamNumber : " + winTeamNumber);
        // TODO : 아래 if 구문 noticeBox 타입 다르게 처리 필요
        if(winTeamNumber == -1)
        {
            Debug.Log("StageEnd : Draw");
            // Draw
        }
        else if(TeamController.GetTeamByTeamNumber(winTeamNumber).IsCpuTeam() == true)
        {
            Debug.Log("StageEnd : Lose");
            // Lose
        }
        else
        {
            Debug.Log("StageEnd : Win");
            // Team + winTeamNumber Win
        }

        StartCoroutine(noticeBox.ShowNoticeBox(NoticeType.Victory));
    }
}