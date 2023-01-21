namespace ClientOmAuth
{
    partial class ClientFrm
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
            this.SamlGroup = new System.Windows.Forms.GroupBox();
            this.FbaLst = new System.Windows.Forms.ListBox();
            this.txtCookie = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.GetRestDataBtn = new System.Windows.Forms.Button();
            this.AdfsTxt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SamlList = new System.Windows.Forms.ListBox();
            this.SamlListBtn = new System.Windows.Forms.Button();
            this.SamlTxt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.txtResults = new System.Windows.Forms.RichTextBox();
            this.SamlGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SamlGroup
            // 
            this.SamlGroup.Controls.Add(this.FbaLst);
            this.SamlGroup.Controls.Add(this.txtCookie);
            this.SamlGroup.Controls.Add(this.label11);
            this.SamlGroup.Controls.Add(this.txtPassword);
            this.SamlGroup.Controls.Add(this.label10);
            this.SamlGroup.Controls.Add(this.txtDomain);
            this.SamlGroup.Controls.Add(this.label9);
            this.SamlGroup.Controls.Add(this.txtUserName);
            this.SamlGroup.Controls.Add(this.label8);
            this.SamlGroup.Controls.Add(this.GetRestDataBtn);
            this.SamlGroup.Controls.Add(this.AdfsTxt);
            this.SamlGroup.Controls.Add(this.label4);
            this.SamlGroup.Controls.Add(this.SamlList);
            this.SamlGroup.Controls.Add(this.SamlListBtn);
            this.SamlGroup.Controls.Add(this.SamlTxt);
            this.SamlGroup.Controls.Add(this.label6);
            this.SamlGroup.Location = new System.Drawing.Point(4, 1);
            this.SamlGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SamlGroup.Name = "SamlGroup";
            this.SamlGroup.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SamlGroup.Size = new System.Drawing.Size(656, 245);
            this.SamlGroup.TabIndex = 9;
            this.SamlGroup.TabStop = false;
            this.SamlGroup.Text = "SAML Claims";
            this.SamlGroup.Enter += new System.EventHandler(this.SamlGroup_Enter);
            // 
            // FbaLst
            // 
            this.FbaLst.FormattingEnabled = true;
            this.FbaLst.ItemHeight = 16;
            this.FbaLst.Location = new System.Drawing.Point(303, 94);
            this.FbaLst.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FbaLst.Name = "FbaLst";
            this.FbaLst.Size = new System.Drawing.Size(344, 84);
            this.FbaLst.TabIndex = 26;
            // 
            // txtCookie
            // 
            this.txtCookie.Location = new System.Drawing.Point(97, 190);
            this.txtCookie.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCookie.Name = "txtCookie";
            this.txtCookie.Size = new System.Drawing.Size(545, 22);
            this.txtCookie.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 192);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 17);
            this.label11.TabIndex = 24;
            this.label11.Text = "FEDAuth:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(97, 158);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(195, 22);
            this.txtPassword.TabIndex = 23;
            this.txtPassword.Text = "blah";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 160);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 17);
            this.label10.TabIndex = 22;
            this.label10.Text = "Password:";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(99, 126);
            this.txtDomain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(195, 22);
            this.txtDomain.TabIndex = 21;
            this.txtDomain.Text = "domain";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 128);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 17);
            this.label9.TabIndex = 20;
            this.label9.Text = "Domain:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(99, 94);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(195, 22);
            this.txtUserName.TabIndex = 19;
            this.txtUserName.Text = "bob";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 96);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 17);
            this.label8.TabIndex = 18;
            this.label8.Text = "Username:";
            // 
            // GetRestDataBtn
            // 
            this.GetRestDataBtn.Enabled = false;
            this.GetRestDataBtn.Location = new System.Drawing.Point(301, 60);
            this.GetRestDataBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GetRestDataBtn.Name = "GetRestDataBtn";
            this.GetRestDataBtn.Size = new System.Drawing.Size(100, 28);
            this.GetRestDataBtn.TabIndex = 15;
            this.GetRestDataBtn.Text = "Get REST";
            this.GetRestDataBtn.UseVisualStyleBackColor = true;
            this.GetRestDataBtn.Click += new System.EventHandler(this.GetRestDataBtn_Click);
            // 
            // AdfsTxt
            // 
            this.AdfsTxt.Location = new System.Drawing.Point(99, 60);
            this.AdfsTxt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AdfsTxt.Name = "AdfsTxt";
            this.AdfsTxt.Size = new System.Drawing.Size(195, 22);
            this.AdfsTxt.TabIndex = 14;
            this.AdfsTxt.Text = "https://adfs.domain.local";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 63);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "ADFS Server:";
            // 
            // SamlList
            // 
            this.SamlList.DisplayMember = "Title";
            this.SamlList.FormattingEnabled = true;
            this.SamlList.ItemHeight = 16;
            this.SamlList.Location = new System.Drawing.Point(411, 21);
            this.SamlList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SamlList.Name = "SamlList";
            this.SamlList.Size = new System.Drawing.Size(235, 68);
            this.SamlList.TabIndex = 11;
            // 
            // SamlListBtn
            // 
            this.SamlListBtn.Location = new System.Drawing.Point(303, 21);
            this.SamlListBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SamlListBtn.Name = "SamlListBtn";
            this.SamlListBtn.Size = new System.Drawing.Size(100, 28);
            this.SamlListBtn.TabIndex = 10;
            this.SamlListBtn.Text = "Get Lists";
            this.SamlListBtn.UseVisualStyleBackColor = true;
            this.SamlListBtn.Click += new System.EventHandler(this.SamlListBtn_Click);
            // 
            // SamlTxt
            // 
            this.SamlTxt.Location = new System.Drawing.Point(99, 23);
            this.SamlTxt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SamlTxt.Name = "SamlTxt";
            this.SamlTxt.Size = new System.Drawing.Size(195, 22);
            this.SamlTxt.TabIndex = 9;
            this.SamlTxt.Text = "https://yoursharepointsite.local";
            this.SamlTxt.TextChanged += new System.EventHandler(this.SamlTxt_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 27);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 17);
            this.label6.TabIndex = 8;
            this.label6.Text = "SAML Site:";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(344, 254);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(303, 473);
            this.txtOutput.TabIndex = 10;
            this.txtOutput.Text = "";
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(16, 254);
            this.txtResults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(303, 473);
            this.txtResults.TabIndex = 11;
            this.txtResults.Text = "";
            // 
            // ClientFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 742);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.SamlGroup);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ClientFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client OM Auth Test";
            this.SamlGroup.ResumeLayout(false);
            this.SamlGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox SamlGroup;
        private System.Windows.Forms.ListBox SamlList;
        private System.Windows.Forms.Button SamlListBtn;
        private System.Windows.Forms.TextBox SamlTxt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox AdfsTxt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button GetRestDataBtn;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCookie;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.ListBox FbaLst;
        private System.Windows.Forms.RichTextBox txtResults;
    }
}

