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
	[Register ("APIViewController")]
	partial class APIViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField commandTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView outputTextView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch showXmlSwitch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton submitButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (commandTextField != null) {
				commandTextField.Dispose ();
				commandTextField = null;
			}
			if (outputTextView != null) {
				outputTextView.Dispose ();
				outputTextView = null;
			}
			if (showXmlSwitch != null) {
				showXmlSwitch.Dispose ();
				showXmlSwitch = null;
			}
			if (submitButton != null) {
				submitButton.Dispose ();
				submitButton = null;
			}
		}
	}
}
