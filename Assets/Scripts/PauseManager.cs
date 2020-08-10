public static class PauseManager
{
    private static bool isPaused = false;
    public static void PauseGamePlay()
    {
        isPaused = !isPaused;
        GameManager.EnablePlayerConstraints(isPaused);
        GameManager.instance.userInterfaceManager.EnablePauseCanvas(isPaused);
    }
}
