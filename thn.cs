
/*

Test(){
	//invoke sql test
	//invoke pyexec test
	//invoke fileparse test
}

IEnumerable<int> DetectVersions(){
	List<int> intList = getversionnumbers();
	List<int> detectedintList = new List<int>();
	foreach(int version in intList){
		if(Directory.Exists($"{currentDirectory}{PyUtilityClass.slash}version{version}"){
			detectedintList.Add(version);
		}
	}
	return detectedintList;
}

Dump(){
	
	DetectVersions();
	//file parse -> parses from files found in detected versions
	//use needsRevision field to mark in sql database
	//weapontable -> insert into database
}

Proofread(){
	DetectVersions();
	// look for sql database 
	// if not found, look for the versions (i.e. detectversions)
	// look for data that needsRevision
	// go through each one
	// have a process for each statistic
}

Inventory(){
	// check database
	// 1. check for versions that exist
	// 2. check for weapons in the given version
	// if database not found:
	// look for files
	// use DetectVersion()
	// check each directory for the weapons in there
	// compare against sql database for the given version
}

*/