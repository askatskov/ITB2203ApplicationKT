namespace ITB2203Application.Model
{
    public class Session
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string AuditoriumName { get; set; }
        public DateTime StartTime { get; set; }

		public List<Ticket> Tickets { get; set; } = new();
		public Movie? Movie { get; set; }
	}
}
