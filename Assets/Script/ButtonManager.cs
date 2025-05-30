using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public LevelSelectManager levelSelectManager;
    public GameObject htpPanel;
    public TransitionManager transitionManager;

    public void OnPlayButton()
    {
        transitionManager.StartTransition(() =>
        {
            mainMenuPanel.SetActive(false);
            levelsPanel.SetActive(true);
        });
    }
    public void BackMenuButton()
    {
        transitionManager.StartTransition(() =>
        {
            
            levelsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        });
    }

     public void OnBackLevelsButton()
{
         transitionManager.StartTransition(() =>
        {
            level1.SetActive(false);
            level2.SetActive(false);
            level3.SetActive(false);
            levelsPanel.SetActive(true);
            levelSelectManager.UpdateLevelLocks();
        });
}
     public void ChonLevel1(){
          transitionManager.StartTransition(() =>
        {
            levelsPanel.SetActive(false);
            level1.SetActive(true);
        });
}
public void ResetLevel1(){
          transitionManager.StartTransition(() =>
        {
            level1.SetActive(false);
            level1.SetActive(true);
        });
}
public void ChonLevel2(){
          transitionManager.StartTransition(() =>
        {
            levelsPanel.SetActive(false);
            level2.SetActive(true);
        });
}
public void ResetLevel2(){
          transitionManager.StartTransition(() =>
        {
            level2.SetActive(false);
            level2.SetActive(true);
        });
}
public void ChonLevel3(){
          transitionManager.StartTransition(() =>
        {
            levelsPanel.SetActive(false);
            level3.SetActive(true);
        });
}
public void ResetLevel3(){
          transitionManager.StartTransition(() =>
        {
            level3.SetActive(false);
            level3.SetActive(true);
        });
}
public void OutHtpPanel(){
htpPanel.SetActive(false);
}
}

