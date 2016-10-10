using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Xml;

namespace tcrip4lib
{
	public class Relay
	{
		public int Number = 1;
		public string Name { get; set; }
		public bool On { get; set; }
	}

	public class Status
	{
		public List<Relay> Relais { get; set; } = new List<Relay>();
		public bool Pot { get; } = false;
	}

	public class TCRIP4
	{
		public string Host { get; set; } = "192.168.0.201";
		public int RelayCount { get; set; } = 4;

		private string status_url = "/status.xml";
		private string toggle_url = "/leds.cgi";


		public TCRIP4()
		{
		}

		public async Task<bool> SwitchOn(int number)
		{
			var status = await GetStatus();
			if (status.Relais.Find((Relay obj) => obj.Number == number).On == false)
			{
				var on = await ToggleRelay(number);

				return on;
			}

			return true;
		}

		public async Task<bool> SwitchOff(int number)
		{
			var status = await GetStatus();
			if (status.Relais.Find((Relay obj) => obj.Number == number).On == true)
			{
				var on = await ToggleRelay(number);

				return on;
			}

			return false;
		}

		public async Task<bool> ToggleRelay(int number)
		{
			HttpClient httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("http://" + Host);

			// WTF
		 	await httpClient.PostAsync(toggle_url + "?led=" + number, new StringContent(""));

			var status = await GetStatus();
			return status.Relais.Find((Relay obj) => obj.Number == number).On;
		}

		public async Task<Status> GetStatus()
		{
			HttpClient httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri("http://" + Host);

			Status status = new Status();

			var ret = await httpClient.GetAsync(status_url);
			var xmlstring = await ret.Content.ReadAsStringAsync();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlstring);
			var lednodes = doc.SelectNodes("/response/*[starts-with(name(), 'led')]");

			int relayId = 1;
			foreach (XmlNode node in lednodes)
			{
				Relay relay = new Relay();
				relay.Number = relayId;
				relay.Name = $"Relay { relayId }";
				relay.On = (node.InnerText == "1") ? true : false;

				status.Relais.Add(relay);

				relayId++;
			}

			return status;
		}

		public async Task<int> GetNumberOfRelais()
		{
			var status = await GetStatus();

			return status.Relais.Count;
		}
	}
}
