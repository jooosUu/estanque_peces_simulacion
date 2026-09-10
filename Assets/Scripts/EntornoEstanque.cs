using UnityEngine;

namespace EstanqueDePeces
{
    public class EntornoEstanque : MonoBehaviour
    {
        private void Start()
        {
            Construir();
        }

        public void Construir()
        {
            GameObject agua = new GameObject("Fondo");
            agua.transform.SetParent(transform);
            agua.transform.position = new Vector3(0, -1.2f, 5f);
            agua.transform.localScale = new Vector3(18f, 9f, 1f);

            var sr = agua.AddComponent<SpriteRenderer>();
            sr.sprite = GeneradorRecursosEstanque.SpriteAguaFondo;
            sr.sortingOrder = -10;
        }
    }
}
