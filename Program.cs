using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var endpointUrls = new Dictionary<string, Dictionary<string, object>> {
            { "https://api-dev.centralnacionalunimed.com.br/appcnu/api/v1.2/BuscarDadosContrato/PessoaFisica/08650003673945090",  null },
            { "https://api.centralnacionalunimed.com.br/appcnu/api/v1.0/BuscarCarencias/08650002486437313",  null },
            { "https://api.centralnacionalunimed.com.br/appcnu/api/v2.0/BuscarCartaoBeneficiario/08650003124196005",  null },
            { "https://api-dev.centralnacionalunimed.com.br/appcnu/api/v1.0/BuscarDadosBeneficiario/08650003124196005",  null },
            { "https://api-dev.centralnacionalunimed.com.br/beneficiario/api/v2.0/BuscarNumAssociadoCpf/24710579261",  null },
            { "https://api-dev.centralnacionalunimed.com.br/appcnu/api/v1.0/ValidarLogin", new Dictionary<string, object> { { "login", "08650003676091009" }, { "senha", "unimed00" } } }
        };

        var authorizationToken = "Y251YXBpOjRadXIzQFAx";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authorizationToken}");

            await Task.WhenAll(endpointUrls.Select(async endpointUrl =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    HttpResponseMessage response;

                    if (endpointUrl.Value == null)
                    {
                        response = await httpClient.GetAsync(endpointUrl.Key);
                    }
                    else
                    {
                        var content = new StringContent(JsonSerializer.Serialize(endpointUrl.Value), Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync(endpointUrl.Key, content);
                    }

                    var elapsedTime = stopwatch.Elapsed;
                    var elapsedTimeFormatted = elapsedTime.ToString("s\\.fff");
                    var comBrIndex = endpointUrl.Key.IndexOf(".com.br/") + ".com.br/".Length;

                    if (response.IsSuccessStatusCode)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{endpointUrl.Key.Substring(comBrIndex)} {elapsedTime.TotalMilliseconds:F2} ms {response.StatusCode}.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{endpointUrl.Key.Substring(comBrIndex)} {response.StatusCode} {elapsedTime.TotalMilliseconds:F2} ms {response.StatusCode}.");
                        Console.ResetColor();
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ocorreu um erro ao tentar acessar o endpoint {endpointUrl.Key}: {ex.Message}");
                    Console.ResetColor();
                }
            }));
        }
    }
}
