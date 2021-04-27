using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnButtonPress : MonoBehaviour
{
    public string sceneName;
    public string buttonName;

    void Update()
    {
        if(Input.GetButtonDown(buttonName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
