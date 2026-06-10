using UnityEngine;
using TMPro; // Necessário para os textos
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configurações de UI")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI errosText; 
    public TextMeshProUGUI tempoText; 
    public TextMeshProUGUI faseText; // Texto para mostrar "FASE: 1"
    public GameObject painelGameOver;

    [Header("Status do Jogo")]
    public int score = 0;
    public int erros = 0;
    public int maxErros = 3;
    public float tempoRestante = 30f;
    public int faseAtual = 1;
    
    [Header("Dificuldade")]
    public float multiplicadorVelocidade = 1.0f;
    public float aumentoPorFase = 0.4f; // Aumenta 40% a cada fase

    [Header("Sistema de Vidas Visual")]
    public UnityEngine.UI.Image[] iconesVida; // Arraste os 3 corações para cá
    public Sprite vidaVazia; // Opcional: um sprite de slot vazio

    [Header("Sistema de Áudio (SFX)")]
    public AudioSource audioSourceSFX; // O "alto-falante" dos efeitos
    public AudioClip somAcerto;        // Som de ponto
    public AudioClip somErro;          // Som de vida perdida
    public AudioClip somGameOver;      // ADICIONADO: Som quando o jogo acaba de vez!

    [Header("Sistema de Áudio (Música)")]
    public AudioSource audioMusicaJogo; // Arraste o objeto "MusicaJogo" para cá no Inspector!
    public float tempoDoFade = 1.5f;    // Tempo em segundos para a música sumir completamente

    [Header("Feedback de Nova Fase")]
    public GameObject textoLevelUp; // Arraste o 'TextoLevelUp' para cá
    public float tempoExibicaoTexto = 1.5f; // Tempo que o aviso fica na tela
    public AudioClip somLevelUp; // Som que toca na transição de fase

    private bool jogoAtivo = true;

    void Awake()
    {
        // Sistema para garantir que só exista um GameManager
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Garante que o tempo está rodando e o painel está desligado
        Time.timeScale = 1; 
        jogoAtivo = true;
        
        if (painelGameOver != null) 
            painelGameOver.SetActive(false);

        if (textoLevelUp != null)
            textoLevelUp.SetActive(false); // Garante que o texto de level up começa escondido

        AtualizarUI();
    }

    void Update()
    {
        if (!jogoAtivo) return;

        // Controle do Cronômetro
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

        // TOCA O SOM DE ACERTO
        if (audioSourceSFX != null && somAcerto != null) {
            audioSourceSFX.PlayOneShot(somAcerto);
        }
    
        AtualizarUI();
    }

    public void RegistrarErro()
    {
        if (!jogoAtivo) return;

        // TOCA O SOM DE ERRO
        if (audioSourceSFX != null && somErro != null) 
        {
            audioSourceSFX.PlayOneShot(somErro);
        }

        int indiceVida = (maxErros - 1) - erros;
        if (indiceVida >= 0 && indiceVida < iconesVida.Length)
        {
            iconesVida[indiceVida].gameObject.SetActive(false);
            
            // Opcional: Se quiser trocar o sprite em vez de sumir, descomente a linha abaixo:
            // iconesVida[indiceVida].sprite = vidaVazia;
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
        tempoRestante = 30f; // Mantém fixo em 30 segundos
        multiplicadorVelocidade += aumentoPorFase; // Aumenta a velocidade dos lixos
        Debug.Log("Nova Fase: " + faseAtual);
        
        // ATIVA O AVISO VISUAL DE UPGRADE
        if (textoLevelUp != null)
        {
            StartCoroutine(MostrarAvisoLevelUp());
        }

        // TOCA O SOM DE TRANSICÃO DE FASE
        if (audioSourceSFX != null && somLevelUp != null) {
            audioSourceSFX.PlayOneShot(somLevelUp);
        }

        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (scoreText != null) scoreText.text = " " + score;
        if (errosText != null) errosText.text = " " + erros + "/" + maxErros; // Adicionado "/" para melhor leitura se usar texto
        if (faseText != null) faseText.text = "FASE: " + faseAtual;
        
        // Exibe o tempo sem nenhuma casa decimal (F0)
        if (tempoText != null) 
            tempoText.text = "TEMPO: " + tempoRestante.ToString("F0");
    }

    void GameOver()
    {
        jogoAtivo = false;
        
        // TOCA O SOM DE FIM DE JOGO
        if (audioSourceSFX != null && somGameOver != null)
        {
            audioSourceSFX.PlayOneShot(somGameOver);
        }

        // Inicia o efeito de sumir com a música de fundo
        if (audioMusicaJogo != null)
        {
            StartCoroutine(FadeOutMusica());
        }

        Time.timeScale = 0; // Pausa o jogo fisicamente (lixos param de cair)
        if (painelGameOver != null) 
            painelGameOver.SetActive(true);
    }

    // Coroutine para mostrar o texto e sumir depois de um tempo
    System.Collections.IEnumerator MostrarAvisoLevelUp()
    {
        textoLevelUp.SetActive(true); // Mostra o texto na tela
        
        // Espera o tempo determinado (ex: 1.5 segundos) sem parar o jogo
        yield return new WaitForSeconds(tempoExibicaoTexto); 
        
        textoLevelUp.SetActive(false); // Some com o texto
    }

    // Coroutine corrigida do Fade Out da música
    System.Collections.IEnumerator FadeOutMusica()
    {
        float volumeInicial = audioMusicaJogo.volume;

        // Enquanto o tempo do fade estiver rolando, diminui o volume gradualmente
        for (float t = 0; t < tempoDoFade; t += Time.unscaledDeltaTime)
        {
            // Usamos unscaledDeltaTime porque o Time.timeScale foi zerado no GameOver
            audioMusicaJogo.volume = Mathf.Lerp(volumeInicial, 0, t / tempoDoFade);
            yield return null;
        }

        audioMusicaJogo.volume = 0;
        audioMusicaJogo.Stop(); // Para a música de vez após sumir o som
    }

    // Funções para os Botões da UI
    public void ReiniciarJogo()
    {
        // Reinicia a cena que estiver aberta no momento
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }
}