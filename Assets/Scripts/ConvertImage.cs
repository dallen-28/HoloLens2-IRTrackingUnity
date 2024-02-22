using UnityEngine;
using System.IO;
using System;
using System.Threading;
using System.Linq;

public class ConvertImage : MonoBehaviour
{
    public string imagePath = "Textures/sample.jpg"; // Replace with your image path

    public int width = 640;
    public int height = 480;
    public Material shaderMaterial;

    void Start()
    {
        LoadImageAndConvertToNV12();
    }

    void LoadImageAndConvertToNV12()
    {
        // Load the image from file
        Texture2D rgbTexture = LoadRGBImage(imagePath);

        if (rgbTexture != null)
        {
            // Convert RGB to NV12
            byte[] nv12Data = RGBToNV12(rgbTexture);

            // Create Y and UV textures
            Texture2D yTexture = CreateTexture(rgbTexture.width, rgbTexture.height, TextureFormat.Alpha8);
            Texture2D uvTexture = CreateTexture(rgbTexture.width / 2, rgbTexture.height / 2, TextureFormat.RG16);

            // Set Y texture with NV12 Y data
            byte[] yData = new byte[rgbTexture.width * rgbTexture.height];
            Array.Copy(nv12Data, yData, yData.Length);
            yTexture.LoadRawTextureData(yData);
            yTexture.Apply();

            // Set UV texture with NV12 UV data
            int uvDataSize = rgbTexture.width * rgbTexture.height / 2;
            byte[] uvData = new byte[uvDataSize];
            Array.Copy(nv12Data, nv12Data.Length - uvDataSize, uvData, 0, uvDataSize);
            uvTexture.LoadRawTextureData(uvData);
            uvTexture.Apply();

            // Assign the textures to the shader material
            //shaderMaterial.SetTexture("_MainTex", yTexture);
            //shaderMaterial.SetTexture("_CbCrTex", uvTexture);

            shaderMaterial.SetTexture("_YTex", yTexture);
            shaderMaterial.SetTexture("_UVTex", uvTexture);
        }
    }

    Texture2D CreateTexture(int width, int height, TextureFormat format)
    {
        Texture2D texture = new Texture2D(width, height, format, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        return texture;
    }

    Texture2D LoadRGBImage(string path)
    {
        string filePath = Application.persistentDataPath + this.imagePath;

        byte[] fileData = System.IO.File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadImage(fileData); // Load the image data into the texture
        byte[] texArray = texture.GetRawTextureData();
        //File.WriteAllBytes("C:/d/Test/NV12Test3.raw", texArray);
        return texture;
    }

    byte[] RGBToNV12(Texture2D rgbTexture)
    {
        //int uvIndex = 0;

        Color[] pixels = rgbTexture.GetPixels();
        int width = rgbTexture.width;
        int height = rgbTexture.height;

        // NV12 format consists of Y plane followed by interleaved UV plane
        int YSize = width * height;
        int UVSize = width * height / 2;

        byte[] yData = new byte[YSize];
        byte[] uvData = new byte[UVSize];
        byte[] nv12Data = new byte[YSize + UVSize];

        // Convert RGB to YUV and store in NV12 format
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixel = pixels[y * width + x];

                byte Y = (byte)(pixel.r * 0.299 *255 + pixel.g * 0.587 * 255 + pixel.b * 0.114 * 255);


                byte U = (byte)(pixel.r * -0.168736 * 255 - pixel.g * 0.331264 * 255 + pixel.b * 255 * 0.5 + 128);
                byte V = (byte)(pixel.r * 0.5 * 255 - pixel.g * 0.418688 * 255 - pixel.b * 255 * 0.081312 + 128);
                //byte Y = (byte)((0.257 * pixel.r * 255) + (0.504 * pixel.g * 255) + (0.098 * pixel.b * 255) + 16);
                //byte U = (byte)((-0.148 * pixel.r * 255) - (0.291 * pixel.g * 255) + (0.439 * pixel.b * 255) + 128);
                //byte V = (byte)((0.439 * pixel.r * 255) - (0.368 * pixel.g * 255) - (0.071 * pixel.b * 255) + 128);

                yData[y * width + x] = Y;

                if (y % 2 == 0 && x % 2 == 0)
                {

                    int uvIndex = YSize + (y / 2) * width / 2 + x / 2;
                    nv12Data[uvIndex] = U;
                    nv12Data[uvIndex + 1] = V;

                    //byte U = (byte)((-0.148 * pixel.r * 255) - (0.291 * pixel.g * 255) + (0.439 * pixel.b * 255) + 128);
                    //byte V = (byte)((0.439 * pixel.r * 255) - (0.368 * pixel.g * 255) - (0.071 * pixel.b * 255) + 128);
                    //uvData[uvIndex++] = U;
                    //uvIndex++;
                    //uvData[uvIndex++] = V;
                    //uvIndex++;
                    //int uvIndex = YSize + (y / 2) * (width / 2) + (x / 2);
                    //int uvIndex = YSize + (y * width + x);
                    //int uvIndex = YSize + ((y * width + x)/2);
                    //[uvIndex] = U;
                    //nv12Data[uvIndex + 1] = V;

                }
            }
        }

        // public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)


        nv12Data = yData.Concat(uvData).ToArray();

        File.WriteAllBytes("C:/d/Test/NV12Test.raw", nv12Data);

        return nv12Data;


    }
}
