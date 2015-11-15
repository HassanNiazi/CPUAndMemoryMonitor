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
using System.Threading;
using System.Diagnostics;
using System.Speech.Synthesis;
namespace CpuAndMemMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        PerformanceCounter perfCountCPULoad = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        PerformanceCounter perfCountCPUTemp = new PerformanceCounter("Thermal Zone Information", "Temperature", @"\_TZ.TZS0");
        PerformanceCounter perfCountSysMem = new PerformanceCounter("Memory", "Available MBytes");
        Thread updateStats;
        SpeechSynthesizer jarvis = new SpeechSynthesizer();
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        
        public MainWindow()
        {
            
            InitializeComponent();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            jarvis.SelectVoiceByHints(VoiceGender.Female,VoiceAge.Teen);

            jarvis.SpeakAsync("Application Started");
            // Code 
            //    Thread updateStats = new Thread(MainFunctions);
            //    updateStats.Start();

            ni.Icon = new System.Drawing.Icon("Main.ico");
            ni.Visible = true;
            ni.Click += Ni_Click;



        }

        private void Ni_Click(object sender, EventArgs e)
        {
          MessageBoxResult res =  MessageBox.Show("Exit Application?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(res == MessageBoxResult.Yes)
            {
                if(updateStats.IsAlive)
                {
                    updateStats.Abort();
                }
                this.Close();
            }
        }

        private void MainFunctions()
        {

          while (true)
            {
                try {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        CPULoad.Text = ((int)perfCountCPULoad.NextValue()).ToString() + "%";
                        CPUTemp.Text = ((int)(perfCountCPUTemp.NextValue() - 273)).ToString() + "C";
                        if (perfCountCPUTemp.NextValue()-273 > 85)
                            jarvis.SpeakAsync("Warning!!! High CPU temperature Reached");
                        
                        Memory.Text = ((int)perfCountSysMem.NextValue()).ToString() + "MB";
                    }));
                }
                catch
                {
                }
                Thread.Sleep(1200);

            }
        }

        private void mainWindow_Activated(object sender, EventArgs e)
        {
        // PerformanceCounter perfCountCPULoad = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
             updateStats = new Thread(MainFunctions);
                updateStats.Start();
        }

        private void mainWindow_Deactivated(object sender, EventArgs e)
        {

        }
    }

}
