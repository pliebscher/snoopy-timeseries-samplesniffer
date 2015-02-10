Imports System.ComponentModel

Public Class EnumIntValueConverter
    Inherits EnumConverter

    Private _Type As Type

    Public Sub New(enumType As Type)
        MyBase.New(enumType)
        _Type = enumType
    End Sub

    Public Overrides Function CanConvertTo(context As ITypeDescriptorContext, destinationType As Type) As Boolean
        Return destinationType = GetType(String)
    End Function

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object, destinationType As Type) As Object
        Return CInt(value).ToString
    End Function

    Public Overrides Function CanConvertFrom(context As ITypeDescriptorContext, sourceType As Type) As Boolean
        Return sourceType = GetType(String)
    End Function

    Public Overrides Function ConvertFrom(context As ITypeDescriptorContext, culture As Globalization.CultureInfo, value As Object) As Object
        Return [Enum].Parse(_Type, value.ToString)
    End Function

End Class
