' get past MouseData not being initialized warning...it needs to be there for p/invoke
Imports System
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Public Class SendInputClass
    Structure MouseInput

        Public X As Integer

        Public Y As Integer

        Public MouseData As UInteger

        Public Flags As UInteger

        Public Time As UInteger

        Public ExtraInfo As IntPtr
    End Structure

    Structure Input

        Public Type As Integer

        Public MouseInput As MouseInput
    End Structure

    Public Const InputMouse As Integer = 0

    Public Const MouseEventMove As Integer = 1

    Public Const MouseEventLeftDown As Integer = 2

    Public Const MouseEventLeftUp As Integer = 4

    Public Const MouseEventRightDown As Integer = 8

    Public Const MouseEventRightUp As Integer = 16

    Public Const MouseEventAbsolute As Integer = 32768

    Private Shared lastLeftDown As Boolean

    Private Declare Function SendInput Lib "user32.dll" (ByVal numInputs As UInteger, ByVal inputs() As Input, ByVal size As Integer) As UInteger

    Public Sub SendMouseInput(ByVal positionX As Integer, ByVal positionY As Integer, ByVal maxX As Integer, ByVal maxY As Integer, ByVal leftDown As Boolean)
        If (positionX > Integer.MaxValue) Then
            Throw New ArgumentOutOfRangeException("positionX")
        End If

        If (positionY > Integer.MaxValue) Then
            Throw New ArgumentOutOfRangeException("positionY")
        End If

        Dim i() As Input = New Input((2) - 1) {}
        ' move the mouse to the position specified
        i(0) = New Input
        i(0).Type = InputMouse
        i(0).MouseInput.X = ((positionX * 65535) _
                        / maxX)
        i(0).MouseInput.Y = ((positionY * 65535) _
                        / maxY)
        i(0).MouseInput.Flags = (MouseEventAbsolute Or MouseEventMove)
        ' determine if we need to send a mouse down or mouse up event
        If (Not lastLeftDown _
                        AndAlso leftDown) Then
            i(1) = New Input
            i(1).Type = InputMouse
            i(1).MouseInput.Flags = MouseEventLeftDown
        ElseIf (lastLeftDown _
                        AndAlso Not leftDown) Then
            i(1) = New Input
            i(1).Type = InputMouse
            i(1).MouseInput.Flags = MouseEventLeftUp
        End If

        lastLeftDown = leftDown
        ' send it off
        Dim result As UInteger = SendInput(2, i, Marshal.SizeOf(i(0)))
        If (result = 0) Then
            Throw New Win32Exception(Marshal.GetLastWin32Error)
        End If

    End Sub
End Class
