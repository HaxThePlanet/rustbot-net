Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Threading
Imports System.Security.Cryptography
Imports XnaFan.ImageComparison

Module Utilitys
    <DllImport("user32.dll")>
    Private Sub mouse_event(ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal dwData As Integer, ByVal dwExtraInfo As Integer)
    End Sub
    Private Const MOUSEEVENTF_MOVE As Integer = &H1

    Private Declare Function GetCursorPos Lib "user32.dll" (ByRef lpPoint As Point) As Boolean

    Private Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)

    <DllImport("User32.dll", SetLastError:=False, CallingConvention:=CallingConvention.StdCall, CharSet:=CharSet.Auto)>
    Public Function MapVirtualKey(ByVal uCode As UInt32, ByVal uMapType As UInt32) As UInt32
    End Function

    Public Const KEYEVENTF_EXTENDEDKEY = &H1    'Key DOWN
    Public Const KEYEVENTF_KEYUP = &H2          'Key UP

    Private Const VK_LBUTTON = &H1
    Private Const VK_RBUTTON = &H2

    Private Const MOUSEEVENTF_LEFTDOWN = &H2
    Private Const MOUSEEVENTF_LEFTUP = &H4
    Private Const MOUSEEVENTF_RIGHTDOWN = &H8
    Private Const MOUSEEVENTF_RIGHTUP = &H10

    Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Long, ByVal dx As Long, ByVal dy As Long, ByVal cButtons As Long, ByVal dwExtraInfo As Long)

    Const SPI_GETKEYBOARDDELAY = 22
    Const SPI_GETKEYBOARDSPEED = 10

    Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer,
    ByRef lpvParam As Integer, ByVal fuWinIni As Integer) As Integer

    Public theProb As Integer
    Public xmin As Integer
    Public ymin As Integer
    Public xmax As Integer
    Public ymax As Integer
    Public theCenterline As Integer
    Dim objectCenterline As Integer
    Dim lastGlobalWidth As Integer = -1
    Dim highestProb As Integer = -1
    Dim lastObjectCenterline

    Public Function GetObjectsVerticleLinePosition(objects As String) As String
        lastObjectCenterline = 0
        lastGlobalWidth = 0
        highestProb = 0

        Dim theSplit = Split(objects, vbCrLf)
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

            Debug.Print("xmin = " & xmin)
            'Debug.Print("ymin = " & ymin)
            'Debug.Print("xmax = " & xmax)
            'Debug.Print("ymax = " & ymax)

            'find his width
            Dim hisWidth As Integer = xmax - xmin

            'right type?
            If Label.Contains("tree") Then
                'right criteria?
                If hisWidth > lastGlobalWidth Then
                    'yes
                    highestProb = theProbNew

                    'set his width                     
                    lastGlobalWidth = hisWidth

                    'lets get his centerline
                    objectCenterline = (hisWidth / 2) + xmin

                    'set it
                    lastObjectCenterline = objectCenterline

                    'last obj
                    LastObject = theSplitNext(6)
                End If
            End If
        Next

        If lastObjectCenterline = Nothing Then lastObjectCenterline = 0

        If lastObjectCenterline = 0 Then
            Debug.Print("")
        Else
            Debug.Print("Pointing at widest object = " & LastObject & " " & LabelToObjectName(LastObject) & " " & Label)
        End If

        Return lastObjectCenterline
    End Function

    Public Function GetGlobalMousePosition() As Point
        Dim pt As New Point
        GetCursorPos(pt)

        Return pt
    End Function

    Public Function DetectObjects() As String
        Dim Output As String
        Debug.Print("begin detecting objects")

        'kill
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processmedone.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processmedone.png")
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv")

        Debug.Print("taking screenshot")

        'take screen
        TakeScreenShotWhole("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")

        Debug.Print("done taking screenshot, waiting for spreadsheet")

        'wait for spreadsheet
        Do Until File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv")
            Thread.Sleep(100)
        Loop

        Debug.Print("done spreadsheet")

        'show preview
        Form1.previewImageEvent = True

        'read all text
        Dim fs As FileStream = New FileStream("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim sr As StreamReader = New StreamReader(fs)
        Dim value As String = sr.ReadToEnd

        fs.Close()
        sr.Close()

        Try
            If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processmedone.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processmedone.png")
            If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv")
        Catch
            Debug.Print("Couldnt del csv")
        End Try

        Debug.Print("done detect objects")

        'return it
        Return value
    End Function

    Public Function LabelToObjectName(label As Integer) As String
        'Dim labels As New Collection
        'Dim readIn As String
        'Dim ints As Integer = 3

        'Using Reader As New StreamReader("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Model_Weights\data_classes.txt")
        '    While Reader.EndOfStream = False
        '        readIn = Reader.ReadLine()
        '        labels.Add(readIn, ints)

        '        ints += 1
        '    End While

        '    Reader.Close()
        '    Reader.Dispose()
        'End Using


        'If labels.Contains(label) Then
        '    Return labels.Item(label - 3)
        'Else
        '    Return "UNKNOWN LABEL"
        'End If

        If label = 0 Then
            Return "wooddoorfront"
        End If
        If label = 1 Then
            Return "wooddoorback"
        End If
        If label = 2 Then
            Return "metaldoor"
        End If
        If label = 3 Then
            Return "treeforest"
        End If
        If label = 4 Then
            Return "treedesert"
        End If
        If label = 5 Then
            Return "cactus"
        End If
        If label = 6 Then
            Return "sulfur"
        End If
        If label = 7 Then
            Return "buildrock"
        End If
        If label = 9 Then
            Return "player"
        End If
        If label = 13 Then
            Return "woodinventory"
        End If
        If label = 10 Then
            Return "woodinventory"
        End If
        If label = 12 Then
            Return "mushrooms"
        End If
        If label = 13 Then
            Return "dead"
        End If
        If label = 14 Then
            Return "someinventory"
        End If
        If label = 15 Then
            Return "water"
        End If
        If label = 16 Then
            Return "refinery"
        End If
        If label = 17 Then
            Return "largeoven"
        End If
        If label = 18 Then
            Return "wood1000"
        End If
        If label = 19 Then
            Return "truck"
        End If
        If label = 20 Then
            Return "barrel"
        End If
        If label = 21 Then
            Return "worthlessbarrel"
        End If
        If label = 22 Then
            Return "sleepingbag"
        End If
        If label = 23 Then
            Return "stump"
        End If
        If label = 24 Then
            Return "stump"
        End If
        If label = 25 Then
            Return "helicopter"
        End If
        If label = 26 Then
            Return "sleeping"
        End If
        If label = 27 Then
            Return "train"
        End If
        If label = 28 Then
            Return "wounded"
        End If
        If label = 29 Then
            Return "weed"
        End If
        If label = 30 Then
            Return "buildingpriv"
        End If
        If label = 31 Then
            Return "woodwallfront"
        End If
        If label = 32 Then
            Return "starving"
        End If
        If label = 33 Then
            Return "dehydrated"
        End If
        If label = 34 Then
            Return "hemp"
        End If
        If label = 35 Then
            Return "boat"
        End If
        If label = 36 Then
            Return "noinventory"
        End If
        If label = 41 Then
            Return "gatheringwood"
        End If

        Return ""
    End Function

    Public Sub downSampleImage(path As String)
        'resize
        Dim psi As New ProcessStartInfo("C:\Users\bob\source\repos\RustBot\RustBot\bin\Debug\utils\downsample.exe", path & " -resize 4000x4000 " & path)
        Dim p As New Process
        p.StartInfo = psi
        psi.WindowStyle = ProcessWindowStyle.Hidden
        p.Start()
        p.WaitForExit()
    End Sub

    Public Sub Run(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        Dim targetTime As DateTime = DateTime.Now().AddMilliseconds(durationInMilli)
        Dim kb_delay As Integer
        Dim kb_speed As Integer

        SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, kb_delay, 0)
        SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, kb_speed, 0)

        keybd_event(key, MapVirtualKey(key, 0), 0, 0) ' key pressed      

        'shift?
        If shift Then
            'yes
            ShiftDwn()
        End If

        While targetTime.Subtract(DateTime.Now()).TotalMilliseconds > 0
            System.Threading.Thread.Sleep(kb_delay + kb_speed)
            If jumping Then
                If targetTime.Subtract(DateTime.Now()).TotalMilliseconds.ToString.Contains("00") Then
                    Jump()
                End If
            End If
        End While

        keybd_event(key, MapVirtualKey(key, 0), 2, 0) ' key released                

        'was shift down?
        If shift Then
            'yes, pull up
            ShiftUP()
        End If
    End Sub

    Public Sub KeyDownUp(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        Dim targetTime As DateTime = DateTime.Now().AddMilliseconds(durationInMilli)
        Dim kb_delay As Integer
        Dim kb_speed As Integer

        SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, kb_delay, 0)
        SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, kb_speed, 0)

        keybd_event(key, MapVirtualKey(key, 0), 0, 0) ' key pressed      

        While targetTime.Subtract(DateTime.Now()).TotalMilliseconds > 0
            System.Threading.Thread.Sleep(kb_delay + kb_speed)
        End While

        keybd_event(key, MapVirtualKey(key, 0), 2, 0) ' key released                
    End Sub

    Public Sub KeyDownOnly(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        Dim targetTime As DateTime = DateTime.Now().AddMilliseconds(durationInMilli)
        Dim kb_delay As Integer
        Dim kb_speed As Integer

        SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, kb_delay, 0)
        SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, kb_speed, 0)

        If shift Then
            ShiftDwn()
        End If

        keybd_event(key, MapVirtualKey(key, 0), 0, 0) ' key pressed    

        ResponsiveSleep(durationInMilli)
    End Sub

    Public Sub KeyUpOnly(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        Dim targetTime As DateTime = DateTime.Now().AddMilliseconds(durationInMilli)
        Dim kb_delay As Integer
        Dim kb_speed As Integer

        SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, kb_delay, 0)
        SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, kb_speed, 0)

        keybd_event(key, MapVirtualKey(key, 0), 2, 0) ' key released        

        If shift Then
            ShiftUP()
        End If
    End Sub

    Public Sub LeftMouseClick()
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        Application.DoEvents()
        mouse_event(MOUSEEVENTF_LEFTUP, 6, 0, 0, 0)
        Application.DoEvents()
    End Sub

    Public Sub LeftMouseClick(timeInMilli As Integer)
        'down
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
        Application.DoEvents()
        'wait
        ResponsiveSleep(timeInMilli)
        'up
        mouse_event(MOUSEEVENTF_LEFTUP, 6, 0, 0, 0)
        Application.DoEvents()
    End Sub

    Private Sub ShiftDwn()
        keybd_event(Keys.ShiftKey, MapVirtualKey(Keys.ShiftKey, 0), 0, 0)
    End Sub

    Public Sub Jump()
        keybd_event(Keys.Space, MapVirtualKey(Keys.Space, 0), 0, 0)
        keybd_event(Keys.Space, MapVirtualKey(Keys.Space, 0), KEYEVENTF_KEYUP, 0)
    End Sub

    Public Sub ShiftUP()
        keybd_event(Keys.ShiftKey, MapVirtualKey(Keys.ShiftKey, 0), KEYEVENTF_KEYUP, 0)
    End Sub

    Public Sub TakeScreenShotArea(file As String, width As Integer, height As Integer, sourceX As Integer, sourceY As Integer, destinationX As Integer, destinationY As Integer)
        Debug.Print("taking screenshot area")

        Dim printscreen As Bitmap = New Bitmap(width, height)
        Dim graphics As Graphics = Graphics.FromImage(CType(printscreen, Image))
        graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, printscreen.Size)
        printscreen.Save(file, ImageFormat.Png)

waitagain:
        If IsFileUnavailable(file) Then
            ResponsiveSleep(10)
            GoTo waitagain
        End If

        Debug.Print("done taking screenshot area")
    End Sub

    Public Sub TakeScreenShotWhole(file As String)
        On Error Resume Next

        Dim sc = New ScreenCapturer()
        Using bitmap = sc.Capture()
            bitmap.Save(file, ImageFormat.Png)
            bitmap.Dispose()
        End Using
        sc = Nothing

        'wait for available file lock
waitagain:
        If IsFileUnavailable(file) Then
            ResponsiveSleep(10)
            GoTo waitagain
        End If

        'downsample, this fux xy coords in rec dont use
        'downSampleImage("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")

        'move it
        System.IO.File.Move("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png", "C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processmedone.png")
    End Sub

    Public Function IsFileUnavailable(ByVal path As String) As Boolean
        ' if file doesn't exist, return true
        If Not System.IO.File.Exists(path) Then
            Return True
        End If

        Dim file As FileInfo = New FileInfo(path)
        Dim stream As FileStream = Nothing
        Try
            stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)
        Catch ex As IOException
            'the file is unavailable because it is:
            'still being written to
            'or being processed by another thread
            'or does not exist (has already been processed)
            Return True
        Finally
            If (Not (stream) Is Nothing) Then
                stream.Close()
            End If

        End Try

        'file is not locked
        Return False
    End Function

    ''Take handle window screenshot
    Public Sub TakeScreenshotWindow(file As String)
        Dim windowHandle As IntPtr
        Dim processes As Process() = Process.GetProcessesByName("RustClient")

        For Each p As Process In processes
            windowHandle = p.MainWindowHandle
            Exit For
        Next


        Dim SC As New ScreenShot.ScreenCapture
        Dim img As Image = SC.CaptureScreen()

        SC.CaptureWindowToFile(windowHandle, file, Imaging.ImageFormat.Png)
    End Sub

    Public Sub ResponsiveSleep(ByRef iMilliSeconds As Integer)
        Dim i As Integer, iHalfSeconds As Integer = iMilliSeconds / 500
        For i = 1 To iHalfSeconds
            Threading.Thread.Sleep(500)
            Application.DoEvents()
        Next i
    End Sub

    Public Function GetProcessHandle() As String
        Dim processlist() As Process = Process.GetProcesses
        For Each process As Process In processlist
            If process.ProcessName = "RustClient" Then
                Try
                    Return process.MainWindowHandle
                Catch ex As Exception
                    Return ""
                End Try
            End If
        Next

    End Function

    Public Sub ResizeImageOnDisk(path As String)
        Dim strFileName = System.IO.Path.GetFileName(path)
        Try
            Dim original As Image = Image.FromFile(path)
            Dim resized As Image = ResizeImage(original, New Size(800, 800))
            Dim memStream As MemoryStream = New MemoryStream()
            resized.Save(memStream, ImageFormat.Jpeg)

            Dim file As New FileStream(path, FileMode.Create, FileAccess.Write)

            memStream.WriteTo(file)
            file.Close()
            memStream.Close()
        Catch ex As Exception
        End Try

    End Sub

    Public Function ResizeImage(img As Image, size As Size) As Image
        Return ResizeImage(img, size.Width, size.Height)
    End Function

    Public Function ResizeImage(bmp As Bitmap, width As Integer, height As Integer) As Image
        Return ResizeImage(DirectCast(bmp, Image), width, height)
    End Function

    Public Function ResizeImage(bmp As Bitmap, size As Size) As Image
        Return ResizeImage(DirectCast(bmp, Image), size.Width, size.Height)
    End Function

    Public Function fastCompareImages(path1 As String, path2 As String) As Double
        downSampleImage(path1)
        downSampleImage(path2)

        Dim img1 As Bitmap = New Bitmap(path1)
        Dim img2 As Bitmap = New Bitmap(path2)
        If (img1.Size <> img2.Size) Then
            Console.Error.WriteLine("Images are of different sizes")
            Return 0.0
        End If

        Dim diff As Single = 0
        Dim y As Integer = 0
        Do While (y < img1.Height)
            Dim x As Integer = 0
            Do While (x < img1.Width)
                Dim pixel1 As Color = img1.GetPixel(x, y)
                Dim pixel2 As Color = img2.GetPixel(x, y)
                diff = (diff + Math.Abs((pixel1.R - pixel2.R)))
                diff = (diff + Math.Abs((pixel1.G - pixel2.G)))
                Try
                    diff = (diff + Math.Abs((pixel1.B - pixel2.B)))
                Catch
                End Try
                x = (x + 1)
            Loop
            y = (y + 1)
        Loop

        Console.WriteLine("diff: {0} %", (100 * ((diff / 255) / (img1.Width * (img1.Height * 3)))))
    End Function

    Public Function compareImages(ByVal bmp1 As String, ByVal bmp2 As String, Optional ByVal threshold As Byte = 3) As Single
        'get the full path of the images
        Dim image1Path As String = Path.Combine("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Ocr", bmp1)
        Dim image2Path As String = Path.Combine("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Ocr", bmp2)

        Dim firstBmp As Bitmap = CType(Image.FromFile(image1Path), Bitmap)
        Dim secondBmp As Bitmap = CType(Image.FromFile(image2Path), Bitmap)

        'get the difference as a bitmap
        firstBmp.GetDifferenceImage(secondBmp, True).Save((image1Path + "_diff.png"))

        Dim theDiff As Single = firstBmp.PercentageDifference(secondBmp, threshold) * 100

        Debug.Print(String.Format("Difference: {0:0.0} %", (theDiff)))

        firstBmp.Dispose()
        secondBmp.Dispose()

        Return theDiff
    End Function

    'Public Function compareImages(path1 As String, path2 As String) As Double
    '    Debug.Print("compareImages, starting downsample")
    '    'downSampleImage(path1)
    '    'downSampleImage(path2)

    '    Debug.Print("compareImages, starting comparison")

    '    'compare the two
    '    Console.WriteLine(("Comparing: " + (bmp1 + (" and " _
    '                    + (bmp2 + (", with a threshold of " + threshold))))))
    '    Dim firstBmp As Bitmap = CType(Image.FromFile(path1), Bitmap)
    '    Dim secondBmp As Bitmap = CType(Image.FromFile(path2), Bitmap)
    '    'get the difference as a bitmap
    '    firstBmp.GetDifferenceImage(secondBmp, True).Save((path1 + "_diff.png"))
    '    Console.WriteLine(String.Format("Difference: {0:0.0} %", (firstBmp.PercentageDifference(secondBmp, threshold) * 100)))
    '    Console.WriteLine(String.Format("BhattacharyyaDifference: {0:0.0} %", (firstBmp.BhattacharyyaDifference(secondBmp) * 100)))


    '    Debug.Print("compareImages, done")

    '    Return Math.Round((100 * ((diff / 255) / (img1.Width * (img1.Height * 3)))))
    'End Function

    Public Sub ShowInventory()
        KeyDownUp(Keys.Tab, False, 500, False)
        ResponsiveSleep(500)
    End Sub

    Public Sub HideInventory()
        KeyDownUp(Keys.Tab, False, 500, False)
        ResponsiveSleep(500)
    End Sub

    'Public Function GetGenericTagging()
    '    Dim Output As String

    '    Using info As Process = New Process()
    '        info.StartInfo.FileName = "python.exe"
    '        info.StartInfo.Arguments = "generictagging.py"
    '        info.StartInfo.UseShellExecute = False
    '        info.StartInfo.RedirectStandardOutput = True
    '        info.StartInfo.CreateNoWindow = True
    '        info.Start()
    '        Output = info.StandardOutput.ReadToEnd()
    '        info.WaitForExit()
    '    End Using

    '    Dim json As JObject = JObject.Parse(Output)
    '    Dim converted As String = json.ToString.Replace(" ", "").Replace(vbCrLf, "").Replace(Chr(34), "").Replace("{", "").Replace("[", "").Replace("}", "").Replace("]", "")

    '    Dim TheSplit = Split(converted, "tags:")
    '    Dim TheSplit2 = Split(TheSplit(1), "task_id")
    '    Dim TheSplit3 = Split(TheSplit2(0), ",")

    '    'drop blank
    '    Array.Resize(TheSplit3, TheSplit3.Length - 1)

    '    'return array
    '    Return TheSplit3
    'End Function

    'Public Function DetectWater() As Boolean
    '    TakeScreenShotWhole("processme.png")

    '    Dim tags As Array = GetGenericTagging()

    '    For I = 0 To tags.Length - 1
    '        If tags(I).ToString.Contains("sea") Or tags(I).ToString.Contains("ocean") Or tags(I).ToString.Contains("water") Or tags(I).ToString.Contains("lake") Or tags(I).ToString.Contains("stream") Then
    '            Return True
    '        End If
    '    Next

    '    Return False
    'End Function

    'Public Function DoGenericTagging() As Collection
    '    TakeScreenShotWhole("processmedone.bmp")
    '    Dim collection As New Collection

    '    Dim tags As Array = GetGenericTagging()

    '    'do we have wood?
    '    For I = 0 To tags.Length - 1
    '        If tags(I).ToString.Contains("wood") Or tags(I).ToString.Contains("tree") Or tags(I).ToString.Contains("forest") Then
    '            'yes, return highest probability
    '            collection.Add(tags(I), tags(I - 1))
    '        End If
    '    Next

    'End Function

    Public Function DetectWoodInventory() As Boolean
        Dim Output As String

        Using compiler As Process = New Process()
            compiler.StartInfo.FileName = "python.exe"
            compiler.StartInfo.Arguments = "makerequest.py"
            compiler.StartInfo.UseShellExecute = False
            compiler.StartInfo.RedirectStandardOutput = True
            compiler.StartInfo.CreateNoWindow = True
            compiler.Start()
            Output = compiler.StandardOutput.ReadToEnd()
            compiler.WaitForExit()

            If Output.Contains("task_id") Then
                'were done, find best label
                Dim FirstSplit = Split(Output, "best_label")
                If FirstSplit(1).Contains("nowood") Then
                    'no wood
                    Return False
                Else
                    'has wood
                    Return True
                End If
            End If
        End Using

    End Function

    Public Function Distance3D(ByVal x1 As Integer, ByVal y1 As Integer, ByVal z1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal z2 As Integer) As Integer
        '     __________________________________note

        'd = √ (x2-x1)^2 + (y2-y1)^2 + (z2-z1)^2
        '
        'Our end result
        Dim result As Integer = 0
        'Take x2-x1, then square it
        Dim part1 As Double = Math.Pow(x2 - x1, 2)
        'Take y2-y1, then sqaure it
        Dim part2 As Double = Math.Pow(y2 - y1, 2)
        'Take z2-z1, then square it
        Dim part3 As Double = Math.Pow(z2 - z1, 2)
        'Add both of the parts together
        Dim underRadical As Double = part1 + part2 + part3
        'Get the square root of the parts
        result = CInt(Math.Sqrt(underRadical))
        'Return our result
        Return result
    End Function

    Public Function AreWeStuck() As Boolean
        Dim postBefore = GetCurrentPosition()

        Dim tempadd1 As Double
        For I = 0 To postBefore.Length - 1
            tempadd1 = tempadd1 + LTrim(RTrim(Double.Parse(postBefore.GetValue(I))))
        Next

        Debug.Print("Starting stuck run")

        'run see if we moved
        Run(Keys.W, False, 500, False)
        ResponsiveSleep(500)

        Dim tempadd2 As Double

        Debug.Print("Done stuck run")
        'after
        Dim posAfter = GetCurrentPosition()

        For I = 0 To posAfter.Length - 1
            tempadd2 = tempadd2 + LTrim(RTrim(Double.Parse(posAfter.GetValue(I))))
        Next

        If tempadd2 <> tempadd1 Then
            'bump me                        
            Debug.Print("We have moved")
            Return False
        Else
            Debug.Print("We have NOT moved")
            Return True
        End If
    End Function

    Public Function GetCurrentPosition() As Array
        On Error Resume Next

        Debug.Print("Getting Position")

        'bring up console
        KeyDownUp(Keys.F1, False, 1, False)
        'min
        ResponsiveSleep(500)

        ShiftUP()

        'get position
        KeyDownUp(Keys.C, False, 1, False)
        KeyDownUp(Keys.L, False, 1, False)
        KeyDownUp(Keys.I, False, 1, False)
        KeyDownUp(Keys.E, False, 1, False)
        KeyDownUp(Keys.N, False, 1, False)
        KeyDownUp(Keys.T, False, 1, False)
        KeyDownUp(Keys.OemPeriod, False, 1, False)
        KeyDownUp(Keys.P, False, 1, False)
        KeyDownUp(Keys.R, False, 1, False)
        KeyDownUp(Keys.I, False, 1, False)
        KeyDownUp(Keys.N, False, 1, False)
        KeyDownUp(Keys.T, False, 1, False)
        KeyDownUp(Keys.P, False, 1, False)
        KeyDownUp(Keys.O, False, 1, False)
        KeyDownUp(Keys.S, False, 1, False)
        KeyDownUp(Keys.Enter, False, 1, False)
        ResponsiveSleep(250)
        KeyDownUp(Keys.Escape, False, 1, False)

        ResponsiveSleep(500)
        'min    

        'read log
        Dim fs As FileStream = New FileStream("C:\Program Files (x86)\Steam\steamapps\common\Rust\output_log.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim sr As StreamReader = New StreamReader(fs)
        Dim lines = Split(sr.ReadToEnd, vbCr)
        Dim output As String = lines(lines.Count - 2).Replace(")", "")
        Dim TheSplit = Split(output, "(")
        Dim TheSplit2 = Split(TheSplit(1), ",")

        Dim distance As Integer = Distance3D(TheSplit2(0), TheSplit2(1), TheSplit2(2), Form1.homex1, Form1.homey1, Form1.homez1)

        TheSplit(1) = TheSplit(1) & "," & distance
        TheSplit2 = Split(TheSplit(1), ",")

        fs.Close()
        sr.Close()

        Return TheSplit2
    End Function

    ' Function to obtain the desired hash of a file
    Function hash_generator(ByVal hash_type As String, ByVal file_name As String)

        ' We declare the variable : hash
        Dim hash
        If hash_type.ToLower = "md5" Then
            ' Initializes a md5 hash object
            hash = MD5.Create
        ElseIf hash_type.ToLower = "sha1" Then
            ' Initializes a SHA-1 hash object
            hash = SHA1.Create()
        ElseIf hash_type.ToLower = "sha256" Then
            ' Initializes a SHA-256 hash object
            hash = SHA256.Create()
        Else
            MsgBox("Unknown type of hash : " & hash_type, MsgBoxStyle.Critical)
            Return False
        End If

        ' We declare a variable to be an array of bytes
        Dim hashValue() As Byte

        ' We create a FileStream for the file passed as a parameter
        'Dim fileStream As FileStream = File.OpenRead(file_name)

        Dim fileStream As FileStream = New FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

        ' We position the cursor at the beginning of stream
        fileStream.Position = 0
        ' We calculate the hash of the file
        hashValue = hash.ComputeHash(fileStream)
        ' The array of bytes is converted into hexadecimal before it can be read easily
        Dim hash_hex = PrintByteArray(hashValue)

        ' We close the open file
        fileStream.Close()

        ' The hash is returned
        Return hash_hex

    End Function

    ' We traverse the array of bytes and converting each byte in hexadecimal
    Public Function PrintByteArray(ByVal array() As Byte)
        Dim hex_value As String = ""

        ' We traverse the array of bytes
        Dim i As Integer
        For i = 0 To array.Length - 1

            ' We convert each byte in hexadecimal
            hex_value += array(i).ToString("X2")

        Next i

        ' We return the string in lowercase
        Return hex_value.ToLower

    End Function

    ' md5 is a reserved name, so we named the function : md5_hash
    Function md5_hash(ByVal file_name As String)
        Return hash_generator("md5", file_name)
    End Function

    Function sha_1(ByVal file_name As String)
        Return hash_generator("sha1", file_name)
    End Function

    Function sha_256(ByVal file_name As String)
        Return hash_generator("sha256", file_name)
    End Function
End Module
