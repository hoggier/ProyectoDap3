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
        IngresoCaja objIngcaja = new IngresoCaja();
        
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

        private void CerrarCaja()
        {
            var objCaja = from Caja c in objContexto.Cajas
                          where c.idCaja == Clases.CCajero.cajaid
                          select c;
            foreach (Caja x in objCaja)
            {
                x.estado = "Cerrada";
            }                        
            objContexto.SaveChanges();            
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

        public void almacenarCxC()
        {
            if (!Clases.CCajero.cxCguardada)
            {
                CxC objCuenta = new CxC();
                objCuenta.estado = "En Espera";
                objCuenta.fechahora = DateTime.Now;
                objCuenta.montoPagar = decimal.Parse(txbMontoCuenta.Text);
                Clases.CCajero.montopagar = objCuenta.montoPagar;
                Clases.CCajero.saldo = objCuenta.montoPagar;
                txbMontoCuenta.IsReadOnly = true;
                Clases.CCajero.cxCguardada = true;
                objContexto.AddToCxCs(objCuenta);
                objContexto.SaveChanges();
            }
        }

        public void terminarPago()
        {
            var objCxc = from CxC c in objContexto.CxCs
                         where c.estado == "En Espera"
                         select c;
            foreach (CxC x in objCxc)
            {
                Clases.CCajero.CxCid = x.idCxC;
                x.estado = "Pagado";
            }
            objContexto.SaveChanges();

            objIngcaja.idCxC = Clases.CCajero.CxCid;
            objIngcaja.idCaja = Clases.CCajero.cajaid;
            lblSaldo.Content = "0";
            lblCambio.Content = (Clases.CCajero.saldo + (Clases.CCajero.saldo * -2)).ToString();
            objContexto.AddToIngresoCajas(objIngcaja);
            objContexto.SaveChanges();

            txbMontoCuenta.Text = "";
            txbMontoIngreso.Text = "";
            Clases.CCajero.cxCguardada = false;
            Clases.CCajero.montopagar = 0;
            Clases.CCajero.montoingresado = 0;
            Clases.CCajero.saldo = 0;
            Clases.CCajero.cambio = 0;

            txbMontoCuenta.IsReadOnly = false;
            objIngcaja = new IngresoCaja();
            MessageBox.Show("Se termino de realizar el pago");
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

        private void btnCerrarCaja_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selHabilitarTurno.IsEnabled = true;
                selHabilitarTurno.IsSelected = true;
                selCerrar.IsEnabled = false;
                selEgresos.IsEnabled = false;
                selCobros.IsEnabled = false;
                CerrarCaja();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }            
        }

        private void btnRetirarDotacion_Click(object sender, RoutedEventArgs e)
        {
            var objDotacion = from Dotacion d in objContexto.Dotacions
                              where d.caja == Clases.CCajero.cajaid
                              select d;
            foreach (Dotacion x in objDotacion)
            {
                lblRetiroBss.Content = x.efectivo.ToString();
                x.entregado = "S";
            }
            objContexto.SaveChanges();
            detalles.Visibility = Visibility.Visible;
        }

        private void btnRetirar_Click(object sender, RoutedEventArgs e)
        {
            EgresoCaja objEgreso = new EgresoCaja();
            objEgreso.idCaja = Clases.CCajero.cajaid;
            objEgreso.MontoEgreso = 0;
            objContexto.AddToEgresoCajas(objEgreso);
            objContexto.SaveChanges();
            var objE = from EgresoCaja ec in objContexto.EgresoCajas
                       select ec;
            foreach (EgresoCaja x in objE)
            {
                Clases.CCajero.egresocajaid = x.idEgreso;
            }

            DetallesEgreso objDetalle = new DetallesEgreso();
            objDetalle.IdEgreso = Clases.CCajero.egresocajaid;
            objDetalle.c200bs = int.Parse(txbbs200.Text);
            objDetalle.c100bs = int.Parse(txbbs100.Text);
            objDetalle.c50bs = int.Parse(txbbs50.Text);

            Clases.CCajero.totalbs = (objDetalle.c200bs * 200) + (objDetalle.c100bs * 100) + (objDetalle.c50bs * 50);

            objDetalle.c100dolares = int.Parse(txbdolar100.Text);
            Clases.CCajero.totaldolares = objDetalle.c100dolares * 100;
            objDetalle.c50dolares = int.Parse(txbdolar50.Text);
            Clases.CCajero.totaldolares += (objDetalle.c50dolares * 50);
            objDetalle.c20dolares = int.Parse(txbdolar20.Text);
            Clases.CCajero.totaldolares += (objDetalle.c20dolares * 20);
            objDetalle.c10dolares = int.Parse(txbdolar10.Text);
            Clases.CCajero.totaldolares += (objDetalle.c10dolares * 10);
            objDetalle.c5dolares = int.Parse(txbdolar5.Text);
            Clases.CCajero.totaldolares += (objDetalle.c5dolares * 5);
            objDetalle.c1dolares = int.Parse(txbdolar1.Text);
            Clases.CCajero.totaldolares += (objDetalle.c1dolares * 1);
            Clases.CCajero.totaldolaresenbs = Clases.CCajero.totaldolares * Clases.CTipoCambio.tcambio;
            Clases.CCajero.totalretiro = Clases.CCajero.totaldolaresenbs + Clases.CCajero.totalbs;

            objDetalle.TCambio = Clases.CTipoCambio.tcambio;

            objContexto.AddToDetallesEgresoes(objDetalle);
            objContexto.SaveChanges();

            lblRetiroBss.Content = Clases.CCajero.totalbs.ToString();
            lblRetirodolaress.Content = Clases.CCajero.totaldolares.ToString();
            lblSusenBss.Content = Clases.CCajero.totaldolaresenbs.ToString();
            lblTotalRetiros.Content = Clases.CCajero.totalretiro.ToString();

            detalles.Visibility = Visibility.Visible;
        }                        
        
        private void btnEfectivo_Click(object sender, RoutedEventArgs e)
        {
            almacenarCxC();
            objIngcaja.bs = decimal.Parse(txbMontoIngreso.Text);
            Clases.CCajero.saldo = Clases.CCajero.saldo - objIngcaja.bs;
            if (Clases.CCajero.saldo > 0)
            {
                lblSaldo.Content = Clases.CCajero.saldo.ToString();
                lblCambio.Content = "-" + Clases.CCajero.saldo.ToString();
            }
            else
            {
                terminarPago();
            }
        }

        private void btnVisa_Click(object sender, RoutedEventArgs e)
        {
            almacenarCxC();
            objIngcaja.visa = decimal.Parse(txbMontoIngreso.Text);
            Clases.CCajero.saldo = Clases.CCajero.saldo - objIngcaja.visa;
            if (Clases.CCajero.saldo > 0)
            {
                lblSaldo.Content = Clases.CCajero.saldo.ToString();
                lblCambio.Content = "-" + Clases.CCajero.saldo.ToString();
            }
            else
            {
                terminarPago();
            }
        }

        private void btnMaster_Click(object sender, RoutedEventArgs e)
        {
            almacenarCxC();
            objIngcaja.master = decimal.Parse(txbMontoIngreso.Text);
            Clases.CCajero.saldo = Clases.CCajero.saldo - objIngcaja.master;
            if (Clases.CCajero.saldo > 0)
            {
                lblSaldo.Content = Clases.CCajero.saldo.ToString();
                lblCambio.Content = "-" + Clases.CCajero.saldo.ToString();
            }
            else
            {
                terminarPago();
            }
        }

        private void btnPromo_Click(object sender, RoutedEventArgs e)
        {
            almacenarCxC();
            objIngcaja.promocion = decimal.Parse(txbMontoIngreso.Text);
            Clases.CCajero.saldo = Clases.CCajero.saldo - objIngcaja.promocion;
            if (Clases.CCajero.saldo > 0)
            {
                lblSaldo.Content = Clases.CCajero.saldo.ToString();
                lblCambio.Content = "-" + Clases.CCajero.saldo.ToString();
            }
            else
            {
                terminarPago();
            }
        }

        private void btnDolar_Click(object sender, RoutedEventArgs e)
        {
            almacenarCxC();
            objIngcaja.dolares = decimal.Parse(txbMontoIngreso.Text);
            objIngcaja.bolivianosTCambio = objIngcaja.dolares * Clases.CTipoCambio.tcambio;
            objIngcaja.TCambio = Clases.CTipoCambio.tcambio;

            Clases.CCajero.saldo = Clases.CCajero.saldo - objIngcaja.bolivianosTCambio;
            if (Clases.CCajero.saldo > 0)
            {
                lblSaldo.Content = Clases.CCajero.saldo.ToString();
                lblCambio.Content = "-" + Clases.CCajero.saldo.ToString();
            }
            else
            {
                terminarPago();
            }
        }

    }
}
