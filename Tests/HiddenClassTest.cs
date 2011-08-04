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
    public class HiddenClassTest
    {
        [Test]
        public void FieldTest()
        {
            dynamic exposed = Exposed.New(Type.GetType("TestSubjects.HiddenClass, TestSubjects"));
            string password = exposed.password;
            Assert.IsNull(password);

            exposed.password = "TestValue";
            password = exposed.password;
            Assert.AreEqual("TestValue", password);
        }

        [Test]
        public void MethodTest()
        {
            dynamic exposed = Exposed.New(Type.GetType("TestSubjects.HiddenClass, TestSubjects"));
            string password = exposed.GeneratePassword(8);

            Assert.AreEqual(exposed.Password, password);
        }

        [Test]
        [ExpectedException(typeof(MissingMemberException))]
        public void PropertyFailedTest()
        {
            dynamic exposed = Exposed.New(Type.GetType("TestSubjects.HiddenClass, TestSubjects"));
            int count = exposed.Count;
            Assert.Fail("Private field is not accessible from a child class.");
        }

        [Test]
        public void PropertyTest()
        {
            dynamic exposed = Exposed.New(Type.GetType("TestSubjects.HiddenClass, TestSubjects"));
            int count = exposed.Countz;
            Assert.AreEqual(0, count);

            exposed.Countz = 9;
            count = exposed.Countz;
            Assert.AreEqual(9, count);
        }
    }
}
