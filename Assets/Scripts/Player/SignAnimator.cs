using UnityEngine;
using System.Collections;
using TMPro;

public class SignAnimator : MonoBehaviour
{
    public Transform signTransform;
    public TMP_Text junkNameText, junkDescriptionText;
    [SerializeField] private Transform playerTransform;

    private Coroutine currentRoutine;

    private enum SignState { Hidden, Rising, Visible, Dropping, Swapping }
    private SignState currentState = SignState.Hidden;

    private float visibleDuration = 2f;
    private float lastPopTime = -999f;

    private Vector3 finalOffset = new Vector3(-4f, 1f, 0f);
    private Vector3 startOffset = new Vector3(-4f, -7f, 0f);

    void Awake()
    {
        if (playerTransform == null)
            Debug.LogWarning("SignAnimator: playerTransform not assigned!");
    }

    public void PopSign(string junkName, string junkDescription)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = (currentState == SignState.Visible || currentState == SignState.Rising)
            ? StartCoroutine(SwapSign(junkName, junkDescription))
            : StartCoroutine(ShowSign(junkName, junkDescription));
    }


    IEnumerator ShowSign(string junkName, string junkDescription)
    {
        SetState(SignState.Rising);

        SetTextAlpha(junkNameText, 0f);
        SetTextAlpha(junkDescriptionText, 0f);
        junkNameText.text = junkName;
        junkDescriptionText.text = junkDescription;

        Vector3 startPos = playerTransform.position + startOffset;
        Vector3 endPos = playerTransform.position + finalOffset;

        signTransform.position = startPos;
        yield return AnimateMove(startPos, endPos, 0.3f);

        signTransform.localScale = Vector3.one * 0.6f;
        yield return AnimateScale(signTransform.localScale, Vector3.one * 1.1f, 0.2f);
        yield return AnimateScale(Vector3.one * 1.1f, Vector3.one, 0.1f);
        yield return AnimateFade(0f, 1f, 0.3f);

        SetState(SignState.Visible);
        lastPopTime = Time.time;

        while (Time.time - lastPopTime < visibleDuration)
            yield return null;

        StartHidingSign();
    }

    IEnumerator SwapSign(string newName, string newDescription)
    {
        currentState = SignState.Swapping;

        yield return AnimateFade(1f, 0f, 0.2f);

        Vector3 dropTarget = playerTransform.position + startOffset;
        yield return AnimateMove(signTransform.position, dropTarget, 0.3f);

        junkNameText.text = newName;
        junkDescriptionText.text = newDescription;
        SetTextAlpha(junkNameText, 0f);
        SetTextAlpha(junkDescriptionText, 0f);

        Vector3 riseTarget = playerTransform.position + finalOffset;
        yield return AnimateMove(dropTarget, riseTarget, 0.3f);

        signTransform.localScale = Vector3.one * 0.6f;
        yield return AnimateScale(signTransform.localScale, Vector3.one * 1.1f, 0.2f);
        yield return AnimateScale(Vector3.one * 1.1f, Vector3.one, 0.1f);
        yield return AnimateFade(0f, 1f, 0.3f);

        SetState(SignState.Visible);
        lastPopTime = Time.time;

        while (Time.time - lastPopTime < visibleDuration)
            yield return null;

        StartHidingSign();
    }
    void StartHidingSign()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(HideSign());
    }
   
    IEnumerator HideSign()
    {
        if (currentState == SignState.Hidden) yield break;

        SetState(SignState.Dropping);

        yield return AnimateFade(1f, 0f, 0.3f);

        Vector3 end = playerTransform.position + startOffset;
        yield return AnimateMove(signTransform.position, end, 0.3f);

        // 🔒 Safety final state lock-in
        signTransform.position = end;
        signTransform.localScale = Vector3.one;
        SetTextAlpha(junkNameText, 0f);
        SetTextAlpha(junkDescriptionText, 0f);

        SetState(SignState.Hidden);
        currentRoutine = null;
    }

    IEnumerator AnimateMove(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = Mathf.Sin((t / duration) * Mathf.PI * 0.5f);
            signTransform.position = Vector3.Lerp(from, to, eased);
            yield return null;
        }
    }

    IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = Mathf.Sin((t / duration) * Mathf.PI * 0.5f);
            signTransform.localScale = Vector3.Lerp(from, to, eased);
            yield return null;
        }
    }

    IEnumerator AnimateFade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetTextAlpha(junkNameText, alpha);
            SetTextAlpha(junkDescriptionText, alpha);
            yield return null;
        }
    }

    void SetTextAlpha(TMP_Text text, float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }

    public void ForceHide()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        signTransform.position = playerTransform.position + startOffset;
        signTransform.localScale = Vector3.one;
        SetTextAlpha(junkNameText, 0f);
        SetTextAlpha(junkDescriptionText, 0f);
        SetState(SignState.Hidden);
        currentRoutine = null;
    }

    void SetState(SignState newState)
    {
        currentState = newState;
        // future debug logs, hooks, transitions go here
    }
}
