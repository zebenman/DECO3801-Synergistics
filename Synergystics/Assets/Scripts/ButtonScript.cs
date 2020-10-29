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
        string currentScene = SceneManager.GetActiveScene().name;
        if (GameController.Instance != null && GameController.Instance.StoryManager.PeekNextThread() == null && scene_name.Equals(SceneInformation.INTERMISSION_SCREEN))
        {
            // Go to the final summary screen if we have no more story to do
            scene_name = SceneInformation.FINAL_SUMMARY;
        }

        SceneManager.LoadSceneAsync(scene_name).completed += (a) =>
        {
            if(GameController.Instance != null)
            {
                GameController.Instance.OnSceneTransition(scene_name, currentScene);
            }
        };
    }
}
