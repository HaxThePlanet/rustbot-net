Imports System.Runtime.InteropServices

Public Class ScreenCapturer
    <DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(hWnd As IntPtr, ByRef rect As Rect) As IntPtr
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Rect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    Public Function Capture(Optional activeWindowOnly As Boolean = True) As Bitmap
        On Error Resume Next

        Dim bounds As Rectangle

        If Not activeWindowOnly Then
            bounds = Screen.GetBounds(Point.Empty)
            CursorPosition = Cursor.Position
        Else
            Dim foregroundWindowsHandle = GetForegroundWindow()
            Dim rect = New Rect()
            GetWindowRect(foregroundWindowsHandle, rect)
            bounds = New Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top)
            CursorPosition = New Point(Cursor.Position.X - rect.Left, Cursor.Position.Y - rect.Top)
        End If

        Dim result = New Bitmap(bounds.Width, bounds.Height)

        Using g = Graphics.FromImage(result)
            g.CopyFromScreen(New Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size)
        End Using

        Return result
    End Function

    Public Property CursorPosition() As Point
End Class
