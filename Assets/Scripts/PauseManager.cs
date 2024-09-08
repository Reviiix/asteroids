using Assets.Scripts;
using Assets.Scripts.Obstacles;
using Assets.Scripts.Player;
using Player;
using Statistics;

public static class PauseManager
{
    public static bool isPaused;

    public static void PauseButtonPressed()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }

    private static void PauseGame(bool state)
    {
        GameManager.Instance.userInterfaceManager.EnablePauseCanvas(state);
        PlayerManager.EnablePlayerConstraints(state);
        
        ObstacleManager.moveObstacles = !state;
        BulletManager.moveBullets = !state;
        PlayerShooting.canShoot = !state;
        EnableGamePlayMethods(!state);
    }

    private static void EnableGamePlayMethods(bool state)
    {
        AudioManager.SetPauseVolume(state);
        if (state)
        {
            TimeTracker.ResumeTimer();
            GameManager.Instance.obstacleManager.StartCreatObstacleSequence();
            return;
        }
        TimeTracker.PauseTimer();
        GameManager.Instance.obstacleManager.StopCreatObstacleSequence();
    }
    
}
