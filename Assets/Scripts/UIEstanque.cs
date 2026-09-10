using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EstanqueDePeces
{
    public class UIEstanque : MonoBehaviour
    {
        private ControladorEstanque controlador;

        private Text txtInfo;
        private Text txtBotonAuto;
        private Transform contenedorFilas;
        private Font fuente;

        private void Awake()
        {
            controlador = FindAnyObjectByType<ControladorEstanque>();
            fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (fuente == null)
                fuente = Font.CreateDynamicFontFromOSFont("Arial", 14);
        }

        private void Start()
        {
            ConfigurarEventosInput();
            CrearUI();
            Suscribir();
            ActualizarTexto();
        }

        private void ConfigurarEventosInput()
        {
            var es = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (es == null)
            {
                GameObject obj = new GameObject("EventSystem");
                es = obj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            }

            var stand = es.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            var tipoInput = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");

            if (tipoInput != null)
            {
                if (stand != null)
                    DestroyImmediate(stand);

                if (es.GetComponent(tipoInput) == null)
                    es.gameObject.AddComponent(tipoInput);
            }
            else if (stand == null)
            {
                es.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private void Suscribir()
        {
            if (controlador != null)
            {
                controlador.OnDiaAvanzado += OnDiaAvanzado;
                controlador.OnSimulacionReiniciada += OnSimulacionReiniciada;
                controlador.OnAutoCambiado += OnAutoCambiado;
            }
        }

        private void OnDestroy()
        {
            if (controlador != null)
            {
                controlador.OnDiaAvanzado -= OnDiaAvanzado;
                controlador.OnSimulacionReiniciada -= OnSimulacionReiniciada;
                controlador.OnAutoCambiado -= OnAutoCambiado;
            }
        }

        private void CrearUI()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 50;
            }

            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();

            CrearBarraSuperior(canvas.transform);
            CrearBotones(canvas.transform);
            CrearTabla(canvas.transform);
        }

        private void CrearBarraSuperior(Transform padre)
        {
            GameObject barra = CrearPanel("Barra", padre, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -10), new Vector2(-40, 48), new Color(0.12f, 0.12f, 0.12f, 0.85f));

            txtInfo = CrearTexto("TextoInfo", barra.transform, "Día: 0  |  Peces: 100.00", 18, FontStyle.Bold, Color.white);
            RectTransform rt = txtInfo.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(20, 0);
            rt.offsetMax = new Vector2(-20, 0);
            txtInfo.alignment = TextAnchor.MiddleLeft;
        }

        private void CrearBotones(Transform padre)
        {
            GameObject panel = CrearPanel("Botones", padre, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(20, 20), new Vector2(360, 54), new Color(0.12f, 0.12f, 0.12f, 0.85f));

            GameObject fila = new GameObject("Fila");
            fila.transform.SetParent(panel.transform, false);
            RectTransform rtFila = fila.AddComponent<RectTransform>();
            rtFila.anchorMin = Vector2.zero;
            rtFila.anchorMax = Vector2.one;
            rtFila.offsetMin = new Vector2(8, 8);
            rtFila.offsetMax = new Vector2(-8, -8);

            var hlg = fila.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            CrearBoton("BtnAvanzar", fila.transform, "Avanzar Día", new Color(0.28f, 0.28f, 0.28f), () =>
            {
                if (controlador != null) controlador.AvanzarUnDia();
            });

            var btnAuto = CrearBoton("BtnAuto", fila.transform, "Pausar", new Color(0.28f, 0.28f, 0.28f), () =>
            {
                if (controlador != null) controlador.AlternarAutoSimulacion();
            });
            txtBotonAuto = btnAuto.GetComponentInChildren<Text>();

            CrearBoton("BtnReiniciar", fila.transform, "Reiniciar", new Color(0.28f, 0.28f, 0.28f), () =>
            {
                if (controlador != null) controlador.ReiniciarSimulacion();
            });
        }

        private void CrearTabla(Transform padre)
        {
            GameObject panel = CrearPanel("Tabla", padre, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f),
                new Vector2(-20, -50), new Vector2(760, -120), new Color(0.08f, 0.08f, 0.08f, 0.90f));

            Text titulo = CrearTexto("Titulo", panel.transform, "Tabla de Datos", 14, FontStyle.Bold, Color.white);
            RectTransform rtT = titulo.rectTransform;
            rtT.anchorMin = new Vector2(0, 1f);
            rtT.anchorMax = new Vector2(1, 1f);
            rtT.pivot = new Vector2(0.5f, 1f);
            rtT.anchoredPosition = new Vector2(0, -8);
            rtT.sizeDelta = new Vector2(-20, 24);
            titulo.alignment = TextAnchor.MiddleCenter;

            GameObject cabecera = new GameObject("Cabecera");
            cabecera.transform.SetParent(panel.transform, false);
            RectTransform rtH = cabecera.AddComponent<RectTransform>();
            rtH.anchorMin = new Vector2(0, 1f);
            rtH.anchorMax = new Vector2(1, 1f);
            rtH.pivot = new Vector2(0.5f, 1f);
            rtH.anchoredPosition = new Vector2(0, -36);
            rtH.sizeDelta = new Vector2(-16, 32);

            var imgH = cabecera.AddComponent<Image>();
            imgH.color = new Color(0.22f, 0.22f, 0.22f, 0.95f);

            var hlgH = cabecera.AddComponent<HorizontalLayoutGroup>();
            hlgH.childControlWidth = true;
            hlgH.childControlHeight = true;
            hlgH.spacing = 2;

            string[] columnas = { "Día", "P. Inicial", "Pesca", "P. Restante", "+10%", "P. Final", "¿Vacío?" };
            foreach (var col in columnas)
            {
                Text t = CrearTexto($"Col_{col}", cabecera.transform, col, 12, FontStyle.Bold, Color.white);
                t.alignment = TextAnchor.MiddleCenter;
            }

            GameObject scroll = new GameObject("Scroll");
            scroll.transform.SetParent(panel.transform, false);
            RectTransform rtS = scroll.AddComponent<RectTransform>();
            rtS.anchorMin = new Vector2(0, 0f);
            rtS.anchorMax = new Vector2(1, 1f);
            rtS.offsetMin = new Vector2(8, 8);
            rtS.offsetMax = new Vector2(-8, -72);

            var sr = scroll.AddComponent<ScrollRect>();
            sr.horizontal = false;
            sr.vertical = true;

            GameObject view = new GameObject("View");
            view.transform.SetParent(scroll.transform, false);
            RectTransform rtV = view.AddComponent<RectTransform>();
            rtV.anchorMin = Vector2.zero;
            rtV.anchorMax = Vector2.one;
            rtV.sizeDelta = Vector2.zero;
            view.AddComponent<RectMask2D>();

            GameObject content = new GameObject("Content");
            content.transform.SetParent(view.transform, false);
            RectTransform rtC = content.AddComponent<RectTransform>();
            rtC.anchorMin = new Vector2(0, 1f);
            rtC.anchorMax = new Vector2(1, 1f);
            rtC.pivot = new Vector2(0.5f, 1f);
            rtC.sizeDelta = new Vector2(0, 250);

            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 3;

            var csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            sr.content = rtC;
            sr.viewport = rtV;

            contenedorFilas = content.transform;
        }

        private void ActualizarTexto()
        {
            if (controlador != null && controlador.Modelo != null && txtInfo != null)
            {
                var m = controlador.Modelo;
                txtInfo.text = $"Día: {m.DiaActual}  |  Peces: {m.PoblacionActual:F2}";
            }
        }

        private void OnDiaAvanzado(RegistroDia reg)
        {
            if (txtInfo != null)
                txtInfo.text = $"Día: {reg.dia + 1}  |  Peces: {reg.poblacionFinal:F2}";

            AgregarFila(reg);
        }

        private void OnSimulacionReiniciada()
        {
            ActualizarTexto();

            if (contenedorFilas != null)
            {
                for (int i = contenedorFilas.childCount - 1; i >= 0; i--)
                    Destroy(contenedorFilas.GetChild(i).gameObject);
            }

            if (txtBotonAuto != null)
                txtBotonAuto.text = (controlador != null && controlador.AutoSimulando) ? "Pausar" : "Auto";
        }

        private void OnAutoCambiado(bool activo)
        {
            if (txtBotonAuto != null)
                txtBotonAuto.text = activo ? "Pausar" : "Auto";
        }

        private void AgregarFila(RegistroDia reg)
        {
            if (contenedorFilas == null) return;

            GameObject fila = new GameObject($"Fila_{reg.dia}");
            fila.transform.SetParent(contenedorFilas, false);

            var rt = fila.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 26);

            var img = fila.AddComponent<Image>();
            bool esPar = (reg.dia % 2 == 0);
            img.color = esPar ? new Color(0.14f, 0.14f, 0.14f, 0.9f) : new Color(0.18f, 0.18f, 0.18f, 0.9f);

            var hlg = fila.AddComponent<HorizontalLayoutGroup>();
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.spacing = 2;

            string[] valores = {
                reg.dia.ToString(),
                reg.poblacionInicial.ToString("F2"),
                reg.pesca.ToString("F2"),
                reg.poblacionRestante.ToString("F2"),
                $"+{reg.nuevosNacimientos:F2}",
                reg.poblacionFinal.ToString("F2"),
                reg.estanqueVacio ? "Sí" : "No"
            };

            for (int i = 0; i < valores.Length; i++)
            {
                Text t = CrearTexto($"C_{i}", fila.transform, valores[i], 12, FontStyle.Normal, Color.white);
                t.alignment = TextAnchor.MiddleCenter;
            }
        }

        private GameObject CrearPanel(string nombre, Transform padre, Vector2 aMin, Vector2 aMax, Vector2 piv,
            Vector2 pos, Vector2 tam, Color col)
        {
            GameObject obj = new GameObject(nombre);
            obj.transform.SetParent(padre, false);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = aMin;
            rt.anchorMax = aMax;
            rt.pivot = piv;
            rt.anchoredPosition = pos;
            rt.sizeDelta = tam;

            var img = obj.AddComponent<Image>();
            img.color = col;
            return obj;
        }

        private Text CrearTexto(string nombre, Transform padre, string texto, int tamano, FontStyle estilo, Color col)
        {
            GameObject obj = new GameObject(nombre);
            obj.transform.SetParent(padre, false);
            RectTransform rt = obj.AddComponent<RectTransform>();
            var txt = obj.AddComponent<Text>();
            txt.text = texto;
            txt.font = fuente;
            txt.fontSize = tamano;
            txt.fontStyle = estilo;
            txt.color = col;
            txt.alignment = TextAnchor.MiddleLeft;
            return txt;
        }

        private Button CrearBoton(string nombre, Transform padre, string texto, Color col, UnityEngine.Events.UnityAction accion)
        {
            GameObject obj = new GameObject(nombre);
            obj.transform.SetParent(padre, false);
            RectTransform rt = obj.AddComponent<RectTransform>();

            var img = obj.AddComponent<Image>();
            img.color = col;

            var btn = obj.AddComponent<Button>();
            if (accion != null)
                btn.onClick.AddListener(accion);

            Text txt = CrearTexto("Texto", obj.transform, texto, 12, FontStyle.Bold, Color.white);
            RectTransform rtTxt = txt.rectTransform;
            rtTxt.anchorMin = Vector2.zero;
            rtTxt.anchorMax = Vector2.one;
            rtTxt.sizeDelta = Vector2.zero;
            txt.alignment = TextAnchor.MiddleCenter;

            return btn;
        }
    }
}
