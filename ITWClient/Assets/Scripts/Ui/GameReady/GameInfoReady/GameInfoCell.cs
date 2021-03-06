﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoCell : MonoBehaviour {
    private GameObject selectedObj;

    void Awake()
    {
        selectedObj = transform.Find("Select").gameObject;
    }

    public void SelectCell(bool select) {
        if (selectedObj != null)
        {
            selectedObj.SetActive(select);
        }
    }
}
