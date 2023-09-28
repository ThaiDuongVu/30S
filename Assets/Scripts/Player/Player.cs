using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Magnet magnet;

    public void SetFlipped(bool value)
    {
        sprite.flipX = value;
    }
}
