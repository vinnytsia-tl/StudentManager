using ADProvider;
using ADProvider.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagerUI
{
    public partial class MainForm : Form
    {
        private StudentManager sm;
        private BackgroundWorker bw;

        public MainForm(ADConfig config)
        {
            sm = new StudentManager(config);
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            InitializeComponent();
        }

        private void progressBarCallBack(ActionStatus status, double progress)
        {
            bw.ReportProgress((int)(100 * progress), status);
        }

        private string[] getStudentRow(User student) => new string[]
            { student.SamAccountName, student.NameUA.FirstName, student.NameUA.MiddleName,
              student.NameUA.SurName, student.Group, student.DisplayName, student.Name };

        private void MainForm_Load(object sender, EventArgs e)
        {
            bw.DoWork += (object _sender, DoWorkEventArgs eventArgs) =>
            {
                sm.LoadADContexts(progressBarCallBack);
                sm.LoadFromAD(progressBarCallBack);
            };
            bw.ProgressChanged += (object _sender, ProgressChangedEventArgs eventArgs) =>
            {
                statusLabel.Text = eventArgs.UserState.ToString();
                statusProgressBar.Value = eventArgs.ProgressPercentage;
                statusProgressLabel.Text = $"{eventArgs.ProgressPercentage}%";
            };
            bw.RunWorkerCompleted += (object _sender, RunWorkerCompletedEventArgs eventArgs) =>
            {
                sm.GetStudents().ForEach(student => { studentsView.Rows.Add(getStudentRow(student)); });
            };
            bw.RunWorkerAsync();
        }

        private void studentsView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (studentsView.Columns[e.ColumnIndex].ReadOnly)
                return;

            var headerText = studentsView.Columns[e.ColumnIndex].HeaderText;
            var oldData = studentsView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            string SAM = "";
            string name = "";

            for (int i = 0; i < studentsView.Columns.Count; i++)
            {
                switch(studentsView.Columns[i].HeaderText)
                {
                    case "SAM":
                        SAM = studentsView.Rows[e.RowIndex].Cells[i].Value.ToString();
                        break;
                    case "Name":
                        name = studentsView.Rows[e.RowIndex].Cells[i].Value.ToString();
                        break;
                }
            }

            var newData = Interaction.InputBox($"Enter new {headerText} for {name}", $"Updating {headerText}", oldData);
            if (!string.IsNullOrEmpty(newData))
            {
                User student = null;
                switch (headerText)
                {
                    case "Group":
                        student = sm.UpdateGroup(SAM, newData);
                        break;
                }
                studentsView.Rows[e.RowIndex].SetValues(getStudentRow(student));
            }
        }
    }
}
