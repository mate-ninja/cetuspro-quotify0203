using Npgsql.Internal.Postgres;

namespace cetuspro0203.Entities
{
    public class Cytaty
    {
        public int Id { get; set; }
        public required string Cytat { get; set; }
        public required string Autor { get; set; }
        public required DateTime CzasUtworzenia { get; set; }
    }
}
