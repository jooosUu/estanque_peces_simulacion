using UnityEngine;

namespace EstanqueDePeces
{
    public class AgentePez : MonoBehaviour
    {
        private static readonly Color[] colores = new Color[]
        {
            new Color(1f, 0.5f, 0.1f),
            new Color(0.2f, 0.7f, 1f),
            new Color(0.9f, 0.2f, 0.2f),
            new Color(1f, 0.85f, 0.1f),
            new Color(0.2f, 0.85f, 0.4f),
            new Color(0.8f, 0.3f, 0.9f),
            new Color(1f, 0.6f, 0.75f)
        };

        private SpriteRenderer sr;
        private Vector2 minLim = new Vector2(-6.5f, -4.0f);
        private Vector2 maxLim = new Vector2(6.5f, 0.2f);

        private Vector2 destino;
        private float velocidad;
        private float tiempoCambio;
        private float desfase;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = gameObject.AddComponent<SpriteRenderer>();

            desfase = Random.Range(0f, Mathf.PI * 2f);
            velocidad = Random.Range(1.3f, 2.3f);
        }

        public void Configurar(Vector2 min, Vector2 max)
        {
            minLim = min;
            maxLim = max;

            if (sr == null)
                sr = GetComponent<SpriteRenderer>();

            sr.sprite = GeneradorRecursosEstanque.SpritePez;
            sr.sortingOrder = 10;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.color = colores[Random.Range(0, colores.Length)];

            transform.localScale = Vector3.one * 0.9f;

            NuevoDestino();
        }

        private void Update()
        {
            if (Time.time >= tiempoCambio || Vector2.Distance(transform.position, destino) < 0.4f)
            {
                NuevoDestino();
            }

            Vector2 pos = transform.position;
            Vector2 dir = (destino - pos).normalized;

            transform.position = Vector2.MoveTowards(pos, destino, velocidad * Time.deltaTime);

            float ondulacion = Mathf.Sin((Time.time * 8f) + desfase) * 5f;

            if (Mathf.Abs(dir.x) > 0.05f)
            {
                bool izq = dir.x < 0;
                sr.flipX = izq;
                float angulo = Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg * (izq ? -1f : 1f);
                transform.rotation = Quaternion.Euler(0, 0, angulo + ondulacion);
            }
        }

        private void NuevoDestino()
        {
            float x = Random.Range(minLim.x, maxLim.x);
            float y = Random.Range(minLim.y, maxLim.y);
            destino = new Vector2(x, y);
            tiempoCambio = Time.time + Random.Range(2.5f, 5.0f);
            velocidad = Random.Range(1.2f, 2.2f);
        }
    }
}
