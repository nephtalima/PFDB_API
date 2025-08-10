import sys
sys.path.append('C:\\Users\\Aethelhelm\\AppData\\Local\\Programs\\Python\\Python312\\Lib\\site-packages')
sys.path.append('C:\\Users\\Aethelhelm\\AppData\\Local\\Programs\\Python\\Python313\\Lib\\site-packages')
sys.path.append('/mnt/bulkdata/Programming/PFDB/PFDB_API/lib/python3.12/site-packages')

import numpy
import cv2
import pytesseract
import os



os.environ["TESSDATA_PREFIX"] = '/mnt/bulkdata/Programming/PFDB/PFFB_API/ImageParserForAPI/tessbin/tessdata_best-4.1.0'
image = cv2.imread("0_1.png")
cropped_image = image[280:520, 1250:1520]
cv2.imwrite("0_1_cropped.png", cropped_image)
print(
    pytesseract.image_to_string(image, config='--psm 6')
)

print(
    pytesseract.image_to_string(cropped_image, config='--psm 6')
)

# okay so i made this file because i thought that impa was the bottleneck in the scanning operation, but it seems to be my poor implementation of the factory