using System;
using commapModels.Models;
using MongoDB.Driver;
using FluentScheduler;
using meetCreationModule.Models;

namespace meetCreationModule.Services
{
    public class MeetCreationService
    {
        private static UpdateDefinition<Meet> deleteMeet = Builders<Meet>.Update.Set(p => p.State, MeetState.deleted);
        private static UpdateDefinition<Meet> activeMeet = Builders<Meet>.Update.Set(p => p.State, MeetState.active);
        private static UpdateDefinition<Meet> endMeet = Builders<Meet>.Update.Set(p => p.State, MeetState.ended);

        private readonly IMongoCollection<Meet> _meets;

        public MeetCreationService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meets = database.GetCollection<Meet>(settings.CollectionName);
            JobManager.Initialize();
            Console.WriteLine("MeetCreationService: created");
            SheduleStateUpdates();
            UpdateStates();
        }

        public Meet Create(Meet meet)
        {
            _meets.InsertOne(meet);
            JobManager.AddJob(UpdateStates, (s) => s.NonReentrant().ToRunOnceAt(meet.StartTime));
            JobManager.AddJob(UpdateStates, (s) => s.NonReentrant().ToRunOnceAt(meet.EndTime));
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

        //to shedule updates for data that already in DB
        public void SheduleStateUpdates()
        {
            _meets.FindSync(meet => meet.State == MeetState.planned && meet.StartTime >= DateTime.Now)
                .ForEachAsync(meet => {
                    JobManager.AddJob(UpdateStates, (s) => s.NonReentrant().ToRunOnceAt(meet.StartTime));
                    JobManager.AddJob(UpdateStates, (s) => s.NonReentrant().ToRunOnceAt(meet.EndTime));
                    }
                    );
            _meets.FindSync(meet => meet.State == MeetState.active && meet.EndTime >= DateTime.Now)
                .ForEachAsync(meet => JobManager.AddJob(UpdateStates, (s) => s.NonReentrant().ToRunOnceAt(meet.EndTime)));
        }

        public void UpdateStates()
        {
            _meets.UpdateMany(meet => meet.State == MeetState.planned && meet.StartTime < DateTime.Now, activeMeet);
            _meets.UpdateMany(meet => meet.State == MeetState.active && meet.EndTime < DateTime.Now, endMeet);
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
