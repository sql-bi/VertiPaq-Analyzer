namespace TestPowerBI
{
    partial class mainForm
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
            this.listWorkspaces = new System.Windows.Forms.ListBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.SignOutButton = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.listDatasets = new System.Windows.Forms.ListBox();
            this.btnVPAX = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listWorkspaces
            // 
            this.listWorkspaces.FormattingEnabled = true;
            this.listWorkspaces.ItemHeight = 20;
            this.listWorkspaces.Location = new System.Drawing.Point(34, 162);
            this.listWorkspaces.Name = "listWorkspaces";
            this.listWorkspaces.Size = new System.Drawing.Size(351, 324);
            this.listWorkspaces.TabIndex = 0;
            this.listWorkspaces.SelectedIndexChanged += new System.EventHandler(this.listWorkspaces_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(34, 21);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(144, 48);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // ResultText
            // 
            this.ResultText.Location = new System.Drawing.Point(822, 21);
            this.ResultText.Multiline = true;
            this.ResultText.Name = "ResultText";
            this.ResultText.Size = new System.Drawing.Size(490, 275);
            this.ResultText.TabIndex = 2;
            // 
            // SignOutButton
            // 
            this.SignOutButton.Location = new System.Drawing.Point(241, 21);
            this.SignOutButton.Name = "SignOutButton";
            this.SignOutButton.Size = new System.Drawing.Size(144, 48);
            this.SignOutButton.TabIndex = 4;
            this.SignOutButton.Text = "Sign Out";
            this.SignOutButton.UseVisualStyleBackColor = true;
            this.SignOutButton.Click += new System.EventHandler(this.SignOutButton_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(34, 89);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(144, 48);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // listDatasets
            // 
            this.listDatasets.FormattingEnabled = true;
            this.listDatasets.ItemHeight = 20;
            this.listDatasets.Location = new System.Drawing.Point(417, 162);
            this.listDatasets.Name = "listDatasets";
            this.listDatasets.Size = new System.Drawing.Size(351, 324);
            this.listDatasets.TabIndex = 6;
            // 
            // btnVPAX
            // 
            this.btnVPAX.Location = new System.Drawing.Point(822, 438);
            this.btnVPAX.Name = "btnVPAX";
            this.btnVPAX.Size = new System.Drawing.Size(144, 48);
            this.btnVPAX.TabIndex = 7;
            this.btnVPAX.Text = "Export VPAX";
            this.btnVPAX.UseVisualStyleBackColor = true;
            this.btnVPAX.Click += new System.EventHandler(this.btnVPAX_Click);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 518);
            this.Controls.Add(this.btnVPAX);
            this.Controls.Add(this.listDatasets);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.SignOutButton);
            this.Controls.Add(this.ResultText);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.listWorkspaces);
            this.Name = "mainForm";
            this.Text = "VPAX for Power BI";
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listWorkspaces;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.Button SignOutButton;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ListBox listDatasets;
        private System.Windows.Forms.Button btnVPAX;
    }
}

