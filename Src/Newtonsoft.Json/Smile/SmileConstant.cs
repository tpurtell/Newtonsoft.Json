using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json.Smile
{

	public enum VALUE_CONSTANTS : byte
	{
		ShortSharedValueStringReference_BEGIN = 0x00,
		ShortSharedValueStringReference_END = 0x1F,

		EmptyString = 0x20,
		Null = 0x21,
		False = 0x22,
		True = 0x23,

		Integer = 0x24,
		Long = 0x25,
		BigInteger = 0x26,
		//0x27, reserved

		Float = 0x28,
		Double = 0x29,
		BigDecimal = 0x2A,
		//0x2B ~ 0x3F, reserved

		TinyAscii_BEGIN = 0x40,
		TinyAscii_END = 0x5F,
		SmallAscii_BEGIN = 0x60,
		SmallAscii_END = 0x7F,

		TinyUnicode_BEGIN = 0x80,
		TinyUnicode_END = 0x9F,
		SmallUnicode_BEGIN = 0xA0,
		SmallUnicode_END = 0xBF,

		SmallInteger_BEGIN = 0xC0,	//0xC0 ~ 0xDF, -16 ~ +15
		SmallInteger_END = 0xDF,	//0xC0 ~ 0xDF, -16 ~ +15

		LongAsciiText = 0xE0,
		LongUnicodeText = 0xE4,
		Binary7Bit = 0xE8,

		SharedStringReference = 0xEC,	//not implementated
		//0xF0 ~ 0xF7, reserved

		StartArray = 0xF8,
		EndArray = 0xF9,
		StartObject = 0xFA,
		EndObject = 0xFB,

		StringEndMarker = 0xFC,
		RawBinary = 0xFD,				//not implementated
		//0xFE, reserved

		EndOfContentMarker = 0xFF		//not used?
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

	public enum KEY_TYPES : byte
	{
		EmptyString = 0x20,
		LongSharedKeyNameReference_BEGIN = 0x30,
		LongSharedKeyNameReference_END = 0x33,
		LongUnicodeName = 0x34,
		BAD_0X3A = 0x3A,
		ShortSharedKeyNameReference_BEGIN = 0x40,
		ShortSharedKeyNameReference_END = 0x7F,
		ShortAsciiName_BEGIN = 0x80,
		ShortAsciiName_END = 0xBF,
		ShortUnicodeName_BEGIN = 0xC0,
		ShortUnicodeName_END = 0xF7,
		EndObject = 0xFB,
	}

	public class SmileConstant
	{
		//damn, Jackson v2.4.3 doesn't follow spec. It shares unicode key name longer than 64bytes.
		public const bool SHARE_LONG_UNICODE_KEY_NAME = true;
	}
}
