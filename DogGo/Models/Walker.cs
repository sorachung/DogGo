using System.Collections.Generic;
using System.Linq;
using System;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NeighborhoodId { get; set; }
        public string ImageUrl { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Walks> Walks { get; set; }
        public string TotalWalkTime
        {
            get
            {
                TimeSpan ts2 = TimeSpan.FromSeconds(Walks.Sum(walk => walk.Duration));
                return string.Format("{0:D2} hr {1:D2} min", ts2.Hours, ts2.Minutes);
            }
        }

    }
}