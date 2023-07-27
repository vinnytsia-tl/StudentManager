namespace StudentManagerUI
{
    partial class ValidationForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusProgressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.studentsView = new System.Windows.Forms.DataGridView();
            this.ColumnSAM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCurrentDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCorrectDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCurrentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCorrectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.studentsView)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.statusProgressBar,
            this.statusProgressLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(999, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(66, 17);
            this.statusLabel.Text = "statusLabel";
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(300, 16);
            // 
            // statusProgressLabel
            // 
            this.statusProgressLabel.Name = "statusProgressLabel";
            this.statusProgressLabel.Size = new System.Drawing.Size(23, 17);
            this.statusProgressLabel.Text = "0%";
            // 
            // studentsView
            // 
            this.studentsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.studentsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSAM,
            this.ColumnCurrentDisplayName,
            this.ColumnCorrectDisplayName,
            this.ColumnCurrentName,
            this.ColumnCorrectName});
            this.studentsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.studentsView.Location = new System.Drawing.Point(0, 0);
            this.studentsView.Name = "studentsView";
            this.studentsView.Size = new System.Drawing.Size(999, 428);
            this.studentsView.TabIndex = 3;
            // 
            // ColumnSAM
            // 
            this.ColumnSAM.HeaderText = "SAM";
            this.ColumnSAM.Name = "ColumnSAM";
            this.ColumnSAM.ReadOnly = true;
            // 
            // ColumnCurrentDisplayName
            // 
            this.ColumnCurrentDisplayName.HeaderText = "CurrentDisplayName";
            this.ColumnCurrentDisplayName.Name = "ColumnCurrentDisplayName";
            this.ColumnCurrentDisplayName.ReadOnly = true;
            this.ColumnCurrentDisplayName.Width = 150;
            // 
            // ColumnCorrectDisplayName
            // 
            this.ColumnCorrectDisplayName.HeaderText = "CorrectDisplayName";
            this.ColumnCorrectDisplayName.Name = "ColumnCorrectDisplayName";
            this.ColumnCorrectDisplayName.ReadOnly = true;
            this.ColumnCorrectDisplayName.Width = 150;
            // 
            // ColumnCurrentName
            // 
            this.ColumnCurrentName.HeaderText = "CurrentName";
            this.ColumnCurrentName.Name = "ColumnCurrentName";
            this.ColumnCurrentName.ReadOnly = true;
            this.ColumnCurrentName.Width = 250;
            // 
            // ColumnCorrectName
            // 
            this.ColumnCorrectName.HeaderText = "CorrectName";
            this.ColumnCorrectName.Name = "ColumnCorrectName";
            this.ColumnCorrectName.Width = 250;
            // 
            // ValidationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(999, 450);
            this.Controls.Add(this.studentsView);
            this.Controls.Add(this.statusStrip1);
            this.Name = "ValidationForm";
            this.Text = "ValidationForm";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.studentsView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripProgressBar statusProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusProgressLabel;
        private System.Windows.Forms.DataGridView studentsView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSAM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCurrentDisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCorrectDisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCurrentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCorrectName;
    }
}