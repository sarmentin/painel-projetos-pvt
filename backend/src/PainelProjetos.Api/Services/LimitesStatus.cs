namespace PainelProjetos.Api.Services;

// Limites da classificação de status (seção 4.3 do DESIGN.md).
// Os valores abaixo são o padrão; podem ser alterados pelo appsettings.json.
public class LimitesStatus
{
    public decimal ConsumoAtencao { get; set; } = 80;
    public decimal ConsumoCritico { get; set; } = 100;
    public decimal DesvioAtencao { get; set; } = 10;
    public decimal DesvioCritico { get; set; } = 20;
}