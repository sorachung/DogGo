using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hmmm... You should really add a Name...")]
        [MaxLength(35)]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [DisplayName("Neighborhood")]
        public int NeighborhoodId { get; set; }

        [Url]
        [DisplayName("Image URL")]
        public string ImageUrl { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Walks> Walks { get; set; }
        public string TotalWalkTime
        {
            get
            {
                if (Walks != null)
                {
                    TimeSpan ts2 = TimeSpan.FromSeconds(Walks.Sum(walk => walk.Duration));
                    return string.Format("{0:D2} hr {1:D2} min", ts2.Hours, ts2.Minutes);
                }
                return null;
            }
        }

    }
}