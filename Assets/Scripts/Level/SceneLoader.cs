using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField]
    private SceneTypes sceneToLoad;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadSceneAsync(sceneToLoad.ToString());
        }
    }

    enum SceneTypes
    {
        Level_1,
        Level_2,
    }
}
