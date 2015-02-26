using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.TeamFoundation.Build.Controls;
using System.Reflection;
using ShiningDragon.TFSProd.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using ShiningDragon.TFSProd.Common.Controls;


namespace ShiningDragon.TFSProd.TFS.Builds
{
    internal partial class WorkSpaceMappingDialog : Form
    {
        internal WorkSpaceMappingDialog(List<WorkspaceBranchMapping> _originalMappings, VersionControlServer _versionControlServer)
        {
            InitializeComponent();

            AddColumns();

            originalMappings = _originalMappings;
            branchedMappings = new List<WorkspaceBranchMapping>();
            versionControlServer = _versionControlServer;

            buildControlsAssembly = Assembly.Load("Microsoft.TeamFoundation.Build.Controls, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var alltypes = buildControlsAssembly.GetTypes();
            var t = alltypes.First(item => item.Name == "VersionControlHelper");
            methodShowServerFolderBrowser = t.GetMethod("ShowServerFolderBrowser", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
                null,
                new Type[] { typeof(VersionControlServer), typeof(string), typeof(string).MakeByRefType() },
                null);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (WorkspaceBranchMapping mapping in originalMappings)
            {
                string[] row = new string[] { mapping.SourceServerPath, mapping.TargetServerPath };
                dataGridViewWorkSpace.Rows.Add(row);
                DataGridViewInvisibleButtonCell cell = dataGridViewWorkSpace[2, dataGridViewWorkSpace.Rows.Count - 1] as DataGridViewInvisibleButtonCell;
                //cell.IsVisible = false;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewWorkSpace.Rows)
            {
                branchedMappings.Add(new WorkspaceBranchMapping(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString()));
            }
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void dataGridViewWorkSpcae_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells.  
            if (e.RowIndex < 0 || e.ColumnIndex != 2 || methodShowServerFolderBrowser == null)
                return;

            DataGridViewInvisibleButtonCell cell = dataGridViewWorkSpace[2, e.RowIndex] as DataGridViewInvisibleButtonCell;
            if (cell.IsVisible)
            {
                string currentPath = dataGridViewWorkSpace[0, e.RowIndex].Value.ToString();
                string newPath = SelectServerFolder(currentPath);
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    dataGridViewWorkSpace[1, e.RowIndex].Value = newPath;
                }
            }
        }

        private void dataGridViewWorkSpace_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex == 0)
                return;

            //DataGridViewInvisibleButtonCell cell = dataGridViewWorkSpace[2, e.RowIndex] as DataGridViewInvisibleButtonCell;
            //cell.IsVisible = true;
            //dataGridViewWorkSpace.Refresh();
        }

        private void dataGridViewWorkSpace_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex == 0)
                return;

            //DataGridViewInvisibleButtonCell cell = dataGridViewWorkSpace[2, e.RowIndex] as DataGridViewInvisibleButtonCell;
            //cell.IsVisible = false;
            //dataGridViewWorkSpace.Refresh();
        }

        private string SelectServerFolder(string currentPath)
        {
            //Assembly controlsAssembly = Assembly.GetAssembly(typeof(Microsoft.TeamFoundation.VersionControl.Controls.ControlAddItemsExclude));
            //Type vcChooseItemDialogType = controlsAssembly.GetType("Microsoft.TeamFoundation.VersionControl.Controls.DialogChooseItem");
            //ConstructorInfo ci = vcChooseItemDialogType.GetConstructor(
            //                   BindingFlags.Instance | BindingFlags.NonPublic,
            //                   null,
            //                   new Type[] { typeof(VersionControlServer) },
            //                   null);
            //Form _chooseItemDialog = (Form)ci.Invoke(new object[] { versionControlServer });
            //_chooseItemDialog.ShowDialog();

            string newPath = string.Empty;
            object[] args = new object[] { versionControlServer, currentPath, newPath };
            methodShowServerFolderBrowser.Invoke(null, args);

            return args[2] as string;
        }

        private void AddColumns()
        {
            columnSourceFolder = new DataGridViewTextBoxColumn();
            columnSourceFolder.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columnSourceFolder.HeaderText = "Original Folder";
            columnSourceFolder.Name = "columnSourceFolder";
            columnSourceFolder.ReadOnly = true;
            dataGridViewWorkSpace.Columns.Add(columnSourceFolder);

            // Add target folder column
            columnTargetFolder = new DataGridViewDontDrawRightBorderColumn();
            columnTargetFolder.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            columnTargetFolder.HeaderText = "Branched Folder";
            columnTargetFolder.Name = "columnTargetFolder";
            dataGridViewWorkSpace.Columns.Add(columnTargetFolder);

            // Add the custom button column
            columnSourceControlPicker = new DataGridViewInvisibleButtonColumn();
            columnSourceControlPicker.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            columnSourceControlPicker.HeaderText = "";
            columnSourceControlPicker.Name = "columnSourceControlPicker";
            columnSourceControlPicker.ReadOnly = true;
            columnSourceControlPicker.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            columnSourceControlPicker.Text = "...";
            columnSourceControlPicker.UseColumnTextForButtonValue = true;
            columnSourceControlPicker.Width = 5;
            columnSourceControlPicker.DefaultCellStyle.SelectionBackColor = columnSourceControlPicker.DefaultCellStyle.BackColor;
            columnSourceControlPicker.DefaultCellStyle.SelectionForeColor = columnSourceControlPicker.DefaultCellStyle.ForeColor;
            dataGridViewWorkSpace.Columns.Add(columnSourceControlPicker);
        }

        #region instance data

        internal List<WorkspaceBranchMapping> BranchedMappings
        {
            get
            {
                return branchedMappings;
            }
        }
        private List<WorkspaceBranchMapping> branchedMappings;
        private List<WorkspaceBranchMapping> originalMappings;
        private VersionControlServer versionControlServer;
        private Assembly buildControlsAssembly;
        private MethodInfo methodShowServerFolderBrowser;

        private DataGridViewInvisibleButtonColumn columnSourceControlPicker;
        private DataGridViewDontDrawRightBorderColumn columnTargetFolder;
        private DataGridViewTextBoxColumn columnSourceFolder;

        #endregion
    }
}
