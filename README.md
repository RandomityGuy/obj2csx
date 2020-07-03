# obj2csx
A converter to convert obj files to Torque Constructor compatible and exportable csx file

# Usage
## Basic Usage
`obj2csx <model.obj>`
## Options
`obj2csx <model.obj> [-help] [-split n] [-flipnormals] [-map]`  
help: show help  
split: split the obj file in n parts, easier for Constructor to convert  
flipnormals: flip the normals of the model  
map: convert the obj file to map, I advise you to not use this option and discard map2dif completely because its a terrible format, use Constructor's built in export instead  
## Export via Torque Constructor
Set your Constructor export settings as follows  
![Settings](https://imgur.com/xRlOFSg.png)  
Then export the dif via File > Export  
![Export](https://imgur.com/27GpXYw.png)  
Now wait for a while till the dif converts, you shouldn't have any issues unless there are too many brushes in that case you will have to split it  
The exported dif will work in PlatinumQuest. For making it work in MBG use https://higuy.me/diffix/  
If the exported dif is heavily broken, that means you have not set up the constructor settings properly as per above images.
