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
					var request = new DescribeInstancesRequest();
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

					if (IsRunning(instance))
					{
						Stop(instance);
						item.SubItems[StateColumnIndex].Text = "stopping";
					}
					else
					{
						Start(instance);
						item.SubItems[StateColumnIndex].Text = "pending";
					}

					Refresh();
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

		void Start(RunningInstance instance)
		{
			using (var client = CreateClient())
			{
				var request = new StartInstancesRequest();
				request.InstanceId.Add(instance.InstanceId);
				client.StartInstances(request);
			}
		}

		void Stop(RunningInstance instance)
		{
			using (var client = CreateClient())
			{
				var request = new StopInstancesRequest();
				request.InstanceId.Add(instance.InstanceId);
				client.StopInstances(request);
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

		private const int StateColumnIndex = 2;
		private const int ActionColumnIndex = 3;
	}
}
