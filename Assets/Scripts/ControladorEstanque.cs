using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EstanqueDePeces
{
    public class ControladorEstanque : MonoBehaviour
    {
        public float poblacionInicial = 100f;
        public float tasaReproduccion = 0.10f;
        public float tasaPesca = 2f;

        public Vector2 limitesAguaMin = new Vector2(-6.5f, -4.0f);
        public Vector2 limitesAguaMax = new Vector2(6.5f, 0.4f);

        public int maxPeces = 50;
        public float tiempoEntreDias = 1.5f;

        private ModeloEstanque modelo;
        private List<AgentePez> peces = new List<AgentePez>();
        private Transform contenedorPeces;

        private bool autoSimulando;
        private Coroutine coroutineAuto;
        private bool procesandoPaso;

        public System.Action<RegistroDia> OnDiaAvanzado;
        public System.Action OnSimulacionReiniciada;
        public System.Action<bool> OnAutoCambiado;

        public ModeloEstanque Modelo => modelo;
        public bool AutoSimulando => autoSimulando;

        private void Awake()
        {
            modelo = new ModeloEstanque(poblacionInicial, tasaReproduccion, tasaPesca);
            contenedorPeces = new GameObject("Peces").transform;
            contenedorPeces.SetParent(transform);
        }

        private void Start()
        {
            ReiniciarSimulacion();
            IniciarAutoSimulacion();
        }

        public void ReiniciarSimulacion()
        {
            if (autoSimulando)
                DetenerAutoSimulacion();

            modelo.Reiniciar();

            for (int i = peces.Count - 1; i >= 0; i--)
            {
                if (peces[i] != null)
                    Destroy(peces[i].gameObject);
            }
            peces.Clear();

            int cantidad = CalcularCantidadVisual(modelo.PoblacionActual);
            for (int i = 0; i < cantidad; i++)
            {
                CrearPez();
            }

            OnSimulacionReiniciada?.Invoke();
        }

        public void AvanzarUnDia()
        {
            if (procesandoPaso) return;
            EjecutarPaso();
        }

        private void EjecutarPaso()
        {
            procesandoPaso = true;

            RegistroDia reg = modelo.AvanzarDia();

            // Pesca: se sacan los peces
            int aPescar = Mathf.RoundToInt(reg.pesca);
            for (int i = 0; i < aPescar && peces.Count > 0; i++)
            {
                var pez = peces[0];
                peces.RemoveAt(0);
                if (pez != null)
                    Destroy(pez.gameObject);
            }

            peces.RemoveAll(p => p == null);

            // Nacimientos
            int objetivo = CalcularCantidadVisual(reg.poblacionFinal);
            int nuevos = objetivo - peces.Count;
            if (nuevos > 0)
            {
                for (int i = 0; i < nuevos; i++)
                    CrearPez();
            }
            else if (reg.poblacionFinal <= 0f)
            {
                for (int i = peces.Count - 1; i >= 0; i--)
                {
                    if (peces[i] != null)
                        Destroy(peces[i].gameObject);
                }
                peces.Clear();
            }

            OnDiaAvanzado?.Invoke(reg);
            procesandoPaso = false;
        }

        private int CalcularCantidadVisual(float pob)
        {
            if (pob <= 0f) return 0;
            float factor = Mathf.Clamp(pob / 100f, 0.1f, 2.5f);
            int n = Mathf.RoundToInt(20f * factor);
            return Mathf.Clamp(n, 1, maxPeces);
        }

        private void CrearPez()
        {
            GameObject obj = new GameObject("Pez");
            obj.transform.SetParent(contenedorPeces);

            float x = Random.Range(limitesAguaMin.x + 0.5f, limitesAguaMax.x - 0.5f);
            float y = Random.Range(limitesAguaMin.y + 0.5f, limitesAguaMax.y - 0.5f);
            obj.transform.position = new Vector3(x, y, 0f);

            var agente = obj.AddComponent<AgentePez>();
            agente.Configurar(limitesAguaMin, limitesAguaMax);

            peces.Add(agente);
        }

        public void AlternarAutoSimulacion()
        {
            if (autoSimulando)
                DetenerAutoSimulacion();
            else
                IniciarAutoSimulacion();
        }

        public void IniciarAutoSimulacion()
        {
            if (autoSimulando) return;
            autoSimulando = true;
            coroutineAuto = StartCoroutine(BucleAuto());
            OnAutoCambiado?.Invoke(true);
        }

        public void DetenerAutoSimulacion()
        {
            if (!autoSimulando) return;
            autoSimulando = false;
            if (coroutineAuto != null)
            {
                StopCoroutine(coroutineAuto);
                coroutineAuto = null;
            }
            OnAutoCambiado?.Invoke(false);
        }

        private IEnumerator BucleAuto()
        {
            yield return new WaitForSeconds(1.0f);

            while (autoSimulando)
            {
                if (!procesandoPaso)
                    EjecutarPaso();

                if (modelo.EstanqueVacio)
                {
                    DetenerAutoSimulacion();
                    yield break;
                }

                yield return new WaitForSeconds(tiempoEntreDias);
            }
        }
    }
}
