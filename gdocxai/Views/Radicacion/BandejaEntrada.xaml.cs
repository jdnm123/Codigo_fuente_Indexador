using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Gestion.DAL;
using MaterialDesignThemes.Wpf;

namespace Indexai.Views.Radicacion
{
    /// <summary>
    /// Interaction logic for BandejaEntrada.xaml
    /// </summary>
    public partial class BandejaEntrada : UserControl
    {

        public BandejaEntrada()
        {
            InitializeComponent();

            //Lista de Radicados Entrantes
            var listaRadicadosEntrantes = new ObservableCollection<tr_radicado>(EntitiesRepository.Entities.tr_radicado).ToList();
            foreach (var radicadoEntrante in listaRadicadosEntrantes)
            {
                String titulo = radicadoEntrante.asunto;
                String asunto = "Asunto Radicado";
                mesajesPanel.Children.Add(buildMensajeCard(titulo,asunto));
            }

        }

        private static Card buildMensajeCard(String titulo, String asunto)
        {
            Card card = new Card
            {
                Width = 250,
                Height = 70,
                Margin = new Thickness(3)
            };
            Grid cardGrid = new Grid();
            cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            cardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

            cardGrid.Children.Add(new TextBlock
            {
                Margin = new Thickness(5, 5, 0, 0),
                Text = titulo,
                Style = Application.Current.TryFindResource("MaterialDesignSubtitle1TextBlock") as Style,
                FontWeight = FontWeights.Bold
            });

            Button btn = new Button
            {
                Margin = new Thickness(0, 0, 16, -25),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Style = Application.Current.TryFindResource("MaterialDesignFloatingActionMiniAccentButton") as Style,
            };

            TextBlock txt = new TextBlock
            {
                Margin = new Thickness(10, 0, 58, 0),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = asunto,
            };

            btn.Content = new PackIcon
            {
                Kind = PackIconKind.Email, 
            };

            Grid.SetRow(btn, 0);
            Grid.SetRow(txt, 1);


            cardGrid.Children.Add(btn);
            cardGrid.Children.Add(txt);
            card.Content = cardGrid;
            return card;
        }

       
    }
}
