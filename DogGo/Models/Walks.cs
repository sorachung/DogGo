using System;

namespace DogGo.Models
{
    public class Walks
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int WalkerId { get; set; }
        public int DogId { get; set; }
        public Walker Walker { get; set; }
        public Dog Dog { get; set; }
        public string DurationFormated
        {
            get
            {
                return string.Format("{0} min", Duration/60);
            }
        }

    }
}
