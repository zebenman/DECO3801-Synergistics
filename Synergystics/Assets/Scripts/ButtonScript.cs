using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    //public Button yourButton;

    // Start is called before the first frame update
    //void Start()
    //{
        //Button btn = yourButton.GetComponent<Button>();
        //btn.onClick.AddListener(TaskOnClick);

    //}

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    //void TaskOnClick()
    //{
    //    Debug.Log("You have clicked the button!");
    //}

    // Load a given scene
    public void Btn_change_scene(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }


}
