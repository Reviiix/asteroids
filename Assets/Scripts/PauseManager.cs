public static class PauseManager
{
    private static bool _isPaused = false;
    public static void PauseGamePlay()
    {
        _isPaused = !_isPaused;
        GameManager.EnablePlayerConstraints(_isPaused);
        GameManager.instance.userInterfaceManager.EnablePauseCanvas(_isPaused);
    }
}
