using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour
{
    [Header("Stats")]
    public new string name;
    public float value;
    [SerializeField] private float baseDurability;

    [SerializeField] private Light2D spriteLight;

    [Header("Prefab References")]
    [SerializeField] private ParticleSystem splashPrefabs;
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private PopText damageTextPrefab;

    private float _currentDurability;
    public float DurabilityPercentage => (int)(_currentDurability / baseDurability * 100f);

    private DistanceJoint2D _distanceJoint;
    private Rigidbody2D _rigidbody;

    #region Unity Events

    private void Awake()
    {
        _distanceJoint = GetComponent<DistanceJoint2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spriteLight.enabled = false;
        _currentDurability = baseDurability;

        SetHighlight(false);
        SetJointBody(null);
    }

    #endregion

    public void SetHighlight(bool value)
    {
        spriteLight.enabled = value;
    }

    public void SetJointBody(Rigidbody2D body)
    {
        _distanceJoint.connectedBody = body;
        _distanceJoint.enabled = body != null;
    }

    private void TakeDamage(float damage, Vector2 contactPoint)
    {
        _currentDurability -= damage;

        if (damage > 10f)
        {
            // Play a splash effect at contact point 
            Instantiate(splashPrefabs, contactPoint, Quaternion.identity).transform.up = _rigidbody.velocity.normalized;
            // Spawn a little text that tells the damage number
            Instantiate(damageTextPrefab, contactPoint, Quaternion.identity).SetText(((int)damage).ToString());
        }

        // Item explode after taking too much damage
        if (_currentDurability <= 0f) Explode();
    }

    private void Explode()
    {
        // Play some effects before destroying
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Instantiate(damageTextPrefab, transform.position, Quaternion.identity).SetText($"{name} destroyed");
        GameController.Instance.PlaySlowMotionEffect();
        CameraShaker.Instance.Shake(CameraShakeMode.Normal);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Magnet")) return;

        var magnitude = _rigidbody.velocity.magnitude;
        var contactPoint = other.GetContact(0).point;
        var damage = magnitude * 10f;

        // Deal damage to item
        TakeDamage(damage, contactPoint);

        // Shake the camera based on how hard the item hit
        float intensity, duration;
        intensity = duration = magnitude / 60f;
        CameraShaker.Instance.Shake(duration, intensity, 2f);
    }
}
