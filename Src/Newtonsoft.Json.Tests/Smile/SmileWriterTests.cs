#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
#if !(NET20 || NET35 || PORTABLE || ASPNETCORE50)
using System.Numerics;
#endif
using System.Text;
using System.Text.RegularExpressions;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#elif ASPNETCORE50
using Xunit;
using Test = Xunit.FactAttribute;
using Assert = Newtonsoft.Json.Tests.XUnitAssert;
#else
using NUnit.Framework;
#endif
using Newtonsoft.Json.Smile;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Tests.TestObjects;
using System.Globalization;
#if NET20
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Tests.Smile
{
    [TestFixture]
	public partial class SmileWriterTests : TestFixtureBase
    {
		[Test]
		public void WriteSingleObject_LongToInt()
		{
			JsonSerializer serializer = new JsonSerializer();

			MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			People p1 = GenUser1();

			serializer.Serialize(writer, p1);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual(SmileTestData.User1, smile);
		}

		[Test]
        public void WriteSingleObject()
        {
			JsonSerializer serializer = new JsonSerializer();

            MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			//writer.WriteStartObject();
			//writer.WritePropertyName("name");
			//writer.WriteStartObject();
			//writer.WritePropertyName("first");
			//writer.WriteValue("Mary");
			//writer.WritePropertyName("last");
			//writer.WriteValue("Jane");
			//writer.WriteEndObject();
			//writer.WritePropertyName("gender");
			//writer.WriteValue("FEMALE");
			//writer.WritePropertyName("verified");
			//writer.WriteValue(true);
			//writer.WritePropertyName("salary");
			//writer.WriteValue(67890);
			//TODO: how to test 7bit encoded byte array?
			//writer.WritePropertyName("userImage");
			//writer.WriteValue("FooBar!!");			
			//writer.WriteEndObject();

			People p2 = GenUser2();

			serializer.Serialize(writer, p2);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual(SmileTestData.User2, smile);
        }

		[Test]
		public void WriteListObject()
		{
			JsonSerializer serializer = new JsonSerializer();

			MemoryStream ms = new MemoryStream();
			SmileWriter writer = new SmileWriter(ms);

			People p1 = GenUser1();
			People p2 = GenUser2();

			List<People> list = new List<People>();
			list.Add(p1);
			list.Add(p2);
			serializer.Serialize(writer, list);

			string smile = BytesToHex(ms.ToArray());
			Assert.AreEqual(SmileTestData.Users, smile);
		}
	}

	public partial class SmileWriterTests
	{
		People GenUser1()
		{
			People people = new People();
			people.name = new People.Name();
			people.name.first = "Joe";
			people.name.last = "Sixpack";
			people.gender = "MALE";
			people.verified = false;
			people.salary = 12345;
			people.deposit = 12345L;
			people.userImage = Convert.FromBase64String("Rm9vYmFyIQ==");
			people.weight = 67.6543F;
			people.height = 181.2345;
			people.notes = new string('1', 74);
			people.notes_chinese = new string('男', 76);
			people.null_string = null;
			people.small_integer = -16;
			people.long_column_65 = "abc65";
			people.long_column_175 = "abc175";
			people.short_column = "sA";
			return people;
		}

		People GenUser2()
		{
			People people = new People();
			people.name = new People.Name();
			people.name.first = "Mary";
			people.name.last = "Jane";
			people.gender = "FEMALE";
			people.verified = true;
			people.salary = 67890;
			people.deposit = 9876543210L;
			people.userImage = Convert.FromBase64String("SEVMTE9XT1JMRCE=");
			people.weight = 42.3456F;
			people.height = 168.7654;
			people.notes = new string('2', 78);
			people.notes_chinese = new string('女', 80);
			people.null_string = null;
			people.small_integer = 12;
			people.long_column_65 = "def65";
			people.long_column_175 = "def175";
			people.short_column = "sB";
			return people;
		}
	}
}