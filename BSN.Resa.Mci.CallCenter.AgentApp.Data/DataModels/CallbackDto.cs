using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
	public class CallbackDto
	{
		public CallbackDto(string number, double rank)
		{
			Number = number;
			Rank = rank;	
		}

		public string Number
		{
			get;
		}

		public double Rank
		{
			get;
		}
	}
}
