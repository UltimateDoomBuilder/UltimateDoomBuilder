
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.IO
{
    class PcxImageReader : IImageReader
    {
        public Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety)
        {
            offsetx = int.MinValue;
            offsety = int.MinValue;

            using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
            {
                int manufacturer = reader.ReadByte(); // 10=ZSoft
                int version = reader.ReadByte();
                // 0=PC Paintbrush v2.5
                // 2=PC Paintbrush v2.8 w palette information
                // 3=PC Paintbrush v2.8 w/o palette information
                // 4=PC Paintbrush/Windows
                // 5=PC Paintbrush v3.0+
                int encoding = reader.ReadByte(); // 1 = RLE, none other known
                int bitsPerComponent = reader.ReadByte();
                int leftMargin = reader.ReadUInt16();
                int topMargin = reader.ReadUInt16();
                int rightMargin = reader.ReadUInt16();
                int bottomMargin = reader.ReadUInt16();
                int dpiX = reader.ReadUInt16();
                int dpiY = reader.ReadUInt16();
                byte[] egaPalette = reader.ReadBytes(48); // 16 RGB triplets
                reader.ReadByte(); // reserved
                int numColorPlanes = reader.ReadByte();
                int planePitch = reader.ReadUInt16(); // always even
                int paletteInfo = reader.ReadUInt16(); // 1=color/bw palette, 2=grayscale image
                int screenwidth = reader.ReadUInt16();
                int screenheight = reader.ReadUInt16();
                reader.ReadBytes(54); // reserved

                int width = rightMargin - leftMargin + 1;
                int height = bottomMargin - topMargin + 1;

                if (width == 0 || height == 0)
                    throw new InvalidDataException("Invalid pcx image file");

                int vgaPaletteID = 0;
                byte[] vgaPalette = null;

                int srcpitch = numColorPlanes * planePitch;
                byte[] scanlines = new byte[srcpitch * height];

                int pos = 0;
                while (pos < scanlines.Length)
                {
                    byte value = reader.ReadByte();
                    if ((value & 0xc0) == 0xc0) // two last bits
                    {
                        byte length = (byte)(value & 0x3f);
                        value = reader.ReadByte();
                        while (length > 0)
                        {
                            scanlines[pos++] = value;
                            length--;
                        }
                    }
                    else
                    {
                        scanlines[pos++] = value;
                    }
                }

                byte[] imageData = new byte[width * height * 4];
                int destpitch = width * 4;

                if (bitsPerComponent == 4 && numColorPlanes == 1 && paletteInfo < 2) // 16 colors from a palette
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int srcshift = ((x & 1) << 2);
                            int srcoffset = (x >> 1) + y * srcpitch;
                            int palentry = (scanlines[srcoffset] >> srcshift) & 15;
                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = egaPalette[palentry * 3];
                            imageData[offset + 1] = egaPalette[palentry * 3 + 1];
                            imageData[offset + 0] = egaPalette[palentry * 3 + 2];
                            imageData[offset + 3] = 255;
                        }
                    }
                }
                else if (bitsPerComponent == 4 && numColorPlanes == 4 && paletteInfo == 2) // 4096 colors with 16 levels of transparency
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int srcshift = ((x & 1) << 2);
                            int srcoffset = (x >> 1) + y * srcpitch;
                            int red = (scanlines[srcoffset] >> srcshift) & 15;
                            int green = (scanlines[srcoffset + planePitch] >> srcshift) & 15;
                            int blue = (scanlines[srcoffset + planePitch * 2] >> srcshift) & 15;
                            int alpha = (scanlines[srcoffset + planePitch * 3] >> srcshift) & 15;

                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = (byte)((red * 255 + 7) / 15);
                            imageData[offset + 1] = (byte)((green * 255 + 7) / 15);
                            imageData[offset + 0] = (byte)((blue * 255 + 7) / 15);
                            imageData[offset + 3] = (byte)((alpha * 255 + 7) / 15);
                        }
                    }
                }
                else if (bitsPerComponent == 8 && numColorPlanes == 1 && paletteInfo < 2) // 256 colors from a palette
                {
                    if (version == 5)
                    {
                        vgaPaletteID = reader.ReadByte(); // 0x0c
                        vgaPalette = reader.ReadBytes(768); // 256 RGB triplets
                    }
                    else
                    {
                        throw new InvalidDataException("No vga palette in pcx");
                    }

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int palentry = scanlines[x + y * srcpitch];
                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = vgaPalette[palentry * 3];
                            imageData[offset + 1] = vgaPalette[palentry * 3 + 1];
                            imageData[offset + 0] = vgaPalette[palentry * 3 + 2];
                            imageData[offset + 3] = 255;
                        }
                    }
                }
                else if (bitsPerComponent == 8 && numColorPlanes == 1 && paletteInfo == 2) // 256 shades of gray
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            byte gray = scanlines[x + y * srcpitch];
                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = gray;
                            imageData[offset + 1] = gray;
                            imageData[offset + 0] = gray;
                            imageData[offset + 3] = 255;
                        }
                    }
                }
                else if (bitsPerComponent == 8 && numColorPlanes == 3) // 24 true color
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int srcoffset = x + y * srcpitch;
                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = scanlines[srcoffset];
                            imageData[offset + 1] = scanlines[srcoffset + planePitch];
                            imageData[offset + 0] = scanlines[srcoffset + planePitch * 2];
                            imageData[offset + 3] = 255;
                        }
                    }
                }
                else if (bitsPerComponent == 8 && numColorPlanes == 4) // 24 true color with alpha channel
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int srcoffset = x + y * srcpitch;
                            int offset = x * 4 + y * destpitch;
                            imageData[offset + 2] = scanlines[srcoffset];
                            imageData[offset + 1] = scanlines[srcoffset + planePitch];
                            imageData[offset + 0] = scanlines[srcoffset + planePitch * 2];
                            imageData[offset + 3] = scanlines[srcoffset + planePitch * 3];
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException(string.Format("Unsupported pcx subformat (bits={0}, planes={1})", bitsPerComponent, numColorPlanes));
                }

                return new Bitmap(width, height, destpitch, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(imageData, 0));
            }
        }
    }

    class TgaImageReader : IImageReader
    {
        public Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety)
        {
            offsetx = int.MinValue;
            offsety = int.MinValue;

            using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
            {
                read_header(reader);
                read_image_id(reader);
                read_color_map(reader);
                read_image_data(reader);
                decode_palette();
                byte[] image = decode_image();
                return new Bitmap(image_width, image_height, image_width * 4, PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(image, 0));
            }
        }

        void read_header(BinaryReader reader)
        {
            id_length = reader.ReadByte();
            colormap_type = reader.ReadByte();
            image_type = reader.ReadByte();

            colormap_orig = reader.ReadUInt16();
            colormap_length = reader.ReadUInt16();
            colormap_entry_size = reader.ReadByte();

            image_x_orig = reader.ReadInt16();
            image_y_orig = reader.ReadInt16();
            image_width = reader.ReadUInt16();
            image_height = reader.ReadUInt16();
            image_pixel_size = reader.ReadByte();
            image_descriptor = reader.ReadByte();

            if (colormap_type > 1)
                throw new InvalidDataException("Invalid or unsupported targa image file");

            if (image_type != 1 && image_type != 2 && image_type != 3 && image_type != 9 && image_type != 10 && image_type != 11)
                throw new InvalidDataException("Invalid or unsupported targa image type");

            if (image_width == 0 || image_height == 0)
                throw new InvalidDataException("Invalid targa image file");

            if (colormap_type == 0)
                colormap_length = 0;

            bytes_per_colormap_entry = (colormap_entry_size + 7) / 8;
            bytes_per_pixel_entry = (image_pixel_size + 7) / 8;

            if (bytes_per_pixel_entry > 4)
                throw new InvalidDataException("Unsupported targa image file");

            num_alpha_bits = image_descriptor & 0x0f;
            right_to_left = (image_descriptor & 0x10) != 0;
            top_down = (image_descriptor & 0x20) != 0;
        }

        void read_image_id(BinaryReader reader)
        {
            image_id = reader.ReadBytes(id_length);
        }

        void read_color_map(BinaryReader reader)
        {
            colormap_data = reader.ReadBytes(bytes_per_colormap_entry * colormap_length);
        }

        void read_image_data(BinaryReader reader)
        {
            if (image_type == 9 || image_type == 10 || image_type == 11) // RLE compressed
            {
                image_data = new byte[bytes_per_pixel_entry * image_width * image_height];

                byte[] rle_data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

                int input = 0;
                int output = 0;
                int pixels_left = image_width * image_height;
                int input_available = rle_data.Length;
                while (pixels_left > 0 && input_available > 0)
                {
                    int code = rle_data[input];
                    int count = (code & 0x7f) + 1;
                    bool rle_packet = (code & 0x80) != 0;

                    input++;
                    input_available--;

                    if (rle_packet)
                    {
                        if (bytes_per_pixel_entry > input_available && pixels_left >= count) // Check for buffer overruns
                            break;

                        for (int i = 0; i < count; i++)
                        {
                            for (int j = 0; j < bytes_per_pixel_entry; j++)
                                image_data[output + i * bytes_per_pixel_entry + j] = rle_data[input + j];
                        }

                        input += bytes_per_pixel_entry;
                    }
                    else
                    {
                        if (count * bytes_per_pixel_entry >= input_available && pixels_left >= count) // Check for buffer overruns
                            break;

                        for (int j = 0; j < count * bytes_per_pixel_entry; j++)
                            image_data[output + j] = rle_data[input + j];

                        input += count * bytes_per_pixel_entry;
                    }

                    output += bytes_per_pixel_entry * count;
                    pixels_left -= count;
                }
            }
            else
            {
                image_data = reader.ReadBytes(bytes_per_pixel_entry * image_width * image_height);
            }
        }

        void decode_palette()
        {
            palette = new byte[colormap_length * 4];
            if (image_type == 1 || image_type == 9)
            {
                if (colormap_entry_size == 32)
                {
                    for (int i = 0; i < colormap_length * 4; i++)
                        palette[i] = colormap_data[i];

                    if (num_alpha_bits == 0)
                    {
                        for (int i = 0; i < colormap_length; i++)
                            palette[i * 4 + 3] = 255;
                    }
                }
                else if (colormap_entry_size == 24)
                {
                    for (int i = 0; i < colormap_length; i++)
                    {
                        palette[i * 4] = colormap_data[i * 3];
                        palette[i * 4 + 1] = colormap_data[i * 3 + 1];
                        palette[i * 4 + 2] = colormap_data[i * 3 + 2];
                        palette[i * 4 + 3] = 255;
                    }
                }
                else if (colormap_entry_size == 16) // 5,5,5,1
                {
                    for (int i = 0; i < colormap_length; i++)
                    {
                        int color = colormap_data[i * 2] | (((int)colormap_data[i * 2 + 1]) << 8);
                        int alpha_bit = (num_alpha_bits != 0) ? ((color >> 15) & 0x1) : 1;

                        palette[i * 4] = (byte)(((color >> 10) & 0x1f) << 3);
                        palette[i * 4 + 1] = (byte)(((color >> 5) & 0x1f) << 3);
                        palette[i * 4 + 2] = (byte)((color & 0x1f) << 3);
                        palette[i * 4 + 3] = (byte)((alpha_bit == 1) ? 255 : 0);
                    }
                }
                else
                {
                    throw new InvalidDataException("Unsupported targa image file");
                }
            }
        }

        byte[] decode_image()
        {
            // single color-map index for Pseudo-Color
            // Attribute, Red, Green and Blue ordered data for True-Color
            // independent color-map indices for Direct-Color

            if (image_type == 0) // no image data
            {
                return new byte[image_width * image_height * 4];
            }
            else if (image_type == 1 || image_type == 9) // color-mapped
            {
                return decode_color_mapped();
            }
            else if (image_type == 2 || image_type == 10) // true-color
            {
                return decode_true_color();
            }
            else if (image_type == 3 || image_type == 11) // black-and-white
            {
                return decode_grayscale();
            }
            else
            {
                throw new InvalidDataException("Invalid or unsupported targa image file");
            }
        }

        byte[] decode_color_mapped()
        {
            var image = new byte[image_width * image_height * 4];

            for (int y = 0; y < image_height; y++)
            {
                int output_line = (top_down ? y : image_height - y - 1) * image_width * 4;
                for (int x = 0; x < image_width; x++)
                {
                    int inx = (x + y * image_width) * bytes_per_pixel_entry;
                    int outx = output_line + (right_to_left ? image_width - 1 - x : x) * 4;

                    int index = 0;
                    for (int i = 0; i < bytes_per_pixel_entry; i++)
                        index |= ((int)image_data[inx + i]) << (i * 8);
                    index = Math.Min(Math.Max(index - colormap_orig, 0), (int)colormap_length - 1);
                    index *= 4;

                    image[outx] = palette[index];
                    image[outx + 1] = palette[index + 1];
                    image[outx + 2] = palette[index + 2];
                    image[outx + 3] = palette[index + 3];
                }
            }

            return image;
        }

        byte[] decode_true_color()
        {
            var image = new byte[image_width * image_height * 4];

            if (image_pixel_size == 32)
            {
                for (int y = 0; y < image_height; y++)
                {
                    int input_line = y * image_width * 4;
                    int output_line = (top_down ? y : image_height - y - 1) * image_width * 4;
                    for (int x = 0; x < image_width; x++)
                    {
                        int inx = input_line + x * 4;
                        int outx = output_line + (right_to_left ? image_width - 1 - x : x) * 4;
                        image[outx] = image_data[inx];
                        image[outx + 1] = image_data[inx + 1];
                        image[outx + 2] = image_data[inx + 2];
                        image[outx + 3] = (num_alpha_bits != 0) ? image_data[inx + 3] : (byte)255;
                    }
                }
            }
            else if (image_pixel_size == 24)
            {
                for (int y = 0; y < image_height; y++)
                {
                    int input_line = y * image_width * 3;
                    int output_line = (top_down ? y : image_height - y - 1) * image_width * 4;
                    for (int x = 0; x < image_width; x++)
                    {
                        int inx = input_line + x * 3;
                        int outx = output_line + (right_to_left ? image_width - 1 - x : x) * 4;
                        image[outx] = image_data[inx];
                        image[outx + 1] = image_data[inx + 1];
                        image[outx + 2] = image_data[inx + 2];
                        image[outx + 3] = 255;
                    }
                }
            }
            else if (image_pixel_size == 16)
            {
                for (int y = 0; y < image_height; y++)
                {
                    int input_line = y * image_width * 2;
                    int output_line = (top_down ? y : image_height - y - 1) * image_width * 4;
                    for (int x = 0; x < image_width; x++)
                    {
                        int inx = input_line + x * 2;
                        int outx = output_line + (right_to_left ? image_width - 1 - x : x) * 4;

                        int color = image_data[inx] | (((int)image_data[inx + 1]) << 8);
                        int alpha_bit = (num_alpha_bits != 0) ? ((color >> 15) & 0x1) : 1;

                        image[outx] = (byte)(((color >> 10) & 0x1f) << 3);
                        image[outx + 1] = (byte)(((color >> 5) & 0x1f) << 3);
                        image[outx + 2] = (byte)((color & 0x1f) << 3);
                        image[outx + 3] = (byte)((alpha_bit == 1) ? 255 : 0);
                    }
                }
            }
            else
            {
                throw new InvalidDataException("Unsupported targa image file");
            }

            return image;
        }

        byte[] decode_grayscale()
        {
            var image = new byte[image_width * image_height * 4];

            if (image_pixel_size == 8)
            {
                for (int y = 0; y < image_height; y++)
                {
                    int input_line = y * image_width;
                    int output_line = (top_down ? y : image_height - y - 1) * image_width * 4;
                    for (int x = 0; x < image_width; x++)
                    {
                        int inx = input_line + x;
                        int outx = output_line + (right_to_left ? image_width - 1 - x : x) * 4;
                        image[outx] = image_data[inx];
                        image[outx + 1] = image_data[inx];
                        image[outx + 2] = image_data[inx];
                        image[outx + 3] = 255;
                    }
                }
            }
            else
            {
                throw new InvalidDataException("Unsupported targa image file");
            }

            return image;
        }

        byte id_length;
        byte colormap_type;
        byte image_type;

        ushort colormap_orig;
        ushort colormap_length;
        ushort colormap_entry_size;

        short image_x_orig;
        short image_y_orig;
        ushort image_width;
        ushort image_height;
        byte image_pixel_size;
        byte image_descriptor;

        int bytes_per_colormap_entry;
        int bytes_per_pixel_entry;

        int num_alpha_bits;
        bool right_to_left;
        bool top_down;

        byte[] image_id;
        byte[] colormap_data;
        byte[] palette;
        byte[] image_data;
    }

    class FrameworkImageReader : IImageReader
    {
        bool isPng;

        public FrameworkImageReader(bool isPng)
        {
            this.isPng = isPng;
        }

        public Bitmap ReadAsBitmap(Stream stream, out int offsetx, out int offsety)
        {
            using (var image = Image.FromStream(new NoCloseStream(stream)))
            {
                ReadPngOffsets(stream, out offsetx, out offsety);
                return new Bitmap(image);
            }
        }

        void ReadPngOffsets(Stream stream, out int offsetx, out int offsety)
        {
            offsetx = int.MinValue;
            offsety = int.MinValue;

            if (isPng)
            {
                stream.Position = 8;
                using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
                {
                    // Read chunks untill we encounter either "grAb" or "IDAT"
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        // Big Endian!
                        int chunklength = ToInt32BigEndian(reader.ReadBytes(4));
                        string chunkname = new string(reader.ReadChars(4));

                        if (chunkname == "grAb")
                        {
                            offsetx = ToInt32BigEndian(reader.ReadBytes(4));
                            offsety = ToInt32BigEndian(reader.ReadBytes(4));
                            break;
                        }
                        else if (chunkname == "IDAT")
                        {
                            break;
                        }
                        else
                        {
                            // Skip the rest of the chunk
                            reader.BaseStream.Position += chunklength + 4;
                        }
                    }
                }
            }
        }

        static int ToInt32BigEndian(byte[] buffer)
        {
            return (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
        }

        // This wraps a stream but makes Close/Dispose do nothing. That prevents.net's Image.FromStream from closing it as we want to fall back to other image loaders.
        class NoCloseStream : Stream
        {
            Stream stream;
            public NoCloseStream(Stream s) { stream = s; }
            public override bool CanRead { get { return stream.CanRead; } }
            public override bool CanSeek { get { return stream.CanSeek; } }
            public override bool CanWrite { get { return stream.CanWrite; } }
            public override bool CanTimeout { get { return stream.CanTimeout; } }
            public override int ReadTimeout { get { return stream.ReadTimeout; } }
            public override int WriteTimeout { get { return stream.WriteTimeout; } }
            public override int EndRead(IAsyncResult asyncResult) { return stream.EndRead(asyncResult); }
            public override void EndWrite(IAsyncResult asyncResult) { stream.EndWrite(asyncResult); }
            public override int ReadByte() { return stream.ReadByte(); }
            public override long Length { get { return stream.Length; } }
            public override void WriteByte(byte value) { stream.WriteByte(value); }
            public override long Position { get { return stream.Position; } set { stream.Position = value; } }
            public override void Flush() { stream.Flush(); }
            public override int Read(byte[] buffer, int offset, int count) { return stream.Read(buffer, offset, count); }
            public override long Seek(long offset, SeekOrigin origin) { return stream.Seek(offset, origin); }
            public override void SetLength(long value) { stream.SetLength(value); }
            public override void Write(byte[] buffer, int offset, int count) { stream.Write(buffer, offset, count); }
        }
    }
}
