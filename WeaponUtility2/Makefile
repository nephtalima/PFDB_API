OUTPUT_DIR := bin/Debug/net8.0

$(OUTPUT_DIR)/WeaponUtility2.dll: WeaponUtility2.cs
	dotnet build --no-dependencies --output $(OUTPUT_DIR) --configuration Debug

all:
	dotnet build --output $(OUTPUT_DIR) --configuration Debug


.PHONY: clean
clean:
	rm $(OUTPUT_DIR)/*