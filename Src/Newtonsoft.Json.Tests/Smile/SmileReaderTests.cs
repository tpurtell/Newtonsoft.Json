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
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Tests.TestObjects;
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
using Newtonsoft.Json.Tests.Serialization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Newtonsoft.Json.Tests.Smile
{
    [TestFixture]
	public partial class SmileReaderTests : TestFixtureBase
    {
        [Test]
        public void ReadSingleObject_LongToInt()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes(SmileTestData.User1);

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			this.VerifyUser1(people);
		}

        [Test]
        public void ReadOne()
        {
            JsonSerializer serializer = new JsonSerializer();

            byte[] data = SoapHexBinary.Parse("3A290A01FA8072FA8023CA8062FA817574FA43FA807022806123806C6E6C6F6E6764616E3A2F2F4F4E452F6C64746573742D312F6B4B77717843634E316D784F4A695A52324B44464C673D3DFBFBFBFBFB").Value;

            MemoryStream ms = new MemoryStream(data);

            SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
            var x = serializer.Deserialize<JObject>(reader);
        }
        [Test]
        public void ReadTwo()
        {
            JsonSerializer serializer = new JsonSerializer();

            byte[] data = SoapHexBinary.Parse("3A290A01FA8072FA8023C6803DFA807624A6FBFBFB").Value;

            MemoryStream ms = new MemoryStream(data);

            SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
            var x = serializer.Deserialize<JObject>(reader);
        }

        [Test]
        public void ReadSingleObject()
        {
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes(SmileTestData.User2);

            MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, false, DateTimeKind.Local);
			People people = serializer.Deserialize<People>(reader);

			this.VerifyUser2(people);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.StartObject, reader.TokenType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.PropertyName, reader.TokenType);
			//Assert.AreEqual("name", reader.Value);
			//Assert.AreEqual(typeof(string), reader.ValueType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.Integer, reader.TokenType);
			//Assert.AreEqual(1L, reader.Value);
			//Assert.AreEqual(typeof(long), reader.ValueType);

			//Assert.IsTrue(reader.Read());
			//Assert.AreEqual(JsonToken.EndObject, reader.TokenType);

			//Assert.IsFalse(reader.Read());
			//Assert.AreEqual(JsonToken.None, reader.TokenType);
        }

		[Test]
		public void ReadListObject()
		{
			JsonSerializer serializer = new JsonSerializer();

			byte[] data = HexToBytes(SmileTestData.Users);

			MemoryStream ms = new MemoryStream(data);

			SmileReader reader = new SmileReader(ms, true, DateTimeKind.Local);
			List<People> list = serializer.Deserialize<List<People>>(reader);

			Assert.NotNull(list);
			Assert.AreEqual(2, list.Count);

			People people = list.FirstOrDefault();
			this.VerifyUser1(people);

			people = list.Skip(1).FirstOrDefault();
			this.VerifyUser2(people);
		}
   }

	public partial class SmileReaderTests
	{
		void VerifyUser1(People people)
		{
			Assert.NotNull(people);
			Assert.NotNull(people.name);
			Assert.AreEqual("Joe", people.name.first);
			Assert.AreEqual("Sixpack", people.name.last);

			Assert.AreEqual("MALE", people.gender);
			Assert.AreEqual(false, people.verified);

			Assert.AreEqual(12345, people.salary);
			Assert.AreEqual(12345L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(7, people.userImage.Length);

			Assert.AreEqual(67.6543F, people.weight);
			Assert.AreEqual(181.2345, people.height);

			Assert.NotNull(people.notes);
			Assert.AreEqual(74, people.notes.Length);
			Assert.AreEqual('1', people.notes[0]);
			Assert.NotNull(people.notes_chinese);
			Assert.AreEqual(76, people.notes_chinese.Length);
			Assert.AreEqual('男', people.notes_chinese[0]);

			Assert.Null(people.null_string);
			Assert.AreEqual(-16, people.small_integer);
			Assert.AreEqual("abc65", people.long_column_65);
			Assert.AreEqual("abc175", people.long_column_175);
			Assert.AreEqual("sA", people.short_column);

		}

		void VerifyUser2(People people)
		{
			Assert.NotNull(people);
			Assert.NotNull(people.name);
			Assert.AreEqual("Mary", people.name.first);
			Assert.AreEqual("Jane", people.name.last);

			Assert.AreEqual("FEMALE", people.gender);
			Assert.AreEqual(true, people.verified);

			Assert.AreEqual(67890, people.salary);
			Assert.AreEqual(9876543210L, people.deposit);

			Assert.NotNull(people.userImage);
			Assert.AreEqual(11, people.userImage.Length);

			Assert.AreEqual(42.3456F, people.weight);
			Assert.AreEqual(168.7654, people.height);

			Assert.NotNull(people.notes);
			Assert.AreEqual(78, people.notes.Length);
			Assert.AreEqual('2', people.notes[0]);
			Assert.NotNull(people.notes_chinese);
			Assert.AreEqual(80, people.notes_chinese.Length);
			Assert.AreEqual('女', people.notes_chinese[0]);

			Assert.Null(people.null_string);
			Assert.AreEqual(12, people.small_integer);
			Assert.AreEqual("def65", people.long_column_65);
			Assert.AreEqual("def175", people.long_column_175);
			Assert.AreEqual("sB", people.short_column);
		}
	}
}