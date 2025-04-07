using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
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

                        string mapa = $"https://embed.windy.com/embed.html?" +
                                      $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                                      $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                                      $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}";

                        wv_mapa.Source = mapa;

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

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude}";

                    lbl_coords.Text = local_disp;

                    // pega nome da cidade que está nas coordenadas
                    GetCidade(local.Latitude, local.Longitude);

                } else
                {
                    lbl_coords.Text = "Nenhuma localização";
                }

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch(PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void GetCidade(double lat, double lon)
        {

            try
            {

                IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);

                Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }
            }catch(Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da Cidade", ex.Message, "Ok");
            }
        }
 

    }
}
