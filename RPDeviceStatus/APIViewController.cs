/*	File:			APIViewController.cs
 * 	Author:			Matthew Fuller
 * 	Date:			5/3/16
 * 	Description:	ViewController to manage the API entry view in RPDeviceStatus
 */

using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace RPDeviceStatus
{
	partial class APIViewController : UIViewController
	{
		public HDXDevice apiHDX { get; set; } // Set by main ViewController during segue

		public APIViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			commandTextField.ShouldReturn = TextFieldShouldReturn;

			/*
			 * SubmitButton Handler
			 */
			submitButton.TouchUpInside += (object sender, EventArgs e) => {
				submitAction();
			};

		}

		// Hide keyboard when return key is pressed
		private bool TextFieldShouldReturn(UITextField tfield)
		{
			tfield.ResignFirstResponder ();
			submitAction ();
			return true;
		}

		private void submitAction()
		{
			if (apiHDX != null) {
				// Should we show the unparsed XML output?
				apiHDX.showXML = (showXmlSwitch.On);
				try {
					if (commandTextField.Text != "") {
						// Replace spaces between arguments with %20 for URL formatting
						outputTextView.Text = apiHDX.doApiCommand (apiHDX.prepareApiUrl(commandTextField.Text.Replace (" ", "%20")));
					}

				} catch (Exception e) {
					var okAlertController = UIAlertController.Create("Error", e.Message, UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					PresentViewController(okAlertController, true, null);
				}
			}
		}
	}
}
