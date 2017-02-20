// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace RPDeviceStatus
{
	[Register ("ProfileViewController")]
	partial class ProfileViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView profileTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (profileTableView != null) {
				profileTableView.Dispose ();
				profileTableView = null;
			}
		}
	}
}
