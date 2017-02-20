/*	File:			TableSource.cs
 * 	Author:			Matthew Fuller
 * 	Date:			5/3/16
 * 	Description:	Class to provide a source for a UITableView 
 */

using System;
using UIKit;
using Foundation;

using System.Collections.Generic;

namespace RPDeviceStatus
{
	public class TableSource : UITableViewSource {

		List<string> TableItems;
		string CellIdentifier = "TableCell";
		bool useIndicators; // Determines whether or not to show status indicator .pngs

		public TableSource (List<string> items)
		{
			TableItems = items;
			useIndicators = false;
		}

		/*
		 * Alternate constructor with a bool to allow ImageViews to be set for each
		 * table cell.
		 */
		public TableSource(List<string> items, bool statusIndicators)
		{
			TableItems = items;
			useIndicators = statusIndicators;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TableItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (CellIdentifier);
			string item = TableItems[indexPath.Row];

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier); 
			}
			cell.BackgroundColor = UIColor.Black;
			cell.TextLabel.TextColor = UIColor.White;
			cell.TextLabel.Text = item;

			// Set appropriate color based on parameter status
			if (useIndicators) {
				if (item.Contains ("down") || item.Contains ("unknown")) {
					cell.ImageView.Image = UIImage.FromFile ("status_indicator_red.png");
				} else if (item.Contains ("up")) {
					cell.ImageView.Image = UIImage.FromFile ("status_indicator_green.png");
				} else if (item.Contains ("off")) {
					cell.ImageView.Image = UIImage.FromFile ("status_indicator_grey.png");
				}
			}
			return cell;
		}
	}
}

