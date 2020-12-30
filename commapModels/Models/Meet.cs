using System;
using MongoDB.Bson;
using MongoDB.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace commapModels.Models
{
    public class Meet
    {
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

        //[JsonIgnore] //used to not allow changing by client //!!
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(MeetState.planned)]
        public MeetState State { get; set; }
    }

    public enum MeetState : Int32
    {
        deleted = -1,
        ended = 0,
        planned = 1,
        active = 2
    }
}
