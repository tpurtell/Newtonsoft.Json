# Newtonsoft.Json

This is a modified version of Newtonsoft.Json where the Portable version has been modified to
- have better startup performance particularly on mobile by using a concurrent dictionary for properties
- have reduced size by eliminating dynamic feature 
- support compacy binary representation using the SMILE format

Smile has some unimplemented features
- big integers
- not properly using shared string ids for encoding
- ....
