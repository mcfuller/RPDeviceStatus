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
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton apiButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView Device { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField ipTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField passwordTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton profileButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISegmentedControl selectedDeviceSegment { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton statusButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField usernameTextField { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (apiButton != null) {
				apiButton.Dispose ();
				apiButton = null;
			}
			if (Device != null) {
				Device.Dispose ();
				Device = null;
			}
			if (ipTextField != null) {
				ipTextField.Dispose ();
				ipTextField = null;
			}
			if (passwordTextField != null) {
				passwordTextField.Dispose ();
				passwordTextField = null;
			}
			if (profileButton != null) {
				profileButton.Dispose ();
				profileButton = null;
			}
			if (selectedDeviceSegment != null) {
				selectedDeviceSegment.Dispose ();
				selectedDeviceSegment = null;
			}
			if (statusButton != null) {
				statusButton.Dispose ();
				statusButton = null;
			}
			if (usernameTextField != null) {
				usernameTextField.Dispose ();
				usernameTextField = null;
			}
		}
	}
}
