using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Magnet : MonoBehaviour
{
    [Header("General References")]
    [SerializeField] private Rigidbody2D attachedTarget;
    [SerializeField] private LineRenderer chain;

    [Header("Force Field References")]
    [SerializeField] private Transform redPoint;
    [SerializeField] private Transform bluePoint;
    [SerializeField] private LineRenderer redForceField;
    [SerializeField] private LineRenderer blueForceField;

    [Header("Prefab References")]
    [SerializeField] private PopText popTextPrefab;

    private Item _highlightedItem;
    private Item _currentItem;

    private DistanceJoint2D _distanceJoint;
    private Rigidbody2D _rigidbody;

    private InputManager _inputManager;

    #region Unity Events

    private void OnEnable()
    {
        _inputManager = new InputManager();

        // Handle player fire input
        _inputManager.Player.Fire.performed += FireOnPerformed;
        _inputManager.Player.Fire.canceled += FireOnCanceled;

        _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager.Disable();
    }

    private void Awake()
    {
        _distanceJoint = GetComponent<DistanceJoint2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _distanceJoint.connectedBody = attachedTarget.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        chain.SetPosition(0, attachedTarget.position);
        chain.SetPosition(1, transform.position);

        redForceField.SetPosition(0, redPoint.position);
        redForceField.SetPosition(1, _currentItem ? _currentItem.transform.position : redPoint.position);

        blueForceField.SetPosition(0, bluePoint.position);
        blueForceField.SetPosition(1, _currentItem ? _currentItem.transform.position : bluePoint.position);
    }

    #endregion

    #region Input Handlers

    private void FireOnPerformed(InputAction.CallbackContext context)
    {
        if (GameController.Instance.State != GameState.InProgress) return;
        GrabItem(_highlightedItem);
    }

    private void FireOnCanceled(InputAction.CallbackContext context)
    {
        if (GameController.Instance.State != GameState.InProgress) return;
        ReleaseCurrentItem();
    }

    #endregion

    private void HighlightItem(Item item)
    {
        if (!item) return;

        _highlightedItem = item;
        _highlightedItem.SetHighlight(true);
    }

    private void UnhighlightCurrentItem()
    {
        if (!_highlightedItem) return;

        _highlightedItem.SetHighlight(false);
        _highlightedItem = null;
    }

    #region Grab & Release Methods

    public void GrabItem(Item item)
    {
        if (!item) return;

        _currentItem = item;
        _currentItem.SetJointBody(_rigidbody);

        Instantiate(popTextPrefab, transform.position, Quaternion.identity).SetText($"Grabbed {_currentItem.name}");
    }

    public void ReleaseCurrentItem()
    {
        if (!_currentItem) return;

        Instantiate(popTextPrefab, transform.position, Quaternion.identity).SetText($"Released {_currentItem.name}");

        UnhighlightCurrentItem();
        _currentItem.SetJointBody(null);
        _currentItem = null;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        var item = other.GetComponentInParent<Item>();
        if (item) HighlightItem(item);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var item = other.GetComponentInParent<Item>();
        if (item && item != _currentItem) UnhighlightCurrentItem();
    }
}
