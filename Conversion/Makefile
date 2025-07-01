MAKE_DIR = $(PWD)
PARENT_DIR := $(shell cd $(MAKE_DIR)/.. && pwd)
IMAGEPARSER_DIR := $(PARENT_DIR)/ImageParserForAPI

OUTPUT_DIR 		:= bin/Debug/net8.0
CONCRETE_DIR	:= $(MAKE_DIR)/ConcreteClasses
CONCRETE_FILES	:= $(shell find $(CONCRETE_DIR) -name '*.cs')
INTERFACE_DIR 	:= $(MAKE_DIR)/Interfaces
INTERFACE_FILES	:= $(shell find $(INTERFACE_DIR) -name '*.cs')
CURDIR_FILES	:= $(shell find $(MAKE_DIR) -name '*.cs')
ALL_FILES		:= $(INTERFACE_FILES) $(CONCRETE_FILES) $(CURDIR_FILES)


$(OUTPUT_DIR)/Conversion.dll: $(ALL_FILES)
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
