Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports TicTacToe
Module Module1

    Public relays As New List(Of Relay)

    <Extension()>
    Public Function CopyContent(obj1 As Object, obj2 As Object)
        Dim t1 As Type = obj1.GetType
        Dim t2 As Type = obj2.GetType
        Dim info1() As PropertyInfo = t1.GetProperties()
        Dim info2() As PropertyInfo = t2.GetProperties()
        For index = 0 To info2.Length - 1
            info1(indexOfProperty(info1, info2(index).Name)).SetValue(obj1, info2(index).GetValue(obj2))
        Next
        Return obj1
    End Function

    <Extension()>
    Public Function indexOfProperty(info() As PropertyInfo, prop As String) As Integer
        Dim i As Integer = 0
        For Each p In info
            If (p.Name.ToLower().Equals(prop.ToLower())) Then
                Return i
            Else
                i += 1
            End If
        Next
        Return -1
    End Function

    <Extension()>
    Public Function GetValue(obj As Object, prop As String)
        Dim t1 As Type = obj.GetType
        Dim info1() As PropertyInfo = t1.GetProperties()
        Return info1(indexOfProperty(info1, prop)).GetValue(obj)
    End Function

    <Extension()>
    Sub SetValue(obj As Object, prop As String, value As Object)
        Dim t1 As Type = obj.GetType
        Dim info1() As PropertyInfo = t1.GetProperties()
        Dim propType As Type = info1(indexOfProperty(info1, prop)).PropertyType
        value = Convert.ChangeType(value, propType)
        info1(indexOfProperty(info1, prop)).SetValue(obj, value)
    End Sub

    <Extension()>
    Function Random(l As List(Of String)) As String
        Randomize()
        If l.Count = 0 Then
            Return Nothing
        End If
        Return l(Math.Floor(Rnd() * l.Count))
    End Function

    <Extension()>
    Function Clone(obj As Object)
        Dim t1 As Type = obj.GetType
        Dim info1() As PropertyInfo = t1.GetProperties()
        Dim obj1 As New Object()
        obj1 = Convert.ChangeType(obj, t1)
        Dim info2() As PropertyInfo = obj1.GetType().GetProperties()
        For index = 0 To info1.Length - 1
            info2(indexOfProperty(info1, info1(index).Name)).SetValue(obj1, info1(index).GetValue(obj))
        Next
        Return obj1
    End Function

    <Extension()>
    Function ToBytes(obj As Object) As Byte()
        Dim ms As New System.IO.MemoryStream()
        Dim bin As New Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        bin.Serialize(ms, obj)
        Return ms.ToArray()
    End Function

    <Extension()>
    Function ToJson(obj As Object) As String
        Return Newtonsoft.Json.JsonConvert.SerializeObject(obj)
    End Function

    Public Function ToXElement(obj As Object, Optional obj_type As Type = Nothing)
        If obj_type Is Nothing Then
            obj_type = obj.GetType()
        End If
        Dim xs As New XmlSerializer(obj_type)
        Using ms As MemoryStream = New MemoryStream()
            xs.Serialize(ms, obj)
            ms.Position = 0
            Using read As XmlReader = XmlReader.Create(ms)
                Dim element As XElement = XElement.Load(read)
                Return element
            End Using
        End Using
    End Function

    <Extension()>
    Sub SaveToFile(arr() As Byte, f As String)
        Dim fs As New System.IO.FileStream(f, IO.FileMode.CreateNew)
        fs.Write(arr, 0, arr.Length)
        fs.Flush()
        fs.Close()
    End Sub

    <Extension()>
    Sub SaveToFile(txt As String, f As String)
        My.Computer.FileSystem.WriteAllText(f, txt, False)
    End Sub

    <Extension()>
    Public Function SortBy(arr, f)
        Dim swapped As Boolean = True
        Do While swapped
            swapped = False

            For i = 0 To arr.Count() - 2
                Dim b = arr(i).GetValue(f) > arr(i + 1).GetValue(f)
                If b Then
                    Dim temp = arr(i)
                    arr(i) = arr(i + 1)
                    arr(i + 1) = temp
                    swapped = True
                End If
            Next
        Loop
        Return arr
    End Function

    Function GetRandomString(Optional count As Integer = 64) As String
        Randomize()
        Dim ret As String = ""
        For index = 1 To count
            Dim r = Rnd()
            If r <= 0.92 Then
                ret &= Char.ConvertFromUtf32(97 + Math.Floor(r * 26)).ToString()
            Else
                ret &= " "
            End If
        Next
        Return ret
    End Function
End Module
