using System.Collections.Generic;
using commapModels.Models;
using MongoDB.Driver;

namespace commap.Services
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
            return _meets.FindSync<Meet>(meet => meet.State > MeetState.ended).ToList();
        }
    }
}
