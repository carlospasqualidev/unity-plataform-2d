using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class SceneLoader : MonoBehaviour
{
    public SceneTypes sceneToLoad;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.SceneLoad(sceneToLoad);
        }
    }  
}
