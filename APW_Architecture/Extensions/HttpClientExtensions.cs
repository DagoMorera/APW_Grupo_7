namespace APW.Architecture.Extensions;

// Metodos de extension para HttpClient
public static class HttpClientExtensions
{
    // Agrega (o reemplaza) un header por defecto en el HttpClient
    public static void AddDefaultRequestHeader(this HttpClient client, string name, string value)
    {
        var defaultHeaders = client.DefaultRequestHeaders;
        if (defaultHeaders.Contains(name))
            defaultHeaders.Remove(name);
        defaultHeaders.Add(name, value);
    }
}