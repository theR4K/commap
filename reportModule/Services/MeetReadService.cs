using System.Collections.Generic;
using commapModels.Models;
using reportModule.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace reportModule.Services
{
    public class MeetReadService
    {
        private readonly IMongoCollection<Meet> _meets;

        public MeetReadService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meets = database.GetCollection<Meet>(settings.CollectionName);
        }

        public List<Meet> Get()
        {
            return _meets.FindSync(Meet => true).ToList();
        }

        public List<Meet> Get(FilterDefinition<Meet> filter)
        {
            return _meets.FindSync(filter).ToList();
        }
    }
}
