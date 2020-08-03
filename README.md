# AMLibraryDS
AMPlayerDS is an audio player written entirely in **Visual Basic .NET** and based on the .NET Framework and WinForms

###### Decoding
Various audio formats are supported in playback, including mp3, ogg, opus, wav, wma and compac disc:
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

It should not be understood as a player to which continuous development will follow or to which end user support is provided. This is an open source project that I want to share with anyone who has to approach the development of audio applications or anyone who may find it useful.

It is not a perfect project because it is developed after work in my spare time, so surely there are unresolved bugs or better methods to write the code, if someone finds better solutions it is nice if he wanted to share them with the whole github community.

Anyone can clone, modify and publish the repository, always citing the link of the original project.
