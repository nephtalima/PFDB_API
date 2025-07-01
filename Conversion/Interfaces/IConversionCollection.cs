using System.Collections.Generic;
using PFDB.StatisticStructure;

namespace PFDB.ConversionUtility;


/// <summary>
/// Interface that defines a collection of conversions for weapons. Serves as a wrapper layer for <see cref="List{IConversion}"/> with additional fields.
/// </summary>
public interface IConversionCollection : IPFDBCollection
{
	/// <summary>
	/// Indicates whether the collection has a default conversion. If true, then <see cref="DefaultConversion"/> will contain the first (and only) default conversion.
	/// </summary>
	bool HasDefaultConversion { get; }

	/// <summary>
	/// The <see cref="IEnumerable{IConversion}"/> that contains the conversions.
	/// </summary>
	IEnumerable<IConversion> Conversions { get; }

	/// <summary>
	/// If <see cref="HasDefaultConversion"/> is true, this will contain the first (and only) default conversion.
	/// </summary>
	IDefaultConversion DefaultConversion { get; }

	/// <summary>
	/// Defines a method to add to the conversion collection. <para>Will fail if trying to add an <see cref="IDefaultConversion"/> to this list if it already contains one (i.e. when <see cref="HasDefaultConversion"/> is true), and will also fail if the conversion's weapon identification does not equal the current conversion sollection's weapon identification.</para>
	/// </summary>
	/// <param name="conversion">The conversion to be added to the current collection. Cannot be a <see cref="IDefaultConversion"/> if this collection already contains one.</param>
	void Add(IConversion conversion);

	/// <summary>
	/// Defines a method to add to the conversion collection. <para>Will fail if trying to add an <see cref="IDefaultConversion"/> to this list if it already contains one (i.e. when <see cref="HasDefaultConversion"/> is true), and will also fail if the conversion's weapon identification does not equal the current conversion sollection's weapon identification.</para>
	/// </summary>
	/// <param name="conversionCollection">The collection of conversions to be added to the current collection. Cannot contain a <see cref="IDefaultConversion"/> if this collection already contains one.</param>
	void AddRange(IConversionCollection conversionCollection);

	/// <inheritdoc cref="IConversionCollection.AddRange(IConversionCollection)"/>
	void AddRange(IEnumerable<IConversion> conversionCollection);
}

