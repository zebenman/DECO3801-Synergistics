using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreYouSureScript : MonoBehaviour
{
    public Button ConfirmYes;
    public Button ConfirmNo;

    public void Setup(UnityEngine.Events.UnityAction confirmYes, UnityEngine.Events.UnityAction confirmNo)
    {
        ConfirmYes.onClick.AddListener(confirmYes);
        ConfirmNo.onClick.AddListener(confirmNo);
    }
}
