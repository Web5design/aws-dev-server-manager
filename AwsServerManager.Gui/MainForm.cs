using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

using AwsServerManager.Shared;

namespace AwsServerManager.Gui
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			Application.Idle += ApplicationOnIdle;
		}

		private bool _initDone;

		private void ApplicationOnIdle(object sender, EventArgs eventArgs)
		{
			if (!_initDone)
			{
				foreach (var item in RegionEndpoint.EnumerableAllRegions)
				{
					RegionSelector.Items.Add(item);
				}
				RegionSelector.SelectedItem = AwsUtil.Region;

				_initDone = true;
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			RefreshList();
		}

		private void RegionSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			var item = RegionSelector.SelectedItem;
			if (item != null)
				RefreshList();
		}

		void RefreshList()
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				InstancesListView.Items.Clear();

				using (var client = CreateClient())
				{
					var request = new DescribeInstancesRequest().WithFilter(GetFilters());
					var reservations = client.DescribeInstances(request).DescribeInstancesResult.Reservation;
					foreach (var reservation in reservations)
					{
						foreach (var instance in reservation.RunningInstance)
						{
							var item = InstancesListView.Items.Add(instance.InstanceId);
							item.Tag = instance;

							var tags = instance.Tag;
							var instanceName = GetTag(tags, "Name");
							item.SubItems.Add(instanceName);
							var instanceState = instance.InstanceState;
							item.SubItems.Add(instanceState.Name);

							var action = IsRunning(instance) ? "[Stop]" : "[Start]";
							item.SubItems.Add(action);
						}
					}
				}
			}
			catch (Exception exc)
			{
				Report(exc);
			}
			finally
			{
				Cursor = Cursors.Arrow;
			}
		}

		static string GetTag(IEnumerable<Tag> tags, string name)
		{
			var found = tags.Where(val => string.Equals(val.Key, name, StringComparison.OrdinalIgnoreCase)).ToArray();
			if (!found.Any())
				return null;
			return found.First().Value;
		}

		static bool IsRunning(RunningInstance instance)
		{
			return !string.Equals(instance.InstanceState.Name, "stopped", StringComparison.OrdinalIgnoreCase);
		}

		AmazonEC2Client CreateClient()
		{
			return AwsUtil.CreateClient(AwsRegion);
		}

		private void InstancesListView_MouseClick(object sender, MouseEventArgs e)
		{
			try
			{
				var mousePos = InstancesListView.PointToClient(MousePosition);
				var hitTest = InstancesListView.HitTest(mousePos);
				var columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);

				if (IsActionColumn(columnIndex))
				{
					Cursor = Cursors.WaitCursor;

					var item = hitTest.Item;
					var instance = (RunningInstance)hitTest.Item.Tag;

					item.SubItems[StateColumnIndex].Text =
						IsRunning(instance)
						? Stop(instance).Name
						: Start(instance).Name;
				}
			}
			catch (Exception exc)
			{
				Report(exc);
			}
			finally
			{
				Cursor = Cursors.Arrow;
			}
		}

		InstanceState Start(RunningInstance instance)
		{
			using (var client = CreateClient())
			{
				var request = new StartInstancesRequest();
				request.InstanceId.Add(instance.InstanceId);
				var response = client.StartInstances(request);
				return response.StartInstancesResult.StartingInstances[0].CurrentState;
			}
		}

		InstanceState Stop(RunningInstance instance)
		{
			using (var client = CreateClient())
			{
				var request = new StopInstancesRequest();
				request.InstanceId.Add(instance.InstanceId);
				var response = client.StopInstances(request);
				return response.StopInstancesResult.StoppingInstances[0].CurrentState;
			}
		}

		private void Report(Exception exc)
		{
			MessageBox.Show(this, exc.ToString(), this.Name);
		}

		private RegionEndpoint AwsRegion
		{
			get { return (RegionEndpoint)RegionSelector.SelectedItem; }
		}

		static bool IsActionColumn(int index)
		{
			return (index == ActionColumnIndex);
		}

		private void InstancesListView_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				var mousePos = InstancesListView.PointToClient(MousePosition);
				var hitTest = InstancesListView.HitTest(mousePos);

				if (hitTest.Item != null)
				{
					var columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
					if (IsActionColumn(columnIndex))
					{
						Cursor = Cursors.Hand;
						return;
					}
				}

				if (Cursor == Cursors.Hand)
					Cursor = Cursors.Arrow;
			}
			catch (Exception exc)
			{
				Report(exc);
			}
		}

		static Filter[] GetFilters()
		{
			var res = new List<Filter>
				{
					new Filter().WithName("root-device-type").WithValue("ebs")
				};

			var text = ConfigurationManager.AppSettings["InstanceFilter"];
			var items = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var item in items)
			{
				var parts = item.Split('=');
				if (parts.Length != 2)
					throw new ApplicationException(string.Format("Invalid filter parameter: '{0}'", item));

				var tagName = "tag:" + parts[0];
				var filter = new Filter().WithName(tagName).WithValue(parts[1]);
				res.Add(filter);
			}

			return res.ToArray();
		}

		private const int StateColumnIndex = 2;
		private const int ActionColumnIndex = 3;
	}
}
