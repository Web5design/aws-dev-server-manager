namespace AwsServerManager.Gui
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.RefreshButton = new System.Windows.Forms.Button();
			this.InstancesListView = new System.Windows.Forms.ListView();
			this.InstanceIdColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.InstanceNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.StatusColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ActionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.RegionSelector = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// RefreshButton
			// 
			this.RefreshButton.Location = new System.Drawing.Point(812, 8);
			this.RefreshButton.Name = "RefreshButton";
			this.RefreshButton.Size = new System.Drawing.Size(82, 23);
			this.RefreshButton.TabIndex = 0;
			this.RefreshButton.Text = "Refresh";
			this.RefreshButton.UseVisualStyleBackColor = true;
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			// 
			// InstancesListView
			// 
			this.InstancesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.InstancesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.InstanceIdColumn,
            this.InstanceNameColumn,
            this.StatusColumn,
            this.ActionColumn});
			this.InstancesListView.FullRowSelect = true;
			this.InstancesListView.Location = new System.Drawing.Point(12, 47);
			this.InstancesListView.Name = "InstancesListView";
			this.InstancesListView.Size = new System.Drawing.Size(882, 599);
			this.InstancesListView.TabIndex = 1;
			this.InstancesListView.UseCompatibleStateImageBehavior = false;
			this.InstancesListView.View = System.Windows.Forms.View.Details;
			this.InstancesListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.InstancesListView_MouseClick);
			// 
			// InstanceIdColumn
			// 
			this.InstanceIdColumn.Text = "Id";
			// 
			// InstanceNameColumn
			// 
			this.InstanceNameColumn.Text = "Name";
			// 
			// StatusColumn
			// 
			this.StatusColumn.Text = "Status";
			// 
			// ActionColumn
			// 
			this.ActionColumn.Text = "Action";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Region";
			// 
			// RegionSelector
			// 
			this.RegionSelector.FormattingEnabled = true;
			this.RegionSelector.Location = new System.Drawing.Point(60, 8);
			this.RegionSelector.Name = "RegionSelector";
			this.RegionSelector.Size = new System.Drawing.Size(221, 21);
			this.RegionSelector.TabIndex = 3;
			this.RegionSelector.SelectedIndexChanged += new System.EventHandler(this.RegionSelector_SelectedIndexChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(906, 658);
			this.Controls.Add(this.RegionSelector);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.InstancesListView);
			this.Controls.Add(this.RefreshButton);
			this.Name = "MainForm";
			this.Text = "AWS Server Manager";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button RefreshButton;
		private System.Windows.Forms.ListView InstancesListView;
		private System.Windows.Forms.ColumnHeader InstanceIdColumn;
		private System.Windows.Forms.ColumnHeader InstanceNameColumn;
		private System.Windows.Forms.ColumnHeader StatusColumn;
		private System.Windows.Forms.ColumnHeader ActionColumn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox RegionSelector;
	}
}

