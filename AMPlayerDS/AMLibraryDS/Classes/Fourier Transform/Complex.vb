Public Class Complex

    Public re As Double
    Public im As Double

    Public Sub New()
        re = 0
        im = 0
    End Sub

    Public Shared Operator +(ByVal left As Complex, ByVal right As Complex) As Complex
        Dim result As New Complex

        result.re = left.re + right.re
        result.im = left.im + right.im

        Return result
    End Operator

    Public Shared Operator -(ByVal left As Complex, ByVal right As Complex) As Complex
        Dim result As New Complex

        result.re = left.re - right.re
        result.im = left.im - right.im

        Return result
    End Operator

    Public Shared Operator *(ByVal left As Complex, ByVal right As Complex) As Complex
        Dim result As New Complex

        result.re = (left.re * right.re) - (left.im * right.im)
        result.im = (left.im * right.re) + (left.re * right.im)

        Return result
    End Operator

    Public Shared Operator /(ByVal left As Complex, ByVal right As Complex) As Complex
        Dim result As New Complex

        result.re = (left.re * right.re) + (left.im * right.im)
        result.re = result.re / ((right.re * right.re) + (right.im * right.im))

        result.im = (right.re * left.im) - (left.re * right.im)
        result.im = result.im / ((right.re * right.re) + (right.im * right.im))

        Return result
    End Operator
End Class
