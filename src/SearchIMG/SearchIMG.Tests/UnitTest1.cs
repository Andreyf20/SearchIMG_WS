using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchIMG.Models;
using System.Drawing;

namespace SearchIMG.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private ImageProcessing baseimg = ImageProcessing.getSingleton();

        [TestMethod]
        public void Singleton_test() {
            Assert.IsNotNull(baseimg);
            ImageProcessing baseimg2 = ImageProcessing.getSingleton();
            Assert.AreEqual(baseimg, baseimg2);
        }

        [TestMethod]
        public void BinToDec_test(){
            Assert.AreEqual(5, baseimg.binTodec("101"));
            Assert.AreEqual(255, baseimg.binTodec("11111111"));
            Assert.AreEqual(237, baseimg.binTodec("11101101"));
        }

        [TestMethod]
        public void imgToBase_test()
        {
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.White);
            Assert.IsNull(baseimg.ImageToBase64(null));
            Assert.IsNotNull(baseimg.ImageToBase64(b));
        }

        [TestMethod]
        public void sadBitmap_test()
        {
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.White);
            Bitmap c = baseimg.sad(b);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void createHistogram_test()
        {
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.White);
            int[] temp = baseimg.createHistogram(b, false);
            Assert.AreEqual(1, temp[0]);
            temp = baseimg.createHistogram(b, true);
            Assert.AreEqual(1, temp[0]);
        }
    }
}
