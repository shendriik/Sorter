# Sorter.Data <br/> 
Generates test data file filled with string lines in format **RandomNumber. Random text**<br/> 
Run with command line arsg, example: **Settings:SizeInBytes=1073741824 Settings:FilePath=C:\folder\input.txt**<br/> 
Additional generator settings are specified in **appsettings.json** file:<br/> 

```
"Settings": {<br/> 
    "DuplicateEachLineNumber" : 5, <- Each n line in file will have duplicated text part
    "MinWordsCount" : 7,           <- Minimum words count for text part
    "MaxWordsCount" : 10,          <- Maximum words count for text part
    "MaxNumber" : 10000            <- Max number for number part generation
  }
```  

# Sorter <br/> 
Sorts big file filled with string lines in format **RandomNumber. Random text**.<br/> 
Based on External Merge Sort algorithm. Splits file into small sorted files, then merges them using K-Way Merge algorithm based on min heap.<br/> 
Run with command line args, example: **Settings:Path=C:\folder Settings:SourceFileName=input.txt Settings:DestinationFileName=output.txt**<br/> 
Strings RAM buffer length settings can be redefined in appsettings.json file.<br/> 
