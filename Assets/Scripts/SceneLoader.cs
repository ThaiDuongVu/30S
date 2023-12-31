using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Singleton

    private static SceneLoader _sceneLoaderInstance;

    public static SceneLoader Instance
    {
        get
        {
            if (_sceneLoaderInstance == null) _sceneLoaderInstance = FindObjectOfType<SceneLoader>();
            return _sceneLoaderInstance;
        }
    }

    #endregion

    private Animator _cameraAnimator;
    private static readonly int OutroTrigger = Animator.StringToHash("outro");
    [SerializeField] private AnimationClip cameraOutroAnimationClip;

    private string _sceneToLoad = "";

    #region Unity Event

    private void Awake()
    {
        if (Camera.main is { }) _cameraAnimator = Camera.main.GetComponent<Animator>();
    }

    #endregion

    private IEnumerator Load()
    {
        // Load scene in background but don't allow transition
        var asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        // Play camera animation
        _cameraAnimator.SetTrigger(OutroTrigger);

        // Wait for camera animation to complete
        yield return new WaitForSecondsRealtime(cameraOutroAnimationClip.averageDuration);

        // Allow transition to new scene
        asyncOperation.allowSceneActivation = true;
    }

    public void Load(string scene)
    {
        _sceneToLoad = scene;
        StartCoroutine(Load());
    }

    public void LoadNextLevel()
    {
        var levelIndex = Convert.ToInt32(SceneManager.GetActiveScene().name.Substring(5, 2));
        Load($"Level{(levelIndex < 9 ? "0" : "")}{levelIndex + 1}");
    }

    public void Restart()
    {
        // Reload current active scene
        Load(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}