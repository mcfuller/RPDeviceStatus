/*	File:			ProfileViewController.cs
 * 	Author:			Matthew Fuller
 * 	Date:			5/3/16
 * 	Description:	ViewController to display device configuration profile as
 * 					a UITableView
 */

using System;
using UIKit;
using System.Collections.Generic;

namespace RPDeviceStatus
{
	partial class ProfileViewController : UIViewController
	{
		public RPDevice device { get; set;}
		UISearchBar searchBar;
		List<string> searchResults;

		public ProfileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			profileTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			profileTableView.BackgroundColor = UIColor.Black;
			searchResults = new List<string> ();
			if (device != null)
			{
				var paramList = getProfileList(device);
				if (paramList != null && paramList.Count > 0)
				{
					// Allow table cells to resize for landscape view
					searchResults.AddRange(paramList);
					profileTableView.Source = new TableSource(searchResults);

					//Add (table);

					// SearchBar 
					searchBar = new UISearchBar();
					searchBar.Placeholder = "Enter Search Text";
					searchBar.SizeToFit();
					searchBar.AutocorrectionType = UITextAutocorrectionType.No;
					searchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
					searchBar.ShowsCancelButton = true;

					profileTableView.TableHeaderView = searchBar;

					/*
					 * SearchBar TextChanged event handler
					 */
					searchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) =>
					{
						// If all text has been deleted, restore original full profile
						if (e.SearchText.Equals(""))
						{
							searchResults.Clear();
							searchResults.AddRange(paramList);
							profileTableView.ReloadData();
						}
						else
						{
							searchResults.Clear();
							foreach (string item in paramList)
							{
								if (item.Contains(e.SearchText))
								{
									searchResults.Add(item);
								}
							}

							profileTableView.ReloadData();
						}
					};

					/*
					 * SearchBar SearchButton event handler
					 */
					searchBar.SearchButtonClicked += (object sender, EventArgs e) =>
					{
						if (!searchBar.Text.Equals(""))
						{
							searchResults.Clear();
							foreach (string item in paramList)
							{
								if (item.Contains(searchBar.Text))
								{
									searchResults.Add(item);
								}
							}
							profileTableView.ReloadData();
						}

						// Hide keyboard in either case
						searchBar.ResignFirstResponder();
					};

					/*
					 * SearchBar CancelButton event handler
					 */
					searchBar.CancelButtonClicked += (object sender, EventArgs e) =>
					{
						// Don't clear the text entered in the searchBar, just hide the keyboard
						// User can hit the clear button to unfilter the profile

						searchBar.ResignFirstResponder();
					};
				}
				else
				{
					var okAlertController = UIAlertController.Create("Error", "paramList is empty", UIAlertControllerStyle.Alert);
					okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
					PresentViewController(okAlertController, true, null);
				}
			}
			// Print debug error message that RPDevice object is null
			else
			{
				var okAlertController = UIAlertController.Create("Error", "Device is null", UIAlertControllerStyle.Alert);
				okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				PresentViewController(okAlertController, true, null);
			}
		}

		public List<string> getProfileList(RPDevice device)
		{
			var list = new List<string>();
			try
			{
				device.getSession();
				list = device.getProfile();
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

