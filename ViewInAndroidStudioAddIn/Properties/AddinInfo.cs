using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
    "ViewInAndroidStudioAddIn", 
    Namespace = "Taiste",
    Version = "0.1"
)]

[assembly:AddinName ("View In Android Studio")]
[assembly:AddinCategory ("IDE extensions")]
[assembly:AddinDescription ("Addin that adds shortucts for viewing Android xml files in Android Studio")]
[assembly:AddinAuthor ("Taiste")]