using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class Menu : MonoBehaviour
{
    public SceneTypes currentScene;

    void Update()
    {
        if (Input.anyKeyDown == true)
        {
            GameManager.instance.SceneLoad(currentScene);
        }
    }
}
