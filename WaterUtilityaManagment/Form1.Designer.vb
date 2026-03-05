<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        lblWelcome = New Label()
        btnGoToLogin = New Button()
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(btnGoToLogin)
        Controls.Add(lblWelcome)
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Water Utility Management System"

        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblWelcome.Location = New Point(130, 120)
        lblWelcome.Name = "lblWelcome"
        lblWelcome.Size = New Size(540, 30)
        lblWelcome.Text = "Welcome to the Water Utility Management System"

        btnGoToLogin.Location = New Point(320, 210)
        btnGoToLogin.Name = "btnGoToLogin"
        btnGoToLogin.Size = New Size(160, 42)
        btnGoToLogin.Text = "Go to Login"
        btnGoToLogin.UseVisualStyleBackColor = True
    End Sub

    Friend WithEvents lblWelcome As Label
    Friend WithEvents btnGoToLogin As Button

End Class
