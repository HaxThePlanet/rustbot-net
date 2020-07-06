Public Class Constants
    'Rust Game
    Public Const rustStartCommand As String = "C:\Program Files (x86)\Steam\steam.exe -applaunch 252490"
    Public Const rustKillCommand As String = "taskkill /f /im rustclient.exe"
    Public Const rustLoadGameWaitMilli As Integer = 80000
    Public Const rustConnectServerGameWaitMilli As Integer = 120000

    'SQL
    Public Const SqlPort As Integer = 5000
    Public Const SqlServerHostname As String = "71.136.235.82\SQLEXPRESS"
    Public Const SqlServerUser As String = "chad"
    Public Const SqlServerPass As String = "Formatme76_"

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

    'home location, no decimal
    Public Const homex1 As Integer = -597
    Public Const homey1 As Integer = 18
    Public Const homez1 As Integer = 1134

    'this is for stuck detect, narrow view
    Public Const compareWidth As Integer = 50
    Public Const compareHeight As Integer = 500
    Public Const compareSourceX As Integer = 1850
    Public Const compareSourcey As Integer = 400
    Public Const compareDestinationX As Integer = 0
    Public Const compareDestinationy As Integer = 0

    'this is for honing in on an object after we've detected it to prevent hunting around

    '4k
    Public Const compareWidthNarrow As Integer = 250
    Public Shared compareHeightNarrow As Integer = 250
    Public Shared compareSourceXNarrow As Integer = (Screen.PrimaryScreen.Bounds.Width / 2) - 500
    Public Shared compareSourceyNarrow As Integer = (Screen.PrimaryScreen.Bounds.Height / 2) - 500

    Public Const compareDestinationXNarrow As Integer = 0
    Public Const compareDestinationyNarrow As Integer = 0

    'center of screen
    'Public Const myCenterIs As Integer = 1920 '4k
    Public Const myCenterIs As Integer = 960 '4k
    'set mouse sense to .6 
    'turn off motion blur
    'bind keys
    Public Const eyesMoveUpDistance As Integer = -1000 '-2200 4k, -1000 1080p

    Public Const rightTreeHitBump As Integer = -10 '-120 4k, -80 1080p

    'each turn distance required to make 25% of the way around, good for four corner turn rec
    'Public Const eachMoveInFullTurn As Integer = 2220 '4k

    Public Const eachMoveInFullTurn As Integer = -960 '1080p

    'wood count return home
    Public Const woodStacksHomeReturnCount As Integer = 3

    'max rec retries
    Public Const maxRecRetry As Integer = 5

    'harvesting wood inference failure tolerance
    Public Const maxGatheringWoodFailures As Integer = 5 '5 seems to work well

    'front yard radius
    Public Const frontYardRadius As Integer = 25

    'map focus button location
    Public Const xMapFocusButtonLocation As Integer = 112
    Public Const yMapFocusButtonLocation As Integer = 99

End Class
