<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.inventoryText = New System.Windows.Forms.Label()
        Me.inventoryLabel = New System.Windows.Forms.Label()
        Me.movingLabel = New System.Windows.Forms.Label()
        Me.movingText = New System.Windows.Forms.Label()
        Me.lastAction = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.environmentLabel = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.logLabel = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'inventoryText
        '
        Me.inventoryText.AutoSize = True
        Me.inventoryText.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inventoryText.Location = New System.Drawing.Point(12, 9)
        Me.inventoryText.Name = "inventoryText"
        Me.inventoryText.Size = New System.Drawing.Size(146, 37)
        Me.inventoryText.TabIndex = 1
        Me.inventoryText.Text = "Inventory"
        '
        'inventoryLabel
        '
        Me.inventoryLabel.AutoSize = True
        Me.inventoryLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.inventoryLabel.Location = New System.Drawing.Point(248, 9)
        Me.inventoryLabel.Name = "inventoryLabel"
        Me.inventoryLabel.Size = New System.Drawing.Size(0, 37)
        Me.inventoryLabel.TabIndex = 2
        '
        'movingLabel
        '
        Me.movingLabel.AutoSize = True
        Me.movingLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.movingLabel.Location = New System.Drawing.Point(248, 62)
        Me.movingLabel.Name = "movingLabel"
        Me.movingLabel.Size = New System.Drawing.Size(0, 37)
        Me.movingLabel.TabIndex = 4
        '
        'movingText
        '
        Me.movingText.AutoSize = True
        Me.movingText.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.movingText.Location = New System.Drawing.Point(12, 62)
        Me.movingText.Name = "movingText"
        Me.movingText.Size = New System.Drawing.Size(119, 37)
        Me.movingText.TabIndex = 3
        Me.movingText.Text = "Moving"
        Me.movingText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lastAction
        '
        Me.lastAction.AutoSize = True
        Me.lastAction.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lastAction.Location = New System.Drawing.Point(248, 116)
        Me.lastAction.Name = "lastAction"
        Me.lastAction.Size = New System.Drawing.Size(0, 37)
        Me.lastAction.TabIndex = 6
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 116)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(108, 37)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Status"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'environmentLabel
        '
        Me.environmentLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.environmentLabel.Location = New System.Drawing.Point(19, 247)
        Me.environmentLabel.Multiline = True
        Me.environmentLabel.Name = "environmentLabel"
        Me.environmentLabel.Size = New System.Drawing.Size(706, 1063)
        Me.environmentLabel.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 185)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(196, 37)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Environment"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'logLabel
        '
        Me.logLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.logLabel.Location = New System.Drawing.Point(750, 247)
        Me.logLabel.Multiline = True
        Me.logLabel.Name = "logLabel"
        Me.logLabel.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.logLabel.Size = New System.Drawing.Size(706, 1063)
        Me.logLabel.TabIndex = 9
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1727, 1322)
        Me.Controls.Add(Me.logLabel)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.environmentLabel)
        Me.Controls.Add(Me.lastAction)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.movingLabel)
        Me.Controls.Add(Me.movingText)
        Me.Controls.Add(Me.inventoryLabel)
        Me.Controls.Add(Me.inventoryText)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Timer1 As Timer
    Friend WithEvents inventoryText As Label
    Friend WithEvents inventoryLabel As Label
    Friend WithEvents movingLabel As Label
    Friend WithEvents movingText As Label
    Friend WithEvents lastAction As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents environmentLabel As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents logLabel As TextBox
End Class
