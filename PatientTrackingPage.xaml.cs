using MedicalWPF_Session_2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MedicalWPF_Session_2
{
    /// <summary>
    /// Логика взаимодействия для PatientTrackingPage.xaml
    /// </summary>
    public partial class PatientTrackingPage : Window
    {
        public PatientTrackingPage()
        {
            InitializeComponent();
            DrawEmployeeEllipses();
            _ = UpdatePeriodically();
        }

        private const string API_PERSON_MOVE_URL = "http://localhost:5036/api/PersonMoves";

        private List<PersonMove> personMoves = new List<PersonMove>();

        public async void DrawEmployeeEllipses()
        {
            try
            {
                personMoves = new List<PersonMove>();

                using (HttpClient client = new HttpClient())
                {
                    var responce = await client.GetAsync(API_PERSON_MOVE_URL);

                    if (!responce.IsSuccessStatusCode)
                    {
                        MessageBox.Show(responce.ReasonPhrase.ToString());
                    }
                    else
                    {
                        personMoves = JsonConvert.DeserializeObject<List<PersonMove>>(await responce.Content.ReadAsStringAsync());
                    }

                    foreach (var move in personMoves)
                    {
                        var targetedStackPanel = FindStackPanelByTag(MainGrid, int.Parse(move.LastSecurityPointNumber.ToString()));

                        if (targetedStackPanel != null)
                        {
                            Ellipse ellipse = new Ellipse()
                            {
                                Width = 20,
                                Height = 20,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Fill = move.IdPersonRole == 1 ? Brushes.Green : Brushes.Blue,
                            };
                            targetedStackPanel.Children.Add(ellipse);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка: стэк не найден!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private StackPanel FindStackPanelByTag(Grid container, int tagValue)
        {
            foreach (var child in container.Children)
            {
                if (child is StackPanel stackPanel && int.Parse((string)stackPanel.Tag) == tagValue)
                {
                    return stackPanel;
                }

            }
            return null;
        }

        private void DeleteEllipses()
        {
            foreach (var stackPanel in MainGrid.Children)
            {
                if (stackPanel is StackPanel stackpanel)
                {
                    stackpanel.Children.Clear();
                }
            }
        }

        private async Task UpdatePeriodically()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(20));

                DeleteEllipses();
                DrawEmployeeEllipses();
            }
        }
    }
}
