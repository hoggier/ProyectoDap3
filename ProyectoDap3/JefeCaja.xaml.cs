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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoDap3
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class JefeCaja : Window
    {
        public JefeCaja()
        {
            InitializeComponent();
        }

        dbCajaEntities objContexto = new dbCajaEntities();
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblTCActual.Content = Clases.CTipoCambio.tcambio.ToString();
            var objCargos = from Cargo c in objContexto.Cargos
                            select c;
            foreach (Cargo x in objCargos)
            {
                cmbCargo.Items.Add(x.Cargo1);
            }
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            Persona objPer = new Persona();            
            objPer.nombre = txbNom.Text;
            objPer.app = txbApp.Text;
            objPer.apm = txbApm.Text;
            if (cmbCargo.SelectedValue.ToString() == "Cajero")
                objPer.cargo = 2;
            else objPer.cargo = 1;            
            objContexto.AddToPersonas(objPer);
            int a = objContexto.SaveChanges();

            var objP = from Persona p in objContexto.Personas                       
                       select p;
            int idper = 0;            
            foreach(Persona x in objP)
            {
                idper = x.idPersona;
            }
            Usuario objUs = new Usuario();
            objUs.persona = idper;
            objUs.nombreusuario = txbUser.Text;
            objUs.contrasena = txbPass.Password;
            objContexto.AddToUsuarios(objUs);
            int b = objContexto.SaveChanges();

            MessageBox.Show("Usuario Creado Exitosamente");
            
        }

        private void btnTC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TipoCambioDolar objTC = new TipoCambioDolar();
                var objTTC = from TipoCambioDolar c in objContexto.TipoCambioDolars
                             where c.oficial == "s"
                             select c;
                foreach (TipoCambioDolar x in objTTC)
                {
                    x.oficial = "N";
                }
                int a = objContexto.SaveChanges();


                objTC.montoBs = decimal.Parse(txbTC.Text);
                Clases.CTipoCambio.tcambio = decimal.Parse(txbTC.Text);
                lblTCActual.Content = Clases.CTipoCambio.tcambio.ToString();
                objTC.oficial = "S";
                objContexto.AddToTipoCambioDolars(objTC);
                objContexto.SaveChanges();
                MessageBox.Show("Nuevo tipo de cambio almacenado");
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }
    }
}
