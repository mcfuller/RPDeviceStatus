using System;
using System.Collections.Generic;

namespace RPDeviceStatus
{
	public abstract class RPDevice : IDisposable
	{
		protected string ipAddress { get; set; }
		protected string username { get; set; }
		protected string password { get; set; }
		protected string sessionID { get; set; }
		protected bool disposed { get; set; }

		protected RPDevice(string ip, string un, string pw)
		{
			ipAddress = ip;
			username = un;
			password = pw;
		}

		public abstract bool getSession();

		public abstract bool logoutSession();

		public abstract List<string> getStatus();

		public abstract string getSystemName();

		public abstract List<string> getProfile();

		public abstract string doApiCommand(string url);

		// Override Dispose method to call logout() explicitly so we don't 
		// leave stale sessions in our wake
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					logoutSession();
				}
			}
			this.disposed = true;
		}

		// Destructor
		~RPDevice()
		{
			Dispose(false);
		}
	}
}
