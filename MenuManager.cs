using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void Jogar()
    {
       
        SceneManager.LoadScene("Game");
    }

    public void Sair()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
}