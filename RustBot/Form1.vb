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
Imports SimWinInput
Imports IronOcr

Public Class Form1

    <DllImport("user32.dll")>
    Private Shared Sub mouse_event(ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal dwData As Integer, ByVal dwExtraInfo As Integer)
    End Sub

    Public moveMouseEventX As Boolean
    Public moveMouseHowFarX As Integer
    Public moveMouseEventY As Boolean
    Public moveMouseHowFarY As Integer
    Public leftClickEvent As Boolean
    Dim keyDownEvent As Boolean
    Dim keyLocal As Byte
    Dim shiftLocal As Boolean
    Dim durationInMilliLocal As Integer
    Dim jumpingLocal As Boolean
    Public HowFarToRun As Integer = 5000
    Public runEvent As Boolean
    Public Shared previewImageEvent As Boolean
    Public keyDownUpEvent As Boolean
    Public HaveGatheredWood As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Show()
        Me.Refresh()
        Me.BringToFront()
        ResponsiveSleep(1000)

        ShiftUP()

        Dim MainLoopThread As New Thread(AddressOf MainBrain)
        MainLoopThread.IsBackground = True
        MainLoopThread.Start()

        Do
            'bump mouse x
            If moveMouseEventX Then
                moveMouseEventX = False
                mouse_event(Constants.MOUSEEVENTF_MOVE, moveMouseHowFarX, 0, 0, 0)
            End If

            'bump mouse y
            If moveMouseEventY Then
                moveMouseEventY = False
                mouse_event(Constants.MOUSEEVENTF_MOVE, 0, moveMouseHowFarY, 0, 0)
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

            'key down up
            If keyDownUpEvent Then
                keyDownUpEvent = False
                KeyDownUp(keyLocal, shiftLocal, durationInMilliLocal, jumpingLocal)
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

            ResponsiveSleep(10)
            Application.DoEvents()
        Loop
    End Sub

    'Public Sub OcrTakeScreenshot()
    '    'take screen
    '    'TakeScreenShotWhole("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\ocr.png")

    '    Dim Ocr = New AutoOcr()
    '    Dim Result = Ocr.Read("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\ocr.png")
    '    Debug.Print(Result.Text)
    'End Sub

    Public Sub PreviewImg(pic As PictureBox, path As String)
        On Error Resume Next

        Dim strImageFileName As String
        Dim fs As System.IO.FileStream
        fs = New System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read)

        Using ms As MemoryStream = New MemoryStream(File.ReadAllBytes(path))
            pic.Image = Image.FromStream(ms)
        End Using

        fs.Close()
    End Sub

    Public Sub MoveMouseMainThreadX(howFarX As Integer)
        moveMouseHowFarX = howFarX
        moveMouseEventX = True
    End Sub

    Public Sub MoveMouseMainThreadY(howFarY As Integer)
        moveMouseHowFarY = howFarY
        moveMouseEventY = True
    End Sub

    Public Sub KeyDownMainThread(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        keyLocal = key
        shiftLocal = shift
        durationInMilliLocal = durationInMilli
        jumpingLocal = jumping

        keyDownEvent = True
    End Sub

    Public Sub KeyDownUpMainThread(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        keyLocal = key
        shiftLocal = shift
        durationInMilliLocal = durationInMilli
        jumpingLocal = jumping

        keyDownUpEvent = True
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
        Dim objects As String

        'what to look for
        Dim gathering As New Collection
        gathering.Add("gatheringwood", "gatheringwood")

        'bring up rock
        KeyDownUp(Keys.M, False, 1, False)

        'wait for it to come up
        ResponsiveSleep(500)

        '~60 seconds to collect a tree

        Dim firstHit As Boolean = True

        'main loop
        Do
            'test hitting the object to see if we are gathering
            If firstHit Then
                'must be long enough for gathering images to come up
                LeftMouseClick(5000)
                firstHit = False
            Else
                'longer
                LeftMouseClick(15000)
            End If


            'are we collecting?
            objects = DetectObjects(False)

            'are we gathering?
            If DetectSpecificObjects(gathering, objects) Then
                'yes 

                HaveGatheredWood = True

                'only bump if we were gathering
                MoveMouseMainThreadX(-120)
                'center horizon
                MovePlayerEyesToHorizon()

                'yes
                Debug.Print("gathering, continuing to gather")
            Else
                'no
                Debug.Print("not gathering, finishing")

                'center horizon
                MovePlayerEyesToHorizon()

                'go elsewhere in case hitting rock or whatever
                MoveMouseMainThreadX(GetRandom(-3500, 3500))

                Exit Do
            End If

            Application.DoEvents()
        Loop

        'put away rock
        KeyDownUp(Keys.N, False, 1, False)
        ResponsiveSleep(500)
    End Sub

    Private Function DetectSpecificObjectsScreenshot(searchObjects As Collection) As Boolean
        KeyDownUp(Keys.Tab, False, 10, False)
        ResponsiveSleep(500)

        Dim objects As String = DetectObjects(False)

        Dim theSplit = Split(objects, vbCrLf)
        Dim Label As String
        Dim LastObject As String

        Debug.Print("DetectSpecificObjectsScreenshot found " & theSplit.Count - 2 & " objects")
        Application.DoEvents()

        For i = 1 To theSplit.Count - 2
            Dim theSplitNext = Split(theSplit(i), ",")

            'no rec
            If theSplitNext(0) = "" Then Exit For

            Dim theProbNew As String = theSplitNext(7).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmin = theSplitNext(2).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymin = theSplitNext(3).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmax = theSplitNext(4).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymax = theSplitNext(5).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "").Replace("Time", "")
            Label = LabelToObjectName(theSplitNext(6))

            Debug.Print("Processing object = " & theSplitNext(6) & " " & Label)

            'right type?
            If searchObjects.Contains(Label) Then
                KeyDownUp(Keys.Tab, False, 10, False)
                ResponsiveSleep(500)
                Return True
            End If
        Next

        KeyDownUp(Keys.Tab, False, 10, False)
        ResponsiveSleep(500)
        Return False

        Debug.Print("")
    End Function

    Private Function DetectSpecificObjects(searchObjects As Collection, imageObjects As String) As Boolean
        Debug.Print("begin detect specific objects")

        Dim theSplit = Split(imageObjects, vbCrLf)
        Dim Label As String
        Dim LastObject As String

        Debug.Print("Found " & theSplit.Count - 2 & " objects")
        Application.DoEvents()

        For i = 1 To theSplit.Count - 2
            Dim theSplitNext = Split(theSplit(i), ",")

            'no rec
            If theSplitNext(0) = "" Then Exit For

            Dim theProbNew As String = theSplitNext(7).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmin = theSplitNext(2).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymin = theSplitNext(3).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmax = theSplitNext(4).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymax = theSplitNext(5).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "").Replace("Time", "")
            Label = LabelToObjectName(theSplitNext(6))

            Debug.Print("Processing object = " & theSplitNext(6) & " " & Label)

            'right type?
            If searchObjects.Contains(Label) Then
                'KeyDownUp(Keys.Tab, False, 10, False)
                Return True
            End If
        Next

        Return False
    End Function

    Public Sub MovePlayerEyesToHorizon()
        'xmax = 271 (sky)
        'xmin = 89 (ground)
        'xlevel = 360

        'move player eyes to fully down, ground
        MoveMouseMainThreadY(5000)
        'wait for mouse move
        ResponsiveSleep(500)

        'move players eyes back up
        MoveMouseMainThreadY(-2200)
        'wait for mouse move
        ResponsiveSleep(500)
    End Sub

    Private Function AreWeStuck() As Boolean
        'bring up map
        ShowMap()

        'take screen right before run                                       
        TakeScreenShotAreaStuck("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\1.png", Constants.compareWidthNarrow, Constants.compareHeightNarrow, Constants.compareSourceXNarrow, Constants.compareSourceyNarrow, 0, 0)

        'hide map
        HideMap()

        'run
        Run(Keys.W, False, 1500, False)
        'wait for run to complete
        ResponsiveSleep(1800)

        'show map
        ShowMap()

        'take screenshot after run                    
        TakeScreenShotAreaStuck("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\2.png", Constants.compareWidthNarrow, Constants.compareHeightNarrow, Constants.compareSourceXNarrow, Constants.compareSourceyNarrow, 0, 0)

        'hide map
        HideMap()

        'compare images, did we move?
        Dim theDiff As Double = compareImages("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\1.png", "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\2.png")

        'If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\1.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\1.png")
        'If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\2.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\2.png")

        Return theDiff
    End Function

    Private Sub GoWood()
        Dim moveToCenter
        Dim objects As String
        Dim myCenterIs As Integer = 1920
        Dim dead As New Collection
        Dim narrowRec As Boolean = False

        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")

        'center to horizon
        MovePlayerEyesToHorizon()

        'main loop
        Do
            objects = DetectObjects(narrowRec)
            'get rec
            moveToCenter = GetObjectsVerticleLinePosition(objects, "tree")

            'hit inventory tab
            'ShowInventory()

            'are we dead?
            If DetectSpecificObjects(dead, objects) = False Then
                'turn off inventory tab
                'HideInventory()

                ''move until water isn't in view
                'If DetectWater() Then
                '    logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
                '    MoveMouseMainThread(1500)
                '    ResponsiveSleep(500)
                'Else

                'have rec?
                If moveToCenter <> 0 Then
                    narrowRec = True

                    'good rec                                    
                    Debug.Print("good rec, moving to object centerline @" & moveToCenter)

                    'point to objectos
                    MoveMouseMainThreadX(moveToCenter)
                    'wait for mouse move
                    ResponsiveSleep(500)

                    'are we stuck? 
                    If AreWeStuck() = 0 Then
                        'we are stuck
                        Debug.Print("good rec, stuck, performing action")
                        HitTree()

                        'go drop
                        If HaveGatheredWood = True Then
                            Debug.Print("we've got wood, dropping off")
                            HaveGatheredWood = False
                            GoHome()
                            Debug.Print("dropped off, going out to get more wood")
                        End If
                    Else
                        'no
                        Debug.Print("good rec, we are not stuck")

                        'bumping
                        'MoveMouseMainThread(50)                            

                        'run litle
                        'RunMainThread(Keys.W, False, 100, False)
                    End If
                Else
                    narrowRec = False

                    'no rec                
                    Debug.Print("bad rec")

                    If AreWeStuck() = 0 Then
                        'we are stuck
                        Debug.Print("bad rec, we are stuck, performing action")
                        HitTree()

                        'go drop
                        If HaveGatheredWood = True Then
                            Debug.Print("we've got wood, dropping off")
                            HaveGatheredWood = False
                            GoHome()
                        End If
                    Else
                        'no
                        Debug.Print("bad rec, we are not stuck, searching elsewhere!")

                        'bumping                                                
                        MoveMouseMainThreadX(GetRandom(-3500, 3500))

                        'run to a better area
                        RunMainThread(Keys.W, True, 2000, False)
                    End If
                End If
                'End If
            Else
                'turn off inventory tab
                'HideInventory()

                'we are dead, respawn
                KillandRespawn(False)
            End If
        Loop
    End Sub

    Private Sub MainBrain()
        CheckForIllegalCrossThreadCalls = False
        Dim objects As String
        Dim objectCenterIs As String


        'kill old
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")

        'startup
        lastAction.Text = "warming up"
        ResponsiveSleep(10000)

        Dim objectsToFind As New Collection
        objectsToFind.Add("someinventory", "someinventory")
        objectsToFind.Add("woodinventory", "woodinventory")

        'Object detection only!
        'Do
        '    Debug.Print("==================================== " & DateTime.Now)
        '    Debug.Print("Starting object detection " & DateTime.Now)
        '    objects = DetectObjects()
        '    Debug.Print("Done object detection " & DateTime.Now)

        '    'get rec
        '    Debug.Print("Start centerline " & DateTime.Now)
        '    objectCenterIs = GetObjectsVerticleLinePosition(objects)
        '    Debug.Print("Done centerline = " & objectCenterIs & " " & DateTime.Now)
        '    Debug.Print("==================================== " & DateTime.Now)

        '    If objectCenterIs <> 0 Then
        '        'good rec                
        '        Dim ourDifference = objectCenterIs - myCenterIs

        '        Debug.Print("Moving, Our diff is" & ourDifference & " " & DateTime.Now)

        '        'point to object
        '        MoveMouseMainThreadX(ourDifference)
        '    End If
        'Loop

        'GoHarass()
        'GoWood()
        GoHome()

    End Sub

    Public Function KillandRespawn(kill As Boolean)
        'On Error Resume Next
        Debug.Print("KillandRespawn")

        Dim dead As New Collection
        dead.Add("someinventory", "someinventory")
        dead.Add("woodinventory", "woodinventory")
        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")
        dead.Add("map", "map")

        'need kill?
        If kill Then
            Debug.Print("Killing")

            'bring up console
            KeyDownUp(Keys.F1, False, 1, False)
            ResponsiveSleep(500)

            ShiftUP()

            ''kill
            KeyDownUp(Keys.K, False, 1, False)
            KeyDownUp(Keys.I, False, 1, False)
            KeyDownUp(Keys.L, False, 1, False)
            KeyDownUp(Keys.L, False, 1, False)
            KeyDownUp(Keys.Enter, False, 1, False)
            ResponsiveSleep(250)
            KeyDownUp(Keys.Escape, False, 1, False)
            ResponsiveSleep(500)
        End If

        Debug.Print("Waiting for respawn")

        'wait for respawn
        ResponsiveSleep(10000)

trydeadagain:
        'move to respawn
        ClickAllThreeBags()

        'click respawn
        LeftMouseClick()
        ResponsiveSleep(1000)

        'wait to wake up
        ResponsiveSleep(10000)

        Debug.Print("Coming back to life")

        'click wakeup
        LeftMouseClick()

        'get rec
        Dim objects As String = DetectObjects(False)

        'are we dead?
        If DetectSpecificObjects(dead, objects) Then
            'yes
            GoTo trydeadagain
        End If
    End Function

    Private Function fourCornerRadarRec() As String
        'for dead detect
        Dim dead As New Collection
        dead.Add("someinventory", "someinventory")
        dead.Add("woodinventory", "woodinventory")
        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")

        Dim moveToCenter As Integer = 0
        Dim objects As String

        'turn once, look
        For i = 1 To 4
            'general object detectint
            objects = DetectObjects(False)

            'get rec
            moveToCenter = GetObjectsVerticleLinePosition(objects, "door")

            'hit inventory tab
            'ShowInventory()

            'are we dead?
            If DetectSpecificObjects(dead, objects) = False Then
                'good rec?
                If moveToCenter <> 0 Then
                    'good rec   
                    Return moveToCenter
                Else
                    'bad rec, keep looking

                End If
            End If

            'move each dir
            MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
            ResponsiveSleep(500)
        Next

        'no rec
        Return 0
    End Function

    Private Sub WalkIntoDoor()
        'open door
        KeyDownUp(Keys.E, False, 10, False)
        ResponsiveSleep(1000)

        'walk into door
        KeyDownUp(Keys.W, True, 450, False)
        ResponsiveSleep(1000)

        'turn to close door
        MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        ResponsiveSleep(1000)

        'close it
        KeyDownUp(Keys.E, True, 250, False)
        ResponsiveSleep(1000)

        'turn back straight
        MoveMouseMainThreadX(-Constants.eachMoveInFullTurn)
        ResponsiveSleep(1000)
    End Sub

    Private Sub CheckBox()
        'run to box
        RunMainThread(Keys.W, True, 1000, False)
        ResponsiveSleep(1000)

        'open box
        KeyDownUp(Keys.E, False, 10, False)
    End Sub

    Private Sub EmptyMyInventory()
        ResponsiveSleep(1000)

        Win32.SetCursorPos(1400, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1590, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1792, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1991, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2185, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2185, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2368, 1236)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1407, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1611, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1611, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1800, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1980, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1980, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2190, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2374, 1420)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1409, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1610, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1809, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1986, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2185, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2375, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1409, 1610)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1400, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1602, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1800, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2000, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2200, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2400, 1800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1789, 2012)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1986, 2012)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2179, 2012)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(2367, 2012)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        CloseInventory()
    End Sub

    Private Sub CloseInventory()
        'close inventory
        KeyDownUp(Keys.Tab, False, 50, False)
        ResponsiveSleep(500)
    End Sub

    Private Sub TurnAroundAndLeave()
        'move each dir
        MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        ResponsiveSleep(500)

        'pointed at door
        MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        ResponsiveSleep(500)

        'open door
        KeyDownUp(Keys.E, False, 10, False)
        ResponsiveSleep(1000)

        'run out door!
        RunMainThread(Keys.W, False, 800, False)
        ResponsiveSleep(800)

        'move each dir
        MoveMouseMainThreadX(-3800)
        ResponsiveSleep(500)

        'close door
        KeyDownUp(Keys.E, False, 10, False)
        ResponsiveSleep(1000)

        'move each dir
        MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        ResponsiveSleep(500)

        'run away from base!
        RunMainThread(Keys.W, True, 5000, True)
        ResponsiveSleep(1000)
    End Sub

    Private Sub HowFarToRunTime(distanceFromHome As String)
        If distanceFromHome > 100 Then
            RunMainThread(Keys.W, True, 12000, True)
            ResponsiveSleep(12000)
            Exit Sub
        End If

        If distanceFromHome > 75 Then
            RunMainThread(Keys.W, True, 8000, True)
            ResponsiveSleep(8000)
            Exit Sub
        End If

        If distanceFromHome > 50 Then
            RunMainThread(Keys.W, True, 2500, True)
            ResponsiveSleep(2500)
            Exit Sub
        End If

        If distanceFromHome > 40 Then
            RunMainThread(Keys.W, False, 1500, False)
            ResponsiveSleep(1500)
            Exit Sub
        End If

        If distanceFromHome <= 40 Then
            RunMainThread(Keys.W, False, 1500, False)
            ResponsiveSleep(1500)
            Exit Sub
        End If
    End Sub

    Public Sub GoHome()
        'center to horizon
        MovePlayerEyesToHorizon()

        Do
            'get our current position
            Dim currentPosition = GetCurrentPosition()
            Dim distanceFromHome = currentPosition(3)

            logLabel.Text = logLabel.Text & "distance from home: " & distanceFromHome & " Running" & vbCrLf
            Debug.Print("distance from home: " & distanceFromHome & " running")

            HowFarToRunTime(distanceFromHome)

            Dim currentPositionMoved = GetCurrentPosition()
            Dim distanceFromHomeMoved = currentPositionMoved(3)

            logLabel.Text = logLabel.Text & "new distance from home: " & distanceFromHome & vbCrLf
            Debug.Print("new distance from home: " & distanceFromHome)

            Dim changeInDistance = distanceFromHomeMoved - distanceFromHome

            logLabel.Text = logLabel.Text & "change in distance: " & changeInDistance & vbCrLf
            Debug.Print("change in distance: " & changeInDistance)

            'move until water isn't in view
            'If DetectWater() Then
            '    logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
            '    MoveMouseMainThread(1500)
            'End If

            'bad rec?
            If changeInDistance = 0 Then
                logLabel.Text = logLabel.Text & "stuck, bumping" & vbCrLf

                'move right a few deg
                MoveMouseMainThreadX(GetRandom(-3500, 3500))
                ResponsiveSleep(500)

                RunMainThread(Keys.W, True, 1000, True)
            Else
                If distanceFromHomeMoved > 30 Or distanceFromHomeMoved.ToString.Contains("-") Then
                    'closer or farther?
                    If changeInDistance.ToString.Contains("-") Then
                        'closer
                        Debug.Print("")

                        logLabel.Text = logLabel.Text & "We are closer, running long" & vbCrLf
                    Else
                        'farther                        
                        logLabel.Text = logLabel.Text & "We are farther, changing direction" & vbCrLf

                        'move right a few deg                  
                        MoveMouseMainThreadX(1500)
                        ResponsiveSleep(500)
                    End If
                Else
                    'starting looking each direction for a door
                    MoveMouseMainThreadX(2500)
                    ResponsiveSleep(500)

                    'stop running                
                    logLabel.Text = logLabel.Text & "we are home!" & vbCrLf
                    KeyUpOnly(Keys.W, True, 100, False)

                    Debug.Print("entering close base mode")

                    Do
                        'is our base here?
                        Dim moveToCenter As Integer = fourCornerRadarRec()

                        'have an object to point to?
                        If moveToCenter = 0 Then
                            'nope
                            Debug.Print("didn't find a base")

                            'get out of close to home, no base found
                            Exit Do
                        Else
                            Debug.Print("found a base, turning and running to = " & moveToCenter)

                            'yup                
                            MoveMouseMainThreadX(moveToCenter)
                            ResponsiveSleep(500)

                            'run to it        
                            'Run(Keys.W, True, 2500, False)

                            'stuck?
                            If AreWeStuck() = 0 Then
                                'yes, perform action
                                Debug.Print("stuck, performing action")
                                WalkIntoDoor()
                                CheckBox()
                                EmptyMyInventory()
                                TurnAroundAndLeave()
                            Else
                                'not stuck, just keep going
                                Debug.Print("not stuck")
                            End If
                        End If
                    Loop

                    'Debug.Print("We are home, killing and starting over")

                    ''kill myself start over
                    'KillandRespawn(True)

                    ''not going home already there!                    
                End If
            End If
        Loop
    End Sub

    Public Class Win32
        Public Declare Function SetCursorPos Lib "User32.Dll" (ByVal x As Integer, ByVal y As Integer) As Long
        Public Declare Function ClientToScreen Lib "User32.Dll" (ByVal hWnd As IntPtr, ByRef point As POINT) As Boolean
        <StructLayout(LayoutKind.Sequential)>
        Public Structure POINT
            Public x As Integer
            Public y As Integer
        End Structure
    End Class

    Private Sub CenterMouseOnScreen()
        Dim W As Integer = Screen.PrimaryScreen.Bounds.Width / 2
        Dim H As Integer = Screen.PrimaryScreen.Bounds.Height / 2
        Dim p As Win32.POINT = New Win32.POINT
        Win32.ClientToScreen(Me.Handle, p)
        Win32.SetCursorPos(W, H)
    End Sub

    Private Sub ClickAllThreeBags()
        Dim p As Win32.POINT = New Win32.POINT
        Win32.ClientToScreen(Me.Handle, p)

        'bag locations
        '428, 2037
        '911, 2023
        '1642, 2019        

        'first bag
        Win32.SetCursorPos(428, 2037)
        LeftMouseClick()
        leftClickEvent = True
        ResponsiveSleep(1000)

        'second bag
        Win32.SetCursorPos(911, 2023)
        LeftMouseClick()
        leftClickEvent = True
        ResponsiveSleep(1000)

        'third bag
        Win32.SetCursorPos(1642, 2019)
        LeftMouseClick()
        leftClickEvent = True
        ResponsiveSleep(1000)

    End Sub

    Private Shared Function IsNullOrEmpty(ByVal myStringArray() As String) As Boolean
        Return ((myStringArray Is Nothing) OrElse (myStringArray.Length < 1))
    End Function

    Public Sub GoHarass()
        Dim objects As String
        Dim dead As New Collection
        dead.Add("someinventory", "someinventory")
        dead.Add("woodinventory", "woodinventory")
        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")


        Do
            objects = DetectObjects(False)

            'are we dead?
            If DetectSpecificObjects(dead, objects) = False Then
                'we are not dead

                'get our current position
                Dim currentPosition = GetCurrentPosition()
                Dim distanceFromHome = currentPosition(3)

                logLabel.Text = logLabel.Text & "Distance from home: " & distanceFromHome & " Running " & HowFarToRun & vbCrLf
                RunMainThread(Keys.W, True, HowFarToRun, True)
                ResponsiveSleep(HowFarToRun)

                Dim currentPositionMoved As Array = GetCurrentPosition()

                'make sure we have an array
                If IsNullOrEmpty(currentPositionMoved) = False Then
                    Dim distanceFromHomeMoved = currentPositionMoved(3)

                    logLabel.Text = logLabel.Text & "New distance from home: " & distanceFromHome & vbCrLf

                    Dim changeInDistance = distanceFromHomeMoved - distanceFromHome

                    logLabel.Text = logLabel.Text & "Change in distance: " & changeInDistance & vbCrLf

                    'move until water isn't in view
                    'If DetectWater() Then
                    '    logLabel.Text = logLabel.Text & "Detected water, moving" & vbCrLf
                    '    MoveMouseMainThread(1500)
                    'End If

                    If distanceFromHome < 70 Then
                        'at harass location
                        logLabel.Text = logLabel.Text & "We are at harass location, play sound!" & vbCrLf

                        'enable audio blaster!
                        KeyDownMainThread(Keys.V, False, 10, False)
                    End If

                    'bad distance?
                    If changeInDistance = 0 Then
                        logLabel.Text = logLabel.Text & "Stuck, bumping" & vbCrLf

                        'move right a few deg
                        MoveMouseMainThreadX(GetRandom(-1500, 1500))

                        RunMainThread(Keys.W, True, 1000, True)
                        ResponsiveSleep(1000)
                    Else
                        If distanceFromHomeMoved > 70 Or distanceFromHomeMoved.ToString.Contains("-") Then
                            'closer or farther?
                            If changeInDistance.ToString.Contains("-") Then
                                'closer
                                Debug.Print("")

                                logLabel.Text = logLabel.Text & "We are closer, running long" & vbCrLf

                                'headed almost straight?
                                If changeInDistance < -10 Then
                                    HowFarToRun = 5000
                                Else
                                    HowFarToRun = 5000
                                End If
                            Else
                                logLabel.Text = logLabel.Text & "We are farther, changing direction" & vbCrLf

                                'farther
                                Application.DoEvents()
                                ResponsiveSleep(250)
                                'move right a few deg                  
                                MoveMouseMainThreadX(1500)
                                Application.DoEvents()
                                ResponsiveSleep(250)

                                HowFarToRun = 1500
                            End If
                        Else
                            'closer or farther?
                            If changeInDistance.ToString.Contains("-") Then
                                'closer
                                Debug.Print("")

                                logLabel.Text = logLabel.Text & "We are closer, running long" & vbCrLf

                                'headed almost straight?
                                If changeInDistance < -10 Then
                                    HowFarToRun = 5000
                                Else
                                    HowFarToRun = 5000
                                End If
                            Else
                                logLabel.Text = logLabel.Text & "We are farther, changing direction" & vbCrLf

                                'farther
                                Application.DoEvents()
                                ResponsiveSleep(250)
                                'move right a few deg                  
                                MoveMouseMainThreadX(1500)
                                Application.DoEvents()
                                ResponsiveSleep(250)

                                HowFarToRun = 1500
                            End If
                            'at harass

                            'stop running                                    
                            KeyUpOnly(Keys.W, False, 100, False)

                            'at harass location
                            logLabel.Text = logLabel.Text & "We are at harass location, play sound!" & vbCrLf
                            KeyUpOnly(Keys.W, False, 100, False)

                            'enable audio blaster!
                            KeyDownMainThread(Keys.V, False, 10, False)
                        End If
                    End If
                End If
            Else
                logLabel.Text = logLabel.Text & "We are dead, respawning" & vbCrLf

                'deadt
                KillandRespawn(False)
            End If
        Loop


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

    End Sub
End Class