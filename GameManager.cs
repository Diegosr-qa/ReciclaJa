using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configurações de UI")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI errosText; 
    public TextMeshProUGUI tempoText; 
    public TextMeshProUGUI faseText; 
    public GameObject painelGameOver;

    [Header("Status do Jogo")]
    public int score = 0;
    public int erros = 0;
    public int maxErros = 3;
    public float tempoRestante = 30f;
    public int faseAtual = 1;
    
    [Header("Dificuldade")]
    public float multiplicadorVelocidade = 1.0f;
    public float aumentoPorFase = 0.4f; 

    [Header("Sistema de Vidas Visual")]
    public UnityEngine.UI.Image[] iconesVida; 
    public Sprite vidaVazia; 

    [Header("Sistema de Áudio (SFX)")]
    public AudioSource audioSourceSFX; 
    public AudioClip somAcerto;        
    public AudioClip somErro;          
    public AudioClip somGameOver;      

    [Header("Sistema de Áudio (Música)")]
    public AudioSource audioMusicaJogo; 
    public float tempoDoFade = 1.5f;    

    [Header("Feedback de Nova Fase")]
    public GameObject textoLevelUp; 
    public float tempoExibicaoTexto = 1.5f; 
    public AudioClip somLevelUp; 

    private bool jogoAtivo = true;

    void Awake()
    {
        
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 1; 
        jogoAtivo = true;
        
        if (painelGameOver != null) 
            painelGameOver.SetActive(false);

        if (textoLevelUp != null)
            textoLevelUp.SetActive(false); 

        AtualizarUI();
    }

    void Update()
    {
        if (!jogoAtivo) return;

        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
            AtualizarUI();
        }
        else
        {
            SubirDeFase();
        }
    }

    public void AdicionarPontos(int pontos)
    {
        if (!jogoAtivo) return;
        score += pontos;

        if (audioSourceSFX != null && somAcerto != null) {
            audioSourceSFX.PlayOneShot(somAcerto);
        }
    
        AtualizarUI();
    }

    public void RegistrarErro()
    {
        if (!jogoAtivo) return;

        if (audioSourceSFX != null && somErro != null) 
        {
            audioSourceSFX.PlayOneShot(somErro);
        }

        int indiceVida = (maxErros - 1) - erros;
        if (indiceVida >= 0 && indiceVida < iconesVida.Length)
        {
            iconesVida[indiceVida].gameObject.SetActive(false);
            
           
        }

        erros++;
        AtualizarUI();

        if (erros >= maxErros)
        {
            GameOver();
        }
    }

    void SubirDeFase()
    {
        faseAtual++;
        tempoRestante = 30f; 
        multiplicadorVelocidade += aumentoPorFase; 
        Debug.Log("Nova Fase: " + faseAtual);
        
        if (textoLevelUp != null)
        {
            StartCoroutine(MostrarAvisoLevelUp());
        }

        if (audioSourceSFX != null && somLevelUp != null) {
            audioSourceSFX.PlayOneShot(somLevelUp);
        }

        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (scoreText != null) scoreText.text = " " + score;
        if (errosText != null) errosText.text = " " + erros + "/" + maxErros; 
        if (faseText != null) faseText.text = "FASE: " + faseAtual;
        
        
        if (tempoText != null) 
            tempoText.text = "TEMPO: " + tempoRestante.ToString("F0");
    }

    void GameOver()
    {
        jogoAtivo = false;
        
        if (audioSourceSFX != null && somGameOver != null)
        {
            audioSourceSFX.PlayOneShot(somGameOver);
        }

        if (audioMusicaJogo != null)
        {
            StartCoroutine(FadeOutMusica());
        }

        Time.timeScale = 0; 
        if (painelGameOver != null) 
            painelGameOver.SetActive(true);
    }

    System.Collections.IEnumerator MostrarAvisoLevelUp()
    {
        textoLevelUp.SetActive(true); 
        
        
        yield return new WaitForSeconds(tempoExibicaoTexto); 
        
        textoLevelUp.SetActive(false); 
    }

    System.Collections.IEnumerator FadeOutMusica()
    {
        float volumeInicial = audioMusicaJogo.volume;

        for (float t = 0; t < tempoDoFade; t += Time.unscaledDeltaTime)
        {
            audioMusicaJogo.volume = Mathf.Lerp(volumeInicial, 0, t / tempoDoFade);
            yield return null;
        }

        audioMusicaJogo.volume = 0;
        audioMusicaJogo.Stop(); 
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }
}