using UnityEngine;
using System.Collections;

public class Transition
{
    public enum FinishType
    {
        None,
        Destroy,
        Deactivate,
        Activate
    }

    public enum LerpType
    {
        Smooth,
        Accelerate
    }

    public static float SmoothLerp(float t)
    {
        //smoothstep
        return t * t * (3f - 2f * t);
        //smootherstep (https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/)
        // return t * t * t * (t * (6f * t - 15f) + 10f);
    }

    public static float AccelerateLerp(float t)
    {
        return t * t;
    }

    public static float Lerp(float t, LerpType lerp)
    {
        switch (lerp)
        {
            case LerpType.Accelerate:
                return AccelerateLerp(t);
            case LerpType.Smooth:
                return SmoothLerp(t);
            default:
                return t;
        }
    }

    public static IEnumerator Translate(Transform t, Vector3 destination, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = t.position;

        do
        {
            t.position = Vector3.Lerp(startPosition, destination, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            if (t.gameObject == null) yield break;
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destination;
    }

    public static IEnumerator StandingWave(Transform t, Vector3 axis, float amplitude, int periods, float duration)
    {
        float omega = 2 * Mathf.PI * periods / duration;
        Vector3 zero = t.position;
        float elapsed = 0f;

        do
        {
            t.position = zero + amplitude * axis * Mathf.Sin(omega * elapsed);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = zero;
    }

    public static IEnumerator Resize(Transform t, Vector3 destScale, float duration, FinishType finish=FinishType.None)
    {
        float elapsed = 0f;
        Vector3 startScale = t.localScale;

        do
        {
            t.localScale = Vector3.Lerp(startScale, destScale, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            if (t.gameObject == null) yield break;
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.localScale = destScale;

        FinishAction(new GameObject[] { t.gameObject }, finish);
    }

    public static IEnumerator TranslateRescale(GameObject obj, Vector3 destPos, Vector2 destScale, float duration, LerpType lerp)
    {
        float elapsed = 0f;
        float p;

        Transform t = obj.transform;
        Vector3 startScale = t.localScale;

        Vector3 startPos = new Vector3(t.position.x, t.position.y, destPos.z);

        do
        {
            p = Lerp(elapsed / duration, lerp);
            t.position = Vector3.Lerp(startPos, destPos, p);
            t.localScale = Vector3.Lerp(startScale, destScale, p);
            yield return new WaitForEndOfFrame();
            if (t.gameObject == null) yield break;
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destPos;
        t.localScale = destScale;
    }

    public static IEnumerator TranslateResize(GameObject obj, Vector3 destPos, Vector2 destSize, float duration, LerpType lerp)
    {
        //float elapsed = 0f;
        // float p;

        Transform t = obj.transform;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Vector2 startSize = sr.bounds.size;
        Vector3 startScale = t.localScale;
        Vector3 destScale = new Vector3(startScale.x * (destSize.x / startSize.x),
            startScale.y * (destSize.y / startSize.y), t.localScale.z);
        yield return TranslateRescale(obj, destPos, destScale, duration, lerp);

        /*Vector3 startPos = new Vector3(t.position.x, t.position.y, destPos.z);

        do
        {
            p = Lerp(elapsed / duration, lerp);
            t.position = Vector3.Lerp(startPos, destPos, p);
            t.localScale = Vector3.Lerp(startScale, destScale, p);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destPos;
        t.localScale = destScale;*/
    }

    public static IEnumerator TranslateResize(GameObject obj, Vector3 destPos, Vector2 destSize, float duration)
    {
        yield return TranslateResize(obj, destPos, destSize, duration, LerpType.Smooth);
    }

    public static IEnumerator Rotate(Transform t, float duration, float initial, float final)
    {
        float elapsed = 0f;
        float rotation;

        do
        {
            rotation = initial + (final - initial) * SmoothLerp(elapsed / duration);
            t.rotation = Quaternion.identity;
            t.Rotate(Vector3.forward, rotation);
            yield return new WaitForEndOfFrame();
            if (t.gameObject == null) yield break;
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.rotation = Quaternion.identity;
        t.Rotate(Vector3.forward, final);
    }

    public static IEnumerator TranslateMovingTarget(Transform t, Vector3 destination, Transform target, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = t.position;

        Vector3 initialTarget = target.position;

        do
        {
            Vector3 currentTarget = target.position;
            Vector3 dest = destination + (currentTarget - initialTarget);

            t.position = Vector3.Lerp(startPosition, dest, SmoothLerp(elapsed / duration));
            yield return new WaitForEndOfFrame();
            if (t.gameObject == null) yield break;
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        t.position = destination;
    }

    public static void SetColor(Color c, GameObject root, GameObject exclude, bool enableSr)
    {
        foreach (SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>())
        {
            if (exclude != null && sr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            sr.color = c;
            if (enableSr)
            {
                sr.enabled = true;
            }
        }
        foreach (CanvasRenderer cr in root.GetComponentsInChildren<CanvasRenderer>())
        {
            if (exclude != null && cr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            cr.SetColor(c);
        }
    }

    public static void SetRGB(Color c, GameObject root, GameObject exclude)
    {
        foreach (SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>())
        {
            if (exclude != null && sr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            sr.color = new Color(c.r, c.g, c.b, sr.color.a);
        }
        foreach (CanvasRenderer cr in root.GetComponentsInChildren<CanvasRenderer>())
        {
            if (exclude != null && cr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            float a = cr.GetAlpha();
            cr.SetColor(c);
            cr.SetAlpha(a);
        }
    }

    public static void SetAlpha(float alpha, GameObject root, GameObject exclude=null, bool enableSr=false)
    {
        if (root == null)
        {
            return;
        }
        foreach (SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>())
        {
            if (exclude != null && sr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            if (enableSr)
            {
                sr.enabled = true;
            }
        }
        foreach (CanvasRenderer cr in root.GetComponentsInChildren<CanvasRenderer>())
        {
            if (exclude != null && cr.gameObject.transform.IsChildOf(exclude.transform))
            {
                continue;
            }
            cr.SetColor(new Color(cr.GetColor().r, cr.GetColor().g, cr.GetColor().b, alpha));
            //cr.SetAlpha(alpha);
        }
    }

    public static void SetColor(Color c, GameObject root, GameObject exclude)
    {
        SetColor(c, root, exclude, false);
    }

    public static IEnumerator TransitionColor(GameObject root, GameObject exclude, float duration, 
        Color startColor, Color destColor, bool enableSr)
    {
        if (root == null)
        {
            yield return null;
        }

        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            Color c = Color.Lerp(startColor, destColor, t);
            SetColor(c, root, exclude, enableSr);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        SetColor(destColor, root, exclude, enableSr);

        yield return null;
    }

    public static IEnumerator FinishAction(GameObject[] objs, FinishType finish)
    {
        switch (finish)
        {
            case FinishType.None:
                yield return null;
                break;
            case FinishType.Activate:
                foreach (GameObject obj in objs)
                {
                    obj.SetActive(true);
                }
                yield return null;
                break;
            case FinishType.Deactivate:
                foreach (GameObject obj in objs)
                {
                    obj.SetActive(false);
                }
                yield return null;
                break;
            case FinishType.Destroy:
                foreach (GameObject obj in objs)
                {
                    GameObject.Destroy(obj);
                }
                yield return null;
                break;
            default:
                yield return null;
                break;
        }
    }

    public static IEnumerator TransitionColor(GameObject[] objs, float duration, FinishType finish, Color initial, Color final)
    {
        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            Color c = Color.Lerp(initial, final, t);
            foreach (GameObject obj in objs)
            {
                SetColor(c, obj, null);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        foreach (GameObject obj in objs)
        {
            SetColor(final, obj, null);
        }

        yield return FinishAction(objs, finish);

        yield return null;
    }

    public static IEnumerator TransitionColor(GameObject obj, float duration,
        Color startColor, Color destColor, bool enableSr)
    {
        yield return TransitionColor(obj, null, duration, startColor, destColor, enableSr);
    }

    private static IEnumerator TransitionColorRGB(GameObject root, GameObject exclude, float duration, Color initial, Color final)
    {
        if (root == null)
        {
            yield return null;
        }

        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            Color cP = Color.Lerp(initial, final, t);
            SetRGB(cP, root, exclude);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        SetRGB(final, root, exclude);

        yield return null;
    }

    public static IEnumerator TransitionColor(GameObject obj, float duration, Color startColor, Color destColor)
    {
        yield return TransitionColor(obj, null, duration, startColor, destColor, false);
    }

    public static IEnumerator TransitionAlpha(GameObject[] objs, float duration, float initial, float final, 
        FinishType finish=FinishType.None, bool enableSr=false, GameObject exclude=null)
    {
        float t;
        float elapsed = 0f;

        while (elapsed <= duration)
        {
            t = SmoothLerp(elapsed / duration);
            foreach (GameObject obj in objs)
            {
                SetAlpha(initial + t * (final - initial), obj, exclude: exclude, enableSr: enableSr);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        }
        foreach (GameObject obj in objs)
        {
            SetAlpha(final, obj);
        }

        yield return FinishAction(objs, finish);

        yield return null;
    }

    public static IEnumerator TransitionAlpha(GameObject obj, float duration, float initial, float final,
    FinishType finish = FinishType.None, bool enableSr = false, GameObject exclude = null)
    {
        yield return TransitionAlpha(new GameObject[] { obj }, duration, initial, final, finish, enableSr, exclude);
    }

    public static IEnumerator TransitionBrightness(GameObject root, GameObject exclude, float duration, float initial, float final)
    {
        Color startColor = new Color(initial, initial, initial);
        Color finalColor = new Color(final, final, final);

        yield return TransitionColorRGB(root, exclude, duration, startColor, finalColor);
    }

    public static IEnumerator FadeIn(GameObject obj, float duration, bool enableSr=false, GameObject exclude=null)
    {
        yield return TransitionAlpha(obj, duration, 0f, 1f, enableSr: enableSr, exclude: exclude);
    }

    public static IEnumerator FadeOut(GameObject obj, float duration,
        FinishType finish = FinishType.None, bool enableSr = false, GameObject exclude = null)
    {
        yield return FadeOut(new GameObject[] { obj }, duration, finish, enableSr, exclude);
    }

    public static IEnumerator FadeOut(GameObject[] objs, float duration,
        FinishType finish = FinishType.None, bool enableSr = false, GameObject exclude = null)
    {
        yield return TransitionAlpha(objs, duration, 1f, 0f, finish);
    }
}
