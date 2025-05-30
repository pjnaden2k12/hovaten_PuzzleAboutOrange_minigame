using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public GameObject transitionPanel;
    public Image blackCircle;
    public float transitionTime = 0.5f;

    public void StartTransition(System.Action midAction)
    {
        StartCoroutine(DoTransition(midAction));
    }

    private IEnumerator DoTransition(System.Action midAction)
    {
        transitionPanel.SetActive(true);
        yield return StartCoroutine(ScaleCircle(0f, 10f));
        midAction?.Invoke();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(ScaleCircle(10f, 0f));
        transitionPanel.SetActive(false);
    }

    private IEnumerator ScaleCircle(float from, float to)
    {
        float t = 0f;
        Vector3 start = Vector3.one * from;
        Vector3 end = Vector3.one * to;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionTime;
            blackCircle.transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        blackCircle.transform.localScale = end;
    }
}

