using System.Collections;
using UnityEngine;
using HoloToolkit.Unity;

public class CubeManipulator : MonoBehaviour
{
  private Rigidbody _rigidBody;

  private AudioSource _audioSource;

  public int ForceMultiplier = 100;

  public int Id;

  public AudioClip BounceTogetherClip;

  public AudioClip BounceOtherClip;

  private Vector3 _orginalPosition;

  private Quaternion _originalRotation;

  // Use this for initialization
  void Start()
  {
    _rigidBody = GetComponent<Rigidbody>();
    _audioSource = GetComponent<AudioSource>();

    _orginalPosition = transform.position;
    _originalRotation = transform.rotation;
  }

  public void OnSelect(object ray)
  {
    if (!(ray is Ray)) return;
    var rayData = (Ray)ray;

    _rigidBody.AddForceAtPosition(
        new Vector3(
            rayData.direction.x * ForceMultiplier,
            rayData.direction.y * ForceMultiplier,
            rayData.direction.z * ForceMultiplier),
            GazeManager.Instance.Position);
  }

    void OnCollisionEnter(Collision coll)
    {
        // Ignore hits by cursors
        if (coll.gameObject.GetComponent<CursorManager>() != null) return;

        // Play a click on hitting another cube, but only if the it has a higher Id
        // to prevent the same sound being played twice
        var othercube = coll.gameObject.GetComponent<CubeManipulator>();
        if (othercube != null && othercube.Id < Id)
        {
            _audioSource.PlayOneShot(BounceTogetherClip);
        }

        // No cursor, no cube - we hit a wall.
        if (othercube == null)
        {
            if (coll.relativeVelocity.magnitude > 0.1)
            {
                _audioSource.PlayOneShot(BounceOtherClip);
            }
        }
    }

    public IEnumerator OnRevert()
    {
         var recallPosition = transform.position;
         var recallRotation = transform.rotation;

        _rigidBody.isKinematic = true;

        while (transform.position != _orginalPosition &&
               transform.rotation != _originalRotation)
        {
            yield return StartCoroutine(
                MoveObject(transform, recallPosition, _orginalPosition,
                           recallRotation, _originalRotation, 1.0f));
        }

        _rigidBody.isKinematic = false;
    }

    // See http://answers.unity3d.com/questions/711309/movement-script-2.html
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, 
        Quaternion startRot, Quaternion endRot, float time)
    {
        var i = 0.0f;
        var rate = 1.0f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, i));
            thisTransform.rotation = Quaternion.Lerp(startRot, endRot, Mathf.SmoothStep(0f, 1f, i));
            yield return null;
        }
    }
}
