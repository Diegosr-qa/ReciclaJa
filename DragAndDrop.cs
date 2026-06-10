using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private Collider2D meuCollider;
    
    [Header("Configurações")]
    public float velocidadeQueda = 2.0f;
    public string materialType; // Ex: Plastico, Metal, Papel, Vidro

    [Header("Efeitos Visuais")]
    public GameObject efeitoParticula; // Arraste o Prefab da partícula aqui

    private float yVirtual; 

    void Start()
    {
        mainCamera = Camera.main;
        meuCollider = GetComponent<Collider2D>();
        yVirtual = transform.position.y;

        if (GetComponent<Rigidbody2D>()) {
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    void Update()
    {
        yVirtual -= velocidadeQueda * Time.deltaTime;

        HandleInput();

        if (isDragging) {
            SeguirMouse();
        } else {
            transform.position += Vector3.down * (velocidadeQueda * GameManager.instance.multiplicadorVelocidade) * Time.deltaTime;
        }

        if (yVirtual < -6f) {
            if (GameManager.instance != null) GameManager.instance.RegistrarErro();
            Destroy(gameObject);
        }
    }

    void HandleInput()
    {
        // Verifica se há um dispositivo de ponteiro ativo (Mouse ou Touch) usando o Novo Input System
        if (UnityEngine.InputSystem.Pointer.current != null)
        {
            Vector2 posicaoTela = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(posicaoTela);
            mousePos.z = 0;

            // Quando clica/toca na tela
            if (UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame) 
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null && hit.collider == meuCollider) 
                {
                    isDragging = true;
                }
            }

            // Quando solta o clique/dedo
            if (UnityEngine.InputSystem.Pointer.current.press.wasReleasedThisFrame) 
            {
                isDragging = false;
            }
        }
    }

    void SeguirMouse()
    {
        if (UnityEngine.InputSystem.Pointer.current != null)
        {
            Vector2 posicaoTela = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(posicaoTela);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // CASO 1: ACERTOU A LIXEIRA CERTA
        if (collision.CompareTag(materialType))
        {
            SoltarParticula(Color.green); // Partícula VERDE

            GameManager.instance.AdicionarPontos(1);
            Destroy(gameObject);
        }
        // CASO 2: ERROU A LIXEIRA (Bateu em uma lixeira com Tag diferente)
        else if (collision.CompareTag("Plastico") || collision.CompareTag("Papel") || 
                 collision.CompareTag("Vidro") || collision.CompareTag("Metal"))
        {
            SoltarParticula(Color.red); // Partícula VERMELHA

            GameManager.instance.RegistrarErro();
            Destroy(gameObject);
        }
    }

    // Função auxiliar que cria a partícula e muda a cor dela (Mapeada corretamente fora das colisões)
    void SoltarParticula(Color corDesejada)
    {
        if (efeitoParticula != null) 
        {
            // Cria a partícula na posição do lixo
            GameObject novaParticula = Instantiate(efeitoParticula, transform.position, Quaternion.identity);
            
            // Acessa o componente de partículas
            ParticleSystem ps = novaParticula.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                // No Unity, para mudar a cor inicial por código, acessamos o módulo 'main'
                var mainModule = ps.main;
                mainModule.startColor = corDesejada;
            }

            // Destrói o objeto da partícula após meio segundo para não acumular lixo na memória
            Destroy(novaParticula, 0.5f); 
        }
    }
}