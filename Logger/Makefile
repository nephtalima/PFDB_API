OUTPUT_DIR := bin/Debug/net8.0

$(OUTPUT_DIR)/PFDBLogger.dll: PFDBLogger.cs
	dotnet build --no-dependencies --configuration Debug --output $(OUTPUT_DIR)

.PHONY: clean
clean:
	rm $(OUTPUT_DIR)/*