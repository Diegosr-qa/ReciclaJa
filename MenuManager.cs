using UnityEngine;
using UnityEngine.SceneManagement; // Importante para trocar de cena

public class MenuManager : MonoBehaviour
{
    public void Jogar()
    {
        // Carrega a cena do jogo (coloque o nome exato da sua cena)
        SceneManager.LoadScene("Game");
    }

    public void Sair()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }
}