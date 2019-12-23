using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPlayerArch(bool shouldArch) {
        PlayerControl.instance.shouldArch = shouldArch;
    }

    public void setPlayerTuck(bool shouldTuck) {
        PlayerControl.instance.shouldTuck = shouldTuck;
    }

    public void setPlayerLetGo(bool shouldLetGo) {
        PlayerControl.instance.shouldLetGo = shouldLetGo;
    }

    public void setScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void resetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
