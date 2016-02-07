#ViewInAndroidStudio
Add-in that lets you view Android xml files in Android Studio.

#Requirements
The addin requires Android SDK Build Tools rev 23.

Android Studio styled file names. Android Studio is more strict than Xamarin Studio about file names. Only lowercase, 0-9 and underscore are supported.

Custom classes are not found and might require some tweaking.

#Installation on OS X
**Add the mpack file**
Install the View In Android Studio Add-In by cloning or downloading the Taiste.ViewInAndroidStudioAddin_x.x.mpack -file from the repo root and install new Addin from a local file.:

Xamarin Studio > Add-in Manager... > Bottom left corner > Install from file...

**Configure Android Studio**
If your Android Studio installation directory differs from the OS X default "/Applications/Android Studio.app/Contents/MacOS/studio" your can change Android Studio path from:
Xamarin Studio > Preferences... > Android Studio > General > Android Studio executable location

#Usage
**Init the Android Studio project**
First time user should create the Android Studio project. Go to your Xamarin Studio and right click your Xamarin.Android project. From the context menu pick (Re)create Android Studio project. This creates .viewinandroidstudio -project directory in your home folder and opens up Android Studio. Android Studio might not pop up with focus if you have it open.

Android Studio will open up and you will be tasked to import project from gradle. You should be good to go with the default settings. Just hit OK.

You should be good to go with using Android Studio with Xamarin now.

**Using Android Studio with Xamarin Studio**
Right click any .xml file to open up the context menu for opening files with Android Studio. Pick the View in Android Studio option from the menu.

#Contributing
1. Fork it! 
2. Create your feature branch: git checkout -b my-new-feature 
3. Commit your changes: git commit -am 'Add some feature' 
4. Push to the branch: git push origin my-new-feature 
5. Submit a pull request :D 

#History
We wanted the Xcode Interface Editor funcionality in Xamarin Studio for Android. So we made this little add-in for opening the Xamarin.Android files in Android Studio.

#Credits
Taiste
#License
**The MIT License (MIT)**

Copyright (c) 2016 Taiste

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
