# AMPlayerDS
AMPlayerDS is an audio player written entirely in **Visual Basic .NET** and based on the .NET Framework and WinForms

![AMPLayer Demo](main.gif)

###### Decoding
Various audio formats are supported in playback, including mp3, ogg, opus, wav, wma and compact disc:
- MP3 decoding is based on the open source library mpg123
- The ogg decoding is based on the ogg and vorbis library
- The wav and CD decoding are written entirely in visual basic
- The wma (and possibly mp3, wav) decoding is based on the Media Foundation

###### Encoding
Support for transcoding input file types to mp3 (via lame encoder) or wav. Ability to rip CDs to mp3 or wav.
During encoding you can change the audio quality through the libsamplerate library.

###### Visualizations
Application of the Fast Fourier Transform in real time on the audio being reproduced for the graphic visualization of the frequencies

###### Playlists
Support for reading and writing playlists, m3u (probably in the future also .pls)

###### Realtime audio effects
Ability to apply audio effects such as equalizer (Biquad filters) and Phase shift

###### Output
Audio output based on the WaveOut or DirectSound API

###### Final thoughts
This project was born as an attempt to understand the principle behind the operation of common audio players such as winamp, foobar, etc. written in a simple language such as VB.NET

Anyone can clone, modify and publish the repository, always citing the link of the original project. License GPL 3.00
