using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public TetrisBlock CurrentBlock;
    [HideInInspector] public bool Boosted;
    
    [Header("Score Controlling")] 
    [SerializeField] private TMPro.TMP_Text _scoreText;
    [SerializeField] private TMPro.TMP_Text _recordScoreText;
    private int _score;
    [Space(5)]
    
    [Header("When game starts")]
    [SerializeField] private GameObject[] _toDeactivate;
    [SerializeField] private GameObject[] _toActivate;
    [Space(5)]
    
    [Header("Pause Controlling")]
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseButton;
    private bool _gamePaused = false;
    [Space(5)]

    [Header("Brightness Controlling")]
    [SerializeField] private Image _overlayImage;
    private float _brightnessValue = 0.6f;
    private bool _brightnessDecreasing = true;
    
    [SerializeField] private GameObject _gameOverUI;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Brightness"))
            _brightnessValue = PlayerPrefs.GetFloat("Brightness");
        _overlayImage.color = new Color(0, 0, 0, _brightnessValue);

        Time.timeScale = 1;
        _recordScoreText.text = PlayerPrefs.GetInt("Record").ToString("00000");
    }
    
    public void StartGame()
    {
        foreach (var obj in _toDeactivate)
            obj.SetActive(false);
        
        foreach (var obj in _toActivate)
            obj.SetActive(true);
    }

    public void TogglePause()
    {
        if (!_pauseUI.activeSelf)
        {
            _gamePaused = true;

            Time.timeScale = 0;
            _pauseUI.SetActive(true);
        } else
        {
            _gamePaused = false;

            Time.timeScale = 1;
            _pauseUI.SetActive(false);
        }
    }
    
    public void GameOver()
    {
        _pauseButton.SetActive(false);
        if (PlayerPrefs.GetInt("Record") < _score)
            PlayerPrefs.SetInt("Record", _score);
        
        _recordScoreText.text = PlayerPrefs.GetInt("Record").ToString("00000");
        
        _gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    
    #region UI Methods
    public void ChangeBrightness()
    {
        if (_brightnessValue >= 0.6f)
                _brightnessDecreasing = true;

        if (_brightnessValue <= 0.15f)
                _brightnessDecreasing = false;

        if (_brightnessDecreasing)
        {
            if (_brightnessValue > 0.15f)
                _brightnessValue -= 0.15f;
            else
                _brightnessDecreasing = false;
        } else 
        {
            if (_brightnessValue < 0.6f)
                _brightnessValue += 0.15f;
            else 
                _brightnessDecreasing = true;
        }

        _overlayImage.color = new Color(0, 0, 0, _brightnessValue);
        PlayerPrefs.SetFloat("Brightness", _brightnessValue);
    }
    
    public void MoveToSides(int side)  // left: -1 | right: 1
    {
        if (_gamePaused) return;
        CurrentBlock.MoveSidewards(side);
    }

    public void Rotate(int direction)
    {
        if (_gamePaused) return;
        CurrentBlock.Rotate(direction);
    }

    public void Down()
    {
        if (_gamePaused) return;
        Boosted = true;
    }

    public void DownReleased()
    {
        if (_gamePaused) return;
        Boosted = false;
    }
    #endregion
    
    public void IncreaseScore(int increment)
    {
        _score += increment;
        _scoreText.text = _score.ToString("00000");
    }
}