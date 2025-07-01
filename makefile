MAKE_DIR = $(PWD)
OUTPUT_DIR = bin/Debug/net8.0

IMAGEPARSER 	:= $(MAKE_DIR)/ImageParserForAPI
LOGGER 			:= $(MAKE_DIR)/Logger
WEAPONUTILITY 	:= $(MAKE_DIR)/WeaponUtility2
STATSTRUCTURE 	:= $(MAKE_DIR)/StatisticStructure
FILEPARSER 		:= $(MAKE_DIR)/FileParse
PINVOKE 		:= $(MAKE_DIR)/p_invokewrapper
PYTHONEXEC 		:= $(MAKE_DIR)/PyExec
CONVERSION		:= $(MAKE_DIR)/Conversion
WEAPONSTRUCTURE	:= $(MAKE_DIR)/WeaponStructure
COMPONENTTESTER	:= $(MAKE_DIR)/ComponentTester
SQLITEHANDLER	:= $(MAKE_DIR)/SQLiteHandler



.PHONY: individualAll
individualAll:
	@cd $(IMAGEPARSER) && make 
	@cd $(LOGGER) && make 
	@cd $(WEAPONUTILITY) && make 
	@cd $(FILEPARSER) && make 
	@cd $(STATSTRUCTURE) && make 
	@cd $(SQLITEHANDLER) && make 
	@cd $(PYTHONEXEC) && make 
	@cd $(CONVERSION) && make 
	@cd $(WEAPONSTRUCTURE) && make 
	@cd $(COMPONENTTESTER) && make

#	@make -C $(PINVOKE)

.PHONY: publishAll
publishAll:


.PHONY: cleanAll
cleanAll:
	@cd $(IMAGEPARSER) && make clean
	@cd $(LOGGER) && make clean
	@cd $(WEAPONUTILITY) && make clean
	@cd $(FILEPARSER) && make clean
	@cd $(STATSTRUCTURE) && make clean
	@cd $(SQLITEHANDLER) && make clean
	@cd $(PYTHONEXEC) && make clean
	@cd $(CONVERSION) && make clean
	@cd $(WEAPONSTRUCTURE) && make clean
	@cd $(COMPONENTTESTER) && make clean

.PHONY: allDependencies
allDependencies:
	@cd $(COMPONENTTESTER) && make all
	cp -vruaf $(COMPONENTTESTER)/$(OUTPUT_DIR) $(MAKE_DIR)