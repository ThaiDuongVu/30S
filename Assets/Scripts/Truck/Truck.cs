using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private PopText popTextPrefab;
    [SerializeField] private Color moneyColor;
    [SerializeField] private Color negativeMoneyColor;

    private List<Item> _items = new();

    public int ItemsCount => _items.Count;
    public float TotalValue
    {
        get
        {
            var totalValue = 0f;
            foreach (var item in _items) totalValue += item.value;
            return totalValue;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<Item>();
            _items.Add(item);

            var text = Instantiate(popTextPrefab, item.transform.position, Quaternion.identity);
            text.SetText($"${item.value}");
            text.SetColor(moneyColor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // https://stackoverflow.com/questions/36577337/some-objects-were-not-cleaned-up-when-closing-the-scene
        if (!gameObject.scene.isLoaded) return;

        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<Item>();
            if (_items.Contains(item)) _items.Remove(item);

            var text = Instantiate(popTextPrefab, item.transform.position, Quaternion.identity);
            text.SetText($"-${item.value}");
            text.SetColor(negativeMoneyColor);
        }
    }
}
