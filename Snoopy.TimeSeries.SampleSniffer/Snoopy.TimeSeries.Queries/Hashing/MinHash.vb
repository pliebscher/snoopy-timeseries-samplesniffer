Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Diagnostics
Namespace Hashing
    ''' <summary>
    ''' http://blogs.msdn.com/b/spt/archive/2008/06/10/set-similarity-and-min-hash.aspx
    ''' </summary>
    ''' <remarks></remarks>
    Class MinHash

        Private Delegate Function Hash(index As Integer) As Integer
        Private _NumHashFunctions As Integer = 32 ' Default
        Private _HashFunctions As Hash()

        Public Sub New(universeSize As Integer, hashFunctions As Integer)
            _NumHashFunctions = hashFunctions

            _HashFunctions = New Hash(_NumHashFunctions - 1) {}

            Dim r As New Random(11)
            For i As Integer = 0 To _NumHashFunctions - 1
                Dim a As UInteger = CUInt(r.[Next](universeSize))
                Dim b As UInteger = CUInt(r.[Next](universeSize))
                Dim c As UInteger = CUInt(r.[Next](universeSize))
                _HashFunctions(i) = Function(x) QHash(CUInt(x), a, b, c)
            Next

        End Sub

        Public Function Similarity(Of T)(set1 As HashSet(Of T), set2 As HashSet(Of T)) As Double

            Dim numSets As Integer = 2
            Dim bitMap As Dictionary(Of T, Boolean()) = BuildBitMap(set1, set2)

            Dim minHashValues As Integer(,) = GetMinHashSlots(numSets, _NumHashFunctions)

            ComputeMinHashForSet(set1, 0, minHashValues, bitMap)
            ComputeMinHashForSet(set2, 1, minHashValues, bitMap)

            Dim sim As Double = ComputeSimilarityFromSignatures(minHashValues, _NumHashFunctions)

            Return sim
        End Function

        Private Sub ComputeMinHashForSet(Of T)([set] As HashSet(Of T), setIndex As Short, minHashValues As Integer(,), bitArray As Dictionary(Of T, Boolean()))
            Dim index As Integer = 0
            For Each element As T In bitArray.Keys
                For i As Integer = 0 To _NumHashFunctions - 1
                    If [set].Contains(element) Then
                        Dim hindex As Integer = _HashFunctions(i)(index)

                        If hindex < minHashValues(setIndex, i) Then
                            minHashValues(setIndex, i) = hindex
                        End If
                    End If
                Next
                index += 1
            Next
        End Sub

        Private Function GetMinHashSlots(numSets As Integer, numHashFunctions As Integer) As Integer(,)
            Dim minHashValues As Integer(,) = New Integer(numSets - 1, numHashFunctions - 1) {}

            For i As Integer = 0 To numSets - 1
                For j As Integer = 0 To numHashFunctions - 1
                    minHashValues(i, j) = Int32.MaxValue
                Next
            Next
            Return minHashValues
        End Function

        Private Function QHash(x As UInteger, a As UInteger, b As UInteger, c As UInteger) As Integer
            'Modify the hash family as per the size of possible elements in a Set
            Dim hashValue As Integer = CInt((a * (x >> 4) + b * x + c) And 131071)
            Return Math.Abs(hashValue)
        End Function

        Private Function BuildBitMap(Of T)(set1 As HashSet(Of T), set2 As HashSet(Of T)) As Dictionary(Of T, Boolean())
            Dim bitArray As New Dictionary(Of T, Boolean())()
            For Each item As T In set1
                bitArray.Add(item, New Boolean(1) {True, False})
            Next

            For Each item As T In set2
                Dim value As Boolean() = Nothing ' Being passed ByRef below. Nothing suppresses compiler warning.
                If bitArray.TryGetValue(item, value) Then
                    'item is present in set1
                    bitArray(item) = New Boolean(1) {True, True}
                Else
                    'item is not present in set1
                    bitArray.Add(item, New Boolean(1) {False, True})
                End If
            Next
            Return bitArray
        End Function

        Private Function ComputeSimilarityFromSignatures(minHashValues As Integer(,), numHashFunctions As Integer) As Double
            Dim identicalMinHashes As Integer = 0
            For i As Integer = 0 To numHashFunctions - 1
                If minHashValues(0, i) = minHashValues(1, i) Then
                    identicalMinHashes += 1
                End If
            Next
            Return (1.0 * identicalMinHashes) / numHashFunctions
        End Function

    End Class
End Namespace