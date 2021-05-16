using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//aggiunta
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WpfAppSocket2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //indirizzo ip+source port del dispositivo
            IPEndPoint localendpoint = new IPEndPoint(IPAddress.Parse("192.168.178.21"), 56000);

            //dichiarazione+inizializzazione del thread 
            Thread t1 = new Thread(new ParameterizedThreadStart(SocketReceive2));
            //avvio thread
            t1.Start(localendpoint);


        }
        //metodo del thread
        public async void SocketReceive2(object sourceEndPoint) //parametro indirizzo locale 
        {
            IPEndPoint sourceEP = (IPEndPoint)sourceEndPoint;

            Socket t = new Socket(sourceEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //associa il socket a sourceEP
            t.Bind(sourceEP);

            Byte[] byteRicevuti = new byte[256];
            string message = "";

            int bytes = 0;

            await Task.Run(() =>
            {
                //ciclo infinito con ricezione dell'indiirizzo
                while (true)
                {
                    if (t.Available > 0)
                    {
                        message = "";
                        bytes = t.Receive(byteRicevuti, byteRicevuti.Length, 0);
                        message = message + Encoding.ASCII.GetString(byteRicevuti, 0, bytes);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //messaggio ricevuto
                            lblRicezione.Content = message;
                        }));
                    }


                }

            });
        }
        //invio del messaggio
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //try in caso di errore inserimento input
            try
            {
                //ip di destinazione
                IPAddress ipDest = IPAddress.Parse(txtIpAdd.Text);
                //porta di destinazione
                int portDest = int.Parse(txtDestPort.Text);

                IPEndPoint remoteEndPoint = new IPEndPoint(ipDest, portDest);

                Socket s = new Socket(ipDest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                //byte da inviare
                Byte[] byteInviati = Encoding.ASCII.GetBytes(txtMsg.Text);
                //invio messaggio alla porta scelta
                s.SendTo(byteInviati, remoteEndPoint);
            }
            catch
            {
                //caso di errore
                MessageBox.Show("Errore inserimento destinazione");
            }
        }
    }
}
