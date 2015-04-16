﻿using System;

namespace BuildRevisionCounter.Contract
{
	public class Revision
	{
        public string Id { get; set; }
		
		public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public long NextNumber { get; set; }
	}
}