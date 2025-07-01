MAKE_DIR = $(PWD)

OUTPUT_DIR 		:= bin/Debug/net8.0
CONCRETE_DIR	:= $(MAKE_DIR)/ConcreteClasses
CONCRETE_FILES	:= $(shell find $(CONCRETE_DIR) -name '*.cs')
INTERFACE_DIR 	:= $(MAKE_DIR)/Interfaces
INTERFACE_FILES	:= $(shell find $(INTERFACE_DIR) -name '*.cs')
CURDIR_FILES	:= $(shell find $(MAKE_DIR) -name '*.cs')
ALL_FILES		:= $(INTERFACE_FILES) $(CONCRETE_FILES) $(CURDIR_FILES)


$(OUTPUT_DIR)/FileParse.dll: $(ALL_FILES)
	dotnet build --no-dependencies --output $(OUTPUT_DIR) --configuration Debug
	cp $(MAKE_DIR)/*.txt $(OUTPUT_DIR)/

all:
	dotnet build --output $(OUTPUT_DIR) --configuration Debug
	cp $(MAKE_DIR)/*.txt $(OUTPUT_DIR)/

.PHONY: clean
clean:
	rm $(OUTPUT_DIR)/*