public static class PauseManager
{
    public static void PauseGamePlay(bool state = true)
    {
        GameManager.EnablePlayerConstraints(state);
        GameManager.Instance.userInterfaceManager.EnablePauseCanvas(state);
    }
}
