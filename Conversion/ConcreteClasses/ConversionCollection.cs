using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PFDB.ConversionUtility;
using PFDB.Logging;
using PFDB.PythonExecutionUtility;
using PFDB.WeaponUtility;

namespace PFDB.Conversion;

/// <summary>
/// Defines a collection of conversions for weapons. Inherits from <see cref="List{IConversion}"/> with additional fields.
/// </summary>
public class ConversionCollection : List<IConversion>, IConversionCollection
{
	/// <inheritdoc/>
	public bool CollectionNeedsRevision
	{
		get
		{
			return this.Any(x => x.NeedsRevision);
		}
	}
	private WeaponIdentification _WID;

	/// <inheritdoc/>
	public WeaponIdentification WeaponID { get { return _WID; } }

	/// <inheritdoc/>
	public IEnumerable<IConversion> Conversions => this;

	/// <inheritdoc/>
	public IDefaultConversion DefaultConversion => (IDefaultConversion)this.First(x => x is IDefaultConversion);

	/// <inheritdoc/>
	public bool HasDefaultConversion => this.Exists(x => x is IDefaultConversion);

	/// <summary>
	/// Constructor for a collection of <see cref="IPythonExecutor"/>. Populates all fields within the class. Throws an exception in two cases:
	/// <list type="number">
	///		<item>Any of the items in the collection have a mismatch in their unique weapon identifier (<see cref="WeaponIdentification"/>).</item>
	///		<item>The collection contains no elements.</item>
	/// </list>
	/// </summary>
	/// <param name="executors">A non-empty collection of <see cref="IPythonExecutor"/> objects that contain the conversion data.</param>
	/// <exception cref="ArgumentException"></exception>
	public ConversionCollection(IEnumerable<IPythonExecutor> executors)
	{
		//if all elements have the same weaponID, execute the block below
		if(executors.DistinctBy(x => x.Input.WeaponID).Any())
		{
			PFDBLogger.LogFatal("WeaponIdentification mismatch in the IEnumerable<IPythonExecutor> provided.", parameter: executors);
			throw new ArgumentException("WeaponIdentification mismatch in the IEnumerable<IPythonExecutor> provided.");	
		}else if(executors.Any() == false)
		{
			PFDBLogger.LogError("The IEnumerable<IPythonExecutor> contained no elements.", parameter: executors);
			throw new ArgumentException("The IEnumerable<IPythonExecutor> contained no elements.");
		}

		_WID = executors.First().Input.WeaponID;

		foreach (IPythonExecutor executor in executors)
		{
			if (HasDefaultConversion && executors.ToList().Exists(x => x.DefaultConversion))
			{
				//we know that the collection already has a default, we cannot add anymore
				PFDBLogger.LogWarning("Cannot add a default conversion to a collection that aalready contains a default conversion.", parameter: executor);
				continue;
			}

			this.Add(new Conversion(executor));
		}

	}

	/// <summary>
	/// Constructor for default conversion. Populates all fields of the class.
	/// </summary>
	/// <param name="defaultConversion">Default conversion to pass through to this class.</param>
	public ConversionCollection(IDefaultConversion defaultConversion)
	{
		base.Add(defaultConversion);
		_WID = defaultConversion.WeaponID;
	}

	/// <inheritdoc/>
	public new void Add(IConversion conversion)
	{
		if (HasDefaultConversion && conversion is IDefaultConversion || conversion.WeaponID.WeaponType == WeaponType.Grenade || conversion.WeaponID.WeaponType == WeaponType.Melee)
		{
			//we know that the collection already has a default, we cannot add anymore
			PFDBLogger.LogWarning("Cannot add a default conversion to a collection that already contains a default conversion.", parameter: conversion);
			throw new ArgumentException("Cannot add a default conversion to a collection that already contains a default conversion."); //skip to next to try any more
		}
		if(conversion.WeaponID != this._WID)
		{
			PFDBLogger.LogWarning("Cannot add a conversion whose unique weapon identification does not match the current collection's unique weapon identification", parameter: conversion);
		}
		base.Add(conversion);
	}

	/// <inheritdoc/>
	public new void AddRange(IEnumerable<IConversion> conversionCollection)
	{
		if (HasDefaultConversion && conversionCollection.ToList().Exists(x => x is IDefaultConversion) || conversionCollection.Where(x => x.WeaponID.WeaponType == WeaponType.Grenade || x.WeaponID.WeaponType == WeaponType.Melee).Any())
		{
			//we know that the collection already has a default, we cannot add anymore
			PFDBLogger.LogWarning("Cannot add a collection containing a default conversion to a collection that already contains a default conversion.", parameter: conversionCollection);
			throw new ArgumentException("Cannot add a collection containing a default conversion to a collection that already contains a default conversion."); //skip to next to try any more
		}
		if (conversionCollection.Any(x => x.WeaponID != this._WID))
		{
			PFDBLogger.LogWarning("Cannot add a conversion whose unique weapon identification does not match the current collection's unique weapon identification", parameter: conversionCollection);
			throw new ArgumentException("Cannot add a conversion whose unique weapon identification does not match the current collection's unique weapon identification");
		}
		base.AddRange(conversionCollection);
	}

	/// <inheritdoc/>
	public void AddRange(IConversionCollection conversionCollection)
	{
		if (HasDefaultConversion && conversionCollection.HasDefaultConversion || conversionCollection.Conversions.Where(x => x.WeaponID.WeaponType == WeaponType.Grenade || x.WeaponID.WeaponType == WeaponType.Melee).Any())
		{
			//we know that the collection already has a default, we cannot add anymore
			PFDBLogger.LogWarning("Cannot add a collection containing a default conversion to a collection that already contains a default conversion.", parameter: conversionCollection);
			throw new ArgumentException("Cannot add a collection containing a default conversion to a collection that already contains a default conversion."); //skip to next to try any more
		}
		if (conversionCollection.Conversions.Any(x => x.WeaponID != this._WID))
		{
			PFDBLogger.LogWarning("Cannot add a conversion whose unique weapon identification does not match the current collection's unique weapon identification", parameter: conversionCollection);
			throw new ArgumentException("Cannot add a conversion whose unique weapon identification does not match the current collection's unique weapon identification");
		}
		base.AddRange(conversionCollection.Conversions);
	}
}

