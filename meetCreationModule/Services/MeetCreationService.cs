using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using commapModels.Models;
using MongoDB.Driver;

namespace meetCreationModule.Services
{
    public class MeetCreationService
    {
        private static UpdateDefinition<Meet> deleteMeet = Builders<Meet>.Update.Set(p => p.State, MeetState.deleted);
        private readonly IMongoCollection<Meet> _meets;

        public MeetCreationService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meets = database.GetCollection<Meet>(settings.CollectionName);
        }

        public Meet Create(Meet meet)
        {
            _meets.InsertOne(meet);
            return meet;
        }

        public Meet Update(string id, Meet meetIn)
        {
            try
            {
                ReplaceOneResult ror = _meets.ReplaceOne(meet => meet.Id == id, meetIn);
                if (ror.ModifiedCount != 1)
                    return null;
            }
            catch(FormatException ex)
            {
                Console.WriteLine("UPDATE, id: \"" + id + "\" has wrong format");
                return null;
            }
            return meetIn;
        }

        public bool Remove(string id)
        {
            try
            {
                UpdateResult ur = _meets.UpdateOne(meet => meet.Id == id, deleteMeet);
                if (ur.ModifiedCount != 1)
                    return false;
            }catch(FormatException ex)
            {
                Console.WriteLine("DELETE, id: \"" + id + "\" has wrong format");
                return false;
            }
            return true;
        }
    }
}
