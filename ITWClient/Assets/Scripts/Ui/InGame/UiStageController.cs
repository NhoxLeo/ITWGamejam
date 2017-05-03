﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiStageController : MonoBehaviour
{
    [SerializeField]
    private Text remainTimeText;
    [SerializeField]
    private NoticeBox noticeBox;

    private StageController stageController;
    private void Awake()
    {
        stageController = GameObject.FindObjectOfType<StageController>();
        stageController.OnStageStart += OnStageStart;
        stageController.OnStageEnd += OnStageEnd;

        noticeBox.gameObject.SetActive(true);
        noticeBox.InitNoticeBox();
    }

    private void Start()
    {
        remainTimeText.text = ((int)stageController.RemainElapsedTime).ToString();
    }

    private void OnStageStart()
    {
        StartCoroutine(ReadyStartProcess());
    }

    private IEnumerator ReadyStartProcess()
    {
        yield return noticeBox.ShowNoticeBox(NoticeType.ReadyAndFight);
        // yield break;
    }

    private void Update()
    {
        if(stageController.IsStageStarted == true)
        {
            remainTimeText.text = ((int)stageController.RemainElapsedTime).ToString();
        }
    }

    private void OnStageEnd()
    {

    }
}