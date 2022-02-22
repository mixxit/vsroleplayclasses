using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace vsroleplayclasses.src
{
    [JsonObject]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Ability
    {
        public static Ability Create(long id, List<MagicaPower> wordsOfMagic, string characterName, string creatorUid)
        {
            return new Ability()
            {
                Id = id,
                WordsOfMagic = wordsOfMagic,
                CreatorUid = creatorUid,
                Name = GenerateName(characterName, wordsOfMagic)
            };
        }

        private static string GenerateName(string characterName, List<MagicaPower> wordsOfMagic)
        {
            return $"{characterName}'s {string.Join(" ", wordsOfMagic.Select(e => e.ToString()))}";
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string CreatorUid { get; set; }
        public List<MagicaPower> WordsOfMagic { get; set; }


        internal void Cast(Entity target)
        {

        }
    }
}
