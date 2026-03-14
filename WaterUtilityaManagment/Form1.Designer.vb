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
        pnlHero = New Panel()
        pnlCardShadow = New Panel()
        pnlCard = New Panel()
        lblFooter = New Label()
        btnGoToLogin = New Button()
        pnlStatTariff = New Panel()
        lblTariffValue = New Label()
        lblTariffTitle = New Label()
        lblTariffIcon = New Label()
        pnlStatOutstanding = New Panel()
        lblOutstandingValue = New Label()
        lblOutstandingTitle = New Label()
        lblOutstandingIcon = New Label()
        pnlStatCustomers = New Panel()
        lblCustomersValue = New Label()
        lblCustomersTitle = New Label()
        lblCustomersIcon = New Label()
        lblSubtitle = New Label()
        lblTitle = New Label()
        lblLogo = New Label()
        pnlHero.SuspendLayout()
        pnlCard.SuspendLayout()
        pnlStatTariff.SuspendLayout()
        pnlStatOutstanding.SuspendLayout()
        pnlStatCustomers.SuspendLayout()
        SuspendLayout()
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1200, 700)
        BackColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        Controls.Add(pnlHero)
        FormBorderStyle = FormBorderStyle.Sizable
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Water Utility Management"
        WindowState = FormWindowState.Maximized

        pnlHero.Controls.Add(pnlCardShadow)
        pnlHero.Controls.Add(pnlCard)
        pnlHero.Dock = DockStyle.Fill
        pnlHero.Name = "pnlHero"

        pnlCardShadow.BackColor = Color.FromArgb(CByte(35), CByte(0), CByte(0), CByte(0))
        pnlCardShadow.Name = "pnlCardShadow"
        pnlCardShadow.Size = New Size(700, 500)

        pnlCard.BackColor = Color.White
        pnlCard.Controls.Add(lblFooter)
        pnlCard.Controls.Add(btnGoToLogin)
        pnlCard.Controls.Add(pnlStatTariff)
        pnlCard.Controls.Add(pnlStatOutstanding)
        pnlCard.Controls.Add(pnlStatCustomers)
        pnlCard.Controls.Add(lblSubtitle)
        pnlCard.Controls.Add(lblTitle)
        pnlCard.Controls.Add(lblLogo)
        pnlCard.Name = "pnlCard"
        pnlCard.Size = New Size(700, 500)

        lblFooter.Font = New Font("Segoe UI", 9.0F)
        lblFooter.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblFooter.Location = New Point(20, 470)
        lblFooter.Name = "lblFooter"
        lblFooter.Size = New Size(660, 20)
        lblFooter.Text = "v2.0.0  •  © 2026 Water Utility Management System"
        lblFooter.TextAlign = ContentAlignment.MiddleCenter

        btnGoToLogin.BackColor = Color.FromArgb(CByte(39), CByte(174), CByte(96))
        btnGoToLogin.FlatAppearance.BorderSize = 0
        btnGoToLogin.FlatStyle = FlatStyle.Flat
        btnGoToLogin.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        btnGoToLogin.ForeColor = Color.White
        btnGoToLogin.Location = New Point(225, 390)
        btnGoToLogin.Name = "btnGoToLogin"
        btnGoToLogin.Size = New Size(250, 60)
        btnGoToLogin.Text = "🔐 Login to System"
        btnGoToLogin.UseVisualStyleBackColor = False

        pnlStatTariff.BackColor = Color.White
        pnlStatTariff.BorderStyle = BorderStyle.FixedSingle
        pnlStatTariff.Controls.Add(lblTariffValue)
        pnlStatTariff.Controls.Add(lblTariffTitle)
        pnlStatTariff.Controls.Add(lblTariffIcon)
        pnlStatTariff.Location = New Point(474, 208)
        pnlStatTariff.Name = "pnlStatTariff"
        pnlStatTariff.Size = New Size(200, 160)

        lblTariffValue.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblTariffValue.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblTariffValue.Location = New Point(12, 95)
        lblTariffValue.Name = "lblTariffValue"
        lblTariffValue.Size = New Size(174, 32)
        lblTariffValue.Text = "0.00"
        lblTariffValue.TextAlign = ContentAlignment.MiddleCenter

        lblTariffTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblTariffTitle.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblTariffTitle.Location = New Point(12, 62)
        lblTariffTitle.Name = "lblTariffTitle"
        lblTariffTitle.Size = New Size(174, 24)
        lblTariffTitle.Text = "CURRENT TARIFF"
        lblTariffTitle.TextAlign = ContentAlignment.MiddleCenter

        lblTariffIcon.Font = New Font("Segoe UI Emoji", 22.0F)
        lblTariffIcon.ForeColor = Color.FromArgb(CByte(39), CByte(174), CByte(96))
        lblTariffIcon.Location = New Point(65, 12)
        lblTariffIcon.Name = "lblTariffIcon"
        lblTariffIcon.Size = New Size(70, 45)
        lblTariffIcon.Text = "📈"
        lblTariffIcon.TextAlign = ContentAlignment.MiddleCenter

        pnlStatOutstanding.BackColor = Color.White
        pnlStatOutstanding.BorderStyle = BorderStyle.FixedSingle
        pnlStatOutstanding.Controls.Add(lblOutstandingValue)
        pnlStatOutstanding.Controls.Add(lblOutstandingTitle)
        pnlStatOutstanding.Controls.Add(lblOutstandingIcon)
        pnlStatOutstanding.Location = New Point(250, 208)
        pnlStatOutstanding.Name = "pnlStatOutstanding"
        pnlStatOutstanding.Size = New Size(200, 160)

        lblOutstandingValue.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblOutstandingValue.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblOutstandingValue.Location = New Point(12, 95)
        lblOutstandingValue.Name = "lblOutstandingValue"
        lblOutstandingValue.Size = New Size(174, 32)
        lblOutstandingValue.Text = "0.00"
        lblOutstandingValue.TextAlign = ContentAlignment.MiddleCenter

        lblOutstandingTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblOutstandingTitle.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblOutstandingTitle.Location = New Point(12, 62)
        lblOutstandingTitle.Name = "lblOutstandingTitle"
        lblOutstandingTitle.Size = New Size(174, 24)
        lblOutstandingTitle.Text = "OUTSTANDING BALANCE"
        lblOutstandingTitle.TextAlign = ContentAlignment.MiddleCenter

        lblOutstandingIcon.Font = New Font("Segoe UI Emoji", 22.0F)
        lblOutstandingIcon.ForeColor = Color.FromArgb(CByte(231), CByte(76), CByte(60))
        lblOutstandingIcon.Location = New Point(65, 12)
        lblOutstandingIcon.Name = "lblOutstandingIcon"
        lblOutstandingIcon.Size = New Size(70, 45)
        lblOutstandingIcon.Text = "💳"
        lblOutstandingIcon.TextAlign = ContentAlignment.MiddleCenter

        pnlStatCustomers.BackColor = Color.White
        pnlStatCustomers.BorderStyle = BorderStyle.FixedSingle
        pnlStatCustomers.Controls.Add(lblCustomersValue)
        pnlStatCustomers.Controls.Add(lblCustomersTitle)
        pnlStatCustomers.Controls.Add(lblCustomersIcon)
        pnlStatCustomers.Location = New Point(26, 208)
        pnlStatCustomers.Name = "pnlStatCustomers"
        pnlStatCustomers.Size = New Size(200, 160)

        lblCustomersValue.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblCustomersValue.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblCustomersValue.Location = New Point(12, 95)
        lblCustomersValue.Name = "lblCustomersValue"
        lblCustomersValue.Size = New Size(174, 32)
        lblCustomersValue.Text = "0"
        lblCustomersValue.TextAlign = ContentAlignment.MiddleCenter

        lblCustomersTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblCustomersTitle.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblCustomersTitle.Location = New Point(12, 62)
        lblCustomersTitle.Name = "lblCustomersTitle"
        lblCustomersTitle.Size = New Size(174, 24)
        lblCustomersTitle.Text = "TOTAL CUSTOMERS"
        lblCustomersTitle.TextAlign = ContentAlignment.MiddleCenter

        lblCustomersIcon.Font = New Font("Segoe UI Emoji", 22.0F)
        lblCustomersIcon.ForeColor = Color.FromArgb(CByte(52), CByte(152), CByte(219))
        lblCustomersIcon.Location = New Point(65, 12)
        lblCustomersIcon.Name = "lblCustomersIcon"
        lblCustomersIcon.Size = New Size(70, 45)
        lblCustomersIcon.Text = "👥"
        lblCustomersIcon.TextAlign = ContentAlignment.MiddleCenter

        lblSubtitle.AutoSize = True
        lblSubtitle.Font = New Font("Segoe UI", 14.0F)
        lblSubtitle.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblSubtitle.Location = New Point(180, 145)
        lblSubtitle.Name = "lblSubtitle"
        lblSubtitle.Text = "Efficient Billing & Customer Management"

        lblTitle.AutoSize = True
        lblTitle.Font = New Font("Segoe UI", 28.0F, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblTitle.Location = New Point(80, 93)
        lblTitle.Name = "lblTitle"
        lblTitle.Text = "Water Utility Management System"

        lblLogo.AutoSize = True
        lblLogo.Font = New Font("Segoe UI Emoji", 34.0F)
        lblLogo.ForeColor = Color.FromArgb(CByte(52), CByte(152), CByte(219))
        lblLogo.Location = New Point(314, 25)
        lblLogo.Name = "lblLogo"
        lblLogo.Text = "💧"
        pnlHero.ResumeLayout(False)
        pnlCard.ResumeLayout(False)
        pnlCard.PerformLayout()
        pnlStatTariff.ResumeLayout(False)
        pnlStatOutstanding.ResumeLayout(False)
        pnlStatCustomers.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlHero As Panel
    Friend WithEvents pnlCardShadow As Panel
    Friend WithEvents pnlCard As Panel
    Friend WithEvents lblFooter As Label
    Friend WithEvents btnGoToLogin As Button
    Friend WithEvents pnlStatTariff As Panel
    Friend WithEvents lblTariffValue As Label
    Friend WithEvents lblTariffTitle As Label
    Friend WithEvents lblTariffIcon As Label
    Friend WithEvents pnlStatOutstanding As Panel
    Friend WithEvents lblOutstandingValue As Label
    Friend WithEvents lblOutstandingTitle As Label
    Friend WithEvents lblOutstandingIcon As Label
    Friend WithEvents pnlStatCustomers As Panel
    Friend WithEvents lblCustomersValue As Label
    Friend WithEvents lblCustomersTitle As Label
    Friend WithEvents lblCustomersIcon As Label
    Friend WithEvents lblSubtitle As Label
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblLogo As Label

End Class
