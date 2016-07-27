using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
  public string GoToStartCommand = "go to start";

  public string NewGridCommand = "create new grid";

  public string DropCommand = "drop";

  public string DropAllCommand = "drop all";

  public string RevertCommand = "total recall";

  private KeywordRecognizer _keywordRecognizer;

  private MainStarter _mainStarter;

  // Use this for initialization
  void Start()
  {
    _mainStarter = GetComponent<MainStarter>();
    _keywordRecognizer = new KeywordRecognizer(
      new[] { GoToStartCommand, NewGridCommand, DropCommand, DropAllCommand, RevertCommand });
    _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
    _keywordRecognizer.Start();
  }

  private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
  {
    var cmd = args.text;

    if (cmd == NewGridCommand)
    {
      _mainStarter.CreateNewGrid();
    }

    if (cmd == GoToStartCommand)
    {
      if (GazeManager.Instance.Hit)
      {
        GazeManager.Instance.HitInfo.collider.gameObject.SendMessage("OnRevert", true);
      }
    }

    if (cmd == DropCommand)
    {
      if (GazeManager.Instance.Hit)
      {
        GazeManager.Instance.HitInfo.collider.gameObject.SendMessage("OnDrop");
      }
    }

    if (cmd == RevertCommand)
    {
      _mainStarter.RevertAll();
    }

    if (cmd == DropAllCommand)
    {
      _mainStarter.DropAll();
    }
  }

  private void OnDestroy()
  {
    if (_keywordRecognizer != null)
    {
      if (_keywordRecognizer.IsRunning)
      {
        _keywordRecognizer.Stop();
      }
      _keywordRecognizer.Dispose();
    }
  }
}
