using System.Collections;
using UnityEngine;
using TMPro;

public class PopText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    #region Unity Events

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    #endregion

    public void SetText(string message)
    {
        text.SetText(message);
    }

    public void SetColor(Color color)
    {
        text.color = color;
    }
}
