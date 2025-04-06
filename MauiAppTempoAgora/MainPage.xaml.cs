using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Net.NetworkInformation;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_cidade.Text))
            {
                // Verificando a conexão com a internet de forma mais robusta
                if (!VerificarConexao())
                {
                    // Certificando-se de que o DisplayAlert é chamado na thread principal
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Alerta", "Você está sem conexão com a internet. Verifique sua conexão e tente novamente.", "Ok");
                    });
                    return; // Interrompe a execução do método se não houver conexão
                }

                try
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat}º \n " +
                                         $"Longitude: {t.lon}º \n " +
                                         $"Descrição: {t.description} \n" +
                                         $"Velocidade do vento: {t.speed} m/s\n" +
                                         $"Visibilidade: {t.visibility} Km \n" +
                                         $"Nascer do Sol: {t.sunrise} \n " +
                                         $"Por do Sol: {t.sunset} \n " +
                                         $"Temp Máx: {t.temp_max}º \n " +
                                         $"Temp Mín: {t.temp_min}º \n ";

                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        await DisplayAlert("Aviso", "Cidade não encontrada. Por favor, verifique o nome da cidade.", "Ok");
                        lbl_res.Text = "Dados da Previsão.";
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ops", "Ocorreu um erro inesperado: " + ex.Message, "Ok");
                    lbl_res.Text = "Erro ao buscar a previsão.";
                }
            }
            else
            {
                lbl_res.Text = "Preencha a cidade.";
            }
        }

        // Método de verificação de conexão diretamente na MainPage
        private bool VerificarConexao()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return true;
            }

            return false;
        }
    }
}
