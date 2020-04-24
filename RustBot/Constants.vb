Public Class Constants

    Public Const KEYEVENTF_EXTENDEDKEY = &H1    'Key DOWN
    Public Const KEYEVENTF_KEYUP = &H2          'Key UP
    Public Const VK_LBUTTON = &H1
    Public Const VK_RBUTTON = &H2
    Public Const MOUSEEVENTF_LEFTDOWN = &H2
    Public Const MOUSEEVENTF_LEFTUP = &H4
    Public Const MOUSEEVENTF_RIGHTDOWN = &H8
    Public Const MOUSEEVENTF_RIGHTUP = &H10
    Public Const SPI_GETKEYBOARDDELAY = 22
    Public Const SPI_GETKEYBOARDSPEED = 10
    Public Const MOUSEEVENTF_MOVE As Integer = &H1

    'home location
    Public Const homex1 As Integer = 380
    Public Const homey1 As Integer = 18
    Public Const homez1 As Integer = 601

    'this is for stuck detect, narrow view
    Public Const compareWidth As Integer = 50
    Public Const compareHeight As Integer = 500
    Public Const compareSourceX As Integer = 1850
    Public Const compareSourcey As Integer = 400
    Public Const compareDestinationX As Integer = 0
    Public Const compareDestinationy As Integer = 0

    'this is for honing in on an object after we've detected it to prevent hunting around
    Public Const compareWidthNarrow As Integer = 1850
    Public Shared compareHeightNarrow As Integer = Screen.PrimaryScreen.Bounds.Height
    Public Shared compareSourceXNarrow As Integer = (Screen.PrimaryScreen.Bounds.Width / 2) - 1000
    Public Const compareSourceyNarrow As Integer = 400
    Public Const compareDestinationXNarrow As Integer = 0
    Public Const compareDestinationyNarrow As Integer = 0

    'center of screen
    Public Const myCenterIs As Integer = 1920
End Class
