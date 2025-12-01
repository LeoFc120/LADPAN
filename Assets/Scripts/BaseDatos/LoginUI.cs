using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    [Header("Arrastra los objetos aquí")]
    public InputField nameInput;
    public Button loginButton;
    public Text errorText;

    [Header("Configuración")]
    // Asegúrate de que este nombre coincida con el nombre de tu escena de menú de juegos
    public string nextSceneName = "MenuMinijuegos";

    void Start()
    {
        if (errorText) errorText.text = "";

        // Limpiamos la sesión anterior por si acaso
        GameSession.CurrentUser = null;

        loginButton.onClick.AddListener(HacerLogin);
    }

    void HacerLogin()
    {
        string nombreUsuario = nameInput.text.Trim();

        // 1. Validaciones básicas
        if (string.IsNullOrEmpty(nombreUsuario))
        {
            MostrarError("¡Escribe un nombre para jugar!");
            return;
        }

        if (nombreUsuario.Length < 3)
        {
            MostrarError("El nombre es muy corto (mínimo 3 letras).");
            return;
        }

        // Evitar que un alumno intente entrar como Profesor sin contraseña aquí
        if (nombreUsuario.ToLower() == "profesor")
        {
            MostrarError("El profesor debe ingresar en su panel especial.");
            return;
        }

        // 2. Conectar con la Base de Datos
        if (DatabaseManager.Instance != null)
        {
            Usuario usuarioFinal = null;

            // INTENTO A: Registrar como nuevo alumno
            bool sePudoRegistrar = DatabaseManager.Instance.RegistrarAlumno(nombreUsuario, out usuarioFinal);

            // INTENTO B: Si no se pudo (ya existe), intentamos Loguear
            if (!sePudoRegistrar)
            {
                // Buscamos al usuario existente
                usuarioFinal = DatabaseManager.Instance.Login(nombreUsuario);
            }

            // 3. Resultado Final
            if (usuarioFinal != null)
            {
                // ¡Éxito! Guardamos al usuario en la sesión estática
                GameSession.CurrentUser = usuarioFinal;

                errorText.text = "¡Bienvenido " + usuarioFinal.Nombre + "!";
                errorText.color = Color.green;

                // Bloqueamos el botón para que no le den click doble
                loginButton.interactable = false;

                Invoke("CargarJuego", 1.0f);
            }
            else
            {
                MostrarError("Hubo un error al conectar con los datos.");
            }
        }
        else
        {
            Debug.LogError("¡Falta el DatabaseManager en la escena!");
            MostrarError("Error: No se encontró la Base de Datos.");
        }
    }

    void MostrarError(string mensaje)
    {
        if (errorText)
        {
            errorText.text = mensaje;
            errorText.color = Color.red;
        }
    }

    void CargarJuego()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}