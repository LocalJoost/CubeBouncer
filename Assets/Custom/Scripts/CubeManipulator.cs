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

  // Use this for initialization
  void Start()
  {
    _rigidBody = GetComponent<Rigidbody>();
    _audioSource = GetComponent<AudioSource>();
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
}
