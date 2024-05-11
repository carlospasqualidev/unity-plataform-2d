using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKeyDown == true)
        {
            GameManager.instance.SceneLoad(GameManager.SceneTypes.Level_1);
        }     
    }
}
