using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Game,
}

public class SceneLoader : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;

    public void LoadScene(Scene scene)
    {
        StartCoroutine(LoadSceneAsynchronously(scene));
    }

    private IEnumerator LoadSceneAsynchronously(Scene scene)
    {
        var sceneToLoad = SceneManager.LoadSceneAsync(scene.ToString());

        loadingCanvas.SetActive(true);

        while(!sceneToLoad.isDone)
        {
            float progress = Mathf.Clamp01(sceneToLoad.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100.0f) + "%";
            yield return null;
        }
    }
}
