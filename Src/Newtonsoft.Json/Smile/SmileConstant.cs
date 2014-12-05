using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Smile
{

	public enum VALUE_CONSTANTS : byte
	{
		EmptyString = 0x20,
		Null = 0x21,
		False = 0x22,
		True = 0x23,

		Binary7Bit = 0xE8,
	}
	public enum TOKEN_TYPE : byte
	{
		LogicalDataEndMarker = 0xFF,
		Reserved = 0xFE,
		RawBinaryData = 0xFD,
		StringEndMarker = 0xFC,

		EndObject = 0xFB,
		StartObject = 0xFA,
		EndArray = 0xF9,
		StartArray = 0xF8,

		NotUsed = 0x3A,

		NoSpecificHandling = 0x00,
	}

	[Flags]
	public enum SMILE_OPTIONS
	{
		None = 0,
		SharedPropertyNameEnabled = 1,
		SharedStringValueEnabled = 2,
		RawBinaryEnabled = 4,

		DEFAULT = SharedPropertyNameEnabled,
		ALL = SharedPropertyNameEnabled & SharedStringValueEnabled & RawBinaryEnabled,
	}

	class SmileConstant
	{
	}
}
