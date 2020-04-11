Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Diagnostics
Imports System.Net
Imports System.Text
Imports RestSharp
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports System.Text.RegularExpressions
Imports System.IO

Public Class Form1
    <DllImport("user32.dll")>
    Private Shared Sub mouse_event(ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal dwData As Integer, ByVal dwExtraInfo As Integer)
    End Sub
    Private Const MOUSEEVENTF_MOVE As Integer = &H1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim MainLoopThread As New Thread(AddressOf MainBrain)
        MainLoopThread.IsBackground = True
        MainLoopThread.Start()
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Application.Exit()
        End
    End Sub


    Private Sub MainBrain()
        CheckForIllegalCrossThreadCalls = False

        Dim objectCenterIs
        Dim myCenterIs As Integer = 1920

        'startup
        lastAction.Text = "Warming up"
        ResponsiveSleep(5000)

        'main loop
        Do
            Application.DoEvents()

            'get rec
            objectCenterIs = GetObjectsVerticleLinePosition()

            'have rec            
            If objectCenterIs = 0 Then
                'no rec                

                'are we stuck?
                If AreWeStuck() Then
                    'we are stuck
                    Debug.Print("No rec, we are stuck")
                Else
                    'no
                    Debug.Print("No rec, we are not stuck, bumping")

                    'bumping
                    mouse_event(MOUSEEVENTF_MOVE, 25, 0, 0, 0)


                    Application.DoEvents()

                    'run litle
                    KeyDownUp(Keys.W, False, 100, False)

                    Application.DoEvents()
                End If
            Else
                'good rec
                objectCenterIs = objectCenterIs
                Dim ourDifference = objectCenterIs - myCenterIs

                Debug.Print("Good rec," & " objectCenterIs = " & objectCenterIs & " ourdiff = " & ourDifference)

                'have moved?
                If AreWeStuck() Then
                    'no 
                    Debug.Print("We are stuck, perform action")
                Else
                    Debug.Print("We not stuck, correcting position and running")

                    'corect positionp
                    mouse_event(MOUSEEVENTF_MOVE, ourDifference, 0, 0, 0)
                    ResponsiveSleep(500)

                    'run
                    KeyDownUp(Keys.W, False, 500, False)
                End If
            End If

            Application.DoEvents()
            ResponsiveSleep(10)
        Loop

        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()

    End Sub



    Public Sub GoHome()
        Do
            'get our current position
            Dim currentPosition = GetCurrentPosition()
            Dim distanceFromHome = currentPosition(3)

            logLabel.Text = logLabel.Text & "Distance from home: " & distanceFromHome & vbCrLf

            KeyDownOnly(Keys.W, False, 5000, True)

            Jump()

            'wait on run
            Thread.Sleep(1000)

            Jump()

            'wait on run
            Thread.Sleep(1000)

            Jump()

            'wait on run
            Thread.Sleep(1000)

            Jump()

            'wait on run
            Thread.Sleep(1000)

            Jump()

            'wait on run
            Thread.Sleep(1000)

            Dim currentPositionMoved = GetCurrentPosition()
            Dim distanceFromHomeMoved = currentPositionMoved(3)

            logLabel.Text = logLabel.Text & "New distance from home: " & distanceFromHome & vbCrLf

            Dim howClose = distanceFromHomeMoved - distanceFromHome

            logLabel.Text = logLabel.Text & "Change in distance: " & howClose & vbCrLf

            If distanceFromHomeMoved > 15 Or distanceFromHomeMoved.ToString.Contains("-") Then
                'closer or farther?
                If howClose.ToString.Contains("-") Then
                    'closer
                    Debug.Print("")

                    logLabel.Text = logLabel.Text & "We are closer" & vbCrLf
                Else
                    'farther
                    Debug.Print("")
                    'move right a few deg
                    mouse_event(MOUSEEVENTF_MOVE, 1500, 0, 0, 0)

                    logLabel.Text = logLabel.Text & "We are farther" & vbCrLf
                End If
            Else
                'home

                'stop running                
                logLabel.Text = logLabel.Text & "We are home!" & vbCrLf
                Exit Do
            End If

            'move until water isn't in view
            If DetectWater() Then
                logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
                mouse_event(MOUSEEVENTF_MOVE, 1500, 0, 0, 0)
            End If
        Loop
    End Sub

    Public Sub SeekWood()

    End Sub

    Private Sub logLabel_TextChanged(sender As Object, e As EventArgs) Handles logLabel.TextChanged
        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()
    End Sub
End Class