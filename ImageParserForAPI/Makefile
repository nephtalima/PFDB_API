#check changes in impa.py
dist/impa: impa.py
	@echo "Building impa.py:"
	@pwd
	@echo ""
	cd ImageParserForAPI; \
	../bin/python -m PyInstaller --workpath ./build --distpath ./dist -F -n impa -c ./impa.py