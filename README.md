# Sorter.Data 
Generates test data file filled up with string lines in format 'RandomNumber. Random text'. 
Run with command line arg, example: 'Settings:SizeInBytes=1073741824 Settings:FilePath=C:\folder\input.txt' 
Additional generator settings are specified in appsettings.json file:
"Settings": {
    "DuplicateEachLineNumber" : 5, <- Each n line in file will have duplicated text part  
    "MinWordsCount" : 7,           <- Minimum words count for text part
    "MaxWordsCount" : 10,          <- Maximum words count for text part
    "MaxNumber" : 10000            <- Max number for number part generation
  }
  
# Sorter
Sorts big file filled up with string lines in format 'RandomNumber. Random text'.
Based external merge sort algorithm. Splits file into small sorted files, then merges them using k-way merge algorithm based on minheap.
Run with command line arg, example: 'Settings:Path=C:\folder Settings:SourceFileName=input.txt Settings:DestinationFileName=output.txt'
Strings RAM buffer length settings can be redefined in appsettings.json file.
