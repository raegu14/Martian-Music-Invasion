using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineController : MonoBehaviour
{

    public GameObject Supergirl;
    public GameObject SupergirlArm;
    public GameObject SupergirlVineCurled;

    public GameObject LevelController;

    private AddressingController addressingController;

    private Vector3 sgVelocity;

    // Use this for initialization
    void Start()
    {
        addressingController = LevelController.GetComponent<AddressingController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHangingVine(AddressingStep step)
    {
        Vector3 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector3 notePos = new Vector3();
        if (step != null)
        {
            notePos = step.FirstNoteCollider.bounds.center;
        }
        PlaceVineBetween(gameObject, handPos + 0.5f * Vector3.back, notePos + 0.5f * Vector3.back);

        float deltaX = notePos.x - handPos.x;
        float deltaY = notePos.y - handPos.y;

        float mag2 = Vector2.SqrMagnitude(notePos - handPos);
        sgVelocity += addressingController.sgGrav * (new Vector3(deltaX * deltaY, -deltaX * deltaX) / mag2);

        Vector3 direction = Vector3.Normalize(new Vector3(-deltaY, deltaX, 0f));
        sgVelocity = direction * Vector3.Dot(direction, sgVelocity);

        sgVelocity *= addressingController.sgFriction;

        Supergirl.transform.position += sgVelocity;
        handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector3 delta = notePos + Mathf.Sqrt(mag2) * (handPos - notePos).normalized - handPos;
        Supergirl.transform.position += delta;
    }

    float vineLength;

    public void InitializeVineLength(AddressingStep CurrentStep)
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = CurrentStep.FirstNoteCollider.bounds.center;
        vineLength = Vector3.Magnitude(handPos - notePos);
    }

    public void NormalizeVineLength(AddressingStep CurrentStep, float sgSpring, float firstNoteHeight)
    {
        Vector2 handPos = Supergirl.GetComponent<CircleCollider2D>().bounds.center;
        Vector2 notePos = new Vector2();
        if (CurrentStep != null)
        {
            notePos = CurrentStep.FirstNoteCollider.bounds.center;
        }

        float vineGoal = vineLength + (notePos.y - firstNoteHeight);
        Vector2 desiredPos = notePos + vineGoal * (handPos - notePos).normalized;
        Vector3 delta = desiredPos - handPos;

        if (delta.magnitude > sgSpring)
        {
            delta *= (sgSpring / delta.magnitude);
        }

        Supergirl.transform.position += delta;
    }

    private void PlaceVineBetween(GameObject vine, Vector3 start, Vector3 end)
    {
        Transform vt = vine.transform;
        SpriteRenderer vsr = vine.GetComponent<SpriteRenderer>();

        vt.rotation = Quaternion.identity;

        float curWidth = vsr.bounds.size.x;
        float desiredWidth = Vector2.Distance(start, end);

        float deltaX = end.x - start.x;
        float deltaY = end.y - start.y;

        vt.localScale = new Vector3(vt.localScale.x * (desiredWidth / curWidth), vt.localScale.y, 1f);
        vt.position = 0.5f * (start + end);
        vt.position += 0.5f * Vector3.back;
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(deltaY, deltaX);
        vt.Rotate(Vector3.forward, rotation);
    }

    public IEnumerator ThrowVine(GameObject ThrowingVine, Vector3 start, Vector3 dest, float duration)
    {
        float elapsed = 0f;
        Vector3 position;
        do
        {
            position = Vector3.Lerp(start, dest, Transition.SmoothLerp(elapsed / duration));
            if (position != start)
            {
                ThrowingVine.SetActive(true);
                PlaceVineBetween(ThrowingVine, start + 0.5f * Vector3.back, position + 0.5f * Vector3.back);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        ThrowingVine.SetActive(false);
    }

    public IEnumerator RetractVine(GameObject ThrowingVine, Vector3 start, Vector3 dest, float duration)
    {
        float elapsed = 0f;
        Vector3 position;
        do
        {
            position = Vector3.Lerp(dest, start, Transition.SmoothLerp(elapsed / duration));
            if (position != start)
            {
                ThrowingVine.SetActive(true);
                PlaceVineBetween(ThrowingVine, start + 0.5f * Vector3.back, position + 0.5f * Vector3.back);
            }
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);
        ThrowingVine.SetActive(false);
    }
}