﻿using Cadastro_de_receitas_Máquina_de_Oleo.Database;
using Cadastro_de_receitas_Máquina_de_Oleo.Models;
using Cadastro_de_receitas_Máquina_de_Oleo.Types;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cadastro_de_receitas_Máquina_de_Oleo.Components
{
    public partial class AppPartnumber : UserControl
    {
        private readonly Db db;
        public ObservableCollection<Partnumber> _partnumberList = [];
        public int Index { get; set; }

        public AppPartnumber()
        {
            InitializeComponent();

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            LoadPartnumberList();

            _partnumberList.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    dgPartnumber.Items.Refresh();
            };
        }

        private void LoadPartnumberList()
        {
            _partnumberList.Clear();
            _partnumberList = db.LoadPartnumberList();
            dgPartnumber.ItemsSource ??= _partnumberList;

            DataContext = this;
        }

        public void UpdatePartnumberList()
        {
            try
            {
                _partnumberList.Clear();
                _partnumberList = db.LoadPartnumberList();

                dgPartnumber.ItemsSource = _partnumberList;
                dgPartnumber.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro inexperado ao atulizar lista de partnumbers. " + ex.Message,
                    "Atenção"
                );
            }
        }

        private void GetItem()
        {
            try
            {
                var filter = tbFilter.Text;

                IEnumerable<Partnumber> item = _partnumberList.Where(p => p.Code == filter);

                if (item.Any())
                {
                    dgPartnumber.ItemsSource = item;
                }
                else
                {
                    MessageBox.Show("Desenho não encontrado.", "Atenção");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro inexperado ao buscar desenho.", "Erro");
            }
        }

        private void OnTbFilterChange(object sender, RoutedEventArgs e)
        {
            if (tbFilter.Text.Length > 0)
            {
                BtnFindPartnumber.Foreground = Brushes.LawnGreen;
            }
            else
            {
                BtnFindPartnumber.Foreground = Brushes.DarkGray;
                BtnRemoveFilter.Foreground = Brushes.DarkGray;
                BtnRemoveFilter.IsEnabled = false;
            }
        }

        private void ChangeSelection(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Partnumber partnumber = (Partnumber)button.DataContext;

            Index = _partnumberList.IndexOf(partnumber);
        }

        private async void DeletePartnumber(object sender, RoutedEventArgs e)
        {
            ChangeSelection(sender, e);
            MessageBoxResult result = MessageBox.Show(
                "Deseja realmente excluir este partnumber?",
                "Excluir partnumber",
                MessageBoxButton.YesNo
            );
            if (result == MessageBoxResult.Yes)
            {
                bool isDeleted = await db.DeletePartnumber(_partnumberList[Index].Code);
                if (!isDeleted)
                    return;

                _partnumberList.RemoveAt(Index);

                MessageBox.Show("Partnumber excluído com sucesso!");
            }

            dgPartnumber.Items.Refresh();
        }

        private void EditPartnumber(object sender, RoutedEventArgs e)
        {
            try
            {
                var pnindex = (Button)e.OriginalSource;

                if (pnindex.DataContext is Partnumber parnumberObj)
                {

                    EditPartnumber editPartnumber = new(this, parnumberObj, Context.Update);
                    var parentWindow = Window.GetWindow(this) as MainWindow;
                    parentWindow?.Main?.Children.Clear();
                    parentWindow?.Main.Children.Add(editPartnumber);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, Name, MessageBoxButton.OK);
            }
        }

        private void SearchPartnumber(object sender, RoutedEventArgs e)
        {
            if (tbFilter.Text.Length == 0)
                return;

            BtnRemoveFilter.IsEnabled = true;
            BtnRemoveFilter.Foreground = Brushes.Red;

            GetItem();
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            tbFilter.Clear();
            UpdatePartnumberList();
        }

        private void CreatePartnumber(object sender, RoutedEventArgs e)
        {
            EditPartnumber editPartnumber = new(this, new Partnumber("", "", "", "", 0), Context.Create);
            var parentWindow = Window.GetWindow(this) as MainWindow;
            parentWindow?.Main?.Children.Clear();
            parentWindow?.Main.Children.Add(editPartnumber);
        }
    }
}
