namespace Cards_Generic_Engine
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			JoinButton = new Button();
			HostButton = new Button();
			EnterCodeButton = new Button();
			EnterCodeBox = new TextBox();
			EnterCodeLabel = new Label();
			CodeLabel = new Label();
			SuspendLayout();
			// 
			// JoinButton
			// 
			JoinButton.Location = new Point(400, 300);
			JoinButton.Name = "JoinButton";
			JoinButton.Size = new Size(480, 48);
			JoinButton.TabIndex = 0;
			JoinButton.Text = "Join Game";
			JoinButton.UseVisualStyleBackColor = true;
			JoinButton.Click += ShowJoinMenu;
			// 
			// HostButton
			// 
			HostButton.Location = new Point(399, 361);
			HostButton.Name = "HostButton";
			HostButton.Size = new Size(480, 48);
			HostButton.TabIndex = 1;
			HostButton.Text = "Host Game";
			HostButton.UseVisualStyleBackColor = true;
			HostButton.Click += ShowHostMenu;
			// 
			// EnterCodeButton
			// 
			EnterCodeButton.Enabled = false;
			EnterCodeButton.Location = new Point(512, 392);
			EnterCodeButton.Name = "EnterCodeButton";
			EnterCodeButton.Size = new Size(256, 48);
			EnterCodeButton.TabIndex = 0;
			EnterCodeButton.Text = "Join Game";
			EnterCodeButton.UseVisualStyleBackColor = true;
			EnterCodeButton.Visible = false;
			EnterCodeButton.Click += EnterCodeButton_Click;
			// 
			// EnterCodeBox
			// 
			EnterCodeBox.Enabled = false;
			EnterCodeBox.Location = new Point(512, 360);
			EnterCodeBox.Name = "EnterCodeBox";
			EnterCodeBox.Size = new Size(256, 31);
			EnterCodeBox.TabIndex = 1;
			EnterCodeBox.Visible = false;
			// 
			// EnterCodeLabel
			// 
			EnterCodeLabel.Enabled = false;
			EnterCodeLabel.Location = new Point(555, 335);
			EnterCodeLabel.Name = "EnterCodeLabel";
			EnterCodeLabel.Size = new Size(171, 25);
			EnterCodeLabel.TabIndex = 2;
			EnterCodeLabel.Text = "Enter Network Code";
			EnterCodeLabel.TextAlign = ContentAlignment.MiddleCenter;
			EnterCodeLabel.Visible = false;
			// 
			// CodeLabel
			// 
			CodeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			CodeLabel.AutoSize = true;
			CodeLabel.BackColor = Color.Transparent;
			CodeLabel.Location = new Point(1226, 686);
			CodeLabel.Name = "CodeLabel";
			CodeLabel.Size = new Size(51, 25);
			CodeLabel.TabIndex = 3;
			CodeLabel.Text = "code";
			CodeLabel.TextAlign = ContentAlignment.MiddleCenter;
			CodeLabel.Visible = false;
			CodeLabel.Click += CopyCode;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			BackgroundImage = Properties.Resources.Felt;
			BackgroundImageLayout = ImageLayout.Stretch;
			ClientSize = new Size(1280, 720);
			Controls.Add(CodeLabel);
			Controls.Add(HostButton);
			Controls.Add(JoinButton);
			Controls.Add(EnterCodeLabel);
			Controls.Add(EnterCodeBox);
			Controls.Add(EnterCodeButton);
			Name = "Form1";
			Text = "Cards";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button JoinButton;
        private Button HostButton;
        private Button EnterCodeButton;
        private TextBox EnterCodeBox;
        private Label EnterCodeLabel;
		private Label CodeLabel;
	}
}
