using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject collectableItemEffect;
    public GameObject enemyDieEffect;
    public Image[] imageLifeCherrys;
    public TextMeshProUGUI playerScoreText;
    public int maxPlayerLife = 3;
    public int playerLife;
    public int playerScore;
    public Animator fadeAnimator;
    public SceneTypes currentScene;
    public PlayerController playerController;

    void Start()
    {
        if (instance == null)
            instance = this;

        playerLife = maxPlayerLife;
    }

    public void DecrementPlayerLife(int life = 1)
    {
        playerLife -= life;

        if (playerLife < 1)
        {
            playerController.Die();
            SceneLoad(currentScene);

            imageLifeCherrys[0].color = Color.gray;
            imageLifeCherrys[1].color = Color.gray;
            imageLifeCherrys[2].color = Color.gray;

            return;
        }

        imageLifeCherrys[playerLife].color = Color.gray;
    }

    public void IncrementPlayerLife()
    {
        if (playerLife < maxPlayerLife)
        {
            playerLife++;
        }
        imageLifeCherrys[playerLife - 1].color = Color.white;
    }

    public void IncrementPlayerScore()
    {
        playerScore++;
        playerScoreText.text = playerScore.ToString("D3");
    }


    #region SCENE LOADER
    public void SceneLoad(SceneTypes sceneToLoad)
    {
        StartCoroutine(LoadScene(sceneToLoad));
    }


    IEnumerator LoadScene(SceneTypes sceneToLoad)
    {
        yield return null;
        fadeAnimator.SetTrigger("fade");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad.ToString());
    }

    public enum SceneTypes
    {
        Level_1,
        Level_2,
    }
    #endregion
}
