﻿namespace ITB2203Application.Model
{
    public class Ticket
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string SeatNo { get; set; }
        public float Price { get; set; }
		public Session? Session { get; set; }
	}
}
