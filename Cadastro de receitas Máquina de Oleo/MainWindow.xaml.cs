using Cadastro_de_receitas_Máquina_de_Oleo.Components;
using System.Windows;

namespace Cadastro_de_receitas_Máquina_de_Oleo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Children.Add(new AppPartnumber());
        }
    }
}