Imports System.Drawing.Drawing2D

Public Class Form1
    Private ReadOnly _buttonBaseColor As Color = ColorTranslator.FromHtml("#27ae60")
    Private ReadOnly _buttonHoverColor As Color = ColorTranslator.FromHtml("#229954")
    Private ReadOnly _backgroundColor As Color = ColorTranslator.FromHtml("#2c3e50")
    Private ReadOnly _fadeTimer As New Timer() With {.Interval = 25}
    Private pnlWelcomeCard As Panel
    Private pnlWelcomeCardShadow As Panel
    Private pnlDescription As Panel
    Private flpFeatures As FlowLayoutPanel

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "Water Utility Management System – Welcome"
        WindowState = FormWindowState.Maximized
        FormBorderStyle = FormBorderStyle.Sizable
        BackColor = _backgroundColor
        Me.Controls.Clear()

        InitializeWelcomeScreen()
        PositionWelcomeCard()
        UpdateRoundedElements()

        AddHandler _fadeTimer.Tick, AddressOf FadeTimer_Tick
        Opacity = 0
        _fadeTimer.Start()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        PositionWelcomeCard()
        UpdateRoundedElements()
        Invalidate()
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using brush As New LinearGradientBrush(ClientRectangle,
                                               ColorTranslator.FromHtml("#3498db"),
                                               _backgroundColor,
                                               LinearGradientMode.ForwardDiagonal)
            e.Graphics.FillRectangle(brush, ClientRectangle)
        End Using

        Using overlayBrush As New SolidBrush(Color.FromArgb(55, Color.Black))
            e.Graphics.FillRectangle(overlayBrush, ClientRectangle)
        End Using
    End Sub

    Private Sub InitializeWelcomeScreen()
        pnlWelcomeCardShadow = New Panel() With {
            .Name = "pnlWelcomeCardShadow",
            .Size = New Size(800, 500),
            .BackColor = Color.FromArgb(80, 0, 0, 0)
        }

        pnlWelcomeCard = New Panel() With {
            .Name = "pnlWelcomeCard",
            .Size = New Size(800, 500),
            .BackColor = Color.White
        }
        AddHandler pnlWelcomeCard.Paint, AddressOf pnlWelcomeCard_Paint

        Dim lblLogo As New Label() With {
            .AutoSize = False,
            .Text = "💧",
            .Font = New Font("Segoe UI Emoji", 34.0F, FontStyle.Regular),
            .ForeColor = ColorTranslator.FromHtml("#3498db"),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(368, 18),
            .Size = New Size(64, 64)
        }

        Dim lblSystemName As New Label() With {
            .Name = "lblSystemName",
            .AutoSize = False,
            .Text = "Water Utility Management System",
            .Font = New Font("Segoe UI", 28.0F, FontStyle.Bold),
            .ForeColor = _backgroundColor,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(40, 88),
            .Size = New Size(720, 58)
        }

        Dim lblTagline As New Label() With {
            .Name = "lblTagline",
            .AutoSize = False,
            .Text = "Efficient, Transparent, and Modern Water Billing",
            .Font = New Font("Segoe UI", 14.0F, FontStyle.Italic),
            .ForeColor = ColorTranslator.FromHtml("#7f8c8d"),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(40, 148),
            .Size = New Size(720, 36)
        }

        pnlDescription = New Panel() With {
            .Name = "pnlDescription",
            .BackColor = ColorTranslator.FromHtml("#f8f9fa"),
            .Location = New Point(40, 194),
            .Size = New Size(720, 126)
        }

        Dim lblDescription As New Label() With {
            .Name = "lblDescription",
            .Dock = DockStyle.Fill,
            .AutoSize = False,
            .Padding = New Padding(20),
            .Text = "The Water Utility Management System automates the entire water billing cycle—from customer registration and meter reading to bill generation and payment tracking. It provides role-based access for customers, staff, and managers, ensuring secure and efficient operations. With real-time reporting and online payment options, it empowers both the utility provider and its customers.",
            .Font = New Font("Segoe UI", 11.0F, FontStyle.Regular),
            .ForeColor = _backgroundColor,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        pnlDescription.Controls.Add(lblDescription)

        flpFeatures = New FlowLayoutPanel() With {
            .Name = "flpFeatures",
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = False,
            .Location = New Point(40, 332),
            .Size = New Size(720, 54),
            .BackColor = Color.Transparent,
            .Padding = New Padding(0)
        }

        flpFeatures.Controls.Add(CreateFeatureItem("👥  Customer Management"))
        flpFeatures.Controls.Add(CreateFeatureItem("💧  Meter Reading & Billing"))
        flpFeatures.Controls.Add(CreateFeatureItem("📊  Real-time Reports"))

        btnGoToLogin = New Button() With {
            .Name = "btnGoToLogin",
            .Text = "🔐 Go to Login",
            .Size = New Size(250, 60),
            .Location = New Point(275, 396),
            .BackColor = _buttonBaseColor,
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 14.0F, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False
        }
        btnGoToLogin.FlatAppearance.BorderSize = 0
        AddHandler btnGoToLogin.MouseEnter, AddressOf btnGoToLogin_MouseEnter
        AddHandler btnGoToLogin.MouseLeave, AddressOf btnGoToLogin_MouseLeave

        lblFooter = New Label() With {
            .Name = "lblFooter",
            .AutoSize = False,
            .Text = "© 2025 Water Utility Management System. All rights reserved.",
            .Font = New Font("Segoe UI", 8.0F, FontStyle.Regular),
            .ForeColor = ColorTranslator.FromHtml("#95a5a6"),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(40, 466),
            .Size = New Size(720, 22)
        }

        pnlWelcomeCard.Controls.Add(lblLogo)
        pnlWelcomeCard.Controls.Add(lblSystemName)
        pnlWelcomeCard.Controls.Add(lblTagline)
        pnlWelcomeCard.Controls.Add(pnlDescription)
        pnlWelcomeCard.Controls.Add(flpFeatures)
        pnlWelcomeCard.Controls.Add(btnGoToLogin)
        pnlWelcomeCard.Controls.Add(lblFooter)

        Controls.Add(pnlWelcomeCardShadow)
        Controls.Add(pnlWelcomeCard)
        pnlWelcomeCard.BringToFront()
    End Sub

    Private Function CreateFeatureItem(text As String) As Panel
        Dim featurePanel As New Panel() With {
            .BackColor = ColorTranslator.FromHtml("#ecf0f1"),
            .Size = New Size(226, 46),
            .Margin = New Padding(0, 0, 20, 0)
        }

        Dim lblFeature As New Label() With {
            .Dock = DockStyle.Fill,
            .AutoSize = False,
            .Text = text,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = _backgroundColor,
            .TextAlign = ContentAlignment.MiddleCenter
        }

        featurePanel.Controls.Add(lblFeature)
        Return featurePanel
    End Function

    Private Sub PositionWelcomeCard()
        If pnlWelcomeCard Is Nothing OrElse pnlWelcomeCardShadow Is Nothing Then
            Return
        End If

        pnlWelcomeCard.Left = (ClientSize.Width - pnlWelcomeCard.Width) \ 2
        pnlWelcomeCard.Top = (ClientSize.Height - pnlWelcomeCard.Height) \ 2
        pnlWelcomeCardShadow.Left = pnlWelcomeCard.Left + 10
        pnlWelcomeCardShadow.Top = pnlWelcomeCard.Top + 12
    End Sub

    Private Sub UpdateRoundedElements()
        SetRoundedRegion(pnlWelcomeCard, 20)
        SetRoundedRegion(pnlWelcomeCardShadow, 20)
        SetRoundedRegion(pnlDescription, 12)
        SetRoundedRegion(btnGoToLogin, 10)

        If flpFeatures Is Nothing Then
            Return
        End If

        For Each control As Control In flpFeatures.Controls
            Dim featurePanel = TryCast(control, Panel)
            If featurePanel IsNot Nothing Then
                SetRoundedRegion(featurePanel, 10)
            End If
        Next
    End Sub

    Private Sub SetRoundedRegion(control As Control, radius As Integer)
        If control Is Nothing OrElse control.Width <= 0 OrElse control.Height <= 0 Then
            Return
        End If

        Using path As GraphicsPath = CreateRoundedPath(New Rectangle(0, 0, control.Width, control.Height), radius)
            Dim oldRegion As Region = control.Region
            control.Region = New Region(path)
            If oldRegion IsNot Nothing Then
                oldRegion.Dispose()
            End If
        End Using
    End Sub

    Private Sub pnlWelcomeCard_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using path As GraphicsPath = CreateRoundedPath(New Rectangle(0, 0, pnlWelcomeCard.Width - 1, pnlWelcomeCard.Height - 1), 20)
            Using pen As New Pen(ColorTranslator.FromHtml("#dfe6e9"), 1)
                e.Graphics.DrawPath(pen, path)
            End Using
        End Using
    End Sub

    Private Sub FadeTimer_Tick(sender As Object, e As EventArgs)
        If Opacity >= 1 Then
            _fadeTimer.Stop()
            Return
        End If

        Opacity = Math.Min(1, Opacity + 0.08)
    End Sub

    Private Sub btnGoToLogin_MouseEnter(sender As Object, e As EventArgs)
        btnGoToLogin.BackColor = _buttonHoverColor
    End Sub

    Private Sub btnGoToLogin_MouseLeave(sender As Object, e As EventArgs)
        btnGoToLogin.BackColor = _buttonBaseColor
    End Sub

    Private Shared Function CreateRoundedPath(bounds As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim diameter As Integer = radius * 2
        Dim arc As New Rectangle(bounds.X, bounds.Y, diameter, diameter)

        path.AddArc(arc, 180, 90)
        arc.X = bounds.Right - diameter
        path.AddArc(arc, 270, 90)
        arc.Y = bounds.Bottom - diameter
        path.AddArc(arc, 0, 90)
        arc.X = bounds.X
        path.AddArc(arc, 90, 90)
        path.CloseFigure()

        Return path
    End Function

    Private Sub btnGoToLogin_Click(sender As Object, e As EventArgs) Handles btnGoToLogin.Click
        Me.Hide()
        Using loginForm As New frmLogin()
            Dim result As DialogResult = loginForm.ShowDialog(Me)

            If result <> DialogResult.OK Then
                Me.Show()
                Me.Activate()
            End If
        End Using
    End Sub
End Class
