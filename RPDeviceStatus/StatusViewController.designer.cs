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
	[Register ("StatusViewController")]
	partial class StatusViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView Output { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView statusTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Output != null) {
				Output.Dispose ();
				Output = null;
			}
			if (statusTableView != null) {
				statusTableView.Dispose ();
				statusTableView = null;
			}
		}
	}
}
