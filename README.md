# Jellyfin File Based Metadata

Retrieve metadata for a show from a JSON file.

This plugin is meant as an integration point between Jellyfin and an external service which provides metadata.

## Info

The plugin will read the metadata from a JSON file put in the show main folder.

<b>Sample JSON File</b>
```
{
   "Name": "A Nice Show",
   "Description": "Lorem Ipsum",
   "PremiereDate": "2022-01-23",
   "EndDate": "2022-03-14",
   "Studio": "MegaStudioName",
   "Tags": ["Action", "Comedy"]
}
```

As cover the plugin will use the file `.cover.png` put in the show main folder (if the file exists).

This project is WIP.

## Build

The project is build using .NET Core 5.0

To build run `./build.sh`
The plugin will be put in the `target/FileBasedMetadata` folder.
