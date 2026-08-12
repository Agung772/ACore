#if SUPABASE

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ACore
{
    [Table("GameDatabase")]
    public class GameDatabase : BaseModel
    {
        [PrimaryKey("ID")]
        public string Id { get; set; }
        
        public string GameData { get; set; }
    }
}


#endif