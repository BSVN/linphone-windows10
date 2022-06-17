using System;
using System.Collections.Generic;
using System.Text;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
	public class CallbackDto
	{
		public CallbackDto(string number)
		{
			Number = number;
		}

		public string Number
		{
			get;
		}
	}
}
