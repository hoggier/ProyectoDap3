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
    /// Lógica de interacción para Caja.xaml
    /// </summary>
    public partial class  Cajero : Window
    {
        public Cajero()
        {
            InitializeComponent();
        }

        dbCajaEntities objContexto = new dbCajaEntities();

        #region "Metodos"
        private void abrirCaja()
        {
            Caja objCaja = new Caja();
            objCaja.estado = "Abierta";
            objCaja.usuario = Clases.CCajero.cajeroid;
            objCaja.fechahoraapertura = DateTime.Now;
            objContexto.AddToCajas(objCaja);
            objContexto.SaveChanges();
            realizarDotacion();
        }

        private void realizarDotacion()
        {
            var objCaja = from Caja c in objContexto.Cajas
                          where c.estado == "Abierta"
                          select c;
            foreach (Caja x in objCaja)
            {
                Clases.CCajero.cajaid = x.idCaja;
            }

            Dotacion objDot = new Dotacion();
            objDot.efectivo = 500;
            objDot.recibido = "S";
            objDot.entregado = "N";
            objDot.caja = Clases.CCajero.cajaid;
            objContexto.AddToDotacions(objDot);
            objContexto.SaveChanges();
        }

        private void grabarusuarioid()
        {
            var objUsuario = from Usuario u in objContexto.Usuarios
                             where u.persona == Clases.CCajero.personaid
                             select u;
            foreach(Usuario x in objUsuario)
            {
                Clases.CCajero.cajeroid = x.idUsuario;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblCajero.Content = Clases.CCajero.cajero;
            grabarusuarioid();
        }        

        private void btnCaja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selHabilitarTurno.IsEnabled = false;
                selCerrar.IsEnabled = true;
                selEgresos.IsEnabled = true;
                selCobros.IsEnabled = true;
                selCobros.IsSelected = true;
                abrirCaja();                
            }
            catch(Exception a)
            {
                MessageBox.Show(a.Message);
            }

        }
    }
}
