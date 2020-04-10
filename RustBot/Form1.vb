Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Diagnostics
Imports IronOcr
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

        lastAction.Text = "Warming up"

        ResponsiveSleep(5000)

        Do
            ResponsiveSleep(10)

            Dim objectCenterIs
            Try
                'good rec
                objectCenterIs = GetObjectsVerticleLinePosition()

                objectCenterIs = objectCenterIs + xmin

                Debug.Print("objectCenterIs = " & objectCenterIs)

                Dim iAmAt As Integer = 3840
                Dim myCenterIs As Integer = 1920

                Dim ourDifference = objectCenterIs - myCenterIs

                Debug.Print("ourdiff = " & ourDifference)

                mouse_event(MOUSEEVENTF_MOVE, ourDifference, 0, 0, 0)

                If HaveIMovedFromLastCall() = False Then
                    'try to hit tree
                    LeftMouseClick()
                    ResponsiveSleep(1000)
                    LeftMouseClick()
                    ResponsiveSleep(1000)
                    LeftMouseClick()
                    ResponsiveSleep(1000)
                    LeftMouseClick()
                    ResponsiveSleep(1000)

                    'try to open door
                    'KeyDownUp(Keys.E, False, 10, False)
                    'KeyDownUp(Keys.W, True, 250, False)
                    'KeyDownUp(Keys.E, False, 10, False)
                    'KeyDownUp(Keys.W, True, 250, False)
                    'KeyDownUp(Keys.E, False, 10, False)
                Else
                    KeyDownUp(Keys.W, True, 500, False)
                End If



            Catch
                'fail                
                'bump mouse
                mouse_event(MOUSEEVENTF_MOVE, 50, 0, 0, 0)
                KeyDownUp(Keys.W, True, 25, False)
            End Try


        Loop

        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()

    End Sub

    Public lastPosition = 0

    Public Function HaveIMovedFromLastCall() As Boolean
        Dim currentPosition = GetCurrentPosition()

        If currentPosition <> lastPosition Then
            Return True
        Else
            Return False
        End If
    End Function

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