public static class PauseManager
{
    public static void PauseGamePlay(bool state = true)
    {
        GameManager.EnablePlayerConstraints(state);
        GameManager.instance.userInterfaceManager.EnablePauseCanvas(state);
    }
}
