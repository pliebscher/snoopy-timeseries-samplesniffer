Imports System.Runtime.InteropServices
Public Class FormSettings

    Private Sub FormSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        cbSampleRate.SelectedItem = App.SampleRate.ToString
        cbSampleSize.SelectedItem = App.SampleSize.ToString

        If WaveNative.waveInGetNumDevs() = 0 Then
            cbInputDevice.Items.Add("[No Devices]")
            cbInputDevice.SelectedIndex = 0
            cbInputDevice.Enabled = False
            cbSampleRate.Enabled = False
            cbSampleSize.Enabled = False
            Exit Sub
        End If

        cbInputDevice.Items.Add("[Default]")

        For i As Integer = 0 To WaveNative.waveInGetNumDevs() - 1

            Dim dev As New WaveNative.WaveInCaps
            WaveNative.waveInGetDevCaps(i, dev, CUInt(Marshal.SizeOf(dev)))

            cbInputDevice.Items.Add(dev.szPname)

        Next

        cbInputDevice.SelectedIndex = App.AudioInputDevice + 1 ' (Add 1 to offset the default of -1)

    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

        App.SampleRate = CInt(cbSampleRate.SelectedItem)
        App.SampleSize = CInt(cbSampleSize.SelectedItem)
        App.AudioInputDevice = cbInputDevice.SelectedIndex - 1

        Me.Dispose()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Dispose()
    End Sub

End Class