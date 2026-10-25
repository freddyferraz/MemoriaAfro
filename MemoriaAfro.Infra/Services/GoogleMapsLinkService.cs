using MemoriaAfro.Core.Services;
using MemoriaAfro.Core.Struct;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace MemoriaAfro.Infra.Services;


/// <summary>Extração pura (sem rede) a partir do texto da URL.</summary>
public static class GoogleMapsLinkParser
{
    private const string Num = @"-?\d+(?:\.\d+)?";

    // !3d<lat>!4d<lng> = posição exata do pin do local (a mais confiável)
    private static readonly Regex PinRegex =
        new($@"!3d({Num})!4d({Num})", RegexOptions.Compiled);

    // @<lat>,<lng> = centro do mapa na tela (fallback)
    private static readonly Regex ArrobaRegex =
        new($@"@({Num}),({Num})", RegexOptions.Compiled);

    private static readonly Regex ParLatLngRegex =
        new($@"^\s*({Num})\s*,\s*({Num})\s*$", RegexOptions.Compiled);

    private static readonly string[] ParametrosComCoordenadas =
        { "q", "query", "destination", "ll", "center", "daddr" };

    public static Coordenadas? TentarExtrair(string link)
    {
        if (string.IsNullOrWhiteSpace(link)) return null;

        // O link pode ter %2C no lugar da vírgula, etc.
        var texto = Uri.UnescapeDataString(link);

        var m = PinRegex.Match(texto);
        if (m.Success) return Criar(m.Groups[1].Value, m.Groups[2].Value);

        m = ArrobaRegex.Match(texto);
        if (m.Success) return Criar(m.Groups[1].Value, m.Groups[2].Value);

        if (Uri.TryCreate(link, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Query))
        {
            var query = HttpUtility.ParseQueryString(uri.Query);
            foreach (var nome in ParametrosComCoordenadas)
            {
                var valor = query[nome];
                if (string.IsNullOrWhiteSpace(valor)) continue;

                var par = ParLatLngRegex.Match(valor);
                if (par.Success) return Criar(par.Groups[1].Value, par.Groups[2].Value);
            }
        }

        return null;
    }

    private static Coordenadas? Criar(string lat, string lng)
    {
        if (!double.TryParse(lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var la) ||
            !double.TryParse(lng, NumberStyles.Float, CultureInfo.InvariantCulture, out var lo))
            return null;

        if (la is < -90 or > 90 || lo is < -180 or > 180) return null;

        return new Coordenadas(la, lo);
    }
}

public class GoogleMapsLinkService : IGoogleMapsLinkService
{
    private const int MaxRedirecionamentos = 5;

    // Só seguimos redirecionamentos dentro destes domínios (evita SSRF).
    private static readonly string[] HostsPermitidos =
    {
        "maps.app.goo.gl", "goo.gl", "google.com", "google.com.br"
    };

    private readonly HttpClient _http;

    public GoogleMapsLinkService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Coordenadas?> ExtrairCoordenadasAsync(string link, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(link?.Trim(), UriKind.Absolute, out var atual)) return null;

        for (var i = 0; i <= MaxRedirecionamentos; i++)
        {
            // 1) Tenta extrair direto do texto da URL atual.
            var coordenadas = GoogleMapsLinkParser.TentarExtrair(atual.ToString());
            if (coordenadas is not null) return coordenadas;

            // 2) Sem coordenadas: só segue se for um domínio do Google, por HTTPS.
            if (!HostPermitido(atual)) return null;

            using var req = new HttpRequestMessage(HttpMethod.Get, atual);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

            var status = (int)resp.StatusCode;
            if (status is >= 300 and < 400 && resp.Headers.Location is { } destino)
            {
                atual = destino.IsAbsoluteUri ? destino : new Uri(atual, destino);
                continue;
            }

            return null; // Resposta final sem coordenadas na URL.
        }

        return null;
    }

    private static bool HostPermitido(Uri uri)
    {
        if (uri.Scheme != Uri.UriSchemeHttps) return false;

        var host = uri.Host.ToLowerInvariant();
        return HostsPermitidos.Any(h => host == h || host.EndsWith("." + h));
    }
}