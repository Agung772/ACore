#if SUPABASE

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ACore
{
    [Table("players")]
    public class PlayerData : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; }
        
        public string GameData { get; set; }

    }
}


#endif