/*	File:			StatusViewController.cs
 * 	Author:			Matthew Fuller
 * 	Date:			5/3/16
 * 	Description:	ViewController to manage the display of device status parameters
 */

using System;
using UIKit;
using System.Collections.Generic;

namespace RPDeviceStatus
{
	partial class StatusViewController : UIViewController
	{
		public RPDevice device { get; set; }


		public StatusViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Extend width for landscape view
			statusTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			statusTableView.BackgroundColor = UIColor.Black;

			var paramList = getStatusList(device);
			if (paramList != null && paramList.Count > 0)
			{
				// Use alternate constructor with 'true' to display appropriate status indicator
				statusTableView.Source = new TableSource(paramList, true);
			}

		}

		public List<string> getStatusList(RPDevice dev)
		{
			var list = new List<string>();
			try
			{
				dev.getSession();
				list = device.getStatus();
			}
			catch (Exception e)
			{
				var okAlertController = UIAlertController.Create("Error", e.Message, UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
			}
			return list;
		}
	}
}
