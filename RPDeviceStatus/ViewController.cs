/*	File:			ViewController.cs
 * 	Author:			Matthew Fuller
 * 	Date:			5/3/16
 * 	Description:	Main ViewController for RPDeviceStatus application
 */

using System;
using System.Collections.Generic;
using UIKit;
using System.Text.RegularExpressions;

namespace RPDeviceStatus
{
	public partial class ViewController : UIViewController
	{
		public bool doSegue { get; set; } // Determine whether segue should proceed

		// List to collect status parameters from RP Devices
		public List<string> outputList { get; set; }

		public string titleString { get; set; } // Use systemname for title of next ViewControllers

		public RPDevice endpoint;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			doSegue = false; // Prevents segue from occurring if validation has failed
		
			// Set handlers for TextFields
			ipTextField.ShouldReturn += TextFieldShouldReturn;
			usernameTextField.ShouldReturn += TextFieldShouldReturn;
			passwordTextField.ShouldReturn += TextFieldShouldReturn;

		
			/*
			 * StatusButton Handler
			 * */
			statusButton.TouchUpInside += (object sender, EventArgs e) => {
				endpoint = setDevice(ipTextField.Text, usernameTextField.Text, passwordTextField.Text);
				if (endpoint != null)
				{
					PerformSegue("statusSegue", this);
				}
			};

			/*
			 * ProfileButton event handler
			 * */
			profileButton.TouchUpInside += (object sender, EventArgs e) => {
				endpoint = setDevice(ipTextField.Text, usernameTextField.Text, passwordTextField.Text);
				if (endpoint != null)
				{
					PerformSegue("profileSegue", this);
				}
			};

			/*
			 * APIButton event handler
			 * */
			apiButton.TouchUpInside += (object sender, EventArgs e) => {
				endpoint = setDevice(ipTextField.Text, usernameTextField.Text, passwordTextField.Text);
				if (endpoint != null)
				{
					doSegue = true;
					if (selectedDeviceSegment.SelectedSegment == 0)
					{
						PerformSegue("apiSegue", this);
					}
					else
					{
						var okAlertController = UIAlertController.Create("Info:", "Feature only available for HDX devices", UIAlertControllerStyle.Alert);
						okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
						PresentViewController(okAlertController, true, null);
						doSegue = false;
					}
				}
			};
		}

		/*
		 * Handler for moving through TextFields and hiding the keyboard
		 * */
		private bool TextFieldShouldReturn (UITextField textField){
			nint nextTag = textField.Tag + 1;    
			UIResponder nextResponder = this.View.ViewWithTag (nextTag);
			if (nextResponder != null) {
				nextResponder.BecomeFirstResponder ();
			} else {
				// Not found, so remove keyboard.
				textField.ResignFirstResponder ();
			}
			return false; 
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		/*
		 * Pass information on to the appropriate ViewController
		 */
		public override void PrepareForSegue (UIStoryboardSegue segue, Foundation.NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier.Equals ("statusSegue")) {
				var svc = segue.DestinationViewController as StatusViewController;
				svc.device = endpoint;
			} else if (segue.Identifier.Equals ("profileSegue")) {
				var pvc = segue.DestinationViewController as ProfileViewController;
				pvc.device = endpoint;
			} else if (segue.Identifier.Equals ("apiSegue")) {
				var avc = segue.DestinationViewController as APIViewController;
				avc.apiHDX = new HDXDevice (ipTextField.Text, usernameTextField.Text, passwordTextField.Text);
			}
			// reset condition so validation will be run for every visit
			doSegue = false;
		}

		// Don't perform segue if checks have failed!
		public override bool ShouldPerformSegue (string segueIdentifier, Foundation.NSObject sender)
		{
			if (segueIdentifier == "statusSegue" || segueIdentifier == "profileSegue" ||
				segueIdentifier == "apiSegue") {
				if (doSegue == false) {
					return false;
				}
			}
			return base.ShouldPerformSegue (segueIdentifier, sender);
		}

		public RPDevice setDevice(string ip, string un, string pw)
		{
			RPDevice device = null;
			if (isValidIP(ipTextField.Text) && isValidUsername(usernameTextField.Text))
			{
				if (selectedDeviceSegment.SelectedSegment == 0)
				{
					device = new HDXDevice(ip, un, pw);
				}
				else if (selectedDeviceSegment.SelectedSegment == 1)
				{
					device = new GroupSeriesDevice(ip, un, pw);
				}
			}
			return device;
		}


		/*
		 * Ensure IP address is in valid format
		 */
		public bool isValidIP(string ip)
		{
			var success = false;
			Regex validIP = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
			if (!string.IsNullOrEmpty(ip) && validIP.Match (ip).Success) {
					success = true;
				} else {
					UIAlertView error = new UIAlertView ("Error:", "Please enter a valid IP address", null, "OK", null);
					error.Show ();
				}
			return success;
		}

		/*
		 * Prevent an empty string for username, but all other values are acceptable
		 */
		public bool isValidUsername(string un)
		{
			if (string.IsNullOrEmpty(un)) {
				UIAlertView error = new UIAlertView ("Error:", "Please enter a username", null, "OK", null);
				error.Show ();
				return false;
			}
			return true;
		}

		/*
		 * No validation is required for password field since it's possible (but not recommended)
		 * to configure HDX or GroupSeries codecs without passwords. Empty string or any other value
		 * is acceptable.
		 */
			
	}
}

