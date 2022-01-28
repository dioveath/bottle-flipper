using UnityEngine;

abstract class GameState : IState
{
    protected GameManager _gameManager;

    public GameState(GameManager game)
    {
        this._gameManager = game;
    }

    public abstract void Enter();
    public abstract void Exit();

    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();

}


class MenuState : GameState
{
    public MenuState(GameManager game) : base(game) { }

    public override void Enter()
    {
	_gameManager._mainMenu.SetActive(true);
        if (PlayerPrefs.HasKey("highestScore"))
            _gameManager.highestScore = PlayerPrefs.GetInt("highestScore");
	_gameManager.highestScoreText.text = "Current Highest: " + _gameManager.highestScore;
    }
    public override void Exit()
    {
        _gameManager._mainMenu.SetActive(false);
    }
    public override void LogicUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
    }
    public override void PhysicsUpdate() { }
}
 

class PlayState : GameState
{

    public PlayState(GameManager game) : base(game) { }
    public override void Enter() 
    { 
        _gameManager.heroController.Reset();
        _gameManager._inGameMenu.gameObject.SetActive(true);
	
	_gameManager.heroController.onFlipSuccess += OnHeroFlipSuccess;
        _gameManager.heroController.onFlipFailed += OnHeroFlipFailed;

        EventManager.Instance.TriggerEvent(GameManager.SESSION_START);
    }

    void OnHeroFlipSuccess()
    {
        _gameManager._playerInputManager.isControlDisabled = false;
    }

    void OnHeroFlipFailed()
    {
        _gameManager._playerInputManager.isControlDisabled = true;
        _gameManager.gameStateMachine.ChangeState(new FailState(_gameManager));
    }

    public override void LogicUpdate() 
    {
        _gameManager.scoreText.text = "Score: " + StatsManager.Instance.currentScore;
        _gameManager.distanceTravelledText.text = "Distance: " + Mathf.RoundToInt(StatsManager.Instance.GetDistanceCurrentSession()) + " M";
    }

    public override void PhysicsUpdate() {}
    public override void Exit() 
    {
        _gameManager._inGameMenu.gameObject.SetActive(false);
	_gameManager.heroController.onFlipSuccess -= OnHeroFlipSuccess;
        _gameManager.heroController.onFlipFailed -= OnHeroFlipFailed;
        LevelManager.Instance.LevelReset();
    }
}
class PauseState : GameState
{
    public PauseState(GameManager game) : base(game) { }
    public override void Enter() { 
    }
    public override void LogicUpdate() { }
    public override void PhysicsUpdate() { }
    public override void Exit() { }
}

class FailState : GameState
{
    public FailState(GameManager game) : base(game) { }
    public override void Enter() {
        _gameManager._playerInputManager.isControlDisabled = true;
        _gameManager._failMenu.gameObject.SetActive(true);

	_gameManager.scoreMessageText.text = "You scored " + StatsManager.Instance.currentScore + " in this arena! Pretty Lame, if you say!!";
        EventManager.Instance.TriggerEvent(GameManager.SESSION_END);
    }
    public override void LogicUpdate() 
    {
    }
    public override void PhysicsUpdate() { }
    public override void Exit() {
        _gameManager._failMenu.gameObject.SetActive(false);
    }
}



