using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField]
    private SceneTypes sceneToLoad;

    [SerializeField]
    private Animator sceneFaderAnimator;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        yield return null;
        sceneFaderAnimator.SetTrigger("fade");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad.ToString());
    }

    enum SceneTypes
    {
        Level_1,
        Level_2,
    }
}
