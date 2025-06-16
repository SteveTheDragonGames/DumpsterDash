using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float duration = 1f;

    private float timer = 0f;
    private TextMeshPro text;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    public void SetScore(int amount)
    {
        if (text != null)
            text.text = "+" + amount.ToString();
        else
            UnityEngine.Debug.LogWarning("⚠️ FloatingScore: text is null!");
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= duration)
            Destroy(gameObject);
    }
}
