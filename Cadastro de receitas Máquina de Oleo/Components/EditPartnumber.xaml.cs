using Cadastro_de_receitas_Máquina_de_Oleo.Database;
using Cadastro_de_receitas_Máquina_de_Oleo.Models;
using Cadastro_de_receitas_Máquina_de_Oleo.Types;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Components
{
    /// <summary>
    /// Lógica interna para EditPartnumber.xaml
    /// </summary>
    public partial class EditPartnumber : UserControl
    {
        private readonly Db db;
        private readonly List<string> _tanks;
        private readonly List<int> _dosingTime;
        private readonly AppPartnumber _createPartNumber;
        private readonly Context context;
        public readonly string _selectedTank;
        public readonly int selectedDosingTime;

        public EditPartnumber(
            AppPartnumber createPartnumber,
            Partnumber partnumber,
            Context context
        )
        {
            InitializeComponent();
            DataContext = this;
            this.context = context;

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            if (context == Context.Update)
            {
                Title.Content = "Editar Partnumber";
            }
            else
            {
                Title.Content = "Cadastrar Partnumber";
            }

            _createPartNumber = createPartnumber;
            _selectedTank = partnumber.TypeOil;
            selectedDosingTime = partnumber.DosingTime;

            _tanks = ["Tanque 01", "Tanque 02", "Tanque 03"];
            CbTanks.ItemsSource = _tanks;

            _dosingTime = [1, 2];
            CbDosingTime.ItemsSource = _dosingTime;

            TbPartnumber.Text = partnumber.Code;
            TbDescription.Text = partnumber.Description;
            TbVolume.Text = partnumber.Volume;
            CbDosingTime.SelectedValue = partnumber.DosingTime;
            CbTanks.SelectedValue = partnumber.TypeOil;
        }

        private void SetInitialState()
        {
            TbPartnumber.Text = string.Empty;
            TbDescription.Text = string.Empty;
            TbVolume.Text = string.Empty;
            CbDosingTime.SelectedIndex = -1;
            CbTanks.SelectedIndex = -1;
        }

        private void Close()
        {
            AppPartnumber partnumber = new();
            var parentWindow = Window.GetWindow(this) as MainWindow;
            parentWindow?.Main?.Children.Clear();
            parentWindow?.Main.Children.Add(partnumber);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Volume_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Volume_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            // Permitir números, ponto e vírgula
            Regex regex = new Regex(@"^[0-9,]+$");
            return regex.IsMatch(text);
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dosingTime =
                    CbDosingTime.SelectedValue != null ? (int)CbDosingTime.SelectedValue : 0;
                string? tank =
                    CbTanks.SelectedValue != null ? CbTanks.SelectedValue.ToString() : string.Empty;

                if (
                    string.IsNullOrEmpty(TbPartnumber.Text)
                    || string.IsNullOrEmpty(TbDescription.Text)
                    || string.IsNullOrEmpty(TbVolume.Text)
                    || dosingTime == 0
                    || string.IsNullOrEmpty(tank)
                )
                {
                    MessageBox.Show("Preencha todos os campos antes de salvar!", "Atenção");
                    if (TbPartnumber.Text.Length != 15)
                        throw new ArgumentException(
                            "Partnumber deve ter 15 caracteres.",
                            nameof(TbPartnumber.Text)
                        );
                    return;
                }

                if (TbPartnumber.Text.Length != 15)
                    throw new ArgumentException(
                        "Partnumber deve ter 15 caracteres.",
                        nameof(TbPartnumber.Text)
                    );

                bool result = await db.SavePartnumber(
                    new(TbPartnumber.Text, TbDescription.Text, tank, TbVolume.Text, dosingTime)
                );

                if (!result)
                    return;

                MessageBox.Show(
                    $"Desenho {TbPartnumber.Text} {(context == Context.Create ? "cadastrado" : "atualizado")} com sucesso.",
                    "Sucesso"
                );

                _createPartNumber.UpdatePartnumberList();
                SetInitialState();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inexperado. " + ex.Message, "Erro");
            }
        }
    }
}
