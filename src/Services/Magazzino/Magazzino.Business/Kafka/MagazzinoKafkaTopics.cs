using Microsoft.Extensions.DependencyInjection;

namespace Magazzino.Business.Kafka;

// Topic che il Magazzino ASCOLTA
public class MagazzinoKafkaTopics : AbstractKafkaTopics
{
    public string OrdineCreato { get; set; } = "OrdineCreato";
    public string OrdineAnnullato { get; set; } = "OrdineAnnullato";
    public string RiassortimentoCompletato { get; set; } = "RiassortimentoCompletato";

    public override IEnumerable<string> GetTopics()
        => new List<string> { OrdineCreato, OrdineAnnullato, RiassortimentoCompletato };
}
