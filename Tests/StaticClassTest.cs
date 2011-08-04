// Author:
// Leszek Ciesielski (skolima@gmail.com)
//
// (C) 2011 Cognifide
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using ExposedObject;

using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class StaticClassTest
    {
        [Test]
        public void FieldTest()
        {
            dynamic exposed = Exposed.From(Type.GetType("TestSubjects.StaticClass, TestSubjects"));
            string testValue = exposed.testValue;
            Assert.AreEqual("testValue", testValue);

            exposed.testValue = "TestValue";
            testValue = exposed.testValue;
            Assert.AreEqual("TestValue", testValue);
        }

        [Test]
        public void MethodTest()
        {
            dynamic exposed = Exposed.From(Type.GetType("TestSubjects.StaticClass, TestSubjects"));
            decimal convertValue = exposed.ConvertValue(8);

            Assert.AreEqual(exposed.Value, convertValue);
        }

        [Test]
        public void PropertyTest()
        {
            dynamic exposed = Exposed.From(Type.GetType("TestSubjects.StaticClass, TestSubjects"));
            exposed.Value = 9;
            decimal count = exposed.Value;
            Assert.AreEqual(9, count);
        }
    }
}
