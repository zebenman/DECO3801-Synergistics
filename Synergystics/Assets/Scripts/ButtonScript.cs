using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    // Load a given scene
    public void Btn_change_scene(string scene_name)
    {       
        SceneManager.LoadSceneAsync(scene_name).completed += (a) =>
        {                        
            GameController.Instance.OnSceneTransition(scene_name);
        };
    }
}
