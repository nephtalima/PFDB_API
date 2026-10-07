using Microsoft.AspNetCore.Mvc;
using PFDB.SQLite;
using PFDB.WeaponStructure;
using PFDB.WeaponUtility;
using Frontend.Server;
using System.Collections.Generic;

namespace Frontend.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PFDBController : Controller
	{

		public static Dictionary<PhantomForcesVersion, PhantomForcesDataModel> phantomForcesData = new();

		public PFDBController()
		{
			
		}

		[HttpGet("pfdb/{version}")]
		public PhantomForcesDataModel Get(int version)
		{
			return phantomForcesData[new PhantomForcesVersion(version.ToString())];
		}

		
	}
}
