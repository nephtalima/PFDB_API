MAKE_DIR = $(PWD)

OUTPUT_DIR 		:= bin/Debug/net8.0
ALL_FILES		:= $(shell find $(MAKE_DIR) -name '*.cs')


$(OUTPUT_DIR)/SQLiteHandler.dll: $(ALL_FILES)
	dotnet build --no-dependencies --output $(OUTPUT_DIR) --configuration Debug
	cd .. && cp -vaf weapon_database.db $(MAKE_DIR)/$(OUTPUT_DIR)/

all:
	dotnet build --output $(OUTPUT_DIR) --configuration Debug
	cd .. && cp -vaf weapon_database.db $(MAKE_DIR)/$(OUTPUT_DIR)/


.PHONY: clean
clean:
	rm $(OUTPUT_DIR)/*