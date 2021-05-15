# AO-ItemScript-Manager
Anarchy Online manager for scripted fake items

## Usage:

In Settings, set your folder for both the game folder, and the scripts folder of the game instance you're using

Creating a script or menu will add generic sample text into the file to give a base for editing. The preview isn't perfect, but gives a semi-accurate preview of what the item may look like in AO.

When creating a menu, also create a script which calls it. The menu will reside in the AO Game Folder\cd_image\text\help folder, and the script needs to run a **/showfile filename** in order to pop up the menu. This is useful if you have multiple scripts and want to list them all in a single menu. If your menu file is named **Menu.txt**, then the script you create to display will contain **/showfile Menu.txt**. Then, within the Menu.txt file you'll have the text to format a window and display scripts to launch.
  
Note that the scripts themselves need to have all text on a single line (limitation in AO), but that's where the preview makes things a bit easier.

## Example image from in-app

![](https://github.com/skeezerbean/AO-ItemScript-Manager/blob/main/example-inapp.png)

## Example image from in-game (same script)

![](https://github.com/skeezerbean/AO-ItemScript-Manager/blob/main/example-ingame.png)
