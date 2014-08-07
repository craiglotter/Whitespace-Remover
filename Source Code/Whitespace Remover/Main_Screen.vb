Imports System
Imports System.IO
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms


Public Class Main_Screen


    Private lastinputline As String = ""
    Private inputlines As Long = 0
    Private statusmessage As String = ""
    Private statusresults As String = ""
    Private highestPercentageReached As Integer = 0
    Private inputlinesprecount As Long = 0
    Private datelaunched As Date = Now()
    Private pretestdone As Boolean = False
    
    Private whichbuttonpressed As Long = 1

    Private inputsuccess As String = ""



    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                If FullErrors_Checkbox.Checked = True Then
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.ToString
                Else
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.Message.ToString
                End If
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
            End If
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub


    Private Sub Activity_Handler(ByVal Message As String)
        Try
            Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs")
            If dir.Exists = False Then
                dir.Create()
            End If
            dir = Nothing
            Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs\" & Format(Now(), "yyyyMMdd") & "_Activity_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & Message)
            filewriter.Flush()
            filewriter.Close()
            filewriter = Nothing
        Catch ex As Exception
            Error_Handler(ex, "Activity_Logger")
        End Try
    End Sub

    Private Sub Status_Handler(ByVal Message As String)
        Try
            Status_Textbox.Text = Message.ToUpper
            Status_Textbox.Select(0, 0)
        Catch ex As Exception
            Error_Handler(ex, "Status_Handler")
        End Try
    End Sub


    Private Sub Status_Results_Handler(ByVal Message As String)
        Try
            If Message.Length > 0 Then
                If StatusResults_RichtextBox.TextLength + Message.Length >= StatusResults_RichtextBox.MaxLength Then
                    StatusResults_RichtextBox.Clear()
                End If
                StatusResults_RichtextBox.AppendText(Message & vbCrLf)
                Status_Textbox.Select(StatusResults_RichtextBox.Text.Length - 1, 0)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Status_Results_Handler")
        End Try
    End Sub



    Private Sub Browse1_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Browse1_Button.Click
        Status_Handler("Selecting Input File")
        Try
            OpenFileDialog1.Filter = "Text Files|*.txt|All Files|*.*"
            OpenFileDialog1.FileName = ""
            If InputTargetFileType_Textbox.Text = "1" Then
                If File_Exists(InputTargetFile_Textbox.Text) = True Then
                    OpenFileDialog1.FileName = InputTargetFile_Textbox.Text
                End If
            End If

            Dim result As DialogResult = OpenFileDialog1.ShowDialog()
            If result = Windows.Forms.DialogResult.OK Then
                InputTargetFile_Textbox.Text = OpenFileDialog1.FileName
            End If
            InputTargetFileType_Textbox.Text = "1"
        Catch ex As Exception
            Error_Handler(ex, "Browse1_Button_Click")
        End Try
        Status_Handler("Input File Selected")
    End Sub



    Private Function File_Exists(ByVal file_path As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not file_path = "" And Not file_path Is Nothing Then
                Dim dinfo As FileInfo = New FileInfo(file_path)
                If dinfo.Exists = False Then
                    result = False
                Else
                    result = True
                End If
                dinfo = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "File_Exists")
        End Try
        Return result
    End Function

    Private Function Directory_Exists(ByVal directory_path As String) As Boolean
        Dim result As Boolean = False
        Try
            If Not directory_path = "" And Not directory_path Is Nothing Then
                Dim dinfo As DirectoryInfo = New DirectoryInfo(directory_path)
                If dinfo.Exists = False Then
                    result = False
                Else
                    result = True
                End If
                dinfo = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "Directory_Exists")
        End Try
        Return result
    End Function


    Private Sub Main_Screen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Label1.Text = System.String.Format(Label1.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)
            InputTargetFile_Textbox.Text = My.Settings.InputTargetFile_Textbox
            InputTargetFile_Textbox.Select(0, 0)

            'InputTargetPattern_Textbox.Text = My.Settings.InputTargetPattern_Textbox
            'InputTargetPattern_Textbox.Select(0, 0)



            Select Case My.Settings.FullErrors_Checkbox
                Case True
                    FullErrors_Checkbox.Checked = True
                    Exit Select
                Case False
                    FullErrors_Checkbox.Checked = False
                    Exit Select
                Case Else
                    FullErrors_Checkbox.Checked = True
                    Exit Select
            End Select


        Catch ex As Exception
            Error_Handler(ex, "Main_Screen_Load")
        End Try
    End Sub

    Private Sub Main_Screen_Close(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try

            My.Settings.FullErrors_Checkbox = FullErrors_Checkbox.Checked
            My.Settings.InputTargetFile_Textbox = InputTargetFile_Textbox.Text
            'My.Settings.InputTargetPattern_Textbox = InputTargetPattern_Textbox.Text

            My.Settings.Save()
        Catch ex As Exception
            Error_Handler(ex, "Main_Screen_Close")
        End Try

    End Sub





    Private Sub FullErrors_Checkbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FullErrors_Checkbox.CheckedChanged
        Status_Handler("Error Level Reporting Altered")
    End Sub




    Private Sub startAsyncButton1_Click(ByVal sender As System.Object, _
     ByVal e As System.EventArgs) _
     Handles startAsyncButton1.Click
        Try
            whichbuttonpressed = 1
            StartWorker()
        Catch ex As Exception
            Error_Handler(ex, "startAsyncButton2_Click")
        End Try
    End Sub


    Private Sub StartWorker()
        Try
            statusmessage = "Initializing Application for Operation Launch"
            Status_Handler(statusmessage)
            StatusResults_RichtextBox.Clear()

            ' Reset the text in the result label.

            inputsuccess_label.Text = [String].Empty
            inputlines_label.Text = [String].Empty
            lastinputline_label.Text = [String].Empty
            datelaunched_label.Text = [String].Empty




            inputlines = 0
            inputsuccess = 0
            lastinputline = ""
            statusmessage = ""
            highestPercentageReached = 0
            inputlinesprecount = 0
            datelaunched = Now()
            pretestdone = False


            Controls_Enabler("run")


            ' Start the asynchronous operation.
            Me.LinkLabel1.Visible = True
            BackgroundWorker1.RunWorkerAsync("")

        Catch ex As Exception
            Error_Handler(ex, "StartWorker")
        End Try
    End Sub 'startAsyncButton_Click




    Private Sub cancelAsyncButton_Click( _
    ByVal sender As System.Object, _
    ByVal e As System.EventArgs) _
    Handles cancelAsyncButton.Click

        ' Cancel the asynchronous operation.
        Me.BackgroundWorker1.CancelAsync()

        ' Disable the Cancel button.
        cancelAsyncButton.Enabled = False

    End Sub 'cancelAsyncButton_Click

    ' This event handler is where the actual work is done.
    Private Sub backgroundWorker1_DoWork( _
    ByVal sender As Object, _
    ByVal e As DoWorkEventArgs) _
    Handles BackgroundWorker1.DoWork

        ' Get the BackgroundWorker object that raised this event.
        Dim worker As BackgroundWorker = _
            CType(sender, BackgroundWorker)

        ' Assign the result of the computation
        ' to the Result property of the DoWorkEventArgs
        ' object. This is will be available to the 
        ' RunWorkerCompleted eventhandler.
        e.Result = MainWorkerFunction(worker, e)
    End Sub 'backgroundWorker1_DoWork

    ' This event handler deals with the results of the
    ' background operation.
    Private Sub backgroundWorker1_RunWorkerCompleted( _
    ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) _
    Handles BackgroundWorker1.RunWorkerCompleted

        ' First, handle the case where an exception was thrown.
        If Not (e.Error Is Nothing) Then
            Error_Handler(e.Error, "backgroundWorker1_RunWorkerCompleted")
        ElseIf e.Cancelled Then
            ' Next, handle the case where the user canceled the 
            ' operation.
            ' Note that due to a race condition in 
            ' the DoWork event handler, the Cancelled
            ' flag may not have been set, even though
            ' CancelAsync was called.
            Me.ProgressBar1.Value = 0
            inputlines_label.Text = "Cancelled"
            inputsuccess_label.Text = "Cancelled"
            lastinputline_label.Text = "Cancelled"
            datelaunched_label.Text = "Cancelled"
            statusmessage = "Operation Cancelled"
        Else
            ' Finally, handle the case where the operation succeeded.

            Status_Handler(e.Result)

            Me.ProgressBar1.Value = 100
            Me.inputsuccess_label.Text = inputsuccess
            Me.inputlines_label.Text = inputlines
            Me.lastinputline_label.Text = lastinputline
            Me.datelaunched_label.Text = Format(datelaunched, "HH:mm:ss") & " - " & Format(Now, "HH:mm:ss") & " (" & Now.Subtract(Me.datelaunched).TotalSeconds() & " s)"
            Me.LinkLabel1.Visible = True
            statusmessage = "Operation Completed"
        End If
        Final_File_Operations()
        Status_Handler(statusmessage)
        Controls_Enabler("stop")

    End Sub 'backgroundWorker1_RunWorkerCompleted

    Private Sub Final_File_Operations()
        Try
            If File_Exists(InputTargetFile_Textbox.Text & "_XTEMP") = True Then
                If MsgBox("Process Complete. Do you wish to replace your existing text file with the newly created file?", MsgBoxStyle.YesNo, "Process Complete") = MsgBoxResult.Yes Then
                    Try
                        My.Computer.FileSystem.DeleteFile(InputTargetFile_Textbox.Text, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                        My.Computer.FileSystem.MoveFile(InputTargetFile_Textbox.Text & "_XTEMP", InputTargetFile_Textbox.Text)
                    Catch ex As Exception
                        Error_Handler(ex, "Final_File_Operations")
                    End Try
                Else
                    If MsgBox("Do you wish to keep the temporary file (" & InputTargetFile_Textbox.Text & "_XTEMP" & ") that was generated in the process?", MsgBoxStyle.YesNo, "Process Complete") = MsgBoxResult.No Then
                        Try
                            My.Computer.FileSystem.DeleteFile(InputTargetFile_Textbox.Text & "_XTEMP", FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                        Catch ex As Exception
                            Error_Handler(ex, "Final_File_Operations")
                        End Try
                    End If
                End If
            End If


        Catch ex As Exception
            Error_Handler(ex, "Final_File_Operations")
        End Try
    End Sub

    Private Sub Controls_Enabler(ByVal action As String)
        Select Case action.ToLower
            Case "run"
                Me.InputTargetFile_Textbox.Enabled = False
                'Me.InputTargetPattern_Textbox.Enabled = False
                Me.InputTargetFileType_Textbox.Enabled = False


                Me.Browse1_Button.Enabled = False

                Me.startAsyncButton1.Enabled = False
                Me.LinkLabel1.Enabled = False
                ' Disable the Cancel button.
                Me.cancelAsyncButton.Enabled = True
                Exit Select
            Case "stop"
                Me.InputTargetFile_Textbox.Enabled = True
                'Me.InputTargetPattern_Textbox.Enabled = True
                Me.InputTargetFileType_Textbox.Enabled = True


                Me.Browse1_Button.Enabled = True

                Me.startAsyncButton1.Enabled = True
                Me.LinkLabel1.Enabled = True
                ' Disable the Cancel button.
                Me.cancelAsyncButton.Enabled = False
                Exit Select
            Case Else
                Me.InputTargetFile_Textbox.Enabled = False
                'Me.InputTargetPattern_Textbox.Enabled = False
                Me.InputTargetFileType_Textbox.Enabled = False


                Me.Browse1_Button.Enabled = False

                Me.startAsyncButton1.Enabled = False
                Me.LinkLabel1.Enabled = False
                ' Disable the Cancel button.
                Me.cancelAsyncButton.Enabled = True
                Exit Select
        End Select


    End Sub

    ' This event handler updates the progress bar.
    Private Sub backgroundWorker1_ProgressChanged( _
    ByVal sender As Object, ByVal e As ProgressChangedEventArgs) _
    Handles BackgroundWorker1.ProgressChanged

        Me.ProgressBar1.Value = e.ProgressPercentage
        inputlines_label.Text = inputlines
        inputsuccess_label.Text = inputsuccess
        lastinputline_label.Text = lastinputline

        datelaunched_label.Text = Format(datelaunched, "HH:mm:ss") & " - " & Format(Now, "HH:mm:ss") & " (" & Now.Subtract(Me.datelaunched).TotalSeconds() & " s)"

        If statusresults.Length > 0 Then
            Status_Results_Handler(statusresults.Trim)
            statusresults = ""
        End If
        If statusmessage.Length > 0 Then
            Status_Handler(statusmessage)
        End If
        statusresults = ""
        statusmessage = ""
    End Sub

    ' This is the method that does the actual work. 
    Function MainWorkerFunction(ByVal worker As BackgroundWorker, ByVal e As DoWorkEventArgs) As String


        Dim result As String = ""
        If File_Exists(InputTargetFile_Textbox.Text & "_XTEMP") = True Then
            My.Computer.FileSystem.DeleteFile(InputTargetFile_Textbox.Text & "_XTEMP", FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        End If



        Try
            ' Abort the operation if the user has canceled.
            ' Note that a call to CancelAsync may have set 
            ' CancellationPending to true just after the
            ' last invocation of this method exits, so this 
            ' code will not have the opportunity to set the 
            ' DoWorkEventArgs.Cancel flag to true. This means
            ' that RunWorkerCompletedEventArgs.Cancelled will
            ' not be set to true in your RunWorkerCompleted
            ' event handler. This is a race condition.
            If worker.CancellationPending Then
                e.Cancel = True
            End If

            If Me.pretestdone = False Then
                statusmessage = "Calculating Operation Parameters"
                worker.ReportProgress(0)
                PreCount_Function()
                Me.pretestdone = True

            End If

            If worker.CancellationPending Then
                e.Cancel = True
            End If

            statusmessage = "Beginning Operation"
            worker.ReportProgress(0)

            Activity_Handler("Parsing: " & InputTargetFile_Textbox.Text)

            Dim finfo As FileInfo = New FileInfo(InputTargetFile_Textbox.Text)
            If finfo.Exists = True Then
                Dim reader As StreamReader = New StreamReader(InputTargetFile_Textbox.Text)
                Dim writer As StreamWriter = New StreamWriter(InputTargetFile_Textbox.Text & "_XTEMP", True)
                Dim lineread As String = ""
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    If whichbuttonpressed = 1 Then
                        ''If lineread.Length >= NumericUpDown1.Value Then
                        ''    If RadioButton1.Checked = True Then
                        lineread = lineread.Trim()
                        statusresults = statusresults & "Writing: " & lineread & vbCrLf
                        ''Activity_Handler("Writing: " & lineread)
                        inputsuccess = inputsuccess + 1
                        writer.WriteLine(lineread)
                        ''    End If
                        ''    If RadioButton2.Checked = True Then
                        ''        lineread = lineread.Remove(lineread.Length - NumericUpDown1.Value, NumericUpDown1.Value)
                        ''        statusresults = statusresults & "Pass - Writing: " & lineread & vbCrLf
                        ''        Activity_Handler("Pass - Writing: " & lineread)
                        ''        inputsuccess = inputsuccess + 1
                        ''        writer.WriteLine(lineread)
                        ''    End If
                        ''    If RadioButton3.Checked = True Then
                        ''        If ((NumericUpDown2.Value + NumericUpDown1.Value) <= lineread.Length) Then
                        ''            lineread = lineread.Remove(NumericUpDown2.Value - 1, NumericUpDown1.Value)
                        ''            statusresults = statusresults & "Pass - Writing: " & lineread & vbCrLf
                        ''            Activity_Handler("Pass - Writing: " & lineread)
                        ''            inputsuccess = inputsuccess + 1
                        ''            writer.WriteLine(lineread)
                        ''        Else
                        ''            statusresults = statusresults & "Fail - Ignoring: " & lineread & vbCrLf
                        ''            Activity_Handler("Fail - Ignoring: " & lineread)
                        ''        End If
                        ''    End If

                        ''Else
                        ''    statusresults = statusresults & "Fail - Ignoring: " & lineread & vbCrLf
                        ''    Activity_Handler("Fail - Ignoring: " & lineread)
                        ''End If

                    Else
                        'If lineread.IndexOf(InputTargetPattern_Textbox.Text) = -1 Then
                        '    statusresults = statusresults & "Pass - Writing: " & lineread & vbCrLf
                        '    Activity_Handler("Pass - Writing: " & lineread)
                        '    inputsuccess = inputsuccess + 1
                        '    writer.WriteLine(lineread)
                        'Else
                        '    statusresults = statusresults & "Fail - Ignoring: " & lineread & vbCrLf
                        '    Activity_Handler("Fail - Ignoring: " & lineread)
                        'End If
                    End If



                    lastinputline = lineread
                    inputlines = inputlines + 1






                    ' Report progress as a percentage of the total task.
                    Dim percentComplete As Integer = 0
                    If inputlinesprecount > 0 Then
                        percentComplete = CSng(inputlines) / CSng(inputlinesprecount) * 100
                    Else
                        percentComplete = 100
                    End If

                    If percentComplete > highestPercentageReached Then
                        highestPercentageReached = percentComplete
                        statusmessage = "Parsing Input File"
                        worker.ReportProgress(percentComplete)
                    End If
                End While
                writer.Flush()
                writer.Close()
                writer.Dispose()
                reader.Close()
                reader.Dispose()
            End If
            finfo = Nothing





        Catch ex As Exception
            Error_Handler(ex, "MainWorkerFunction")
        End Try
        Return result
    End Function

    Private Sub PreCount_Function()
        Try
            Dim targetcount As Long = 1
            Dim finfo As FileInfo = New FileInfo(InputTargetFile_Textbox.Text)
            If finfo.Exists = True Then
                Dim reader As StreamReader = New StreamReader(InputTargetFile_Textbox.Text)

                Dim lineread As String = ""
                While reader.Peek <> -1
                    lineread = reader.ReadLine
                    targetcount = targetcount + 1
                End While
                reader.Close()
                reader.Dispose()
            End If
            finfo = Nothing

            inputlinesprecount = targetcount


        Catch ex As Exception
            Error_Handler(ex, "PreCount_Function")
        End Try
    End Sub






    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            If File_Exists((Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs\" & Format(Now(), "yyyyMMdd") & "_Activity_Log.txt") = True Then
                Dim systemDirectory As String
                systemDirectory = System.Environment.SystemDirectory
                Dim finfo As FileInfo = New FileInfo((systemDirectory & "\notepad.exe").Replace("\\", "\"))
                If finfo.Exists = True Then
                    Dim apptorun As String
                    apptorun = """" & (systemDirectory & "\notepad.exe").Replace("\\", "\") & """ """ & (Application.StartupPath & "\").Replace("\\", "\") & "Activity Logs\" & Format(Now(), "yyyyMMdd") & "_Activity_Log.txt" & """"
                    Dim procID As Integer = Shell(apptorun, AppWinStyle.NormalFocus, False)
                End If
                finfo = Nothing
            End If
        Catch ex As Exception
            Error_Handler(ex, "LinkLabel1_LinkClicked")
        End Try
    End Sub










    Private Function DosShellCommand(ByVal AppToRun As String) As String
        Dim s As String = ""
        Try
            Dim myProcess As Process = New Process

            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.UseShellExecute = False


            Dim sErr As StreamReader
            Dim sOut As StreamReader
            Dim sIn As StreamWriter


            myProcess.StartInfo.CreateNoWindow = True

            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            'myProcess.StartInfo.FileName = AppToRun

            myProcess.Start()
            sIn = myProcess.StandardInput
            sIn.AutoFlush = True

            sOut = myProcess.StandardOutput()
            sErr = myProcess.StandardError

            sIn.Write(AppToRun & System.Environment.NewLine)
            sIn.Write("exit" & System.Environment.NewLine)
            s = sOut.ReadToEnd()

            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If



            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()


        Catch ex As Exception
            Error_Handler(ex, "DosShellCommand")
        End Try

        Return s
    End Function

    Private Function ApplicationLauncher(ByVal AppToRun As String, ByVal apptorunArgs As String) As String
        Dim s As String = ""
        Try
            Dim myProcess As Process = New Process


            myProcess.StartInfo.UseShellExecute = False


            Dim sErr As StreamReader
            Dim sOut As StreamReader
            Dim sIn As StreamWriter


            myProcess.StartInfo.CreateNoWindow = True

            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            myProcess.StartInfo.FileName = AppToRun
            myProcess.StartInfo.Arguments = apptorunArgs

            myProcess.Start()
            sIn = myProcess.StandardInput
            sIn.AutoFlush = True

            sOut = myProcess.StandardOutput()
            sErr = myProcess.StandardError

            sIn.Write(AppToRun & System.Environment.NewLine)
            sIn.Write("exit" & System.Environment.NewLine)
            s = sOut.ReadToEnd()

            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If

            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()


        Catch ex As Exception
            Error_Handler(ex, "ApplicationLauncher")
        End Try
        Return s
    End Function


    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        StatusResults_RichtextBox.Clear()
    End Sub


    Private Sub startAsyncButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            whichbuttonpressed = 2
            StartWorker()
        Catch ex As Exception
            Error_Handler(ex, "startAsyncButton2_Click")
        End Try
    End Sub



    Private Sub InputTargetFile_Textbox_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles InputTargetFile_Textbox.DragDrop
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim MyFiles() As String
                Dim i As Integer

                ' Assign the files to an array.
                MyFiles = e.Data.GetData(DataFormats.FileDrop)
                ' Loop through the array and add the files to the list.
                'For i = 0 To MyFiles.Length - 1
                If MyFiles.Length > 0 Then
                    Dim finfo As FileInfo = New FileInfo(MyFiles(0))
                    If finfo.Exists = True Then
                        InputTargetFile_Textbox.Text = (MyFiles(0))

                    End If
                End If
                'Next
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub InputTargetFile_Textbox_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles InputTargetFile_Textbox.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.All
            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

End Class
