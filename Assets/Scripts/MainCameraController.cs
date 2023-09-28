using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    #region Singleton

    private static MainCameraController _mainCameraControllerInstance;

    public static MainCameraController Instance
    {
        get
        {
            if (_mainCameraControllerInstance == null) _mainCameraControllerInstance = FindObjectOfType<MainCameraController>();
            return _mainCameraControllerInstance;
        }
    }

    #endregion

    private Animator _animator;
    private static readonly int IntroAnimationTrigger = Animator.StringToHash("intro");
    private static readonly int OutroAnimationTrigger = Animator.StringToHash("outro");

    #region Unity Events

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    #endregion

    #region Intro & Outro

    public void Intro()
    {
        _animator.SetTrigger(IntroAnimationTrigger);
    }

    public void Outro()
    {
        _animator.SetTrigger(OutroAnimationTrigger);
    }

    #endregion
}
