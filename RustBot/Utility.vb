Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Threading

Module Utility
    Dim homex1 As Integer = 1581.9
    Dim homey1 As Integer = 2.0
    Dim homez1 As Integer = 165.1

    <DllImport("user32.dll")>
    Private Sub mouse_event(ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal dwData As Integer, ByVal dwExtraInfo As Integer)
    End Sub
    Private Const MOUSEEVENTF_MOVE As Integer = &H1

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
    Dim lastWidth As Integer = -1
    Dim lastObjectCenterline

    Public Function DetectObjects() As String
        Dim Output As String

        'kill
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")
        If File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv") Then Kill("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv")

        'take screen
        TakeScreenShotWhole("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\processme.png")

        'wait for spreadsheet
        Do Until File.Exists("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv")
            Thread.Sleep(50)
        Loop

        'read all text
        Dim fs As FileStream = New FileStream("C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Image_Detection_Results\Detection_Results.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim sr As StreamReader = New StreamReader(fs)
        Dim value As String = sr.ReadToEnd

        fs.Close()
        sr.Close()

        'return it
        Return value
    End Function

    Public Function GetObjectsVerticleLinePosition() As String

        Dim obj As String = DetectObjects()
        Dim theSplit = Split(obj, vbCrLf)

        Debug.Print(theSplit.Count - 1 & " objects")
        For i = 1 To theSplit.Count - 2
            Dim theSplitNext = Split(theSplit(1), ",")

            'no rec
            If theSplitNext(0) = "" Then Exit For

            Dim theProbNew As String = theSplitNext(7).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmin = theSplitNext(2).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymin = theSplitNext(3).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            xmax = theSplitNext(4).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "")
            ymax = theSplitNext(5).Replace(vbCrLf, "").Replace(Chr(34), "").Replace("(", "").Replace(")", "").Replace("&", "").Replace(",", "").Replace("vbCrLf", "").Replace("Time", "")

            'Debug.Print("xmin = " & xmin)
            'Debug.Print("ymin = " & ymin)
            'Debug.Print("xmax = " & xmax)
            'Debug.Print("ymax = " & ymax)

            'find his width
            Dim theWidth As Integer = xmax - xmin

            Debug.Print("theWidth = " & theWidth)

            'widest?
            If theWidth > lastWidth Then
                'yes
                lastWidth = theWidth

                'lets get his centerline
                objectCenterline = (lastWidth / 2) + xmin

                'set it
                lastObjectCenterline = objectCenterline
            End If

            Debug.Print("objectCenterline = " & theWidth)
        Next

        If lastObjectCenterline = Nothing Then lastObjectCenterline = 0
        Return lastObjectCenterline
    End Function

    Public Sub KeyDownUp(ByVal key As Byte, shift As Boolean, ByVal durationInMilli As Integer, jumping As Boolean)
        Dim targetTime As DateTime = DateTime.Now().AddMilliseconds(durationInMilli)
        Dim kb_delay As Integer
        Dim kb_speed As Integer

        SystemParametersInfo(SPI_GETKEYBOARDDELAY, 0, kb_delay, 0)
        SystemParametersInfo(SPI_GETKEYBOARDSPEED, 0, kb_speed, 0)

        If shift Then
            ShiftDwn()
        End If

        keybd_event(key, MapVirtualKey(key, 0), 0, 0) ' key pressed

        While targetTime.Subtract(DateTime.Now()).TotalMilliseconds > 0
            System.Threading.Thread.Sleep(kb_delay + kb_speed)
            If jumping Then
                If (targetTime.Subtract(DateTime.Now()).TotalMilliseconds).ToString.Contains("00") Then
                    Jump()
                End If
            End If
        End While

        keybd_event(key, MapVirtualKey(key, 0), 2, 0) ' key released

        If shift Then
            ShiftUP()
        End If
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
        mouse_event(MOUSEEVENTF_LEFTUP, 6, 0, 0, 0)
    End Sub

    Private Sub ShiftDwn()
        keybd_event(Keys.ShiftKey, MapVirtualKey(Keys.ShiftKey, 0), 0, 0)
    End Sub

    Public Sub Jump()
        keybd_event(Keys.Space, MapVirtualKey(Keys.Space, 0), 0, 0)
        keybd_event(Keys.Space, MapVirtualKey(Keys.Space, 0), KEYEVENTF_KEYUP, 0)
    End Sub

    Private Sub ShiftUP()
        keybd_event(Keys.ShiftKey, MapVirtualKey(Keys.ShiftKey, 0), KEYEVENTF_KEYUP, 0)
    End Sub

    Public Sub TakeScreenShotWhole(file As String)
        Dim sc = New ScreenCapturer()
        Using bitmap = sc.Capture()
            bitmap.Save(file, ImageFormat.Png)
            bitmap.Dispose()
        End Using

        sc = Nothing
    End Sub

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
            Threading.Thread.Sleep(500) : Application.DoEvents()
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

    Public Function fastCompareImages(path1 As String, path2 As String) As Double
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

    Public Function compareImages(path1 As String, path2 As String) As Double
        Application.DoEvents()

        ' Load the images.
        Dim bm1 As Bitmap = Image.FromFile(path1)
        Dim bm2 As Bitmap = Image.FromFile(path2)

        ' Make a difference image.
        Dim wid As Integer = Math.Min(bm1.Width, bm2.Width)
        Dim hgt As Integer = Math.Min(bm1.Height, bm2.Height)
        Dim bm3 As New Bitmap(wid, hgt)

        ' Create the difference image.
        Dim are_identical As Boolean = True
        Dim r1, g1, b1, r2, g2, b2, r3, g3, b3 As Integer
        Dim eq_color As Color = Color.White
        Dim ne_color As Color = Color.Red

        Dim eachRed As Long

        For x As Integer = 0 To wid - 1
            Application.DoEvents()

            For y As Integer = 0 To hgt - 1
                Application.DoEvents()

                If bm1.GetPixel(x, y).Equals(bm2.GetPixel(x, y)) Then
                    bm3.SetPixel(x, y, eq_color)
                    eachRed += 1
                Else
                    bm3.SetPixel(x, y, ne_color)
                    are_identical = False
                End If
            Next y
        Next x


        Dim totalPixels As Integer = 0
        totalPixels = wid * hgt

        Dim thePercent As Double
        thePercent = eachRed / totalPixels

        ' Display the result.
        'picResult.Image = bm3

        If (bm1.Width <> bm2.Width) OrElse (bm1.Height <>
            bm2.Height) Then are_identical = False
        If are_identical Then
            '    MessageBox.Show("The images are identical")
        Else
            '   MessageBox.Show("The images are different")
        End If

        bm1.Dispose()
        bm2.Dispose()

        Return Math.Round(thePercent * 100, 2)
    End Function

    Public Function GetGenericTagging()
        Dim Output As String

        Using info As Process = New Process()
            info.StartInfo.FileName = "python.exe"
            info.StartInfo.Arguments = "generictagging.py"
            info.StartInfo.UseShellExecute = False
            info.StartInfo.RedirectStandardOutput = True
            info.StartInfo.CreateNoWindow = True
            info.Start()
            Output = info.StandardOutput.ReadToEnd()
            info.WaitForExit()
        End Using

        Dim json As JObject = JObject.Parse(Output)
        Dim converted As String = json.ToString.Replace(" ", "").Replace(vbCrLf, "").Replace(Chr(34), "").Replace("{", "").Replace("[", "").Replace("}", "").Replace("]", "")

        Dim TheSplit = Split(converted, "tags:")
        Dim TheSplit2 = Split(TheSplit(1), "task_id")
        Dim TheSplit3 = Split(TheSplit2(0), ",")

        'drop blank
        Array.Resize(TheSplit3, TheSplit3.Length - 1)

        'return array
        Return TheSplit3
    End Function

    Public Function DetectWater() As Boolean
        TakeScreenShotWhole("processme.bmp")

        Dim tags As Array = GetGenericTagging()

        For I = 0 To tags.Length - 1
            If tags(I).ToString.Contains("sea") Or tags(I).ToString.Contains("ocean") Or tags(I).ToString.Contains("water") Or tags(I).ToString.Contains("lake") Or tags(I).ToString.Contains("stream") Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function DoGenericTagging() As Collection
        TakeScreenShotWhole("processme.bmp")
        Dim collection As New Collection

        Dim tags As Array = GetGenericTagging()

        'do we have wood?
        For I = 0 To tags.Length - 1
            If tags(I).ToString.Contains("wood") Or tags(I).ToString.Contains("tree") Or tags(I).ToString.Contains("forest") Then
                'yes, return highest probability
                collection.Add(tags(I), tags(I - 1))
            End If
        Next

    End Function



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

        'bump
        KeyDownUp(Keys.W, True, 100, False)
        KeyDownUp(Keys.A, True, 100, False)

        'after
        Dim posAfter = GetCurrentPosition()

        Dim tempadd2 As Double
        For I = 0 To posAfter.Length - 1
            tempadd2 = tempadd2 + LTrim(RTrim(Double.Parse(posAfter.GetValue(I))))
        Next

        If tempadd2 <> tempadd1 Then
            Debug.Print("We have moved")
            Return False
        Else
            Debug.Print("We have NOT moved")
            Return True
        End If
    End Function

    Public Function GetCurrentPosition() As Array
        'On Error Resume Next
        Debug.Print("Getting Position")

        'bring up console
        KeyDownUp(Keys.F1, False, 1, False)
        ResponsiveSleep(500)

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
        KeyDownUp(Keys.Escape, False, 1, False)



        'read log
        Dim fs As FileStream = New FileStream("C:\Program Files (x86)\Steam\steamapps\common\Rust\output_log.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim sr As StreamReader = New StreamReader(fs)
        Dim lines = Split(sr.ReadToEnd, vbCr)
        Dim output As String = lines(lines.Count - 2).Replace(")", "")
        Dim TheSplit = Split(output, "(")
        Dim TheSplit2 = Split(TheSplit(1), ",")

        Dim distance As Integer = Distance3D(TheSplit2(0), TheSplit2(1), TheSplit2(2), homex1, homey1, homez1)

        TheSplit(1) = TheSplit(1) & "," & distance
        TheSplit2 = Split(TheSplit(1), ",")

        fs.Close()
        sr.Close()

        Return TheSplit2
    End Function


End Module
