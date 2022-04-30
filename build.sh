#!/bin/bash


set -e

cd "$( dirname "${BASH_SOURCE[0]}" )"


DATE=`date --utc +%FT%R:%S.000000000Z`

if [ -e target ]; then
   echo "-- Cleanup --"
   rm -R target
fi

BUILD_FOLDER="target/FileBasedMetadata"
mkdir -p $BUILD_FOLDER

echo "-- Build --"
dotnet build Jellyfin.Plugin.FileBasedMetadata.sln

echo "-- Assemble --"
cp resources/package/* $BUILD_FOLDER
sed -i "s/TIMESTAMP/$DATE/g" $BUILD_FOLDER/meta.json

cp Jellyfin.Plugin.FileBasedMetadata/bin/Debug/net5.0/Jellyfin.Plugin.FileBasedMetadata.dll $BUILD_FOLDER
