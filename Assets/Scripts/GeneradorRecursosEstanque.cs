using UnityEngine;

namespace EstanqueDePeces
{
    public static class GeneradorRecursosEstanque
    {
        private static Sprite spritePez;
        private static Sprite spriteAguaFondo;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCache()
        {
            spritePez = null;
            spriteAguaFondo = null;
        }

        public static Sprite SpritePez
        {
            get
            {
                if (spritePez == null)
                    spritePez = CrearSpritePez(128, 64);
                return spritePez;
            }
        }

        public static Sprite SpriteAguaFondo
        {
            get
            {
                if (spriteAguaFondo == null)
                    spriteAguaFondo = CrearSpriteAgua(512, 512);
                return spriteAguaFondo;
            }
        }

        private static Sprite CrearSpritePez(int ancho, int alto)
        {
            Texture2D tex = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            Color vacio = new Color(0, 0, 0, 0);

            float cx = ancho * 0.45f;
            float cy = alto * 0.5f;
            float rx = ancho * 0.32f;
            float ry = alto * 0.38f;

            for (int y = 0; y < alto; y++)
            {
                for (int x = 0; x < ancho; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float d = (dx * dx) + (dy * dy);

                    Color c = vacio;

                    if (d <= 1f)
                    {
                        float f = Mathf.InverseLerp(cy - ry, cy + ry, y);
                        c = Color.Lerp(new Color(0.85f, 0.85f, 0.85f, 1f), Color.white, f);

                        float ojoX = cx + rx * 0.58f;
                        float ojoY = cy + ry * 0.22f;
                        float distOjo = Vector2.Distance(new Vector2(x, y), new Vector2(ojoX, ojoY));
                        if (distOjo < alto * 0.10f)
                            c = (distOjo < alto * 0.045f) ? Color.black : Color.white;

                        if (d > 0.88f)
                            c.a *= Mathf.InverseLerp(1f, 0.88f, d);
                    }

                    float colaX = cx - rx * 0.75f;
                    if (x < colaX && x > colaX - ancho * 0.28f)
                    {
                        float fac = (colaX - x) / (ancho * 0.28f);
                        float w = (alto * 0.45f) * fac;
                        if (Mathf.Abs(y - cy) <= w)
                            c = new Color(0.95f, 0.95f, 0.95f, 0.9f * (1f - fac * 0.2f));
                    }

                    if (y > cy + ry * 0.5f && x > cx - rx * 0.2f && x < cx + rx * 0.3f)
                    {
                        if (y <= cy + ry + alto * 0.2f)
                            c = new Color(0.9f, 0.9f, 0.9f, 0.85f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f), 100f);
        }

        private static Sprite CrearSpriteAgua(int ancho, int alto)
        {
            Texture2D tex = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            Color sup = new Color(0.12f, 0.55f, 0.75f, 0.95f);
            Color prof = new Color(0.04f, 0.20f, 0.40f, 1f);

            for (int y = 0; y < alto; y++)
            {
                Color col = Color.Lerp(prof, sup, y / (float)alto);
                for (int x = 0; x < ancho; x++)
                    tex.SetPixel(x, y, col);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
