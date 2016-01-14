#!/bin/bash
create_project() {
    unzip taisteAndroid.zip -d $1
}

link_files() {
    #Link -s all res files from the Xamarin Android project to Android Studio
    find $2 -name '*' -maxdepth 2 -mindepth 1 -exec ln -s {} $1"app/src/main/res" \;
}

rename_files() {
    #Rename the resource files to lowercase.
    #rename all axml files to xml files
    rename -f 'y/A-Z/a-z/' $1"app/src/main/res"*
    find . -iname "*.axml" -exec rename -f .axml .xml '{}' \;
}

if [[ $# -eq 0 ]] ; then
    echo 'usage: android_xamarin_linker.sh [path_android] [path_xamarin]

Program commands are:

    path_android    The location for your new Android project directory. If the directory does not exist, it will be created for you.
    path_xamarin    The path to your current Xamarin Android project resources root with the layout directory.

    Please make sure that you are including the taisteAndroid.zip file with this script. The zip file is a blank Android Studio project and is used as a template.'
    exit 0
else
    create_project $1
    link_files $1 $2
    rename_files $1
fi
