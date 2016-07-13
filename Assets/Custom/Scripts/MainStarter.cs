using System;
using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MainStarter : MonoBehaviour
{
  public GameObject Cube;

  private bool _distanceMeasured;
  private DateTimeOffset _lastInitTime;

  // Use this for initialization
  void Start()
  {
    _distanceMeasured = false;
    _lastInitTime = DateTimeOffset.Now;
  }

  // Update is called once per frame
  void Update()
  {
    if (!_distanceMeasured)
    {
      if(GazeManager.Instance.Hit)
      {
        _distanceMeasured = true;
        CreateGrid(GazeManager.Instance.Position);
      }
      else
      {
        // If we can't find a wall in 10 seconds, create a default grid 
        if((_lastInitTime - DateTimeOffset.Now).Duration() > TimeSpan.FromSeconds(10))
        {
          _distanceMeasured = true;
          CreateGrid(CalculatePositionDeadAhead());
        }
      }
    }
  }

  private Vector3 CalculatePositionDeadAhead()
  {
    var gazeOrigin = Camera.main.transform.position;
    return gazeOrigin + Camera.main.transform.forward * 3.5f;
  }

  private void CreateGrid(Vector3 hitPosition)
  {
    var gazeOrigin = Camera.main.transform.position;
    var rotation = Camera.main.transform.rotation;

    var maxDistance = Vector3.Distance(gazeOrigin, hitPosition);

    transform.position = hitPosition;
    transform.rotation = rotation;

    int id = 0;

    float size = 0.2f;
    float maxZ = maxDistance - 1f;
    float maxX = 0.35f;
    float maxY = 0.35f;
    float z = 1.5f;
    do
    {
      var x = -maxX;
      do
      {
        var y = -maxY;
        do
        {
          CreateCube(id++,
              gazeOrigin + transform.forward * z +
                           transform.right * x +
                           transform.up * y,
              rotation);
          y += size;
        }
        while (y <= maxY);
        x += size;
      }
      while (x <= maxX);
      z += size;
    }
    while (z <= maxZ);
  }

  private void CreateCube(int id, Vector3 location, Quaternion rotation)
  {
    var c = Instantiate(Cube, location, rotation) as GameObject;
    //Rotate around it's own up axis so up points TO the camera
    c.transform.RotateAround(location, transform.up, 180f);
    var m = c.GetComponent<CubeManipulator>();
    m.Id = id;
  }
}
