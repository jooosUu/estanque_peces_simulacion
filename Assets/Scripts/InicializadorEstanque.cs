using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace EstanqueDePeces
{
    public class InicializadorEstanque : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AlCargarJuego()
        {
            if (FindAnyObjectByType<ControladorEstanque>() != null)
                return;

            ConstruirEscena();
        }

#if UNITY_EDITOR
        [MenuItem("Herramientas/Configurar Estanque")]
        public static void ConfigurarDesdeEditor()
        {
            var previo = GameObject.Find("Estanque");
            if (previo != null)
                DestroyImmediate(previo);

            ConstruirEscena();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
#endif

        public static GameObject ConstruirEscena()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
                camObj.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0f, -1.2f, -10f);
            cam.backgroundColor = new Color(0.08f, 0.18f, 0.28f);
            cam.clearFlags = CameraClearFlags.SolidColor;

            GameObject raiz = GameObject.Find("Estanque");
            if (raiz == null)
                raiz = new GameObject("Estanque");

            var entorno = raiz.GetComponentInChildren<EntornoEstanque>();
            if (entorno == null)
            {
                GameObject obj = new GameObject("Fondo");
                obj.transform.SetParent(raiz.transform, false);
                entorno = obj.AddComponent<EntornoEstanque>();
            }

            var controlador = raiz.GetComponentInChildren<ControladorEstanque>();
            if (controlador == null)
            {
                GameObject obj = new GameObject("Controlador");
                obj.transform.SetParent(raiz.transform, false);
                controlador = obj.AddComponent<ControladorEstanque>();
            }

            var ui = raiz.GetComponentInChildren<UIEstanque>();
            if (ui == null)
            {
                GameObject obj = new GameObject("UI");
                obj.transform.SetParent(raiz.transform, false);
                ui = obj.AddComponent<UIEstanque>();
            }

            return raiz;
        }
    }
}
