﻿Imports System.Runtime.InteropServices
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

    Public Shared goHomeBlastAudioVar As Boolean
    Public Shared moveMouseEventX As Boolean
    Public Shared moveMouseHowFarX As Integer
    Public Shared moveMouseEventY As Boolean
    Public Shared moveMouseHowFarY As Integer
    Public Shared leftClickEvent As Boolean
    Public Shared keyDownEvent As Boolean
    Public Shared keyLocal As Byte
    Public Shared shiftLocal As Boolean
    Public Shared durationInMilliLocal As Integer
    Public Shared jumpingLocal As Boolean
    Public Shared HowFarToRun As Integer = 5000
    Public Shared runEvent As Boolean
    Public Shared previewImageEvent As Boolean
    Public Shared keyDownUpEvent As Boolean

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ResponsiveSleep(5000)

        ShiftUP()

        Dim MainLoopThread As New Thread(AddressOf MainBrain)
        MainLoopThread.IsBackground = True
        MainLoopThread.Start()

        Do
            If goHomeBlastAudioVar Then
                goHomeBlastAudioVar = False
                GoHome(True)
            End If

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
                'If File.Exists("C:\Users\bob\Documents\\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then
                '    If IsFileUnavailable("C:\Users\bob\Documents\\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then
                '        ResponsiveSleep(100)
                '        GoTo waitagain
                '    End If
                'Else
                '    GoTo waitagain
                'End If

                'PreviewImg(pic, "C:\Users\bob\Documents\\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")
            End If

            ResponsiveSleep(50)
        Loop
    End Sub

    'Public Sub OcrTakeScreenshot()
    '    'take screen
    '    'TakeScreenShotWhole("C:\Users\bob\yolov5\ocr\ocr.png")

    '    Dim Ocr = New AutoOcr()
    '    Dim Result = Ocr.Read("C:\Users\bob\yolov5\ocr\ocr.png")
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

    Public Shared Sub MoveMouseMainThreadX(howFarX As Integer)
        moveMouseHowFarX = howFarX
        moveMouseEventX = True
    End Sub

    Public Sub MoveMouseMainThreadY(howFarY As Integer)
        moveMouseHowFarY = howFarY
        moveMouseEventY = True
    End Sub

    Public Shared Sub KeyDownMainThread(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
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

    Public Shared Sub RunMainThreadAsync(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        keyLocal = key
        shiftLocal = shift
        durationInMilliLocal = durationInMilli
        jumpingLocal = jumping

        runEvent = True
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ShiftUP()

        WriteLog("killing python")
        Shell("taskkill /f /im python.exe", AppWinStyle.Hide)

        'UNcrouch
        KeyUpOnly(Keys.ControlKey, False, 50, False)
        ResponsiveSleep(500)

        Application.Exit()

        End
    End Sub

    Public keepGathering As Boolean = True
    Private Sub ContiniousTreeHit()
        'loop hitting tree
        Do While keepGathering
            LeftMouseClick()
            ResponsiveSleep(250)
            Application.DoEvents()
        Loop

        'reset
        keepGathering = True
    End Sub

    'Public keepCentering As Boolean = True
    'Private Sub CenterEyesContinous()
    '    'loop hitting tree
    '    Do While keepCentering
    '        LeftMouseClick()
    '        ResponsiveSleep(8000)
    '        Application.DoEvents()
    '        MovePlayerEyesToHorizon()
    '    Loop

    '    'reset
    '    keepCentering = True
    'End Sub


    'new hit tree
    Private Sub HitTree()
        'what to look for
        Dim gathering As New Collection
        gathering.Add("gatheringwood", "gatheringwood")
        Dim woodFails As Integer = 0

        'crouch
        KeyDownOnly(Keys.ControlKey, False, 50, False)
        ResponsiveSleep(250)

        'bring up rock
        KeyDownUp(Keys.M, False, 1, False)

        'wait for it to come up
        ResponsiveSleep(500)

        Dim treeThread As New Thread(AddressOf ContiniousTreeHit)
        treeThread.IsBackground = True
        treeThread.Start()

        'Dim centerEyesThread As New Thread(AddressOf CenterEyesContinous)
        'centerEyesThread.IsBackground = True
        'centerEyesThread.Start()

        'attempt gathering, jump out if we aren't
        Do
            'start inference for wood gather
            If DetectSpecificObjects(gathering, DetectObjects(False)) Then
                'yes
                WriteLog("gathering wood")

                'good inference, reset fail count
                woodFails = 0

                MoveMouseMainThreadX(Constants.rightTreeHitBump)
            Else
                'no
                WriteLog("NOT gathering wood")

                'how many fails?
                If woodFails >= Constants.maxGatheringWoodFailures Then
                    'too many
                    keepGathering = False
                    'keepCentering = False

                    'doneg
                    Exit Do
                End If

                woodFails += 1
            End If

            ResponsiveSleep(1000)
            Application.DoEvents()
        Loop

        MovePlayerEyesToHorizon()

        'UNcrouch
        KeyUpOnly(Keys.ControlKey, False, 50, False)
        ResponsiveSleep(250)

        'hide rock
        KeyDownUp(Keys.M, False, 1, False)
    End Sub

    'Private Sub HitTree()
    '    'crouch
    '    KeyDownOnly(Keys.ControlKey, False, 50, False)
    '    ResponsiveSleep(250)

    '    'what to look for
    '    Dim gathering As New Collection
    '    gathering.Add("gatheringwood", "gatheringwood")

    '    'bring up rock
    '    KeyDownUp(Keys.M, False, 1, False)

    '    'wait for it to come up
    '    ResponsiveSleep(500)

    '    '~60 seconds to collect a tree
    '    Dim firstHit As Boolean = True

    '    'main loop
    '    Do
    '        'test hitting the object to see if we are gathering
    '        If firstHit Then
    '            'must be long enough for gathering images to come up
    '            LeftMouseClick(7000)
    '            firstHit = False
    '        Else
    '            'longer
    '            LeftMouseClick(15000)
    '        End If

    '        'are we gathering?
    '        If DetectSpecificObjects(gathering, DetectObjects(False)) Then
    '            'yes                 

    '            'only bump if we were gathering
    '            MoveMouseMainThreadX(Constants.rightTreeHitBump)
    '            'center horizon
    '            MovePlayerEyesToHorizon()

    '            'yes
    '            WriteMessageToGlobalChat("gathering, continuing to gather")
    '        Else
    '            'try one more time
    '            'are we gathering?
    '            If DetectSpecificObjects(gathering, DetectObjects(False)) Then

    '            Else
    '                'no
    '                WriteMessageToGlobalChat("not gathering, finishing")

    '                'center horizon
    '                MovePlayerEyesToHorizon()

    '                'go elsewhere in case hitting rock or whatever
    '                MoveMouseMainThreadX(GetRandom(-3500, 3500))

    '                Exit Do
    '            End If
    '        End If

    '        Application.DoEvents()
    '    Loop

    '    'put away rock
    '    KeyDownUp(Keys.N, False, 1, False)
    '    ResponsiveSleep(500)

    '    'UNcrouch
    '    KeyUpOnly(Keys.ControlKey, False, 50, False)
    '    ResponsiveSleep(500)
    'End Sub

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
    'End Function

    Public Sub MovePlayerEyesToHorizon()
        'xmax = 271 (sky)
        'xmin = 89 (ground)
        'xlevel = 360

        'move player eyes to fully down, ground
        mouse_event(Constants.MOUSEEVENTF_MOVE, 0, 2500, 0, 0)
        'wait for mouse move
        ResponsiveSleep(250)
        mouse_event(Constants.MOUSEEVENTF_MOVE, 0, 2500, 0, 0)
        'wait for mouse move
        ResponsiveSleep(250)

        'move players eyes back up        
        mouse_event(Constants.MOUSEEVENTF_MOVE, 0, Constants.eyesMoveUpDistance, 0, 0)
        'wait for mouse move
        ResponsiveSleep(250)
    End Sub

    Public Shared Function AreWeStuck() As Boolean
        'bring up map
        ShowMap()

        'take screen right before run                                       
        TakeScreenShotAreaStuck("C:\Users\bob\yolov5\ocr\1.png", Constants.compareWidthNarrow, Constants.compareHeightNarrow, Constants.compareSourceXNarrow, Constants.compareSourceyNarrow, 0, 0)

        'hide map
        HideMap()

        'run
        Run(Keys.W, False, 2500, False) '2500 kind required to make sure we know he moved in ocr 
        'ResponsiveSleep(2500)

        'Run(Keys.W, False, 1500, False)

        'show map
        ShowMap()

        'take screenshot after run                    
        TakeScreenShotAreaStuck("C:\Users\bob\yolov5\ocr\2.png", Constants.compareWidthNarrow, Constants.compareHeightNarrow, Constants.compareSourceXNarrow, Constants.compareSourceyNarrow, 0, 0)


        'hide mapw
        HideMap()

        'compare images, did we move?
        Dim theDiff As Byte(,) = compareImages("C:\Users\bob\yolov5\ocr\1.png", "C:\Users\bob\yolov5\ocr\2.png")

        'If File.Exists("C:\Ugsers\bob\yolov5\ocr\1.png") Then Kill("C:\Users\bob\yolov5\ocr\1.png")
        'If File.Exists("C:\Users\bob\yolov5\ocr\2.png") Then Kill("C:\Users\bob\Documents\\Data\Source_Images\ocr\2.png")

        Dim b As Integer = 0
        Dim d As Integer = 0

        For i = 0 To theDiff.Length
            If d = 15 Then
                b = b + 1

                If b > 15 Then b = 15
            End If

            If theDiff(b, d) > 0 Then
                Return True
            End If

            If d = 15 Then d = 0
            d = d + 1
        Next

        Return False
    End Function

    Private Sub GoWood()
getWoodAgain:

        sendMessage("going to find wood")

        Dim moveToCenter
        Dim objects As Collection
        Dim dead As New Collection
        Dim narrowRec As Boolean = False
        Dim centerlineMinProbability As Double = 0.35

        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")
        dead.Add("wounded", "wounded")

        'center to horizon
        MovePlayerEyesToHorizon()

        'main loop
        Do
            'make day
            makeDayTime()

            'detect objects
            objects = DetectObjects(narrowRec, True)

            'high or low prob settings
            If narrowRec Then
                centerlineMinProbability = 0.5
            Else
                centerlineMinProbability = 0.35
            End If

            'get centerline
            moveToCenter = GetObjectsVerticleLinePosition(objects, "tree", centerlineMinProbability)

            'are we dead?
            If DetectSpecificObjects(dead, objects) = False Then
                'move until water isn't in view
                'If DetectWater() Then
                '    WriteMessageToGlobalChat("Detected water, moving" )
                '    MoveMouseMainThread(1500)
                '    ResponsiveSleep(500)
                'Else

                'have rec?
                If moveToCenter <> 0 Then
                    narrowRec = True

                    'good rec                                    
                    WriteLog("good rec, moving to object centerline = " & moveToCenter)

                    'point to objectos
                    MoveMouseMainThreadX(moveToCenter)
                    'wait for mouse move
                    ResponsiveSleep(250)

                    'are we stuck? 
                    If AreWeStuck() = 0 Then
                        'we are stuck
                        WriteLog("good rec, stuck, performing action")
                        HitTree()

                        'go drop?
                        If detectWoodInventoryCount() > Constants.woodStacksHomeReturnCount Then
                            WriteLog("we've got wood stacks, dropping off")
                            sendMessage("got wood dropping off")
                            GoHome()

                            GoTo getWoodAgain
                        End If
                    Else
                        'no
                        WriteLog("goodw rec, we are not stuck")
                    End If
                Else
                    narrowRec = False

                    'no rec                
                    WriteLog("bad rec")

                    If AreWeStuck() = 0 Then
                        'we are stuck
                        WriteLog("bad rec, we are stuck, performing action")
                        HitTree()

                        'go drop
                        If detectWoodInventoryCount() > Constants.woodStacksHomeReturnCount Then
                            WriteLog("we've got wood, dropping off")
                            sendMessage("got wood dropping off")
                            GoHome()
                        End If
                    Else
                        'try hitting tree just in case
                        'HitTree()

                        'no
                        WriteLog("bad rec, we are NOT STUCK, searching elsewhere!")

                        'bumping                                                
                        MoveMouseMainThreadX(GetRandom(-1500, 1500))
                        ResponsiveSleep(250)

                        'run far away, we radar'd theres nothin
                        KeyDownOnly(Keys.W, True, 5000, False)
                        KeyUpOnly(Keys.W, False, 10, False)
                    End If
                End If
                'End If
            Else
                'we are dead, respawn
                ClickAllBagsAndRespawn()
            End If
        Loop

    End Sub

    Private Sub MainBrain()
        CheckForIllegalCrossThreadCalls = False
        Dim objects As Collection
        Dim objectCenterIs As String

        'always start the cudes
        StartCuda()

        'kill old
        If File.Exists("C:\Users\bob\yolov5\input\processme.png") Then Kill("C:\Users\bob\yolov5\input\processme.png")
        'If File.Exists("C:\Users\bob\Documents\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png") Then Kill("C:\Users\bob\Documents\\Data\Source_Images\Test_Image_Detection_Results\processmedone_rust.png")

        'startup
        ResponsiveSleep(5000)

        'Dim objectsToFind As New Collection
        'objectsToFind.Add("someinventory", "someinventory")
        'objectsToFind.Add("woodinventory", "woodinventory")



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



        'always center our vision to horizon        
        'MovePlayerEyesToHorizon()


        'waiting for a bag in
        'waitForaBagBlocking()        

        'startChromeAudio("https://www.youtube.com/watch?v=3BS5uGSwKwA")        

        'GoHome(True)

        'ClickAllBagsAndRespawn()

        ''retrieve orderscliintpos
        Do
            Dim order As String

            order = tryPickupOrder()
            'have order?
            If order <> "" Then
                Dim theSplit = Split(order, "|")
                Dim ipAddress As String = theSplit(0)
                Dim port As String = theSplit(1)
                Dim coords As String = theSplit(2)
                Dim mode As String = theSplit(3)
                Dim link As String = theSplit(4)
                Dim orderState As String = theSplit(5)
                Dim orderNumber As String = theSplit(6)

                'set map location
                Dim eachSplit = Split(coords, ",")
                Constants.homex1 = LTrim(RTrim(eachSplit(0)))
                Constants.homey1 = LTrim(RTrim(eachSplit(1)))
                Constants.homez1 = LTrim(RTrim(eachSplit(2)))

                'already processed?\
                If orderState.Equals("processing") Then
                    'no, process

                    'execute order
                    executeOrder(order)
                End If
            End If

            ResponsiveSleep(10000)
        Loop


        'DoDoorScan()

        'GoWood()



        'DumpResourcesAtBaseDoorNowAndDie()




        'goDoorScan()        
        'DoDoorScan()

        'CheckChest()
        'EmptyMyInventory()
    End Sub

    Private Function detectWoodInventoryCount() As Integer
        Dim objects As Collection
        Dim woodCount As Integer = 0

        'show inventory
        ShowInventory()

        'check it
        objects = DetectObjects(False)

        'done w inventory
        HideInventory()

        For i = 1 To objects.Count
            If objects.Item(i).contains("wood") Then
                woodCount = woodCount + 1
            End If
        Next

        If woodCount > 0 Then
            sendMessage("i have " & woodCount & " stacks of wood")
        End If

        Return woodCount
    End Function

    Private Sub StartCuda()
        Dim backendThread As New Thread(AddressOf StartPythonBackend)
        backendThread.Start()

        WriteLog("waiting for cuda to come up")
        ResponsiveSleep(10000)
        WriteLog("cuda should be up")
    End Sub
    Public Shared Function fourCornerRadarRec(item As Collection) As String
        Dim moveToCenter As Integer = 0
        Dim objects As Collection

        'turn once, look
        For i = 1 To 4
            'general object detection
            objects = DetectObjects(False)

            For a = 1 To item.Count
                WriteLog("radar, looking for a " & item.Item(a))
                'get rec
                moveToCenter = GetObjectsVerticleLinePosition(objects, item.Item(a))

                'good rec?
                If moveToCenter <> 0 Then
                    'good rec   
                    Return moveToCenter
                Else
                    'bad rec, keep looking
                End If
            Next

            'move each dir
            MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
            ResponsiveSleep(500)
        Next

        'no rec
        Return 0
    End Function

    Public Shared Sub DumpResourcesAtBaseDoorNowAndDie()
        WriteLog("DumpResourcesAtBaseDoorNow(), opening door")

        'open door
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(500)

        WriteLog("DumpResourcesAtBaseDoorNow(), walk into door")
        'LONG walk into door in case edge
        KeyDownUp(Keys.W, False, 2500, False)
        ResponsiveSleep(500)

        WriteLog("DumpResourcesAtBaseDoorNow(), turn to close")
        'turn to close door
        MoveMouseMainThreadX(1500)
        ResponsiveSleep(500)

        WriteLog("DumpResourcesAtBaseDoorNow(), close door")
        'close it
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(500)

        WriteLog("DumpResourcesAtBaseDoorNow(), face the other door")
        'turn back straight to face second door
        MoveMouseMainThreadX(-2220)
        ResponsiveSleep(500)

        'walk into it a lil
        KeyDownUp(Keys.W, False, 100, False)
        ResponsiveSleep(500)

        'open that door
        WriteLog("DumpResourcesAtBaseDoorNow(), open it")
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(500)

        'walk through door
        WriteLog("DumpResourcesAtBaseDoorNow(), walk into it")
        KeyDownUp(Keys.W, False, 750, False)
        ResponsiveSleep(500)

        'turn around
        MoveMouseMainThreadX(-1700)
        ResponsiveSleep(500)

        'close door
        WriteLog("DumpResourcesAtBaseDoorNow(), open it")
        KeyDownUp(Keys.E, False, 50, False)
        ResponsiveSleep(500)

        'face chests
        MoveMouseMainThreadX(1700)
        ResponsiveSleep(500)

        'run to box
        RunMainThreadAsync(Keys.W, True, 2000, False)
        ResponsiveSleep(2000)

        'crouch
        KeyDownOnly(Keys.ControlKey, False, 50, False)

        'check her
        CheckChest()

        'dump me
        EmptyMyInventory()

        'uncrouch
        KeyUpOnly(Keys.ControlKey, False, 50, False)
        ResponsiveSleep(250)

        'we're done kill ourselves go back out get shit
        waitForaBagBlocking()
    End Sub

    Public Shared Sub CheckChest()
        'open box
        KeyDownUp(Keys.E, False, 250, False)
        ResponsiveSleep(250)
        KeyDownUp(Keys.E, False, 250, False)
        ResponsiveSleep(250)
        KeyDownUp(Keys.E, False, 250, False)
        ResponsiveSleep(250)
    End Sub

    Public Shared Sub EmptyMyInventory()
        ResponsiveSleep(1000)

        Win32.SetCursorPos(701, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(800, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(900, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1000, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1100, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1200, 600)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)


        Win32.SetCursorPos(701, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(800, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(900, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1000, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1100, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1200, 700)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)


        Win32.SetCursorPos(701, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(800, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(900, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1000, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1100, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1200, 800)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)


        Win32.SetCursorPos(701, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(800, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(900, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1000, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1100, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1200, 900)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)


        Win32.SetCursorPos(900, 1000)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1000, 1000)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1100, 1000)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        Win32.SetCursorPos(1200, 1000)
        ResponsiveSleep(20)
        RightMouseClick()
        ResponsiveSleep(300)

        CloseInventory()
    End Sub

    '4k
    'Public Sub EmptyMyInventory()
    '    ResponsiveSleep(1000)

    '    Win32.SetCursorPos(1400, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1590, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1792, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1991, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2185, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2185, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2368, 1236)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1407, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1611, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1611, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1800, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1980, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1980, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2190, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2374, 1420)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1409, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1610, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1809, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1986, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2185, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2375, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1409, 1610)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1400, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1602, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1800, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2000, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2200, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2400, 1800)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1789, 2012)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(1986, 2012)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2179, 2012)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    Win32.SetCursorPos(2367, 2012)
    '    ResponsiveSleep(20)
    '    RightMouseClick()
    '    ResponsiveSleep(300)

    '    CloseInventory()
    'End Sub

    Public Shared Sub CloseInventory()
        ShiftUP()

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

    Public Shared Sub HowFarToRunTime(distanceFromHome As String)
        If distanceFromHome > 100 Then
            RunMainThreadAsync(Keys.W, True, 7000, True)
            ResponsiveSleep(7000)
            Exit Sub
        End If

        If distanceFromHome > 75 Then
            RunMainThreadAsync(Keys.W, True, 5000, True)
            ResponsiveSleep(5000)
            Exit Sub
        End If

        If distanceFromHome > 50 Then
            Run(Keys.W, True, 3000, True)
            ResponsiveSleep(3000)
            Exit Sub
        End If

        If distanceFromHome > 40 Then
            Run(Keys.W, True, 2500, False)
            ResponsiveSleep(2500)
            Exit Sub
        End If

        If distanceFromHome <= 40 Then
            RunMainThreadAsync(Keys.W, False, 2500, False)
            ResponsiveSleep(2500)
            Exit Sub
        End If
    End Sub

    Public Shared Function DoDoorScan() As Boolean
        Dim backupDistance As Integer = 2000
        Dim moveForwardDistance As Integer = 2800
        Dim moveLeftDistance As Integer = 4500

        Debug.Print("doing door scan")

        Debug.Print("backing up")

        'backup a bit        
        Run(Keys.S, True, backupDistance, False)

        Debug.Print("detecting doors")

        'check this wall for a door
        Dim objects = DetectObjects(False)

        'get rec for a door
        Dim moveToCenter = GetObjectsVerticleLinePosition(objects, "door")

        If moveToCenter <> 0 Then
            'walk to that door
            Debug.Print("found a door")

            Return True
        Else
            'move and check the next side
            Debug.Print("did not find a door 1, checking again")

            'rotate to next side of building
            Run(Keys.D, False, moveForwardDistance, False)
            Run(Keys.W, False, moveLeftDistance, False)
            MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
            ResponsiveSleep(50)

            'check this wall for a door
            objects = DetectObjects(False)

            'get rec for a door
            moveToCenter = GetObjectsVerticleLinePosition(objects, "door")

            If moveToCenter <> 0 Then
                'walk to that door
                Debug.Print("found a door")
                Return True
            Else
                'move and check the next side
                Debug.Print("did not find a door 2, checking again")

                'rotate to next side of building
                Run(Keys.D, False, moveForwardDistance, False)
                Run(Keys.W, False, moveLeftDistance, False)
                MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
                ResponsiveSleep(50)

                'check this wall for a door
                objects = DetectObjects(False)

                'get rec for a door
                moveToCenter = GetObjectsVerticleLinePosition(objects, "door")

                If moveToCenter <> 0 Then
                    'walk to that door
                    Debug.Print("found a door")
                    Return True
                Else
                    'move and check the next side
                    Debug.Print("did not find a door 3, checking again")

                    'rotate to next side of building
                    Run(Keys.D, False, moveForwardDistance, False)
                    Run(Keys.W, False, moveLeftDistance, False)
                    MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
                    ResponsiveSleep(50)

                    'check this wall for a door
                    objects = DetectObjects(False)

                    'get rec for a door
                    moveToCenter = GetObjectsVerticleLinePosition(objects, "door")

                    If moveToCenter <> 0 Then
                        'walk to that door
                        Debug.Print("found a door")
                        Return True
                    Else
                        'move and check the next side
                        Debug.Print("did not find a door 4, checking again")

                        'rotate to next side of building
                        Run(Keys.D, False, moveForwardDistance, False)
                        Run(Keys.W, False, moveLeftDistance, False)
                        MoveMouseMainThreadX(Constants.eachMoveInFullTurn)
                        ResponsiveSleep(50)
                    End If
                End If
            End If
        End If

        Return False
    End Function

    Public Shared Sub GoHome(Optional audioHarass As Boolean = False)
        WriteLog("gohome")

        Dim foundHomeMessage As Boolean

        Dim objects As New Collection
        Dim dead As New Collection

        'by priority
        Dim findCollection As New Collection
        findCollection.Add("door")
        findCollection.Add("stonewall")

        dead.Add("someinventory", "someinventory")
        dead.Add("woodinventory", "woodinventory")
        dead.Add("respawn", "respawn")
        dead.Add("sleepingbag", "sleepingbag")
        dead.Add("dead", "dead")
        dead.Add("wounded", "wounded")

        Dim distanceChangeCount As Integer = 0

        Do
            'make day
            makeDayTime()
tryAgain:

            'get our current position
            Dim currentPosition As Array = GetCurrentPosition()

            If IsNullOrEmpty(currentPosition) Then
                'WriteMessageToGlobalChat("FAILED TO GET POSITION, TRYING AGAIN")
                GoTo tryAgain
            End If

            Dim distanceFromHome = currentPosition(3)

            WriteLog("distance from home: " & distanceFromHome & ", running")

            HowFarToRunTime(distanceFromHome)

            Dim currentPositionMoved As Array = GetCurrentPosition()

            If IsNullOrEmpty(currentPositionMoved) Then
                WriteLog("FAILED TO GET POSITION, TRYING AGAIN")
                GoTo tryAgain
            End If

            Dim distanceFromHomeMoved = currentPositionMoved(3)

            WriteLog("new distance from home: " & distanceFromHomeMoved)

            Dim changeInDistance = distanceFromHomeMoved - distanceFromHome

            WriteLog("change in distance: " & changeInDistance)

            'move until water isn't in view
            'If DetectWater() Then
            '    WriteMessageToGlobalChat("Detected water, moving" )
            '    MoveMouseMainThread(1500)
            'End If

            'are we dead?           
            objects = DetectObjects(False)
            If DetectSpecificObjects(dead, objects) Then
                ClickAllBagsAndRespawn()
                GoHome(audioHarass)
            End If

            'bad rec?
            If changeInDistance = 0 Then
                WriteLog("stuck, bumping")

                distanceChangeCount = distanceChangeCount + 1

                If distanceChangeCount >= Constants.noDistanceKillCount Then
                    distanceChangeCount = 0
                    DoRespawn(True)
                End If

                'move right a few deg
                MoveMouseMainThreadX(GetRandom(-3500, 3500))
                ResponsiveSleep(500)

                RunMainThreadAsync(Keys.W, True, 1000, True)
            Else
                distanceChangeCount = 0

                If distanceFromHomeMoved > Constants.frontYardRadius Or distanceFromHomeMoved.ToString.Contains("-") Then
                    'closer or farther?
                    If changeInDistance.ToString.Contains("-") Then
                        'closer

                        WriteLog("We are closer, running long")
                    Else
                        'farther                        
                        WriteLog("We are farther, changing direction")

                        'move right a few deg                  
                        MoveMouseMainThreadX(1500)
                        ResponsiveSleep(500)
                    End If
                Else
                    'ok, we are in front yard

                    'starting looking each direction for a door
                    MoveMouseMainThreadX(2500)
                    ResponsiveSleep(500)

                    'stop running                
                    WriteLog("we are home!")

                    'audio harassment?
                    If audioHarass Then
                        'enable audio blaster!
                        KeyDownMainThread(Keys.V, False, 10, False)
                    End If

                    If foundHomeMessage = False Then
                        sendMessage("i am home")
                        foundHomeMessage = True
                    End If

                    WriteLog("entering close base mode, looking for walls")

                    Do
                        'audio harassment?
                        If audioHarass Then
                            'enable audio blaster!
                            KeyDownMainThread(Keys.V, False, 10, False)
                        End If

                        'are we dead?
                        objects = DetectObjects(False)
                        If DetectSpecificObjects(dead, objects) Then
                            ClickAllBagsAndRespawn()
                            GoHome(audioHarass)
                        End If

                        'is our base here?
                        Dim moveToCenter As Integer = fourCornerRadarRec(findCollection)

                        'have an object to point to?
                        If moveToCenter = 0 Then
                            'nope
                            WriteLog("didn't find a base wall")

                            'get out of close to home, no base found
                            Exit Do
                        Else
                            WriteLog("found a base wall, turning and running to = " & moveToCenter)

                            'yup                
                            MoveMouseMainThreadX(moveToCenter)
                            ResponsiveSleep(500)

                            'run to it        
                            Run(Keys.W, True, 5000, False)

                            'stuck?
                            If AreWeStuck() = 0 Then
                                'audio harassment?
                                If audioHarass Then
                                    'enable audio blaster!
                                    KeyDownMainThread(Keys.V, False, 10, False)
                                End If

                                'yes, perform action
                                WriteLog("stuck at wall, doing door scan")
                                If DoDoorScan() Then
                                    Debug.Print("we have a door, inching towards it")

                                    'inching towards door
                                    Do
                                        'are we dead?
                                        objects = DetectObjects(False)
                                        If DetectSpecificObjects(dead, objects) Then
                                            ClickAllBagsAndRespawn()
                                            GoHome(audioHarass)
                                        End If

                                        'audio harassment?
                                        If audioHarass Then
                                            'enable audio blaster!
                                            KeyDownMainThread(Keys.V, False, 10, False)
                                        End If

                                        Debug.Print("we have a door, inching towards it loop")

                                        'detect objects
                                        objects = DetectObjects(False)

                                        'get centerline
                                        moveToCenter = GetObjectsVerticleLinePosition(objects, "door")
                                        'move to centerline
                                        MoveMouseMainThreadX(moveToCenter)
                                        'wait for mouse move
                                        ResponsiveSleep(50)

                                        'run to it        
                                        Run(Keys.W, False, 10, False)

                                        If AreWeStuck() = 0 Then
                                            Debug.Print("we are stuck against door, going in dropoff")

                                            'dumping resources
                                            DumpResourcesAtBaseDoorNowAndDie()

                                            WriteLog("resources dumped, going back out")

                                            Exit Sub
                                        Else
                                            distanceChangeCount = 0
                                        End If
                                    Loop
                                Else
                                    Debug.Print("no door found")
                                End If

                                Exit Do
                            Else
                                distanceChangeCount = 0
                                'not stuck, just keep going
                                WriteLog("not stuck")
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

    Public Shared Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Dim Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function

    Private Sub logLabel_TextChanged(sender As Object, e As EventArgs) Handles logLabel.TextChanged
        logLabel.SelectionStart = logLabel.Text.Length
        logLabel.ScrollToCaret()
    End Sub

    Public lastHash As String

    Private Sub Timer1_Tick(sender As Object, e As EventArgs)

    End Sub
End Class