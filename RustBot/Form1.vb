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
                'wait for available file lockggw
                'waitagain:
                'If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then
                '    If IsFileUnavailable("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then
                '        ResponsiveSleep(100)
                '        GoTo waitagain
                '    End If
                'Else
                '    GoTo waitagain
                'End If

                'PreviewImg(pic, "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")
            End If


            Application.DoEvents()
        Loop
    End Sub

    'Public Sub OcrTakeScreenshot()
    '    'take screen
    '    'TakeScreenShotWhole("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\ocr.png")

    '    Dim Ocr = New AutoOcr()
    '    Dim Result = Ocr.Read("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\ocr\ocr.png")
    '    WriteMessageToGlobalChat(Result.Text)
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

        WriteMessageToGlobalChat("killing python")
        Shell("taskkill /f /im python.exe")

        Application.Exit()
        End
    End Sub

    Private Sub HitTree()
        Dim objects As Collection

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
                WriteMessageToGlobalChat("gathering, continuing to gather")
            Else
                'no
                WriteMessageToGlobalChat("not gathering, finishing")

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

    'Private Function DetectSpecificObjectsScreenshot(searchObjects As Collection) As Boolean
    '    KeyDownUp(Keys.Tab, False, 10, False)
    '    ResponsiveSleep(500)

    '    Dim objects As Collection = DetectObjects(False)
    '    Dim hisLabel As String
    '    Dim LastObject As String

    '    WriteMessageToGlobalChat("DetectSpecificObjectsScreenshot found " & searchObjects.Count - 1 & " objects")
    '    Application.DoEvents()

    '    For i = 1 To searchObjects.Count - 1
    '        Dim theSplitNext = Split(objects(i), " ")

    '        'no rec
    '        If theSplitNext(0) = "" Then Exit For

    '        Dim theProbNew As String = theSplitNext(1)
    '        xmin = theSplitNext(2).Replace("(", "")
    '        ymin = theSplitNext(3).Replace(")", "")
    '        xmax = theSplitNext(4).Replace("(", "")
    '        ymax = theSplitNext(5).Replace(")", "")
    '        hisLabel = theSplitNext(0)

    '        WriteMessageToGlobalChat("Processing object = " & theSplitNext(0) & " " & theProbNew)

    '        'right type?
    '        If searchObjects.Contains(hisLabel) Then
    '            KeyDownUp(Keys.Tab, False, 10, False)
    '            ResponsiveSleep(500)
    '            Return True
    '        End If
    '    Next

    '    KeyDownUp(Keys.Tab, False, 10, False)
    '    ResponsiveSleep(500)
    '    Return False

    '    WriteMessageToGlobalChat("")
    'End Function



    Public Sub MovePlayerEyesToHorizon()
        'xmax = 271 (sky)
        'xmin = 89 (ground)
        'xlevel = 360

        'move player eyes to fully down, ground
        MoveMouseMainThreadY(2500)
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
        'ResponsiveSleep(1800)

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
        Dim objects As Collection
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
                    WriteMessageToGlobalChat("good rec, moving to object centerline = " & moveToCenter)

                    'point to objectos
                    MoveMouseMainThreadX(moveToCenter)
                    'wait for mouse move
                    ResponsiveSleep(10)

                    ''are we stuck? 
                    If AreWeStuck() = 0 Then
                        'we are stuck
                        WriteMessageToGlobalChat("good rec, stuck, performing action")
                        HitTree()

                        'go drop
                        If HaveGatheredWood = True Then
                            WriteMessageToGlobalChat("we've got wood, dropping off")
                            HaveGatheredWood = False
                            GoHome()
                            WriteMessageToGlobalChat("dropped off, going out to get more wood")
                        End If
                    Else
                        'no
                        WriteMessageToGlobalChat("good rec, we are not stuck")

                        'bumping
                        'MoveMouseMainThread(50)                            

                        'run litle
                        'RunMainThread(Keys.W, False, 100, False)
                    End If
                Else
                    narrowRec = False

                    'no rec                
                    WriteMessageToGlobalChat("bad rec")

                    If AreWeStuck() = 0 Then
                        'we are stuck
                        WriteMessageToGlobalChat("bad rec, we are stuck, performing action")
                        HitTree()

                        'go drop
                        If HaveGatheredWood = True Then
                            WriteMessageToGlobalChat("we've got wood, dropping off")
                            HaveGatheredWood = False
                            'GoHome()
                        End If
                    Else
                        'no
                        WriteMessageToGlobalChat("bad rec, we are not stuck, searching elsewhere!")

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
                DoRespawn(False)
            End If
        Loop
    End Sub

    Private Sub MainBrain()
        CheckForIllegalCrossThreadCalls = False
        Dim objects As Collection
        Dim objectCenterIs As String


        'kill old
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")

        'startup
        lastAction.Text = "warming up"
        ResponsiveSleep(5000)
        WriteMessageToGlobalChat("warming up")

        Dim objectsToFind As New Collection
        objectsToFind.Add("someinventory", "someinventory")
        objectsToFind.Add("woodinventory", "woodinventory")

        'Object detection only!
        'Do
        '    WriteMessageToGlobalChat("==================================== " & DateTime.Now)
        '    WriteMessageToGlobalChat("Starting object detection " & DateTime.Now)
        '    objects = DetectObjects()
        '    WriteMessageToGlobalChat("Done object detection " & DateTime.Now)

        '    'get rec
        '    WriteMessageToGlobalChat("Start centerline " & DateTime.Now)
        '    objectCenterIs = GetObjectsVerticleLinePosition(objects)
        '    WriteMessageToGlobalChat("Done centerline = " & objectCenterIs & " " & DateTime.Now)
        '    WriteMessageToGlobalChat("==================================== " & DateTime.Now)

        '    If objectCenterIs <> 0 Then
        '        'good rec                
        '        Dim ourDifference = objectCenterIs - myCenterIs

        '        WriteMessageToGlobalChat("Moving, Our diff is" & ourDifference & " " & DateTime.Now)

        '        'point to object
        '        MoveMouseMainThreadX(ourDifference)
        '    End If
        'Loop

        'GoHarass()

        'always start the cudes
        StartCuda()

        'always center our vision to horizon        
        MovePlayerEyesToHorizon()

        'waiting for a bag in
        waitForaBagBlocking()
        GoWood()


        'GoHome()
        'CheckChest()
        'EmptyMyInventory()
    End Sub
    Private Sub StartCuda()
        Dim backendThread As New Thread(AddressOf StartPythonBackend)
        backendThread.Start()

        WriteMessageToGlobalChat("waiting for cuda to come up")
        ResponsiveSleep(30000)
        WriteMessageToGlobalChat("cuda should be up")
    End Sub


    Private Function fourCornerRadarRec() As String
        'for dead detect
        Dim dead As New Collection
        dead.Add("someinventory", "someinventory")
        dead.Add("woodinventory", "woodinventory")
        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")

        Dim moveToCenter As Integer = 0
        Dim objects As Collection

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

    Private Sub DumpResourcesAtBaseDoorNowAndDie()
        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), opening door")

        'open door
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(1000)

        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), walk into door")
        'walk into door
        KeyDownUp(Keys.W, False, 1500, False)
        ResponsiveSleep(1000)

        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), turn to close")
        'turn to close door
        MoveMouseMainThreadX(2400)
        ResponsiveSleep(1000)

        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), close door")
        'close it
        KeyDownUp(Keys.E, False, 500, False)
        ResponsiveSleep(1000)

        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), face the other door")
        'turn back straight to face second door
        MoveMouseMainThreadX(-1900)
        ResponsiveSleep(1000)
        MoveMouseMainThreadX(-1900)
        ResponsiveSleep(1000)
        MoveMouseMainThreadX(-450)
        ResponsiveSleep(1000)

        'walk into it a lil
        KeyDownUp(Keys.W, False, 100, False)
        ResponsiveSleep(1000)

        'open that door
        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), open it")
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(1000)

        'walk through door
        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), walk into it")
        KeyDownUp(Keys.W, False, 650, False)
        ResponsiveSleep(1000)

        'turn around
        MoveMouseMainThreadX(-2500)
        ResponsiveSleep(500)

        'close door
        WriteMessageToGlobalChat("DumpResourcesAtBaseDoorNow(), open it")
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(1000)

        'face chests
        MoveMouseMainThreadX(2600)
        ResponsiveSleep(500)

        'run to box
        RunMainThread(Keys.W, True, 2000, False)
        ResponsiveSleep(2000)

        'crouch
        KeyDownOnly(Keys.ControlKey, False, 50, False)
        ResponsiveSleep(2000)

        'check her
        CheckChest()

        'dump me
        EmptyMyInventory()

        'we're done kill ourselves go back out get shit
        waitForaBagBlocking()
    End Sub

    Public Sub CheckChest()
        'open box
        KeyDownUp(Keys.E, False, 50, False)
    End Sub

    Public Sub EmptyMyInventory()
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
        'MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        'ResponsiveSleep(500)

        ''pointed at door
        'MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        'ResponsiveSleep(500)

        ''run to door
        'RunMainThread(Keys.W, False, 1200, False)
        'ResponsiveSleep(1200)

        ''open door
        'KeyDownUp(Keys.E, False, 50, False)
        'ResponsiveSleep(1000)

        ''run out door
        'RunMainThread(Keys.W, False, 1000, False)
        'ResponsiveSleep(1000)

        ''turn towards door
        'MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        'ResponsiveSleep(500)

        ''close it
        'KeyDownUp(Keys.E, False, 50, False)
        'ResponsiveSleep(1000)


        ''run out door!
        'RunMainThread(Keys.W, False, 800, False)
        'ResponsiveSleep(800)

        ''move each dir
        'MoveMouseMainThreadX(-3800)
        'ResponsiveSleep(500)

        ''close door
        'KeyDownUp(Keys.E, False, 10, False)
        'ResponsiveSleep(1000)

        ''move each dir
        'MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
        'ResponsiveSleep(500)

        ''run away from base!
        'RunMainThread(Keys.W, True, 5000, True)
        'ResponsiveSleep(1000)
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
        Do
            'get our current position
            Dim currentPosition = GetCurrentPosition()
            Dim distanceFromHome = currentPosition(3)

            logLabel.Text = logLabel.Text & "distance from home: " & distanceFromHome & " Running" & vbCrLf
            WriteMessageToGlobalChat("distance from home: " & distanceFromHome & " running")

            HowFarToRunTime(distanceFromHome)

            Dim currentPositionMoved = GetCurrentPosition()
            Dim distanceFromHomeMoved = currentPositionMoved(3)

            logLabel.Text = logLabel.Text & "new distance from home: " & distanceFromHome & vbCrLf
            WriteMessageToGlobalChat("new distance from home: " & distanceFromHome)

            Dim changeInDistance = distanceFromHomeMoved - distanceFromHome

            logLabel.Text = logLabel.Text & "change in distance: " & changeInDistance & vbCrLf
            WriteMessageToGlobalChat("change in distance: " & changeInDistance)

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
                If distanceFromHomeMoved > 50 Or distanceFromHomeMoved.ToString.Contains("-") Then
                    'closer or farther?
                    If changeInDistance.ToString.Contains("-") Then
                        'closer
                        WriteMessageToGlobalChat("")

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

                    WriteMessageToGlobalChat("entering close base mode")

                    Do
                        'is our base here?
                        Dim moveToCenter As Integer = fourCornerRadarRec()

                        'have an object to point to?
                        If moveToCenter = 0 Then
                            'nope
                            WriteMessageToGlobalChat("didn't find a base")

                            'get out of close to home, no base found
                            Exit Do
                        Else
                            WriteMessageToGlobalChat("found a base, turning and running to = " & moveToCenter)

                            'yup                
                            MoveMouseMainThreadX(moveToCenter)
                            ResponsiveSleep(500)

                            'run to it        
                            'Run(Keys.W, True, 2500, False)

                            'stuck?
                            If AreWeStuck() = 0 Then
                                'yes, perform action
                                WriteMessageToGlobalChat("stuck at door, dumping resources")

                                'dumping resources
                                DumpResourcesAtBaseDoorNowAndDie()

                                WriteMessageToGlobalChat("resources dumped, killing")

                                Exit Do
                            Else
                                'not stuck, just keep going
                                WriteMessageToGlobalChat("not stuck")
                            End If
                        End If
                    Loop

                    'WriteMessageToGlobalChat("We are home, killing and starting over")

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



    Private Shared Function IsNullOrEmpty(ByVal myStringArray() As String) As Boolean
        Return ((myStringArray Is Nothing) OrElse (myStringArray.Length < 1))
    End Function

    Public Sub GoHarass()
        Dim objects As Collection
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
                                WriteMessageToGlobalChat("")

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
                                WriteMessageToGlobalChat("")

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
                DoRespawn(False)
            End If
        Loop


    End Sub

    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Dim Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function

    Private Sub logLabel_TextChanged(sender As Object, e As EventArgs) Handles logLabel.TextChanged
        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()
    End Sub

    Public lastHash As String

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

    End Sub
End Class