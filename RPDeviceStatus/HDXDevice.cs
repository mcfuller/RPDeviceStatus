/*
 * 	File:		HDXDevice.cs
 * 	Author:		Matthew Fuller
 * 	Desc:		NOTE: Various methods of the HDXDevice class have been modified to work within the
 * 				Mono Framework for an iOS application. 
 * 
 * 				HDXDevice Class: Contains methods for interacting with and administering Polycom HDX
 * 				systems using the CGI API command structure (HTTP). 
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace RPDeviceStatus
{

	public class HDXDevice : RPDevice
	{
		public bool showXML {get; set;}

		public HDXDevice(string ip, string un, string pw)
			:base(ip, un, pw)
		{
			// Calls base constructor to set ipAddress, username, password
		}

		public string prepareApiUrl(string command)
		{
			var url = "http://" + ipAddress + "/a_apicommand.cgi?apicommand=" + command;
			return url;
		}

		// Execute API command, return response as string
		public override string doApiCommand(string url)
		{
			var response = "";
			// Send web request and parse XML response
			try
			{
				var request = prepareRequest(url);
				// Check for verbose output option
				if (showXML)
				{
					// Output unparsed XML response from HDX web server
					using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
					{
						string output = sr.ReadToEnd();
						response = output.Replace("\n", Environment.NewLine);
					}
				}
				else
				{
					// Parse the XML for the response status code and response string
					var xr = XmlReader.Create(request.GetResponse().GetResponseStream());
					while (xr.Read())
					{
						if (xr.NodeType == XmlNodeType.Element & xr.Name == "command")
						{
							response = xr.GetAttribute("status");
							xr.ReadToNextSibling("result");
							xr.Read();
							response += (Environment.NewLine + xr.Value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return response;
		}

		// Execute specified CGI command, return output as string
		public string doCgiCommand(string command, string payload)
		{
			string response = "";
			//if (isPinging()) {
			// Send web request and parse XML response
			try
			{
				var request = prepareRequest("http://" + ipAddress + "/" + command);

				// If we need to send data other than the cgi command itself, write it to stream
				if (!payload.Equals(""))
				{
					request.Method = "POST";
					byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(payload);
					request.ContentLength = dataBytes.Length;
					request.ContentType = "application/x-www-form-urlencoded";
					Stream dataStream = request.GetRequestStream();
					dataStream.Write(dataBytes, 0, dataBytes.Length);
					dataStream.Close();
				}
				using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					response = sr.ReadToEnd();
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}

			return response;
		}

		// Acquire valid session_id
		public override bool getSession()
		{
			var success = false;
			if (sessionID == "")
			{
				try
				{
					// Send dummy request to capture session cookie (if present)
					string targetURL = "http://" + ipAddress + "/a_makeacall.htm";
					var request = (HttpWebRequest)WebRequest.Create(targetURL);

					// Since there's no ping for iOS / Mono
					request.Timeout = 5000;
					var cache = new CredentialCache();

					cache.Add(new Uri(targetURL), "Digest", new NetworkCredential(username, password));
					request.Credentials = cache;
					request.AllowAutoRedirect = false;

					// Ignore obnoxious SSL failure generated by ALL Polycom devices because of self-signed certs
					ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

					var response = (HttpWebResponse)request.GetResponse();
					if (response != null)
					{
						sessionID = response.Headers.Get("Set-cookie");
						success = true;
					}
					response.Close();
				}

				catch (Exception ex)
				{
					throw ex;
				}
			}
			//cookieHeader = "Cookie: " + sessionID;
			return success;
		}

		// Check HDX call status
		public bool isInCall()
		{
			string response = "";
			// Send web request and parse XML response
			try
			{
				var request = prepareRequest("http://" + ipAddress + "/a_iscallconnected.cgi");
				using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					response = sr.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			if (response.Equals("0"))
			{
				return false;
			}
			// Assume in call unless proven otherwise
			return true;
		}

		// Time stamp for UI output
		string timeStamp()
		{
			// Capture current time for logging timestamp
			DateTime now = DateTime.Now;
			return "[" + now.ToString() + "] - ";
		}

		// Capture and sort configuration profile as arraylist
		public override List<string> getProfile()
		{
			var profile = new List<string>();
			// Send web request and parse XML response
			try
			{
				var request = (HttpWebRequest)prepareRequest("http://" + ipAddress + "/a_createdatfilecsv.cgi");
				using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					while (!sr.EndOfStream)
					{
						profile.Add(sr.ReadLine());
					}
				}
				profile.Sort();

			}
			catch (Exception ex)
			{
				throw ex;
			}

			return profile;
		}

		// Return formatted WebClient with url, credentials, and session_id
		WebClient prepareClient(string url)
		{
			var client = new WebClient();
			var cache = new CredentialCache();
			// Check if Russian no-encryption version
			try
			{
				cache.Add(new Uri(url), "Digest", new NetworkCredential(username, password));
				var header = "Cookie: session_id=" + sessionID;
				client.Headers.Add(header);

				client.Credentials = cache;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return client;
		}

		// //    Obtain HDX system name, strip extraneous characters
		public override string getSystemName()
		{
			var name = "";
			try
			{
				name = doApiCommand(prepareApiUrl("systemname%20get")).Replace("systemname ", "").Replace("\"", "").Replace(Environment.NewLine, "");
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return name;
		}

		// Return formatted HttpWebRequest with url, credentials, and session_id
		WebRequest prepareRequest(string url)
		{
			var cache = new CredentialCache();

			// Assemble complete request
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Referer = "u_lastlogin.htm";

			cache.Add(new Uri(url), "Digest", new NetworkCredential(username, password));
			var header = "Cookie: session_id=" + sessionID;
			request.Headers.Add(header);
			
			request.Credentials = cache;

			// Add timeout since there's no ping for iOS / Mono
			request.Timeout = 5000;

			return request;
		}

		// Place h.323 call using provided dial string and "auto" call speed
		public void placeCall(string dialString)
		{
			try
			{
				string command = "a_manualdial.cgi?dialnumber=" + dialString + "&speeds=Auto&macconnectiontype=h323";
				doCgiCommand(command, null);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Query HDX call progress during dialing and meeting creation
		public string getCallProgress()
		{
			string progress = "";
			try
			{
				var request = (HttpWebRequest)prepareRequest("http://" + ipAddress + "/a_getlastconnectionmsg.htm");
				using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
				{
					var response = sr.ReadToEnd();
					if (response.ToLower().Contains("connecting"))
					{
						progress = "Connecting...";
					}
					else if (response.ToLower().Contains("disconnecting"))
					{
						progress = "Disconnecting...";
					}
					else if (response.ToLower().Contains("connected") & !response.ToLower().Contains("disconnected"))
					{
						progress = "Connected";
					}
					else if (response.ToLower().Contains("disconnected"))
					{
						progress = "Disconnected";
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return progress;
		}


		// Set the mute to either "on" or "off"
		public void mute(bool muteValue)
		{
			var value = "off";
			if (muteValue)
			{
				value = "on";
			}

			doApiCommand(prepareApiUrl("mute%20near%20" + value));
		}

		// Return the current mute state of the codec

		public bool getMute()
		{
			return doApiCommand(prepareApiUrl("mute%20near%20get")).Contains("on");
		}

		// Collect all status indicators from "systemstatus.xml"
		public override List<string> getStatus()
		{
			var status = new List<statusParameter>();
			var url = "http://" + ipAddress + "/systemstatus.xml";
			try
			{
				var request = prepareRequest(url);
				var xelement = XElement.Load(request.GetResponse().GetResponseStream());
				foreach (XElement element in xelement.Elements())
				{
					var param = new statusParameter();
					var stateList = new List<string>();

					// Read element name into statusParameter label property
					param.label = element.Name.ToString();
					// There may be more than one "state" value, so get them all
					foreach (string item in element.Elements("STATE"))
					{
						stateList.Add(item);
					}
					// Converty ArrayList to String() for the statusParameter state property
					param.state = stateList.ToArray();

					// Add complete statusParameter object to list
					status.Add(param);
				}

				// Add additional status queries not available in systemstatus.xml
				status.AddRange(getSystemParams("config.touchpanelstatus;config.remotesyslogenable"));

			}
			catch (Exception ex)
			{
				throw;
			}

			var statusList = toStringList(status);
			return statusList;
		}

		// Query additional status parameters
		public List<statusParameter> getSystemParams(string systemParams)
		{
			// Capture status parameters not available in systemstatus.xml
			// System variables are taken directly from HDX profile so they can be retrieved even if no
			// "get" command exists in the API. Variables are in the form "config.variableName"
			var status = new List<statusParameter>();
			var url = "http://" + ipAddress + "/querystatus.cgi?variables=" + systemParams;
			var request = prepareRequest(url);
			try
			{
				var xr = XmlReader.Create(request.GetResponse().GetResponseStream());
				var state = new List<string>();
				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element & xr.Name == "variable")
					{
						var paramName = xr.GetAttribute("name").Replace("config.", "").ToUpper();
						// Queries do not return "up" or "down", so this has to be manually determined
						xr.Read();
						if (xr.Value.ToLower().Equals("connected") | xr.Value.ToLower().Equals("true"))
						{
							state.Add("up");
						}
						else
						{
							state.Add("down");
						}
						var newParam = new statusParameter
						{
							label = paramName,
							state = state.ToArray()
						};
						status.Add(newParam);
						state.Clear();
					}
				}
				xr.Close();
			}
			catch (Exception ex)
			{
				throw;
			}

			return status;
		}

		// Utility method to convert a list of statusParameter objects to a list of strings
		List<string> toStringList(List<statusParameter> list)
		{
			List<string> statusList = new List<string>();
			foreach (statusParameter param in list)
			{
				string temp = (param.label + ": ");
				foreach (string state in param.state)
				{
					temp += (state + " ");
				}
				statusList.Add(temp);
			}

			return statusList;
		}


		//  Query and individual system parameter not necessarily available in the HDX profile. Returns the value only.
		public string queryParam(string paramName)
		{
			string value = "";
			string url = "http://" + ipAddress + "/querystatus.cgi?variables=config." + paramName;
			var request = (HttpWebRequest)prepareRequest(url);
			try
			{
				var xr = XmlReader.Create(request.GetResponse().GetResponseStream());
				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element & xr.Name == "variable")
					{
						xr.Read();
						value = xr.Value;
					}
				}

			}
			catch (Exception e)
			{
				throw e;
			}

			return value;
		}

		// Logout remote session
		public override bool logoutSession()
		{
			var success = true;
			try
			{
				doCgiCommand("sessioncmd.cgi?action=logout", null);
			}
			catch (Exception ex)
			{
				success = false;
			}
			finally
			{
				// Ensure sessionID is destroyed in case this object will be reused
				sessionID = "";
			}
			return success;
		}


		//  Change current password to new value
		public string changePassword(string newPassword)
		{
			//  Sessions List and SNMP settings may vary by system, but these values are required to submit the
			//  password change
			string snmpEnabled = queryParam("snmpenabled");

			Console.WriteLine("snmpEnabled: " + snmpEnabled);

			string sessionsEnabled = queryParam("sessionsenabled");

			Console.WriteLine("sessionsEnabled: " + sessionsEnabled);

			string response = doCgiCommand("a_security.cgi",
									"htmfile=a_security.htm&" +
									"enablesecureprofile=Off&" +
									"securemode=False&" +
									"useroompassword=True&" +
									"changemeetingpassword=False&" +
									"adminuserid=admin&" +
									"changeroompassword=True&" +
									"currentroompassword=" + password + "&" +
									"newroompassword=" + newPassword + "&" +
									"confirmroompassword=" + newPassword + "&" +
									"changeremotepassword=False&" +
									"userlogin=False&" +
									"encryptionenable=False&" +
									"allowusersetup=False&" +
									"webenabled=True&" +
									"telnetenabled=True&" +
									"snmpenabled=" + snmpEnabled + "&" +
									"webaccessport=80&" +
									"ntlmversion=Auto&" +
									"sessionsenabled=" + sessionsEnabled + "&" +
									"roomsw=" + newPassword + "&" +
									"roomsw=" + newPassword);

			return response;
		}


		// //-------------------------------------------------------------------------
		// //    statusParameter Class: Formats values for ParameterStatus objects
		// //-------------------------------------------------------------------------
		public class statusParameter
		{
			public string label { get; set; }
			public string[] state { get; set; }

		}

	}
}