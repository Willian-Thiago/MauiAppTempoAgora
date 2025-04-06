using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation; // Adicionando namespace para verificar a conexão

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "dcbe9d56f0b513005f440e27aab0f826";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                $"q={cidade}&units=metric&appid={chave}&lang=pt_br";

            using (HttpClient client = new HttpClient())
            {

                try // Adicionando um bloco try-catch para lidar com exceções de rede
                {

                    HttpResponseMessage resp = await client.GetAsync(url);


                    // Verificando o código de status da resposta HTTP
                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        //Obtendo a visibilidade em metros e convertendo para km
                        int visibilidadeEmMetros = (int)rascunho["visibility"];
                        double visibilidadeEmQuilometros = visibilidadeEmMetros / 1000.0;

                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],

                            // armazenando a visibilidade convertida em km
                            visibility = (int)Math.Round(visibilidadeEmQuilometros),

                            sunrise = sunrise.ToString("HH:mm"),
                            sunset = sunset.ToString("HH:mm"),
                        }; // Fecha o objeto do Tempo
                    }
                    // Verificando se o código de status indica que a cidade não foi encontrada (HTTP 404)
                    else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Retorna null para sinalizar que a cidade não foi encontrada
                        return null;
                    }
                    // Outros códigos de erro podem ser tratados aqui, se necessário
                    else
                    {
                        // Para outros erros, também retornamos null
                        return null;
                    }
                }
                // Captura exceções que podem ocorrer durante a comunicação HTTP, como falta de internet
                catch (HttpRequestException)
                {
                    // Retorna null para sinalizar que houve um erro de conexão
                    return null;
                }               
            } // fecha laço using
             
            return t;
        }       
    }
}
