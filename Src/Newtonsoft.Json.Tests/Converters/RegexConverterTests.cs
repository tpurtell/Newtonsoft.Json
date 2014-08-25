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
#if !NET20 && !NETFX_CORE
using System.Data.Linq;
#endif
#if !NETFX_CORE
using System.Data.SqlTypes;
#endif
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
#if !NETFX_CORE
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#endif
using Newtonsoft.Json.Tests.TestObjects;

namespace Newtonsoft.Json.Tests.Converters
{
    [TestFixture]
    public class RegexConverterTests : TestFixtureBase
    {
        public class RegexTestClass
        {
            public Regex Regex { get; set; }
        }

        [Test]
        public void SerializeToText()
        {
            Regex regex = new Regex("abc", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            string json = JsonConvert.SerializeObject(regex, Formatting.Indented, new RegexConverter());

            Assert.AreEqual(@"{
  ""Pattern"": ""abc"",
  ""Options"": 513
}", json);
        }

        [Test]
        public void SerializeCamelCaseAndStringEnums()
        {
            Regex regex = new Regex("abc", RegexOptions.IgnoreCase);

            string json = JsonConvert.SerializeObject(regex, Formatting.Indented, new JsonSerializerSettings
            {
                Converters = { new RegexConverter(), new StringEnumConverter() { CamelCaseText = true } },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            Assert.AreEqual(@"{
  ""pattern"": ""abc"",
  ""options"": ""ignoreCase""
}", json);
        }

        [Test]
        public void DeserializeCamelCaseAndStringEnums()
        {
            string json = @"{
  ""pattern"": ""abc"",
  ""options"": ""ignoreCase""
}";

            Regex regex = JsonConvert.DeserializeObject<Regex>(json, new JsonSerializerSettings
            {
                Converters = { new RegexConverter() }
            });

            Assert.AreEqual("abc", regex.ToString());
            Assert.AreEqual(RegexOptions.IgnoreCase, regex.Options);
        }

        [Test]
        public void DeserializeISerializeRegexJson()
        {
            string json = @"{
                        ""Regex"": {
                          ""pattern"": ""(hi)"",
                          ""options"": 5,
                          ""matchTimeout"": -10000
                        }
                      }";

            RegexTestClass r = JsonConvert.DeserializeObject<RegexTestClass>(json);

            Assert.AreEqual("(hi)", r.Regex.ToString());
            Assert.AreEqual(RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture, r.Regex.Options);
        }

        [Test]
        public void DeserializeFromText()
        {
            string json = @"{
  ""Pattern"": ""abc"",
  ""Options"": 513
}";

            Regex newRegex = JsonConvert.DeserializeObject<Regex>(json, new RegexConverter());
            Assert.AreEqual("abc", newRegex.ToString());
            Assert.AreEqual(RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, newRegex.Options);
        }


        [Test]
        public void ConvertEmptyRegexJson()
        {
            Regex regex = new Regex("");

            string json = JsonConvert.SerializeObject(new RegexTestClass { Regex = regex }, Formatting.Indented, new RegexConverter());

            Assert.AreEqual(@"{
  ""Regex"": {
    ""Pattern"": """",
    ""Options"": 0
  }
}", json);

            RegexTestClass newRegex = JsonConvert.DeserializeObject<RegexTestClass>(json, new RegexConverter());
            Assert.AreEqual("", newRegex.Regex.ToString());
            Assert.AreEqual(RegexOptions.None, newRegex.Regex.Options);
        }
    }
}