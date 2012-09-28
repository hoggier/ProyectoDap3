using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDap3
{
    /// <summary>
    /// Lógica de interacción para Ingresar.xaml
    /// </summary>
    public partial class Ingresar : Window
    {
        public Ingresar()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        dbCajaEntities objContexto = new dbCajaEntities();
        
        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            var objUsuario = from Usuario u in objContexto.Usuarios
                             where u.nombreusuario == txbUser.Text && u.contrasena == txbPass.Password
                             select u;
            int persona = 0;
            foreach (Usuario x in objUsuario)
            {
                persona = x.persona;
            }
            var objPersona = from Persona p in objContexto.Personas
                             where p.idPersona == persona
                             select p;            
            int cargo = 0;
            foreach (Persona x in objPersona)
            {
                cargo = x.cargo;
                Clases.CCajero.cajero = x.nombre + " " + x.app + " " + x.apm;
                Clases.CCajero.personaid = x.idPersona;
            }
            var objTCambio = from TipoCambioDolar tc in objContexto.TipoCambioDolars
                             where tc.oficial == "S"
                             select tc;
            foreach (TipoCambioDolar x in objTCambio)
            {
                Clases.CTipoCambio.tcambio = x.montoBs;        
            }

            if (cargo == 1)
            {
                JefeCaja vJefeCaja = new JefeCaja();
                vJefeCaja.Show();
            }
            else {
                Cajero vCajero = new Cajero();
                vCajero.Show();
            }
        }
    }
}
