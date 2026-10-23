import sys

import numpy
import cv2
import pytesseract
import os


tempImageDirectory = "pythoncvtesting/"
cv2debug = True
#todo: add option to have many small images or one big image
#also, add comments to save my sanity every time i open this file

# Thin font function
def thinFontPreprocessing(image):
	kernel = numpy.ones((2,2),numpy.uint8)
	erode = cv2.erode(image, kernel, iterations=1)
	return (erode)

def testAndReadImage(image):
	if type(image) is str:
		print("Does the file " + image + " exist? " + str(os.path.exists(image)))
		image = cv2.imread(image)
	return image

def categoryWeaponCrop(imagePath: str, weaponType, version):
	#220,180 -> 475,730
	if(os.path.isdir("\\cropped\\")):
		os.mkdir(os.getcwd() + "\\cropped\\")
	cpath = os.getcwd() + "\\cropped\\"
	image = testAndReadImage(imagePath)
	crop = image[180:730, 220:475]
	for i in range(300):
		if i % 3 == 0:
			filename = cpath + str(weaponType) + "_" + version + str(i) + ".png"
			if(os.path.exists(fi)):
				continue
			else:
				cv2.imwrite(tempImageDirectory + filename, crop)
				print(filename + "saved!")
				break
	#cv2.imwrite("")

def commandFound(argument, option):
	if argument.find(option) >= 0:
		return True
	else:
		return False

def writedebugimage(version, filenamecounter: int, cropname: str, processname: str, image):
	filename = tempImageDirectory + "_" + version + "_" + cropname + "_" + str(filenamecounter) + "_" + processname + ".png"
	cv2.imwrite(filename, image)
	

class ImageParser:
	def __init__(self, tesspath):
		self.resize_scale = 5

		if cv2debug and not os.path.exists(tempImageDirectory):
			os.mkdir(tempImageDirectory)

		# Add trailing slash
		if not(tesspath.endswith("/")): tesspath = tesspath + "/"

		# Look for directory ./tessbin/tessdata or ./tessbin/tessdata_best-4.1.0
		TESS_PATH = tesspath + "tessbin"
		testpath = TESS_PATH + "/tessdata/"
		testpath2 = TESS_PATH + "/tessdata_best-4.1.0"

		# Set environment variable for current shell if not set already
		if(os.path.exists(testpath2)):
			os.environ["TESSDATA_PREFIX"] = testpath2
		elif(os.path.exists(testpath)):
			os.environ["TESSDATA_PREFIX"] = testpath

		# Something for Windows 
		if not(os.path.exists("/usr/bin")):
			pytesseract.pytesseract.tesseract_cmd = TESS_PATH + "\\tesseract"

	def parseTextFromImage(self, imagePath, weaponType, version, rank = 0):

		# Set weapon type
		weaponType = int(weaponType)
		print("Version: " + version)


		crops = self.getListOfCropsFromImage(imagePath,weaponType, version, rank)
		for crop in crops:
			print("=" * 5, crop[0], "=" * 5)
			data = self.getTextFromImage(crop[1],crop[0], version)
			print(data + "\n")
			f = open("file.txt","a")
			f.write(data + "\n")
			f.close()
	
	#version 9.0.2 and 9.0.3
	def cropv902(self, fullscreen_image, weaponType, crops, rank):
		if weaponType == 1: #primary
			#advanced menu crops:
			a_crop = fullscreen_image[280:520, 1250:1520]
			b_crop = fullscreen_image[100:320, 1250:1520]
			w_crop = fullscreen_image[540:760, 1250:1520]
			m_crop = fullscreen_image[730:950, 1250:1520]
			#main crops
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[320:425, 1540:1810]
			f_crop = fullscreen_image[450:670, 1540:1810]
			
			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
		elif weaponType == 2: #secondary
			#advanced menu crops:
			a_crop = fullscreen_image[280:520, 1250:1520]
			b_crop = fullscreen_image[100:320, 1250:1520]
			w_crop = fullscreen_image[540:760, 1250:1520]
			m_crop = fullscreen_image[730:950, 1250:1520]
			#main crops
			r_crop = fullscreen_image[100:320, 1540:1810]
			d_crop = fullscreen_image[295:397, 1540:1810]
			f_crop = fullscreen_image[450:670, 1540:1810]
			
			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
		elif weaponType == 3: #grenade
			r_crop = fullscreen_image[20:250, 1540:1810]
			da_crop = fullscreen_image[250:355, 1540:1810]
			d_crop = fullscreen_image[360:580, 1540:1810]

			if rank == 0:
				crops.append(("DamageAdvanced",da_crop))
				crops.append(("DamageInfoG",d_crop))
			crops.append(("RankInfo",r_crop))
				
		elif weaponType == 4: #melee
			r_crop = fullscreen_image[80:300, 1540:1810]
			a_crop = fullscreen_image[290:510, 1540:1810]

			crops.append(("MRankInfo",r_crop))
			if rank == 0: crops.append(("AnimationSpeeds",a_crop))
		return crops

	#version 10.0.1
	def cropv1001(self, fullscreen_image, weaponType, crops, rank):
		if weaponType == 1: #primary 
			#advanced menu crops: 0_0
			a_crop = fullscreen_image[400:630, 1290:1560]
			b_crop = fullscreen_image[150:380, 1290:1560]
			w_crop = fullscreen_image[555:790, 1290:1560]
			m_crop = fullscreen_image[700:920, 1290:1560]
			#main crops: 0_0
			r_crop = fullscreen_image[150:390, 1580:1850]
			d_crop = fullscreen_image[365:472, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]
			


			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
		elif weaponType == 2:
			#advanced menu crops: 0_0
			a_crop = fullscreen_image[400:630, 1290:1560]
			b_crop = fullscreen_image[150:380, 1290:1560]
			w_crop = fullscreen_image[555:790, 1290:1560]
			m_crop = fullscreen_image[700:920, 1290:1560]
			#main crops: 0_0
			r_crop = fullscreen_image[150:390, 1580:1850]
			d_crop = fullscreen_image[345:447, 1580:1850]
			f_crop = fullscreen_image[480:700, 1580:1850]
			
			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
			
		elif weaponType == 3: #grenade
			r_crop = fullscreen_image[100:300, 1580:1850]
			d_crop = fullscreen_image[290:510, 1580:1850]
			da_crop = fullscreen_image[390:605, 1580:1850]
			
			if rank == 0:
				crops.append(("DamageAdvanced",d_crop))
				crops.append(("DamageAdvanced2",da_crop))
			crops.append(("GRankInfo",r_crop))
		elif weaponType == 4: #melee
			r_crop = fullscreen_image[150:365, 1580:1850]
			a_crop = fullscreen_image[220:440, 1580:1850]

			crops.append(("MRankInfo",r_crop))
			
			if rank == 0: crops.append(("AnimationSpeeds",a_crop))
		return crops

	#version 10.1.0
	def cropv1010(self, fullscreen_image, weaponType, crops, rank):
		if weaponType == 1: #gun
			#advanced menu crops:
			a_crop = fullscreen_image[275:510, 1290:1560] #300:515?
			b_crop = fullscreen_image[120:340, 1290:1560]
			w_crop = fullscreen_image[535:750, 1290:1560]
			m_crop = fullscreen_image[700:910, 1290:1560]
			#main crops
			r_crop = fullscreen_image[150:370, 1580:1850]
			d_crop = fullscreen_image[370:470, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]

			
			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
		elif weaponType == 2:
			a_crop = fullscreen_image[275:510, 1290:1560] #300:515?
			b_crop = fullscreen_image[120:340, 1290:1560]
			w_crop = fullscreen_image[535:750, 1290:1560]
			m_crop = fullscreen_image[700:910, 1290:1560]
			#main crops
			r_crop = fullscreen_image[150:370, 1580:1850]
			d_crop = fullscreen_image[370:470, 1580:1850]
			f_crop = fullscreen_image[500:720, 1580:1850]
			
			if rank == 0:
				crops.append(("Ballistics", b_crop))
				crops.append(("Accuracies", a_crop))
				crops.append(("WeaponCharacteristics",w_crop))
				crops.append(("Miscellaneous",m_crop))
				crops.append(("DamageInfo",d_crop))
				crops.append(("FireInfo",f_crop))
			crops.append(("RankInfo",r_crop))
		elif weaponType == 3: #grenade
			r_crop = fullscreen_image[100:300, 1580:1850]
			d_crop = fullscreen_image[300:395, 1580:1850]
			da_crop = fullscreen_image[390:605, 1580:1850]

			crops.append(("GRankInfo",r_crop))
			if rank == 0:
				crops.append(("DamageAdvanced",d_crop))
				crops.append(("DamageAdvanced2",da_crop))
		elif weaponType == 4: #melee
			r_crop = fullscreen_image[150:365, 1580:1850]
			a_crop = fullscreen_image[240:460, 1580:1850]

			crops.append(("MRankInfo",r_crop))
			
			if rank == 0: crops.append(("AnimationSpeeds",a_crop))
		return crops

	# testing with big crop for advanced stats
	def cropv9999(self, fullscreen_image, weaponType, crops, rank):
		b_crop = fullscreen_image[130:900, 1290:1560]
		#c_crop = fullscreen_image
		crops.append(("bigcrop",b_crop))
		return crops
	# testing with afterglow font
	def cropv9998(self, fullscreen_image, weaponType, crops, rank):
		a_crop = fullscreen_image[240:460, 20:300]
		crops.append(("afterglow",a_crop))
		return crops

	def getListOfCropsFromImage(self, imagePath, weaponType, version, rank):
		fullscreen_image = testAndReadImage(imagePath)
		crops = []


		if version == "902":
			crops = self.cropv902(fullscreen_image, weaponType, crops, rank)
		elif version == "903":
			crops = self.cropv902(fullscreen_image, weaponType, crops, rank)
		elif version == "1001":
			crops = self.cropv1001(fullscreen_image, weaponType, crops, rank)
		elif version == "1010":
			crops = self.cropv1010(fullscreen_image, weaponType, crops, rank)
		elif version == "1011":
			crops = self.cropv1010(fullscreen_image, weaponType, crops, rank)
		elif version == "9999":
			crops = self.cropv9999(fullscreen_image, weaponType, crops, rank)
		elif version == "9998":
			crops = self.cropv9998(fullscreen_image, weaponType, crops, rank)
		else:
			crops = crops

		#todo: add support for 800, 801, 802?
		#likely not needed since I am confident in the text files

		return crops
	
	def getTextFromImage(self, image, cropname, version):

		roi_buffer = 3		

		if type(image) is str:
			image = cv2.imread(image)
		if image.size == 0:
			return "[OCR] Empty Image" #uh oh

		filenamecounter = 0

		
		# Convert to black&white image
		blackandwhite	= cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

		if cv2debug:
			writedebugimage(version, filenamecounter, cropname, "black_and_white", blackandwhite)
			filenamecounter += 1
		resize			= blackandwhite

		

		if cropname != "bigcrop" and cropname != "DamageAdvanced" and cropname != "DamageInfo": #if this isn't bigcrop, scale it
			dim			= (int(blackandwhite.shape[0] * (self.resize_scale + 4)), int(blackandwhite.shape[1] * (self.resize_scale)))
			resize		= cv2.resize(blackandwhite, dim, interpolation=cv2.INTER_AREA)
			
		# this block below was copied from StackOverflow many years ago and I have no idea what it does exactly but it works

		
		sharpened_img	= cv2.bilateralFilter(resize,9,75,75)
		if cv2debug:
			writedebugimage(version, filenamecounter, cropname, "bilateral_filter", sharpened_img)
			filenamecounter += 1
		
		im_bw			= cv2.threshold(sharpened_img, 140, 255, cv2.THRESH_BINARY )[1]
		if cv2debug:
			writedebugimage(version, filenamecounter, cropname, "threshold_filter", im_bw)
			filenamecounter += 1
		
		eroded			= thinFontPreprocessing(im_bw)
		if cv2debug:
			writedebugimage(version, filenamecounter, cropname, "erode_filter", eroded)
			filenamecounter += 1
		
		damageRangeNumberIndices = []
		if cropname == "DamageInfo" or cropname == "DamageAdvanced":
			

			blur			= cv2.GaussianBlur(im_bw, (7,7), 0)
			if cv2debug:
				writedebugimage(version, filenamecounter, cropname, "damage_blur", blur)
				filenamecounter += 1
		
			threshold		= cv2.threshold(blur, 150, 255,  cv2.THRESH_OTSU)[1]
			if cv2debug:
				writedebugimage(version, filenamecounter, cropname, "damage_threshold", threshold)
				filenamecounter += 1
				
		
			kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (6,2))
			if int(version) < 1000:
				kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (8,2)) #big version is around 56, 16
			else:
				kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (4,2)) #big version is around 56, 16
			if cv2debug:
				writedebugimage(version, filenamecounter, cropname, "damage_kernel", kernel)
				filenamecounter += 1
			
			dilate = cv2.dilate(threshold, kernel, iterations=1)
			if cv2debug:
				writedebugimage(version, filenamecounter, cropname, "damage_dilate", threshold)
				filenamecounter += 1
			

			counter= 0
			counts = cv2.findContours(dilate, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
			counts = counts[0] if len(counts) == 2 else counts[1]
			counts = sorted(counts, key=lambda x: cv2.boundingRect(x)[0])
			for c in counts:
				x, y, w, h, = cv2.boundingRect(c)
				if w > 10 or h > 12:
					bufferstarty = y - roi_buffer
					bufferstartx = x - roi_buffer
					bufferendy = y + h + roi_buffer
					bufferendx = x + w + roi_buffer
					if bufferstartx < 0: bufferstartx = 0
					if bufferstarty < 0: bufferstarty = 0

					damageRangeNumberIndices.append((counter,im_bw[bufferstarty:bufferendy , bufferstartx:bufferendx]))
					if cv2debug:
						writedebugimage(version, filenamecounter, cropname, "index_roi_" + str(counter), damageRangeNumberIndices[counter][1])

					counter += 1
					cv2.rectangle(eroded, (x,y), (x+w,y+h), (36, 255, 12), 2)
			

			##### OCR

		if cropname == "GRankInfo" or cropname == "MRankInfo":
			return pytesseract.image_to_string(eroded, config="--psm 6") # (1 or 4) and 6 seem to work best atm
		elif cropname == "DamageInfo" or cropname == "DamageAdvanced":
			data = ""
			for roi in damageRangeNumberIndices:
				data += ("index " + str(roi[0]) + ": ")
				data += pytesseract.image_to_string(roi[1], config="--psm 6")
			return data
		#elif cropname == "afterglow":
			#return pytesseract.image_to_string(bw, config="--psm 6")
		#elif cropname == "FireInfo" :
		#	return pytesseract.image_to_string(sharpened_img, config="--psm 6")
		elif cropname == "Miscellaneous" or cropname == "bigcrop":
			return pytesseract.image_to_string(sharpened_img, config="--psm 6")

		data = pytesseract.image_to_string(sharpened_img, config="--psm 4")
		return data

def checkAndExecuteOtherCommands(argument, imagePath, weaponType, version, parseObject):
	if commandFound(argument, "w"):
		categoryWeaponCrop(imagePath, weaponType, version)
	
	if commandFound(argument, "r"):
		if isinstance(parseObject, ImageParser):
			parseObject.parseTextFromImage(imagePath,weaponType,version,1)

def commandParser():
	if len(sys.argv) == 1:
		print("I force fed myself olives for a month. I hated it, but now I love olives")
		return
	
	if commandFound(sys.argv[1], "--help") or commandFound(sys.argv[1], "-h") or commandFound(sys.argv[1], "help"):
		print("""
Image Parser for Phantom Forces Database
COMMAND USAGE: impa [OPTIONS] {TESSBINPATH} FILEPATH TYPE VERSION ()
		
OPTIONS:
	-h  --help  Displays this help message, and exits
	-c          Tessbin folder exists in the current working directory (TESSBINPATH is ignored)
	-f          Tessbin folder exists in another directory (specified by TESSBINPATH)
	#-w			

	NOTE: -c and -f are mutually exclusive; you cannot use both at the same time.
		
TESSBINPATH: path to /tessbin/ (important for pytesseract). ignored if -c flag is specified

FILEPATH: path to file to be processed

TYPES: Specifies the type of weapon to be processed
	1 = Primary
	2 = Secondary
	3 = Grenade
	4 = Melee

VERSION: specifies the version of Phantom Forces

	Versions to be implemented: 800, 801, 802 
	Currently supported versions: 902, 903, 1001, 1010
	Experimental versions: 9999, 9998

	pen
		""")
		return
	
	if commandFound(sys.argv[1],"c") and commandFound(sys.argv[1], "f"):
		print("-c and -f are mutually exclusive and cannot be used at the same time.")
		return
	


	tessbinpath = ""
	filepath = ""
	weaponType = ""
	version = ""
	ableToExecute = False
	
	if commandFound(sys.argv[1],"c"):
		if len(sys.argv) == 5: #valid
			try:
				tessbinpath = os.environ["TESSBINPATH"]
			except:
				print("TESSBINPATH environment variable not found")
			filepath = sys.argv[2]
			weaponType = sys.argv[3]
			version = sys.argv[4]
		elif len(sys.argv) > 5:
			print("Too many parameters. Expected 4: impa [OPTIONS] FILEPATH TYPE VERSION")
		else:
			print("Too few parameters. Expected 4: impa [OPTIONS] FILEPATH TYPE VERSION")
	elif commandFound(sys.argv[1], "f"):
		if len(sys.argv) == 6: #valid
			tessbinpath = sys.argv[2]
			filepath = sys.argv[3]
			weaponType = sys.argv[4]
			version = sys.argv[5]
		elif len(sys.argv) > 6:
			print("Too many parameters. Expected 5: impa [OPTIONS] TESSBINPATH FILEPATH TYPE VERSION")
		else:
			print("Too few parameters. Expected 5: impa [OPTIONS] TESSBINPATH FILEPATH TYPE VERSION")
	
	if commandFound(sys.argv[1],"r") == False:
		ableToExecute = True
	
	
	pa = ImageParser(tessbinpath)
	if ableToExecute: pa.parseTextFromImage(filepath,weaponType,version)

	checkAndExecuteOtherCommands(sys.argv[1], filepath, weaponType, version, pa)


	return

#pyinstaller command: py -m PyInstaller --onefile --paths=C:\Users\Aethelhelm\AppData\Local\Programs\Python\Python312 imp.py
	
commandParser()
"I force fed myself olives for a month. I hated it, but now I love olives" #quote by dw harder


#pa = ImageParser()
#pa.parseTextFromImage(sys.argv[3],int(sys.argv[4]))
#pa.parseTextFromImage("C:\\Users\\Aethelhelm\\Programming\\source\\repos\\PFDB_API\\ImageParserForAPI\\13_2.png",2)
