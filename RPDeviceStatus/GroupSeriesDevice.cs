﻿/*
 * 	File:		GroupSeriesDevice.cs
 * 	Author:		Matthew Fuller
 * 	Desc:		Class for interacting with the Polycom Group Series (300, 500, 700) systems.
 * 				All interactions require a valid session ID in the header, so getSession()
 * 				should always be the first method called when a GS object will be used. Group Series
 * 				use a REST API for many major operations, but they also have a handful of legacy CGI
 * 				commands from HDX code. 
 * 				Note: Some methods in this implementation have been (hastily) modified to ensure functionality
 * 				within the Mono Framework for an iOS application.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json; // Replacement lib for System.Web.Extensions (JSON serializer/deserializer)

namespace RPDeviceStatus
{
	public class GroupSeriesDevice : RPDevice
	{
		public GroupSeriesDevice(string ip, string un, string pw)
			:base(ip, un, pw)
		{
			// Calls base constructor to set ipAddress, username, and password
		}

		// Group Series requires a session_id value to be added to the
		// header of all requests. Function sets this value and returns true, 
		// or returns false if the sessionID could not be determined.
		public override bool getSession()
		{
			var tempID = "";
			var success = false; 
			if (string.IsNullOrEmpty(sessionID))
			{
				string URL = "https://" + ipAddress + "/rest/session";
				byte[] dataBytes = null;
				var request = WebRequest.Create(URL);
				var cache = new CredentialCache();
				cache.Add(new Uri(URL), "Digest", new NetworkCredential(username, password));

				// No ping for iOS / Mono, so add a timeout
				request.Timeout = 5000;

				// Assemble JSON and headers
				string jsonData = "{\"action\":\"Login\",\"user\":\"" + username + "\",\"password\":\"" + password + "\"}";
				dataBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
				request.Method = "POST";
				request.ContentType = "application/json";
				request.ContentLength = dataBytes.Length;

				// Ignore obnoxious SSL failure generated by ALL Polycom devices because of self-signed certs
				ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

				try
				{
					// Post data to stream
					dynamic writeStream = request.GetRequestStream();
					writeStream.Write(dataBytes, 0, dataBytes.Length);
					writeStream.Close();
					// Read response
					StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
					string json = sr.ReadToEnd();

					var response = JsonConvert.DeserializeObject<GroupSeriesJSONObjects.loginJSONObject>(json);

					if (response.session.TryGetValue("sessionId", out tempID))
					{
						sessionID = tempID;
						success = true;
					}
					else
					{
						sessionID = "";
					}
					sr.Close();
				}

				catch (Exception ex)
				{
					throw ex;
				}
			}

			return success;
		}

		// Parse /rest/system response and capture system name
		public override string getSystemName()
		{
			string systemname = "";
			string URL = "https://" + ipAddress + "/rest/system";
			try
			{
				// For iOS application
				getSession();

				var request = prepareWebRequest(URL);
				// Do not proceed if prepareRequest failed
				if (request != null)
				{
					StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
					string json = sr.ReadToEnd();

					dynamic response = JsonConvert.DeserializeObject<GroupSeriesJSONObjects.systemJSONObject>(json);
					systemname = response.systemName;

					// For iOS application
					logoutSession();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return systemname;
		}

		// Prepare WebRequest with provided URL
		WebRequest prepareWebRequest(string url)
		{
			// Assemble web request with credentials and session ID
			var request = WebRequest.Create(url);
			request.Timeout = 5000;
			request.Headers.Add("Cookie: session_id=" + sessionID);
			return request;
		}

		// Return a list of all Group Series status parameters
		public override List<string> getStatus()
		{
			// Retrieve GroupSeries full system status as a list of statusJSONObjects
			var response = new List<GroupSeriesJSONObjects.statusJSONObject>();
			try
			{
				// For iOS application...
				getSession();

				var request = prepareWebRequest("https://" + ipAddress + "/rest/system/status");
				// Only proceed if web request was successfully prepared
				if (request != null)
				{
					var sr = new StreamReader(request.GetResponse().GetResponseStream());
					string json = sr.ReadToEnd();
					response = JsonConvert.DeserializeObject<List<GroupSeriesJSONObjects.statusJSONObject>>(json);
					sr.Close();

					// For iOS application...
					logoutSession();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			var statusList = toStringList(response);
			return statusList;

		}

		//  Utility method to convert the list of statusJSONObjects into a string for easy output
		List<string> toStringList(List<GroupSeriesJSONObjects.statusJSONObject> list)
		{
			List<string> statusList = new List<string>();
			foreach (GroupSeriesJSONObjects.statusJSONObject statusObj in list)
			{
				string temp = statusObj.languageTag + ": ";
				foreach (string state in statusObj.state)
				{
					temp += (state + " ");
				}
				statusList.Add(temp);
			}

			return statusList;
		}

		// Send HTTP POST request to Group Series REST API. Data can be sent
		// either as "form" or "json", and data is specified with a string
		public string doRestCommand(string url, string method, string contentType, string data)
		{
			string response = "";
			// Convert data string to byte array
			byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(data);
			try
			{
				var postRequest = prepareWebRequest(url);
				if (postRequest != null)
				{
					// Parameter contentType should be either "json" or "form"
					if (contentType.Equals("form"))
					{
						postRequest.ContentType = "application/x-www-form-urlencoded";
					}
					else
					{
						postRequest.ContentType = "application/json";
					}
					postRequest.ContentLength = postBytes.Length;
					postRequest.Method = method.ToUpper();

					// Capture response as string (can be deserialized if needed)
					Stream postStream = postRequest.GetRequestStream();
					postStream.Write(postBytes, 0, postBytes.Length);
					postStream.Close();
					StreamReader sr = new StreamReader(postRequest.GetResponse().GetResponseStream());
					response = sr.ReadToEnd();
					sr.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return response;
		}

		// Send an HTTP GET request to the REST API using the specified URL
		public override string doApiCommand(string url)
		{
			string response = "";
			try
			{
				var request = prepareWebRequest(url);
				if (request != null)
				{
					StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
					response = sr.ReadToEnd();
					sr.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return response;
		}


		// Captures real-time call statistics of all media streams in an
		// active call. Returns list of mediaStatsJSONObjects.
		List<GroupSeriesJSONObjects.mediaStatsJSONObject> getMediaStats(string responseString)
		{
			// responseString is assumed to be the complete string output from apiGet
			// with /rest/conferences/0/mediastats as the url
			List<GroupSeriesJSONObjects.mediaStatsJSONObject> list = null;
			//System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
			try
			{
				list = JsonConvert.DeserializeObject<List<GroupSeriesJSONObjects.mediaStatsJSONObject>>(responseString);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return list;
		}

		// Place call using given dial string
		public void placeCall(string dialString)
		{
			string url = "https://" + ipAddress + "/rest/conferences";
			string postData = "address=" + dialString + "&dialType=AUTO&rate=0";
			try
			{
				doRestCommand(url, "POST", "form", postData);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Gets a list of active conferences
		public string getConferences()
		{
			string conferences = "";
			string url = "https://" + ipAddress + "/rest/conferences";
			try
			{
				conferences = doApiCommand(url);
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return conferences;
		}

		// Disconnect all active calls
		public void hangupCall()
		{
			string url = "https://" + ipAddress + "/rest/conferences/active";
			string postData = "action=hangup";
			try
			{
				doRestCommand(url, "POST", "form", postData);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Mutes / unmutes the microphone input
		public void mute(bool muteState)
		{
			string response = doRestCommand("https://" + ipAddress + "/rest/audio/muted", "PUT", "json", muteState.ToString());
			Console.WriteLine(response);
		}

		// Gets current mute state of GroupSeriesDevice
		public bool getMute()
		{
			if (doApiCommand("https://" + ipAddress + "/rest/audio/muted").ToLower().Equals("true"))
			{
				return true;
			}
			return false;
		}

		public override List<string> getProfile()
		{
			List<string> profile = new List<string>();
			string url = "https://" + ipAddress + "/exportsystemprofile.cgi";
			try
			{
				// For iOS application
				getSession();
				string response = doApiCommand(url);
				using (var sr = new StringReader(response))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						profile.Add(line);
					}
				}

				// For iOS application
				logoutSession();
			}
			catch (Exception ex)
			{
				throw;
			}

			return profile;
		}



		// //-------------------------------------------------------------------------
		// //    Logout session from active list of REST sessions. Unlike HDXs,
		// //    Group Series sessions remain active for 10-15 minutes even with no
		// //    activity. If they are not logged out, they will accumulate and 
		// //    eventually block all connection attempts. Brilliant design!
		// //-------------------------------------------------------------------------
		public override bool logoutSession()
		{
			var success = true;
			// Logout current session to avoid filling up the sessions list with stale connections
			try
			{
				doRestCommand("https://" + ipAddress + "/rest/session", "POST", "json", "{\"action\":\"Logout\"}");

			}
			catch (Exception ex)
			{
				success = false;
			}
			finally
			{
				// Ensure sessionID is destroyed in case this object will be used for further tasks
				sessionID = "";
			}

			return success;
		}

		// Timestamp for UI status update messages
		string timeStamp()
		{
			// Capture current time for logging timestamp
			DateTime now = DateTime.Now;
			return "[" + now.ToString() + "] - ";
		}

		/* DOES NOT WORK IN iOS--PING REQUIRES RAW SOCKET CONNECTION (ELEVATED PERMISSIONS)
		// //-------------------------------------------------------------------------
		// //    Test device for ping response
		// //-------------------------------------------------------------------------
		public bool isPinging()
		{
			Ping ping = new Ping();
			PingReply pingResponse = ping.Send(ipaddress);
			if (pingResponse.Status == IPStatus.Success)
			{
				return true;
			}
			return false;
		}

		*/

	}
}