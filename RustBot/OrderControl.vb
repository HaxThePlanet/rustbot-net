Module OrderControl
    Public Function controlGameState(game As String, state As String)
        If game.Equals("rust") Then
            If state.Equals("start") Then
                'kill first
                Shell(Constants.rustKillCommand)
                ResponsiveSleep(1000)

                Shell("C:\Program Files (x86)\Steam\Steam.exe steam://rungameid/252490")

                'center
                'Form1.Win32.SetCursorPos(500, 500)
                'ResponsiveSleep(250)
                'LeftMouseClick()
                'ResponsiveSleep(250)
                'KeyDownUp(Keys.R, False, 1, False)
                'ResponsiveSleep(250)
                'KeyDownUp(Keys.R, False, 1, False)
                'ResponsiveSleep(250)
                'KeyDownUp(Keys.Enter, False, 1, False)
                'ResponsiveSleep(250)

                'wait for clearout
                'ResponsiveSleep(1000)
                'Shell(Constants.rustStartCommand)
            End If
            If state.Equals("stop") Then
                Shell(Constants.rustKillCommand)
            End If
        End If
    End Function

    Public Function tryPickupOrder() As String
        Return SqlSelect("SELECT top 1 * from [game-bots].[dbo].[rust-bot-state]", "game-bots", "rust-bot-state")
    End Function

    '104.207.149.205|28036|-597.3,18.0,1134.3|harass|https://www.youtube.com/watch?v=3BS5uGSwKwA
    Public Function executeOrder(order As String)
        Debug.Print("executing order " & order)

        Dim theSplit = Split(order, "|")
        Dim ipAddress As String = theSplit(0)
        Dim port As String = theSplit(1)
        Dim coords As String = theSplit(2)
        Dim mode As String = theSplit(3)
        Dim link As String = theSplit(4)
        Dim orderState As String = theSplit(5)
        Dim orderNumber As String = theSplit(6)

        'change db state
        SQL.SqlUpdate("update [game-bots].[dbo].[rust-bot-state] Set [orderState] = 'botstartup' where [orderNumber] = '" & orderNumber & "'", "game-bots", "rust-bot-state")

        'start audio blaster
        startChromeAudio(link)
        ResponsiveSleep(5000)

        'yes, start game
        controlGameState("rust", "start")
        'wait for game to come up
        ResponsiveSleep(Constants.rustLoadGameWaitMilli)

        'bring up console
        KeyDownUp(Keys.F1, False, 1, False)
        'min
        ResponsiveSleep(250)

        'connect to server
        KeyDownUp(Keys.C, False, 1, False)
        KeyDownUp(Keys.O, False, 1, False)
        KeyDownUp(Keys.N, False, 1, False)
        KeyDownUp(Keys.N, False, 1, False)
        KeyDownUp(Keys.E, False, 1, False)
        KeyDownUp(Keys.C, False, 1, False)
        KeyDownUp(Keys.T, False, 1, False)
        KeyDownUp(Keys.Space, False, 1, False)

        'ip address
        For Each c As Char In ipAddress
            If c = " " Then
                KeyDownUp(Keys.Space, False, 1, False)
            End If
            If c = "." Then
                KeyDownUp(Keys.OemPeriod, False, 1, False)
            End If
            If c = "," Then
                KeyDownUp(Keys.Oemcomma, False, 1, False)
            End If

            If c = "0" Then
                KeyDownUp(Keys.NumPad0, False, 1, False)
            End If
            If c = "1" Then
                KeyDownUp(Keys.NumPad1, False, 1, False)
            End If
            If c = "2" Then
                KeyDownUp(Keys.NumPad2, False, 1, False)
            End If
            If c = "3" Then
                KeyDownUp(Keys.NumPad3, False, 1, False)
            End If
            If c = "4" Then
                KeyDownUp(Keys.NumPad4, False, 1, False)
            End If
            If c = "5" Then
                KeyDownUp(Keys.NumPad5, False, 1, False)
            End If
            If c = "6" Then
                KeyDownUp(Keys.NumPad6, False, 1, False)
            End If
            If c = "7" Then
                KeyDownUp(Keys.NumPad7, False, 1, False)
            End If
            If c = "8" Then
                KeyDownUp(Keys.NumPad8, False, 1, False)
            End If
            If c = "9" Then
                KeyDownUp(Keys.NumPad9, False, 1, False)
            End If

            If c = "a" Then
                KeyDownUp(Keys.A, False, 1, False)
            End If
            If c = "b" Then
                KeyDownUp(Keys.B, False, 1, False)
            End If
            If c = "c" Then
                KeyDownUp(Keys.C, False, 1, False)
            End If
            If c = "d" Then
                KeyDownUp(Keys.D, False, 1, False)
            End If
            If c = "e" Then
                KeyDownUp(Keys.E, False, 1, False)
            End If
            If c = "f" Then
                KeyDownUp(Keys.F, False, 1, False)
            End If
            If c = "g" Then
                KeyDownUp(Keys.G, False, 1, False)
            End If
            If c = "h" Then
                KeyDownUp(Keys.H, False, 1, False)
            End If
            If c = "i" Then
                KeyDownUp(Keys.I, False, 1, False)
            End If
            If c = "j" Then
                KeyDownUp(Keys.J, False, 1, False)
            End If
            If c = "k" Then
                KeyDownUp(Keys.K, False, 1, False)
            End If
            If c = "l" Then
                KeyDownUp(Keys.L, False, 1, False)
            End If
            If c = "m" Then
                KeyDownUp(Keys.M, False, 1, False)
            End If
            If c = "n" Then
                KeyDownUp(Keys.N, False, 1, False)
            End If
            If c = "o" Then
                KeyDownUp(Keys.O, False, 1, False)
            End If
            If c = "p" Then
                KeyDownUp(Keys.P, False, 1, False)
            End If
            If c = "q" Then
                KeyDownUp(Keys.Q, False, 1, False)
            End If
            If c = "r" Then
                KeyDownUp(Keys.R, False, 1, False)
            End If
            If c = "s" Then
                KeyDownUp(Keys.S, False, 1, False)
            End If
            If c = "t" Then
                KeyDownUp(Keys.T, False, 1, False)
            End If
            If c = "u" Then
                KeyDownUp(Keys.U, False, 1, False)
            End If
            If c = "v" Then
                KeyDownUp(Keys.V, False, 1, False)
            End If
            If c = "w" Then
                KeyDownUp(Keys.W, False, 1, False)
            End If
            If c = "x" Then
                KeyDownUp(Keys.X, False, 1, False)
            End If
            If c = "y" Then
                KeyDownUp(Keys.Y, False, 1, False)
            End If
            If c = "z" Then
                KeyDownUp(Keys.Z, False, 1, False)
            End If
        Next

        'colon
        KeyDownOnly(Keys.ShiftKey, False, 1, False)
        KeyDownUp(Keys.OemSemicolon, False, 1, False)
        KeyUpOnly(Keys.ShiftKey, False, 1, False)

        'port
        For Each c As Char In port
            If c = " " Then
                KeyDownUp(Keys.Space, False, 1, False)
            End If
            If c = "." Then
                KeyDownUp(Keys.OemPeriod, False, 1, False)
            End If
            If c = "," Then
                KeyDownUp(Keys.Oemcomma, False, 1, False)
            End If

            If c = "0" Then
                KeyDownUp(Keys.NumPad0, False, 1, False)
            End If
            If c = "1" Then
                KeyDownUp(Keys.NumPad1, False, 1, False)
            End If
            If c = "2" Then
                KeyDownUp(Keys.NumPad2, False, 1, False)
            End If
            If c = "3" Then
                KeyDownUp(Keys.NumPad3, False, 1, False)
            End If
            If c = "4" Then
                KeyDownUp(Keys.NumPad4, False, 1, False)
            End If
            If c = "5" Then
                KeyDownUp(Keys.NumPad5, False, 1, False)
            End If
            If c = "6" Then
                KeyDownUp(Keys.NumPad6, False, 1, False)
            End If
            If c = "7" Then
                KeyDownUp(Keys.NumPad7, False, 1, False)
            End If
            If c = "8" Then
                KeyDownUp(Keys.NumPad8, False, 1, False)
            End If
            If c = "9" Then
                KeyDownUp(Keys.NumPad9, False, 1, False)
            End If

            If c = "a" Then
                KeyDownUp(Keys.A, False, 1, False)
            End If
            If c = "b" Then
                KeyDownUp(Keys.B, False, 1, False)
            End If
            If c = "c" Then
                KeyDownUp(Keys.C, False, 1, False)
            End If
            If c = "d" Then
                KeyDownUp(Keys.D, False, 1, False)
            End If
            If c = "e" Then
                KeyDownUp(Keys.E, False, 1, False)
            End If
            If c = "f" Then
                KeyDownUp(Keys.F, False, 1, False)
            End If
            If c = "g" Then
                KeyDownUp(Keys.G, False, 1, False)
            End If
            If c = "h" Then
                KeyDownUp(Keys.H, False, 1, False)
            End If
            If c = "i" Then
                KeyDownUp(Keys.I, False, 1, False)
            End If
            If c = "j" Then
                KeyDownUp(Keys.J, False, 1, False)
            End If
            If c = "k" Then
                KeyDownUp(Keys.K, False, 1, False)
            End If
            If c = "l" Then
                KeyDownUp(Keys.L, False, 1, False)
            End If
            If c = "m" Then
                KeyDownUp(Keys.M, False, 1, False)
            End If
            If c = "n" Then
                KeyDownUp(Keys.N, False, 1, False)
            End If
            If c = "o" Then
                KeyDownUp(Keys.O, False, 1, False)
            End If
            If c = "p" Then
                KeyDownUp(Keys.P, False, 1, False)
            End If
            If c = "q" Then
                KeyDownUp(Keys.Q, False, 1, False)
            End If
            If c = "r" Then
                KeyDownUp(Keys.R, False, 1, False)
            End If
            If c = "s" Then
                KeyDownUp(Keys.S, False, 1, False)
            End If
            If c = "t" Then
                KeyDownUp(Keys.T, False, 1, False)
            End If
            If c = "u" Then
                KeyDownUp(Keys.U, False, 1, False)
            End If
            If c = "v" Then
                KeyDownUp(Keys.V, False, 1, False)
            End If
            If c = "w" Then
                KeyDownUp(Keys.W, False, 1, False)
            End If
            If c = "x" Then
                KeyDownUp(Keys.X, False, 1, False)
            End If
            If c = "y" Then
                KeyDownUp(Keys.Y, False, 1, False)
            End If
            If c = "z" Then
                KeyDownUp(Keys.Z, False, 1, False)
            End If
        Next

        Debug.Print("connecting to server")
        'change db state
        SQL.SqlUpdate("update [game-bots].[dbo].[rust-bot-state] Set [orderState] = 'connectingtomap' where [orderNumber] = '" & orderNumber & "'", "game-bots", "rust-bot-state")

        'connect
        KeyDownUp(Keys.Enter, False, 1, False)
        ResponsiveSleep(250)
        'close console
        KeyDownUp(Keys.F1, False, 1, False)
        ResponsiveSleep(250)

        Debug.Print("waiting for map to load")

        'wait for game map load
        ResponsiveSleep(Constants.rustConnectServerGameWaitMilli)

        Debug.Print("map should be loaded, executing mode = " & mode)

        'wakeup
        LeftMouseClick()
        ResponsiveSleep(500)

        'what command
        If mode.Equals("harass") Then
            SQL.SqlUpdate("update [game-bots].[dbo].[rust-bot-state] Set [orderState] = 'running' where [orderNumber] = '" & orderNumber & "'", "game-bots", "rust-bot-state")

            Form1.GoHome(True)
        End If
    End Function
End Module
