MAKE_DIR = $(PWD)
PARENT_DIR := $(shell cd $(MAKE_DIR)/.. && pwd)
IMAGEPARSER_DIR := $(PARENT_DIR)/ImageParserForAPI

OUTPUT_DIR 		:= bin/Debug/net8.0
CURDIR_FILES	:= $(shell find $(MAKE_DIR) -name '*.cs')
ALL_FILES		:= $(CURDIR_FILES)


$(OUTPUT_DIR)/ComponentTester.dll: $(ALL_FILES)
	dotnet build --no-dependencies --output $(OUTPUT_DIR) --configuration Debug

all:
	make -C $(IMAGEPARSER_DIR)
	cp -vuaf $(IMAGEPARSER_DIR)/dist/impa* $(MAKE_DIR)/$(OUTPUT_DIR)/
	cp -vuraf $(IMAGEPARSER_DIR)/tessbin/ $(MAKE_DIR)/$(OUTPUT_DIR)/
	cp -vuaf $(PARENT_DIR)/weapon_database.db $(MAKE_DIR)/$(OUTPUT_DIR)/
	cp -vuaf $(PARENT_DIR)/PyExec/file.txt $(MAKE_DIR)/$(OUTPUT_DIR)/
	cp -vuaf $(PARENT_DIR)/PyExec/0_2_testimage.png $(MAKE_DIR)/$(OUTPUT_DIR)/
	dotnet build --output $(OUTPUT_DIR) --configuration Debug

.PHONY: clean
clean:
	rm $(OUTPUT_DIR)/*
