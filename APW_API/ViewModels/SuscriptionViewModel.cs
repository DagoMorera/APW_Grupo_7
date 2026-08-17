namespace APW.Api.ViewModels;

// Contrato publico de la Api para Subscription
public class SubscriptionViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SourceId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Datos que se envian para suscribirse/desuscribirse
public class ToggleSubscriptionViewModel
{
    public int UserId { get; set; }
    public int SourceId { get; set; }
}

// Estado resultante despues del toggle
public class ToggleSubscriptionResultViewModel
{
    public bool Subscribed { get; set; }
}