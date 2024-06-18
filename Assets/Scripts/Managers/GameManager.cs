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
    public int playerLife = 2;
    public int playerScore = 0;
    public int maxPlayerScore = 7;
    public Animator fadeAnimator;
    public SceneTypes currentScene;
    public SceneTypes nextScene;
    public PlayerController playerController;

    void Start()
    {
        if (instance == null)
            instance = this;

        InitUI();
    }

    private void InitUI()
    {
        //reset player life
        imageLifeCherrys[0].color = Color.gray;
        imageLifeCherrys[1].color = Color.gray;
        imageLifeCherrys[2].color = Color.gray;
        for (int i = 0; i < playerLife; i++)
        {
            imageLifeCherrys[i].color = Color.white;
        }

        playerScoreText.text = playerScore.ToString() + "/" + maxPlayerScore.ToString();
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
        playerScoreText.text = playerScore.ToString() + "/" + maxPlayerScore.ToString();

        if (playerScore >= maxPlayerScore)
        {
            SceneLoad(nextScene);
        }
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
        Menu,
        Level_1,
        Level_2,
        Final
    }
    #endregion
}
