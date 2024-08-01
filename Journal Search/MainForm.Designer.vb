﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.dgvData = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column8 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column9 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column10 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column11 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtFilter = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnFilter = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportUpdateDatabaseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddOneEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearFilterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutosizeColumnsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MinisizeColumnsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblFilteredCount = New System.Windows.Forms.Label()
        Me.chkAllColumns = New System.Windows.Forms.CheckBox()
        Me.chkPartialMatch = New System.Windows.Forms.CheckBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.prgStatusBar = New System.Windows.Forms.ToolStripProgressBar()
        CType(Me.dgvData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvData
        '
        Me.dgvData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5, Me.Column6, Me.Column7, Me.Column8, Me.Column9, Me.Column10, Me.Column11})
        Me.dgvData.Location = New System.Drawing.Point(0, 74)
        Me.dgvData.Margin = New System.Windows.Forms.Padding(4)
        Me.dgvData.Name = "dgvData"
        Me.dgvData.Size = New System.Drawing.Size(1188, 474)
        Me.dgvData.TabIndex = 0
        '
        'Column1
        '
        Me.Column1.DataPropertyName = "JournalName"
        Me.Column1.HeaderText = "Journal name"
        Me.Column1.Name = "Column1"
        Me.Column1.ReadOnly = True
        '
        'Column2
        '
        Me.Column2.DataPropertyName = "PublisherName"
        Me.Column2.HeaderText = "Publisher"
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        '
        'Column3
        '
        Me.Column3.DataPropertyName = "ISSN1"
        Me.Column3.HeaderText = "ISSN1"
        Me.Column3.Name = "Column3"
        Me.Column3.ReadOnly = True
        '
        'Column4
        '
        Me.Column4.DataPropertyName = "ISSN2"
        Me.Column4.HeaderText = "ISSN2"
        Me.Column4.Name = "Column4"
        Me.Column4.ReadOnly = True
        '
        'Column5
        '
        Me.Column5.DataPropertyName = "Source"
        Me.Column5.HeaderText = "Source"
        Me.Column5.Name = "Column5"
        Me.Column5.ReadOnly = True
        '
        'Column6
        '
        Me.Column6.DataPropertyName = "Ranking"
        Me.Column6.HeaderText = "Ranking"
        Me.Column6.Name = "Column6"
        Me.Column6.ReadOnly = True
        '
        'Column7
        '
        Me.Column7.DataPropertyName = "Rating"
        Me.Column7.HeaderText = "Rating"
        Me.Column7.Name = "Column7"
        Me.Column7.ReadOnly = True
        '
        'Column8
        '
        Me.Column8.DataPropertyName = "HIndex"
        Me.Column8.HeaderText = "H-Index"
        Me.Column8.Name = "Column8"
        Me.Column8.ReadOnly = True
        '
        'Column9
        '
        Me.Column9.DataPropertyName = "Country"
        Me.Column9.HeaderText = "Country"
        Me.Column9.Name = "Column9"
        Me.Column9.ReadOnly = True
        '
        'Column10
        '
        Me.Column10.DataPropertyName = "Categories"
        Me.Column10.HeaderText = "Categories"
        Me.Column10.Name = "Column10"
        Me.Column10.ReadOnly = True
        '
        'Column11
        '
        Me.Column11.DataPropertyName = "Notes"
        Me.Column11.HeaderText = "Notes"
        Me.Column11.Name = "Column11"
        '
        'txtFilter
        '
        Me.txtFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFilter.Location = New System.Drawing.Point(58, 42)
        Me.txtFilter.Margin = New System.Windows.Forms.Padding(4)
        Me.txtFilter.Name = "txtFilter"
        Me.txtFilter.Size = New System.Drawing.Size(618, 22)
        Me.txtFilter.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 46)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(40, 16)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Filter:"
        '
        'btnFilter
        '
        Me.btnFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFilter.Location = New System.Drawing.Point(960, 40)
        Me.btnFilter.Margin = New System.Windows.Forms.Padding(4)
        Me.btnFilter.Name = "btnFilter"
        Me.btnFilter.Size = New System.Drawing.Size(38, 28)
        Me.btnFilter.TabIndex = 3
        Me.btnFilter.Text = ">>"
        Me.btnFilter.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.ViewToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(7, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(1188, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(93, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ImportUpdateDatabaseToolStripMenuItem, Me.AddOneEntryToolStripMenuItem, Me.ClearFilterToolStripMenuItem})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.EditToolStripMenuItem.Text = "&Data"
        '
        'ImportUpdateDatabaseToolStripMenuItem
        '
        Me.ImportUpdateDatabaseToolStripMenuItem.Name = "ImportUpdateDatabaseToolStripMenuItem"
        Me.ImportUpdateDatabaseToolStripMenuItem.Size = New System.Drawing.Size(217, 22)
        Me.ImportUpdateDatabaseToolStripMenuItem.Text = "Import / update database..."
        '
        'AddOneEntryToolStripMenuItem
        '
        Me.AddOneEntryToolStripMenuItem.Name = "AddOneEntryToolStripMenuItem"
        Me.AddOneEntryToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.AddOneEntryToolStripMenuItem.Size = New System.Drawing.Size(217, 22)
        Me.AddOneEntryToolStripMenuItem.Text = "&Add one entry"
        '
        'ClearFilterToolStripMenuItem
        '
        Me.ClearFilterToolStripMenuItem.Name = "ClearFilterToolStripMenuItem"
        Me.ClearFilterToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Q), System.Windows.Forms.Keys)
        Me.ClearFilterToolStripMenuItem.Size = New System.Drawing.Size(217, 22)
        Me.ClearFilterToolStripMenuItem.Text = "&Clear filter"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AutosizeColumnsToolStripMenuItem, Me.MinisizeColumnsToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "&View"
        '
        'AutosizeColumnsToolStripMenuItem
        '
        Me.AutosizeColumnsToolStripMenuItem.Name = "AutosizeColumnsToolStripMenuItem"
        Me.AutosizeColumnsToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
        Me.AutosizeColumnsToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.AutosizeColumnsToolStripMenuItem.Text = "&Autosize columns"
        '
        'MinisizeColumnsToolStripMenuItem
        '
        Me.MinisizeColumnsToolStripMenuItem.Name = "MinisizeColumnsToolStripMenuItem"
        Me.MinisizeColumnsToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.MinisizeColumnsToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.MinisizeColumnsToolStripMenuItem.Text = "&Mini-size columns"
        '
        'lblFilteredCount
        '
        Me.lblFilteredCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFilteredCount.AutoSize = True
        Me.lblFilteredCount.Location = New System.Drawing.Point(1006, 46)
        Me.lblFilteredCount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblFilteredCount.Name = "lblFilteredCount"
        Me.lblFilteredCount.Size = New System.Drawing.Size(0, 16)
        Me.lblFilteredCount.TabIndex = 5
        '
        'chkAllColumns
        '
        Me.chkAllColumns.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkAllColumns.AutoSize = True
        Me.chkAllColumns.Location = New System.Drawing.Point(705, 45)
        Me.chkAllColumns.Name = "chkAllColumns"
        Me.chkAllColumns.Size = New System.Drawing.Size(94, 20)
        Me.chkAllColumns.TabIndex = 6
        Me.chkAllColumns.Text = "&All columns"
        Me.chkAllColumns.UseVisualStyleBackColor = True
        '
        'chkPartialMatch
        '
        Me.chkPartialMatch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkPartialMatch.AutoSize = True
        Me.chkPartialMatch.Checked = True
        Me.chkPartialMatch.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkPartialMatch.Location = New System.Drawing.Point(816, 45)
        Me.chkPartialMatch.Name = "chkPartialMatch"
        Me.chkPartialMatch.Size = New System.Drawing.Size(103, 20)
        Me.chkPartialMatch.TabIndex = 6
        Me.chkPartialMatch.Text = "&Partial match"
        Me.chkPartialMatch.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblStatusLabel1, Me.prgStatusBar})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 552)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1188, 22)
        Me.StatusStrip1.TabIndex = 7
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblStatusLabel1
        '
        Me.lblStatusLabel1.Name = "lblStatusLabel1"
        Me.lblStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'prgStatusBar
        '
        Me.prgStatusBar.Name = "prgStatusBar"
        Me.prgStatusBar.Size = New System.Drawing.Size(400, 16)
        Me.prgStatusBar.Visible = False
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1188, 574)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.chkPartialMatch)
        Me.Controls.Add(Me.chkAllColumns)
        Me.Controls.Add(Me.lblFilteredCount)
        Me.Controls.Add(Me.btnFilter)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtFilter)
        Me.Controls.Add(Me.dgvData)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "MainForm"
        Me.Text = "Journal Search"
        CType(Me.dgvData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvData As DataGridView
    Friend WithEvents txtFilter As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnFilter As Button
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportUpdateDatabaseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lblFilteredCount As Label
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AutosizeColumnsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearFilterToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MinisizeColumnsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkAllColumns As CheckBox
    Friend WithEvents chkPartialMatch As CheckBox
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column7 As DataGridViewTextBoxColumn
    Friend WithEvents Column8 As DataGridViewTextBoxColumn
    Friend WithEvents Column9 As DataGridViewTextBoxColumn
    Friend WithEvents Column10 As DataGridViewTextBoxColumn
    Friend WithEvents Column11 As DataGridViewTextBoxColumn
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents lblStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents prgStatusBar As ToolStripProgressBar
    Friend WithEvents AddOneEntryToolStripMenuItem As ToolStripMenuItem
End Class
