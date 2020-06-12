' Class that implement circle buffer for decoders and encoders
Imports System.Runtime.InteropServices

Public Class CircleBuffer
    Implements IDisposable

    ' Used internally for operations
    Private Enum Operation
        Read = 0
        Write
    End Enum

    Private bMemoryAllocated As Boolean

    Private nBufferSize As Integer
    Private nReadIndex As Integer
    Private nWriteIndex As Integer
    Private nValidReadData As Integer
    Private nValidWriteData As Integer

    Private oLockObject As Object

    Private Buffer As IntPtr

    ' Construct
    Public Sub New()
        bMemoryAllocated = False
        nBufferSize = 0
        nReadIndex = 0
        nWriteIndex = 0
        nValidReadData = 0
        nValidWriteData = 0
    End Sub

    ' Construct
    Public Sub New(ByVal size As Integer)
        Me.New()
        Init(size)
    End Sub

    ''' <summary>
    ''' Init Circle buffer, allocate fixed memory
    ''' </summary>
    ''' <param name="size">Size in bytes of memory</param>
    Public Sub Init(ByVal size As Integer)
        If bMemoryAllocated = False Then
            If size > 0 Then

                ' Alloc unmanaged memory
                Buffer = Marshal.AllocHGlobal(size)
                GC.AddMemoryPressure(size)

                ' Initialize vars
                nBufferSize = size
                nReadIndex = 0
                nWriteIndex = 0
                nValidReadData = 0
                nValidWriteData = nBufferSize
                oLockObject = New Object

                bMemoryAllocated = True
            End If
        Else
            Close()
        End If
    End Sub

    ''' <summary>
    ''' Close Circular Buffer, Free memory
    ''' </summary>
    Public Sub Close()
        If bMemoryAllocated = True Then
            If Buffer <> IntPtr.Zero Then
                Marshal.FreeHGlobal(Buffer)
                GC.RemoveMemoryPressure(nBufferSize)
                oLockObject = Nothing
                bMemoryAllocated = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Reset indexs and get a full size buffer
    ''' </summary>
    Public Sub ClearMemory()
        If bMemoryAllocated = True Then
            nReadIndex = 0
            nWriteIndex = 0
            nValidReadData = 0
            nValidWriteData = nBufferSize
        End If
    End Sub

    ''' <summary>
    ''' Resize an opened Circle Buffer (Nb. Data loss!)
    ''' </summary>
    ''' <param name="size"></param>
    Public Sub ResizeMemory(ByVal size As Integer)
        If bMemoryAllocated = True Then
            ' Free allocated memory
            Marshal.FreeHGlobal(Buffer)
            GC.RemoveMemoryPressure(nBufferSize)

            ' Allocate new memory
            Buffer = Marshal.AllocHGlobal(size)
            GC.AddMemoryPressure(size)

            ' Update value
            nBufferSize = size

            ' Update indexs
            nReadIndex = 0
            nWriteIndex = 0
            nValidReadData = 0
            nValidWriteData = nBufferSize
        End If
    End Sub

    ''' <summary>
    ''' Write data in the buffer, ensure the size
    ''' is less then ValidWriteData property
    ''' or an exception occour
    ''' </summary>
    ''' <param name="array">Byte array of data</param>
    ''' <param name="size">Size to copy</param>
    Public Sub WriteData(ByRef array() As Byte, ByVal size As Integer)
        WriteReadArrayInMemory(array, size, Operation.Write)
    End Sub

    ''' <summary>
    ''' Read data from the buffer, ensure the size
    ''' is less then ValidReadData property
    ''' or an exception occour
    ''' </summary>
    ''' <param name="array">Byte array of data</param>
    ''' <param name="size">Size to copy</param>
    Public Sub ReadData(ByRef array() As Byte, ByVal size As Integer)
        WriteReadArrayInMemory(array, size, Operation.Read)
    End Sub

    ''' <summary>
    ''' Read data from the buffer, ensure the size
    ''' is less then ValidReadData property
    ''' or an exception occour
    ''' </summary>
    ''' <param name="size">Size to copy</param>
    ''' <returns>Byte array of data</returns>
    Public Function ReadData(ByVal size As Integer) As Byte()
        Dim array(size - 1) As Byte
        WriteReadArrayInMemory(array, size, Operation.Read)
        Return array
    End Function

    ''' <summary>
    ''' Get the max size can be readed
    ''' </summary>
    ''' <returns>An int</returns>
    Public ReadOnly Property ValidReadData As Integer
        Get
            Return nValidReadData
        End Get
    End Property

    ''' <summary>
    ''' Get the max size can be written
    ''' </summary>
    ''' <returns>An int</returns>
    Public ReadOnly Property ValidWriteData As Integer
        Get
            Return nValidWriteData
        End Get
    End Property

    ''' <summary>
    ''' Return Buffer size in bytes
    ''' </summary>
    ''' <returns>An int</returns>
    Public ReadOnly Property BufferSize As Integer
        Get
            Return nBufferSize
        End Get
    End Property

    ''' <summary>
    ''' Return the distance between WriteIndex and ReadIndex
    ''' </summary>
    ''' <returns>An int</returns>
    Public ReadOnly Property DistanceBetweenIndexs As Integer
        Get
            Return Math.Abs(nWriteIndex - nReadIndex)
        End Get
    End Property

    ' Used internally
    Private Sub WriteReadArrayInMemory(ByRef array() As Byte,
                                       ByVal size As Integer,
                                       ByVal op As Operation)

        ' If Not initialized exit
        If bMemoryAllocated = False Then Exit Sub ' <-- Exit
        If size < 0 Then Throw New ArgumentOutOfRangeException ' Error of size less than 0

        ' Thread safe (no concorence between thread)
        SyncLock oLockObject
            Select Case op
                Case Operation.Read
                    ' Check if nValidReadData memory is enough 
                    ' for read "size" bytes
                    If size <= nValidReadData Then
                        If nReadIndex + size <= nBufferSize Then

                            ' Copy entire size
                            Marshal.Copy(Buffer + nReadIndex,
                                         array,
                                         0,
                                         size)

                            nReadIndex = nReadIndex + size
                            nValidReadData = nValidReadData - size
                            nValidWriteData = nValidWriteData + size
                        Else
                            ' Copy first part  
                            ' From ReadIndex to BufferSize
                            Marshal.Copy(Buffer + nReadIndex,
                                         array,
                                         0,
                                         nBufferSize - nReadIndex)

                            ' Copy Second part 
                            ' From 0 to remaining data
                            Marshal.Copy(Buffer + 0,
                                         array,
                                         nBufferSize - nReadIndex,
                                         size - (nBufferSize - nReadIndex))

                            nReadIndex = 0 + size - (nBufferSize - nReadIndex)
                            nValidReadData = nValidReadData - size
                            nValidWriteData = nValidWriteData + size
                        End If
                    Else
                        Throw New Exception("The Size is more than valid read data avaiable")
                    End If
                Case Operation.Write

                    ' Check if nValidWriteData memory is enough 
                    ' for write "size" bytes
                    If size <= nValidWriteData Then
                        If nWriteIndex + size <= nBufferSize Then

                            ' Copy entire size
                            Marshal.Copy(array,
                                         0,
                                         Buffer + nWriteIndex,
                                         size)

                            nWriteIndex = nWriteIndex + size
                            nValidReadData = nValidReadData + size
                            nValidWriteData = nValidWriteData - size
                        Else
                            ' Copy first part  
                            ' From writeIndex to BufferSize
                            Marshal.Copy(array,
                                         0,
                                         Buffer + nWriteIndex,
                                         nBufferSize - nWriteIndex)


                            ' Copy Second part 
                            ' From 0 to remaining data
                            Marshal.Copy(array,
                                         nBufferSize - nWriteIndex,
                                         Buffer,
                                         size - (nBufferSize - nWriteIndex))


                            nWriteIndex = 0 + size - (nBufferSize - nWriteIndex)
                            nValidReadData = nValidReadData + size
                            nValidWriteData = nValidWriteData - size
                        End If
                    Else
                        Throw New Exception("The Size is more than valid write data space avaiable")
                    End If
            End Select
        End SyncLock

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
            End If

            Close()

        End If
        disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region






End Class
