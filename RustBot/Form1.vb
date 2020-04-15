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

    Public moveMouseEvent As Boolean
    Public moveMouseHowFar As Integer

    Public leftClickEvent As Boolean

    Dim keyDownEvent As Boolean
    Dim keyLocal As Byte
    Dim shiftLocal As Boolean
    Dim durationInMilliLocal As Integer
    Dim jumpingLocal As Boolean

    Public runEvent As Boolean

    Public Shared previewImageEvent As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Show()
        Me.Refresh()
        Me.BringToFront()

        Application.DoEvents()

        ShiftUP()

        Dim MainLoopThread As New Thread(AddressOf MainBrain)
        MainLoopThread.IsBackground = True
        MainLoopThread.Start()

        Do
            'bump mouse
            If moveMouseEvent Then
                moveMouseEvent = False
                mouse_event(MOUSEEVENTF_MOVE, moveMouseHowFar, 0, 0, 0)
            End If

            'left mouse click
            If leftClickEvent Then
                leftClickEvent = False
                LeftMouseClick()
            End If

            'key down event
            If keyDownEvent Then
                keyDownEvent = False
                KeyDownOnly(keyLocal, shiftLocal, durationInMilliLocal, jumpingLocal)
            End If

            'run
            If runEvent Then
                runEvent = False
                Run(keyLocal, shiftLocal, durationInMilliLocal, jumpingLocal)
            End If

            'load img
            If previewImageEvent Then
                previewImageEvent = False

                PreviewImg(pic, "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")
            End If

            ResponsiveSleep(50)
            Application.DoEvents()
        Loop
    End Sub

    Public Sub PreviewImg(pic As PictureBox, path As String)
        On Error Resume Next

        'ResizeImageOnDisk(path)

        File.Copy("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png", "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust_preview.png", True)


        Dim strImageFileName As String
        Dim fs As System.IO.FileStream
        fs = New System.IO.FileStream("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust_preview.png", System.IO.FileMode.Open, System.IO.FileAccess.Read)

        Using ms As MemoryStream = New MemoryStream(File.ReadAllBytes(path))
            pic.Image = Image.FromStream(ms)
        End Using


        fs.Close()

        Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust_preview.png")
    End Sub

    Public Sub MoveMouseMainThread(howFar As Integer)
        moveMouseHowFar = howFar
        moveMouseEvent = True
    End Sub

    Public Sub KeyDownMainThread(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        keyLocal = key
        shiftLocal = shift
        durationInMilliLocal = durationInMilli
        jumpingLocal = jumping

        keyDownEvent = True
    End Sub

    Public Sub RunMainThread(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        keyLocal = key
        shiftLocal = shift
        durationInMilliLocal = durationInMilli
        jumpingLocal = jumping

        runEvent = True
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ShiftUP()
        Application.Exit()
        End
    End Sub

    Private Sub HitTree()
        KeyDownUp(Keys.M, False, 1, False)
        ResponsiveSleep(2000)
        LeftMouseClick(60000)
        KeyDownUp(Keys.N, False, 1, False)
        ResponsiveSleep(500)
    End Sub


    Private Sub MainBrain()
        CheckForIllegalCrossThreadCalls = False

        Dim objectCenterIs
        Dim objects As String
        Dim myCenterIs As Integer = 1920

        'startup
        lastAction.Text = "Warming up"
        ResponsiveSleep(5000)

        'Object detection only!
        '
        'Do
        '    Debug.Print("==================================== " & DateTime.Now)
        '    Debug.Print("Starting object detection " & DateTime.Now)
        '    objects = DetectObjects()
        '    Debug.Print("Done object detection " & DateTime.Now)

        '    Debug.Print("Start centerline " & DateTime.Now)
        '    'get rec
        '    objectCenterIs = GetObjectsVerticleLinePosition(objects)
        '    Debug.Print("Done centerline " & DateTime.Now)
        '    Debug.Print("=================================e=== " & DateTime.Now)
        'Loop

        'Exit Sub

        'main loop
        Do
            objects = DetectObjects()
            'get rec
            objectCenterIs = GetObjectsVerticleLinePosition(objects)

            ''move until water isn't in view
            'If DetectWater() Then
            '    logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
            '    MoveMouseMainThread(1500)
            '    ResponsiveSleep(500)
            'Else
            'have rec?
            If objectCenterIs <> 0 Then
                    'good rec                
                    Dim ourDifference = objectCenterIs - myCenterIs
                    Debug.Print("Good rec," & " objntectCenterIs = " & objectCenterIs & " ourdiff = " & ourDifference)

                    'point to object
                    MoveMouseMainThread(ourDifference)
                    ResponsiveSleep(500)

                    'are we stuck?
                    If AreWeStuck() Then
                        'we are stuck
                        Debug.Print("GOOD rec, stuck,  PERFORMING ACTION")
                        HitTree()
                    Else
                        'no
                        Debug.Print("good rec, we are not stuck, bumping")

                        'bumping
                        MoveMouseMainThread(50)
                        ResponsiveSleep(500)

                        'run litle
                        RunMainThread(Keys.W, False, 100, False)
                    End If
                Else
                    'no rec                .
                    Debug.Print("bad rec")

                    'are we stuck?
                    If AreWeStuck() Then
                        'we are stuck
                        Debug.Print("BAD rec, we are STUCK, PERFORMING ACTION")
                        HitTree()
                    Else
                        'no
                        Debug.Print("No rec, we are not stuck, searching elsewhere!")

                        'bumping
                        MoveMouseMainThread(1000)
                        ResponsiveSleep(500)

                        'run to a better area
                        RunMainThread(Keys.W, True, 2500, False)
                    End If
                End If
            'End If

            Application.DoEvents()
        Loop

        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()

    End Sub

    Public HowFarToRun As Integer = 5000
    Public Sub GoHome()
        Do
            'get our current position
            Dim currentPosition = GetCurrentPosition()
            Dim distanceFromHome = currentPosition(3)

            logLabel.Text = logLabel.Text & "Distance from home: " & distanceFromHome & " Running " & HowFarToRun & vbCrLf
            Run(Keys.W, True, HowFarToRun, False)

            Dim currentPositionMoved = GetCurrentPosition()
            Dim distanceFromHomeMoved = currentPositionMoved(3)

            logLabel.Text = logLabel.Text & "New distance from home: " & distanceFromHome & vbCrLf

            Dim changeInDistance = distanceFromHomeMoved - distanceFromHome

            logLabel.Text = logLabel.Text & "Change in distance: " & changeInDistance & vbCrLf

            'move until water isn't in view
            'If DetectWater() Then
            '    logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
            '    MoveMouseMainThread(1500)
            'End If

            'bad rec?
            If changeInDistance = 0 Then
                logLabel.Text = logLabel.Text & "Stuck, bumping" & vbCrLf

                'bump dir

                'move right a few deg
                MoveMouseMainThread(GetRandom(-1500, 1500))

                Run(Keys.W, True, 1000, False)
            Else
                If distanceFromHomeMoved > 40 Or distanceFromHomeMoved.ToString.Contains("-") Then
                    'closer or farther?
                    If changeInDistance.ToString.Contains("-") Then
                        'closer
                        Debug.Print("")

                        logLabel.Text = logLabel.Text & "We are closer, running long" & vbCrLf

                        HowFarToRun = 5000
                    Else
                        logLabel.Text = logLabel.Text & "We are farther, changing direction" & vbCrLf

                        'farther
                        Application.DoEvents()
                        ResponsiveSleep(100)
                        'move right a few deg1                        
                        MoveMouseMainThread(1500)

                        Application.DoEvents()
                        ResponsiveSleep(100)


                        HowFarToRun = 1000
                    End If
                Else
                    'home

                    'stop running                
                    logLabel.Text = logLabel.Text & "We are home!" & vbCrLf
                    KeyUpOnly(Keys.W, True, 100, False)

                    GoTo STopNOw
                End If
            End If
        Loop

STopNOw:
    End Sub

    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Dim Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function

    Public Sub SeekWood()

    End Sub

    Private Sub logLabel_TextChanged(sender As Object, e As EventArgs) Handles logLabel.TextChanged
        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()
    End Sub

    Public lastHash As String
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Dim hashIs As String = hash_generator("md5", "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")

        '        If lastHash <> hashIs Then
        'lastHash = hashIs


        'pic.Load("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")


        'End If

        'pic.Refresh()
    End Sub
End Class