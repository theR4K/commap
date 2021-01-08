using System;
using MongoDB.Entities;
using System.Collections.Generic;
using System.Text;

namespace commapModels.Models
{
    interface IMeet
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Coordinates2D Location { get; set; }

        public string Subject { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int MaxMembers { get; set; }

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
