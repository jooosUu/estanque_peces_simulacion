using System;
using System.Collections.Generic;
using UnityEngine;

namespace EstanqueDePeces
{
    [Serializable]
    public class RegistroDia
    {
        public int dia;
        public float poblacionInicial;
        public float pesca;
        public float poblacionRestante;
        public float nuevosNacimientos;
        public float poblacionFinal;
        public bool estanqueVacio;
    }

    public class ModeloEstanque
    {
        public float poblacionInicial = 100f;
        public float tasaReproduccion = 0.10f;
        public float tasaPesca = 2f;

        private float poblacionActual;
        private int diaActual;
        private List<RegistroDia> historial = new List<RegistroDia>();

        public int DiaActual => diaActual;
        public float PoblacionActual => poblacionActual;
        public List<RegistroDia> Historial => historial;
        public bool EstanqueVacio => poblacionActual <= 0f;

        public ModeloEstanque(float p0 = 100f, float r = 0.10f, float c = 2f)
        {
            poblacionInicial = p0;
            tasaReproduccion = r;
            tasaPesca = c;
            Reiniciar();
        }

        public void Reiniciar()
        {
            diaActual = 0;
            poblacionActual = poblacionInicial;
            historial.Clear();
        }

        public RegistroDia AvanzarDia()
        {
            if (poblacionActual <= 0f)
            {
                var vacio = new RegistroDia
                {
                    dia = diaActual,
                    poblacionInicial = 0f,
                    pesca = 0f,
                    poblacionRestante = 0f,
                    nuevosNacimientos = 0f,
                    poblacionFinal = 0f,
                    estanqueVacio = true
                };
                historial.Add(vacio);
                diaActual++;
                return vacio;
            }

            float pt = poblacionActual;
            float pescaEfectiva = Mathf.Min(pt, tasaPesca);
            float restante = Mathf.Max(0f, pt - pescaEfectiva);
            float nacimientos = restante * tasaReproduccion;
            float pt1 = restante + nacimientos;

            var reg = new RegistroDia
            {
                dia = diaActual,
                poblacionInicial = pt,
                pesca = pescaEfectiva,
                poblacionRestante = restante,
                nuevosNacimientos = nacimientos,
                poblacionFinal = pt1,
                estanqueVacio = pt1 <= 0f
            };

            historial.Add(reg);
            poblacionActual = pt1;
            diaActual++;

            return reg;
        }
    }
}
