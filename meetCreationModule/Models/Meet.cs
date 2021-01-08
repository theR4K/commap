using System;
using commapModels.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Newtonsoft.Json;
using System.ComponentModel;

namespace meetCreationModule.Models
{
    public class Meet: IMeet
    {
        [JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public Coordinates2D Location { get; set; }

        public string Subject { get; set; }

        [JsonRequired]
        public DateTime StartTime { get; set; }

        [JsonRequired]
        public DateTime EndTime { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(-1)]
        public int MaxMembers { get; set; }

        [JsonIgnore] //used to not allow changing by client
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(MeetState.planned)]
        public MeetState State { get; set; }
    }
}
