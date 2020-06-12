Public Interface ISoundEffect

    Function Init() As Boolean
    Sub DeInit()

    Sub UpdateWaveFormat(ByVal wfx As WaveFormatEx)

    ReadOnly Property Name() As String

    Sub ConfigurationDialog()

    Sub ProcessSamples(ByRef left() As Double, ByRef right() As Double)

End Interface
