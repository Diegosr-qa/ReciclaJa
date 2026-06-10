using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Camera mainCamera;
    private Collider2D meuCollider;
    
    [Header("Configurações")]
    public float velocidadeQueda = 2.0f;
    public string materialType; 

    [Header("Efeitos Visuais")]
    public GameObject efeitoParticula; 

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
        /
        if (UnityEngine.InputSystem.Pointer.current != null)
        {
            Vector2 posicaoTela = UnityEngine.InputSystem.Pointer.current.position.ReadValue();
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(posicaoTela);
            mousePos.z = 0;

            if (UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame) 
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null && hit.collider == meuCollider) 
                {
                    isDragging = true;
                }
            }

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
        if (collision.CompareTag(materialType))
        {
            SoltarParticula(Color.green); 

            GameManager.instance.AdicionarPontos(1);
            Destroy(gameObject);
        }
    
        else if (collision.CompareTag("Plastico") || collision.CompareTag("Papel") || 
                 collision.CompareTag("Vidro") || collision.CompareTag("Metal"))
        {
            SoltarParticula(Color.red); 

            GameManager.instance.RegistrarErro();
            Destroy(gameObject);
        }
    }

    void SoltarParticula(Color corDesejada)
    {
        if (efeitoParticula != null) 
        {
            GameObject novaParticula = Instantiate(efeitoParticula, transform.position, Quaternion.identity);
            
            ParticleSystem ps = novaParticula.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                var mainModule = ps.main;
                mainModule.startColor = corDesejada;
            }

            Destroy(novaParticula, 0.5f); 
        }
    }
}