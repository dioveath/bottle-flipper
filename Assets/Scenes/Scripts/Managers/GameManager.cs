using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button _retryButton;
    public Button _playButton;
    public Button _backButton;
    public Text highestScoreText;
    public Text scoreText;
    public Text scoreMessageText;
    public Text distanceTravelledText;

    public GameObject _settings;

    public GameObject _mainMenu;
    public GameObject _inGameMenu;
    public GameObject _failMenu;

    public PlayerInputManager _playerInputManager;
    public HeroController heroController;

    public int highestScore = 0;


    public StateMachine gameStateMachine;


    // Global event names for EventManager
    public static string SESSION_START = "START";
    public static string SESSION_END = "END";

    void Start()
    {
        Application.targetFrameRate = 60;
        gameStateMachine = new StateMachine(new MenuState(this));
    }

    void Update()
    {
        gameStateMachine.LogicUpdate();
    }

    void FixedUpdate()
    {
        gameStateMachine.PhysicsUpdate();
    }

    public void OnPlayButtonPressed()
    {
        gameStateMachine.ChangeState(new PlayState(this));
        StartCoroutine(EnableInputAfterDelay());
    }

    public void OnRetryButtonPressed()
    {
        gameStateMachine.ChangeState(new PlayState(this));
        EnableInputAfterDelay();
    }

    public void OnBackButtonPressed()
    {
        gameStateMachine.ChangeState(new MenuState(this));
    }

    IEnumerator EnableInputAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
	_playerInputManager.isControlDisabled = false;
    }    
    

}
