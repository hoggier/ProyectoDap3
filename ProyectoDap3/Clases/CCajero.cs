using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoDap3.Clases
{
    public static class CCajero
    {
        public static string cajero = "";
        public static int cajeroid = 0;
        public static int cajaid = 0;
        public static int personaid = 0;
        public static int egresocajaid = 0;
        public static decimal totalbs = 0;
        public static decimal totaldolares = 0;
        public static decimal totaldolaresenbs = 0;
        public static decimal totalretiro = 0;

        public static int CxCid = 0;
        public static decimal montopagar = 0;
        public static decimal montoingresado = 0;
        public static decimal saldo = 0;
        public static decimal cambio = 0;

        public static bool cxCguardada = false;
        public static bool cajaabierta = false;
    }
}
